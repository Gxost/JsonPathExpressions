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
    using Conversion;

    /// <summary>
    /// JsonPath element of which consists JsonPath expression
    /// </summary>
    public abstract class JsonPathElement : IEquatable<JsonPathElement>
    {
        internal JsonPathElement()
        {
        }

        /// <summary>
        /// Gets JsonPath element type
        /// </summary>
        public abstract JsonPathElementType Type { get; }

        /// <summary>
        /// Tells if the element represents exactly one JSON tree element (object, property or array item)
        /// </summary>
        public abstract bool IsStrict { get; }

        /// <summary>
        /// Tells if the element is normalized
        /// </summary>
        public abstract bool IsNormalized { get; }

        /// <summary>
        /// Returns normalized JsonPath element
        /// </summary>
        /// <returns>Normalized JsonPath element</returns>
        /// <remarks>
        /// If current element is normalized, returns self
        /// </remarks>
        public abstract JsonPathElement GetNormalized();

        /// <summary>
        /// Tells if a set of JSON tree elements represented by the current JsonPath element contain JSON tree elements represented by the passed JsonPath element
        /// </summary>
        /// <param name="element">JsonPath element to check for matching</param>
        /// <returns>True if a set of JSON tree elements represented by the current JsonPath element contain JSON tree elements represented by the passed JsonPath element, false if not</returns>
        /// <remarks>Returns null if it's not possible to check if a set of JSON tree elements represented by the current JsonPath element contain JSON tree elements represented by the passed JsonPath element</remarks>
        public abstract bool? Matches(JsonPathElement element);

        /// <inheritdoc />
        public override string ToString()
        {
            return JsonPathExpressionStringBuilder.Build(this);
        }

        /// <inheritdoc cref="IEquatable{T}"/>
        public abstract bool Equals(JsonPathElement other);

        internal virtual bool? Matches(IReadOnlyList<JsonPathElement> elements, int index, IReadOnlyList<JsonPathElement> otherElements, int otherIndex)
        {
            if (index < 0 || index >= elements.Count)
                throw new ArgumentOutOfRangeException(nameof(index), index, "Elements index is out of range");
            if (otherIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(otherIndex), otherIndex, "Index must not be negative");
            if (!ReferenceEquals(this, elements[index])
                && !(elements[index] is JsonPathRecursiveDescentElement recursiveDescentElement && ReferenceEquals(this, recursiveDescentElement.AppliedToElement)))
            {
                throw new ArgumentException("Elements index must point to current element", nameof(index));
            }

            if (otherIndex >= otherElements.Count)
                return false;

            var otherElement = otherElements[otherIndex];
            bool? matches = Matches(otherElement);
            if (matches == false)
                return false;

            if (index + 1 >= elements.Count)
            {
                return otherIndex + 1 >= otherElements.Count
                    ? matches
                    : false;
            }

            return elements[index + 1].Matches(elements, index + 1, otherElements, otherIndex + 1);
        }
    }
}