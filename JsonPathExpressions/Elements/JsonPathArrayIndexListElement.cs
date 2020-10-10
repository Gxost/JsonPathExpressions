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
    /// JsonPath element representing a list of array indexes
    /// </summary>
    public sealed class JsonPathArrayIndexListElement : JsonPathElement, IEquatable<JsonPathArrayIndexListElement>
    {
        private readonly HashSet<int> _indexes;
        private readonly Lazy<bool> _isNormalized;

        /// <summary>
        /// Create <see cref="JsonPathArrayIndexListElement"/> instance
        /// </summary>
        /// <param name="indexes">Collection of array indexes</param>
        /// <exception cref="ArgumentException">Empty indexes collection provided</exception>
        /// <exception cref="ArgumentOutOfRangeException">At least one index is negative</exception>
        public JsonPathArrayIndexListElement(IReadOnlyCollection<int> indexes)
        {
            if (indexes == null)
                throw new ArgumentNullException(nameof(indexes));
            if (indexes.Count == 0)
                throw new ArgumentException("No indexes provided", nameof(indexes));
            if (indexes.Any(x => x < 0))
                throw new ArgumentOutOfRangeException(nameof(indexes), indexes, "Array index must not be negative");

            _indexes = new HashSet<int>(indexes);
            _isNormalized = new Lazy<bool>(ComputeIsNormalized);
        }

        /// <inheritdoc />
        public override JsonPathElementType Type => JsonPathElementType.ArrayIndexList;

        /// <inheritdoc />
        public override bool IsStrict => Indexes.Count == 1;

        /// <inheritdoc />
        public override bool IsNormalized => _isNormalized.Value;

        /// <summary>
        /// Gets collection of array indexes
        /// </summary>
        /// <remarks>
        /// It is guaranteed that the collection is not empty
        /// </remarks>
        public IReadOnlyCollection<int> Indexes => _indexes;

        /// <inheritdoc />
        public override JsonPathElement GetNormalized()
        {
            if (Indexes.Count == 1)
                return new JsonPathArrayIndexElement(Indexes.First());

            return IsSlice(Indexes, out int? start, out int? end, out int step)
                ? (JsonPathElement)new JsonPathArraySliceElement(start, end, step)
                : this;
        }

        /// <inheritdoc />
        public override bool? Matches(JsonPathElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            switch (element)
            {
                case JsonPathArrayIndexElement arrayIndexElement:
                    return _indexes.Contains(arrayIndexElement.Index);
                case JsonPathArrayIndexListElement arrayIndexListElement:
                    return arrayIndexListElement.Indexes.All(x => _indexes.Contains(x));
                case JsonPathArraySliceElement arraySliceElement:
                    if (arraySliceElement.ContainsAllIndexes)
                        return Indexes.Count == int.MaxValue;

                    return arraySliceElement.GetIndexes()?.All(x => _indexes.Contains(x));
                default:
                    return false;
            }
        }

        /// <inheritdoc cref="IEquatable{T}"/>
        public bool Equals(JsonPathArrayIndexListElement other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return EqualityComparer<int>.Default.CollectionsEqual(Indexes, other.Indexes);
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

            return Equals((JsonPathArrayIndexListElement) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = EqualityComparer<int>.Default.GetCollectionHashCode(Indexes);
                hashCode = (hashCode * 397) ^ GetType().GetHashCode();

                return hashCode;
            }
        }

        private bool ComputeIsNormalized()
        {
            return Indexes.Count > 1
                   && !IsSlice(Indexes, out _, out _, out _);
        }

        private static bool IsSlice(IReadOnlyCollection<int> sortedIndexes, out int? start, out int? end, out int step)
        {
            if (sortedIndexes.Count < 3)
            {
                start = null;
                end = 0;
                step = 1;
                return false;
            }

            int? first = null;
            int? second = null;
            int last = 0;
            step = 0;

            foreach (int index in sortedIndexes)
            {
                if (first == null)
                {
                    first = index;
                    last = index;
                    continue;
                }

                if (second == null)
                {
                    second = index;
                    last = index;
                    step = second.Value - first.Value;
                    continue;
                }

                if (index - last != step)
                {
                    start = 0;
                    end = 0;
                    return false;
                }
            }

            start = first == 0 ? default : first;
            end = last == int.MaxValue ? default(int?) : last + 1;

            return true;
        }
    }
}