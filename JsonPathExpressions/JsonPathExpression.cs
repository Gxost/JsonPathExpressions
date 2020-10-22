#region License
// MIT License
//
// Copyright (c) 2020 Oleksandr Banakh
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

namespace JsonPathExpressions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;
    using Builders;
    using Conversion;
    using Elements;
    using Utils;

    /// <summary>
    /// JsonPath expression
    /// </summary>
    public class JsonPathExpression : IEquatable<JsonPathExpression>
    {
        private readonly Lazy<bool> _isStrict;
        private readonly Lazy<bool> _isNormalized;
        private readonly Lazy<int> _hashCode;
        private readonly Lazy<string> _stringPath;

        /// <summary>
        /// Create <see cref="JsonPathExpression"/> instance from string presentation
        /// </summary>
        /// <param name="jsonPath">JsonPath expression string</param>
        /// <exception cref="ArgumentNullException"><paramref name="jsonPath"/> is null or empty</exception>
        /// <exception cref="JsonPathExpressionParsingException">Unable to parse <paramref name="jsonPath"/></exception>
        public JsonPathExpression(string jsonPath)
            : this(JsonPathExpressionStringParser.Parse(jsonPath))
        {
        }

        /// <summary>
        /// Create <see cref="JsonPathExpression"/> instance with passed elements
        /// </summary>
        /// <param name="elements">Collection of JsonPath elements</param>
        /// <exception cref="ArgumentNullException"><paramref name="elements"/> is null</exception>
        /// <exception cref="ArgumentException">Empty elements collection provided</exception>
        public JsonPathExpression(IReadOnlyCollection<JsonPathElement> elements)
            : this(elements, null)
        {
        }

        /// <summary>
        /// Create <see cref="JsonPathExpression"/> instance with passed elements
        /// </summary>
        /// <param name="elements">Collection of JsonPath elements</param>
        /// <param name="isAbsolutePath">Tells if the path must be absolute or relative</param>
        /// <exception cref="ArgumentNullException"><paramref name="elements"/> is null</exception>
        /// <exception cref="ArgumentException">Empty elements collection provided</exception>
        protected JsonPathExpression(IReadOnlyCollection<JsonPathElement> elements, bool? isAbsolutePath)
        {
            if (elements is null)
                throw new ArgumentNullException(nameof(elements));
            if (elements.Count == 0)
                throw new ArgumentException("No elements provided", nameof(elements));
            if (elements.Contains(null!))
                throw new ArgumentException("At least one element is null", nameof(elements));

            ValidateElements(elements, isAbsolutePath);

            Elements = new List<JsonPathElement>(elements);
            _isStrict = new Lazy<bool>(ComputeIsStrict, LazyThreadSafetyMode.PublicationOnly);
            _isNormalized = new Lazy<bool>(ComputeIsNormalized, LazyThreadSafetyMode.PublicationOnly);
            _hashCode = new Lazy<int>(ComputeHashCode, LazyThreadSafetyMode.PublicationOnly);
            _stringPath = new Lazy<string>(BuildString, LazyThreadSafetyMode.PublicationOnly);
        }

        /// <summary>
        /// Collection of JsonPath elements
        /// </summary>
        /// <remarks>
        /// It is guaranteed that the collection is not empty
        /// </remarks>
        public IReadOnlyList<JsonPathElement> Elements { get; }

        /// <summary>
        /// Last JsonPath element
        /// </summary>
        public JsonPathElement LastElement => Elements[Elements.Count - 1];

        /// <summary>
        /// Returns number of JsonPath elements in <see cref="Elements"/>
        /// </summary>
        public int Length => Elements.Count;

        /// <summary>
        /// Tells if the path starts with <see cref="JsonPathRootElement"/>
        /// </summary>
        public bool IsAbsolute => Elements[0].Type == JsonPathElementType.Root;

        /// <summary>
        /// Tells if the path represents exactly one JSON tree element (object, property or array item)
        /// </summary>
        public bool IsStrict => _isStrict.Value;

        /// <summary>
        /// Tells if the path is normalized
        /// </summary>
        public bool IsNormalized => _isNormalized.Value;

        /// <summary>
        /// Gets JsonPath expression builder
        /// </summary>
        public static IFirstPathElementSyntax Builder => JsonPathExpressionBuilder.Create();

        /// <inheritdoc cref="IEquatable{T}"/>
        public virtual bool Equals(JsonPathExpression other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return EqualityComparer<JsonPathElement>.Default.CollectionsEqual(Elements, other.Elements);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;

            return obj is JsonPathExpression expr && Equals(expr);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _hashCode.Value;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return _stringPath.Value;
        }

        /// <summary>
        /// Check if two JsonPath expressions are equal
        /// </summary>
        /// <param name="left">Left JsonPath expression</param>
        /// <param name="right">Right JsonPath expression</param>
        /// <returns>True if two JsonPath expressions are equal</returns>
        public static bool operator ==(JsonPathExpression left, JsonPathExpression right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Check if two JsonPath expressions are not equal
        /// </summary>
        /// <param name="left">Left JsonPath expression</param>
        /// <param name="right">Right JsonPath expression</param>
        /// <returns>True if two JsonPath expressions are not equal</returns>
        public static bool operator !=(JsonPathExpression left, JsonPathExpression right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Convert <see cref="JsonPathExpression"/> to <see cref="string"/>
        /// </summary>
        /// <param name="path">JsonPath expression</param>
        /// <remarks>
        /// Null <see cref="JsonPathExpression"/> is converted to null <see cref="string"/>
        /// </remarks>
        [return: NotNullIfNotNull("path")]
        public static explicit operator string?(JsonPathExpression? path)
        {
            return path?.ToString();
        }

        /// <summary>
        /// Convert <see cref="string"/> to <see cref="JsonPathExpression"/>
        /// </summary>
        /// <param name="path">String representing JsonPath expression</param>
        /// <remarks>
        /// Null <see cref="string"/> is converted to null <see cref="JsonPathExpression"/>
        /// </remarks>
        [return: NotNullIfNotNull("path")]
        public static explicit operator JsonPathExpression?(string? path)
        {
            return path is null
                ? null
                : new JsonPathExpression(path);
        }

        /// <summary>
        /// Create a JsonPath expression with exactly the same type as current instance
        /// </summary>
        /// <param name="elements">Collection of JsonPath elements</param>
        /// <returns>JsonPath expression with the same type as this</returns>
        public virtual JsonPathExpression Create(IReadOnlyCollection<JsonPathElement> elements)
        {
            return new JsonPathExpression(elements);
        }

        private bool ComputeIsStrict()
        {
            return Elements.All(x => x.IsStrict);
        }

        private bool ComputeIsNormalized()
        {
            return Elements.All(x => x.IsNormalized);
        }

        private int ComputeHashCode()
        {
            return EqualityComparer<JsonPathElement>.Default.GetCollectionHashCode(Elements);
        }

        private string BuildString()
        {
            return JsonPathExpressionStringBuilder.Build(Elements);
        }

        private static void ValidateElements(IReadOnlyCollection<JsonPathElement> elements, bool? isAbsolutePath)
        {
            if (isAbsolutePath != null)
                ValidateFirstElement(elements, isAbsolutePath.Value);

            if (elements.Skip(1).Any(x => x.Type == JsonPathElementType.Root))
                throw new ArgumentException("Root element must only appear at the start of the path", nameof(elements));
        }

        private static void ValidateFirstElement(IReadOnlyCollection<JsonPathElement> elements, bool isAbsolutePath)
        {
            bool startsWithRoot = elements.First().Type == JsonPathElementType.Root;
            if (startsWithRoot == isAbsolutePath)
                return;

            string message = isAbsolutePath
                ? "Absolute path must start with root element"
                : "Relative path must not start with root element";

            throw new ArgumentException(message, nameof(elements));
        }
    }
}