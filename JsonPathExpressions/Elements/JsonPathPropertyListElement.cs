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

namespace JsonPathExpressions.Elements
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Utils;

    /// <summary>
    /// JsonPath element representing list a of properties
    /// </summary>
    public sealed class JsonPathPropertyListElement : JsonPathElement, IEquatable<JsonPathPropertyListElement>
    {
        private readonly HashSet<string> _names;

        /// <summary>
        /// Create <see cref="JsonPathPropertyListElement"/> instance
        /// </summary>
        /// <param name="names">Collection of property names</param>
        /// <exception cref="ArgumentException">Empty property names collection provided</exception>
        /// <exception cref="ArgumentOutOfRangeException">At least one property name is null or contains single quote</exception>
        public JsonPathPropertyListElement(IReadOnlyCollection<string> names)
        {
            if (names == null)
                throw new ArgumentNullException(nameof(names));
            if (names.Count == 0)
                throw new ArgumentException("No names provided", nameof(names));
            if (names.Contains(null))
                throw new ArgumentOutOfRangeException(nameof(names), names, "At least one name is null");
            if (names.Any(x => x.Contains('\'')))
                throw new ArgumentException("Single quote in property name is not allowed", nameof(names));

            _names = new HashSet<string>(names, StringComparer.Ordinal);
        }

        /// <inheritdoc />
        public override JsonPathElementType Type => JsonPathElementType.PropertyList;

        /// <inheritdoc />
        public override bool IsStrict => Names.Count == 1;

        /// <inheritdoc />
        public override bool IsNormalized => Names.Count != 1;

        /// <summary>
        /// Collection of property names
        /// </summary>
        /// <remarks>
        /// It is guaranteed that the collection is not empty
        /// </remarks>
        public IReadOnlyCollection<string> Names => _names;

        /// <inheritdoc />
        public override JsonPathElement GetNormalized()
        {
            if (Names.Count == 1)
                return new JsonPathPropertyElement(Names.First());

            return this;
        }

        /// <inheritdoc />
        public override bool? Matches(JsonPathElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            switch (element)
            {
                case JsonPathPropertyElement propertyElement:
                    return _names.Contains(propertyElement.Name);
                case JsonPathPropertyListElement propertyListElement:
                    return propertyListElement.Names.All(x => _names.Contains(x));
                default:
                    return false;
            }
        }

        /// <inheritdoc cref="IEquatable{T}"/>
        public bool Equals(JsonPathPropertyListElement other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return StringComparer.Ordinal.CollectionsEqual(Names, other.Names);
        }

        /// <inheritdoc />
        public override bool Equals(JsonPathElement other)
        {
            return Equals((object)other);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;

            return Equals((JsonPathPropertyListElement)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = StringComparer.Ordinal.GetCollectionHashCode(Names);
                hashCode = (hashCode * 397) ^ GetType().GetHashCode();

                return hashCode;
            }
        }
    }
}