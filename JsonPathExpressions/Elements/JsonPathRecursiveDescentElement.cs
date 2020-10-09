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

    /// <summary>
    /// JsonPath element representing recursive descent applied to another JsonPath element
    /// </summary>
    public sealed class JsonPathRecursiveDescentElement : JsonPathElement, IEquatable<JsonPathRecursiveDescentElement>
    {
        /// <summary>
        /// Create <see cref="JsonPathRecursiveDescentElement"/> instance
        /// </summary>
        /// <param name="appliedToElement">JsonPath element to which recursive descent is applied</param>
        /// <exception cref="ArgumentException"><paramref name="appliedToElement"/> is <see cref="JsonPathRootElement"/> or <see cref="JsonPathRecursiveDescentElement"/></exception>
        public JsonPathRecursiveDescentElement(JsonPathElement appliedToElement)
        {
            if (appliedToElement == null)
                throw new ArgumentNullException(nameof(appliedToElement));
            if (appliedToElement.Type == JsonPathElementType.Root)
                throw new ArgumentException("Recursive descent must not be applied to root element");
            if (appliedToElement.Type == JsonPathElementType.RecursiveDescent)
                throw new ArgumentException("Recursive descent must not be applied to recursive descent");

            AppliedToElement = appliedToElement;
        }

        /// <inheritdoc />
        public override JsonPathElementType Type => JsonPathElementType.RecursiveDescent;

        /// <inheritdoc />
        public override bool IsStrict => false;

        /// <inheritdoc />
        public override bool IsNormalized => AppliedToElement.IsNormalized;

        /// <summary>
        /// Gets JsonPath element to which recursive descent is applied
        /// </summary>
        public JsonPathElement AppliedToElement { get; }
        
        /// <inheritdoc />
        public override JsonPathElement GetNormalized()
        {
            var normalized = AppliedToElement.GetNormalized();
            if (ReferenceEquals(normalized, AppliedToElement))
                return this;

            return new JsonPathRecursiveDescentElement(normalized);
        }

        /// <inheritdoc />
        public override bool? Matches(JsonPathElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (element is JsonPathRecursiveDescentElement recursiveDescentElement)
                return AppliedToElement.Matches(recursiveDescentElement.AppliedToElement);

            return null;
        }

        /// <inheritdoc cref="IEquatable{T}"/>
        public bool Equals(JsonPathRecursiveDescentElement other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return AppliedToElement.Equals(other.AppliedToElement);
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

            return Equals((JsonPathRecursiveDescentElement)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = AppliedToElement.GetHashCode();
                hashCode = (hashCode * 397) ^ GetType().GetHashCode();

                return hashCode;
            }
        }

        internal override bool? Matches(IReadOnlyList<JsonPathElement> elements, int index, IReadOnlyList<JsonPathElement> otherElements, int otherIndex)
        {
            if (elements == null)
                throw new ArgumentNullException(nameof(elements));
            if (index < 0 || index >= elements.Count)
                throw new ArgumentOutOfRangeException(nameof(index), index, "Elements index is out of range");
            if (otherElements == null)
                throw new ArgumentNullException(nameof(otherElements));
            if (otherIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(otherIndex), otherIndex, "Index must not be negative");
            if (!ReferenceEquals(this, elements[index]))
                throw new ArgumentException("Elements index must point to current element", nameof(index));

            if (otherIndex >= otherElements.Count)
                return false;

            if (otherElements[otherIndex] is JsonPathRecursiveDescentElement)
                return base.Matches(elements, index, otherElements, otherIndex);

            for (int i = otherIndex; i < otherElements.Count; ++i)
            {
                bool? matches = AppliedToElement.Matches(elements, index, otherElements, i);
                if (matches != false)
                    return matches;
            }

            return false;
        }
    }
}