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
    using Builders;
    using Conversion;
    using Elements;

    /// <summary>
    /// JsonPath expression not starting with <see cref="JsonPathRootElement"/>
    /// </summary>
    public sealed class RelativeJsonPathExpression : JsonPathExpression, IEquatable<RelativeJsonPathExpression>
    {
        /// <summary>
        /// Create <see cref="RelativeJsonPathExpression"/> instance from string presentation
        /// </summary>
        /// <param name="jsonPath">JsonPath expression string</param>
        public RelativeJsonPathExpression(string jsonPath)
            : this(JsonPathExpressionStringParser.Parse(jsonPath))
        {
        }

        /// <summary>
        /// Create <see cref="RelativeJsonPathExpression"/> instance with passed elements
        /// </summary>
        /// <param name="elements">Collection of JsonPath elements</param>
        /// <exception cref="ArgumentException">Empty elements collection provided</exception>
        public RelativeJsonPathExpression(IReadOnlyCollection<JsonPathElement> elements)
            : base(elements, false)
        {
        }

        /// <inheritdoc cref="IEquatable{T}"/>
        public bool Equals(RelativeJsonPathExpression other)
        {
            return Equals((JsonPathExpression)other);
        }

        /// <summary>
        /// Gets relative JsonPath expression builder
        /// </summary>
        public static new IFirstRelativePathElementSyntax Builder => RelativeJsonPathExpressionBuilder.Create();

        /// <summary>
        /// Convert <see cref="string"/> to <see cref="RelativeJsonPathExpression"/>
        /// </summary>
        /// <param name="path">String representing JsonPath expression</param>
        /// <remarks>
        /// Null <see cref="string"/> is converted to null <see cref="RelativeJsonPathExpression"/>
        /// </remarks>
        [return: NotNullIfNotNull("path")]
        public static explicit operator RelativeJsonPathExpression?(string? path)
        {
            if (path is null)
                return null;

            return new RelativeJsonPathExpression(path);
        }

        /// <inheritdoc />
        protected internal override JsonPathExpression Create(IReadOnlyCollection<JsonPathElement> elements)
        {
            return new RelativeJsonPathExpression(elements);
        }
    }
}