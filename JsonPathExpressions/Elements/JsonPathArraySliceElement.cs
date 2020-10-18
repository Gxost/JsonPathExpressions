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
    using System.Threading;

    // TODO: add negative start support
    /// <summary>
    /// JsonPath element representing array slice
    /// </summary>
    public sealed class JsonPathArraySliceElement : JsonPathElement, IEquatable<JsonPathArraySliceElement>
    {
        private readonly Lazy<IndexRange?> _indexRange;

        /// <summary>
        /// Create <see cref="JsonPathArraySliceElement"/> instance
        /// </summary>
        /// <param name="start">Slice start</param>
        /// <param name="end">Slice end</param>
        /// <param name="step">Slice step</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="step"/> is zero</exception>
        /// <remarks>
        /// Array slice represents indexes in range from <paramref name="start"/> to <paramref name="end"/> (but not including <paramref name="end"/>) with <paramref name="step"/>
        /// </remarks>
        public JsonPathArraySliceElement(int? start, int? end, int step = 1)
        {
            if (step == 0)
                throw new ArgumentOutOfRangeException(nameof(step), step, "Step is zero");

            Start = start;
            End = end;
            Step = step;

            _indexRange = new Lazy<IndexRange?>(CreateIndexRange, LazyThreadSafetyMode.PublicationOnly);
        }

        /// <inheritdoc />
        public override JsonPathElementType Type => JsonPathElementType.ArraySlice;

        /// <inheritdoc />
        public override bool IsStrict => IndexRange?.IndexCount == 1;

        /// <inheritdoc />
        public override bool IsNormalized => !(Start == 0
                                               || IsNotNormalizedEmptySlice
                                               || IndexCount == 1
                                               || ContainsAllIndexes);

        /// <summary>
        /// Slice start
        /// </summary>
        public int? Start { get; }

        /// <summary>
        /// Slice end
        /// </summary>
        public int? End { get; }

        /// <summary>
        /// Slice step
        /// </summary>
        /// <remarks>
        /// It is guaranteed that the step is not zero
        /// </remarks>
        public int Step { get; }

        /// <summary>
        /// Returns number of indexes in the slice
        /// </summary>
        public int? IndexCount => IsLastElement ? 1 : IndexRange?.IndexCount;

        /// <summary>
        /// Returns true if the slice contains all possible indexes
        /// </summary>
        public bool ContainsAllIndexes => IndexRange?.ContainsAllIndexes ?? false;

        private IndexRange? IndexRange => _indexRange.Value;
        private bool IsLastElement => Start == -1 && End is null && Step == 1;
        private bool IsNotNormalizedEmptySlice => IndexCount == 0 && !(Start is null && End == 0 && Step == 1);

        /// <inheritdoc />
        public override JsonPathElement GetNormalized()
        {
            if (IsNotNormalizedEmptySlice)
                return new JsonPathArraySliceElement(null, 0, 1);
            if (IndexCount == 1)
                return new JsonPathArrayIndexElement(IndexRange!.GetIndexes().First());
            if (ContainsAllIndexes)
                return new JsonPathAnyArrayIndexElement();
            if (Start == 0)
                return new JsonPathArraySliceElement(null, End, Step);

            return this;
        }

        /// <inheritdoc />
        public override bool? Matches(JsonPathElement element)
        {
            if (element is null)
                throw new ArgumentNullException(nameof(element));

            switch (element)
            {
                case JsonPathArrayIndexElement arrayIndexElement:
                    return ContainsIndex(arrayIndexElement.Index);
                case JsonPathAnyArrayIndexElement anyArrayIndexElement:
                    return ContainsAllIndexes;
                case JsonPathArrayIndexListElement arrayIndexListElement:
                    return ContainsIndexes(arrayIndexListElement.Indexes);
                case JsonPathArraySliceElement arraySliceElement:
                    return Matches(arraySliceElement);
                case JsonPathExpressionElement expressionElement:
                    return null;
                default:
                    return false;
            }
        }

        /// <inheritdoc cref="IEquatable{T}"/>
        public bool Equals(JsonPathArraySliceElement other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return (Start ?? 0) == (other.Start ?? 0)
                   && End == other.End
                   && Step == other.Step;
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

            return Equals((JsonPathArraySliceElement) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = ((Start ?? 0) * 397) ^ (End ?? int.MinValue);
                hashCode = (hashCode * 397) ^ Step;
                hashCode = (hashCode * 397) ^ GetType().GetHashCode();

                return hashCode;
            }
        }

        /// <summary>
        /// Check if the slice contains certain array index
        /// </summary>
        /// <param name="index">Array index to check</param>
        /// <returns>True if the slice contains <paramref name="index"/>; false if not; null if it's not possible to check current slice</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is negative</exception>
        public bool? ContainsIndex(int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), index, "Array index must not be negative");

            return IndexRange?.Contains(index);
        }

        /// <summary>
        /// Return enumerable representing all indexes in the slice
        /// </summary>
        /// <returns><see cref="IEnumerable{T}"/> representing all indexes in the slice; null if it's not possible to get indexes in the slice</returns>
        public IEnumerable<int>? GetIndexes()
        {
            return IndexRange?.GetIndexes();
        }

        private bool? ContainsIndexes(IEnumerable<int> indexes)
        {
            if (indexes is null)
                throw new ArgumentNullException(nameof(indexes));

            if (ContainsAllIndexes)
                return true;
            if (Start < 0 || End < 0)
                return null;

            return indexes.All(x => ContainsIndex(x) == true);
        }

        private bool? Matches(JsonPathArraySliceElement other)
        {
            if (Equals(other))
                return true;

            return !(IndexRange is null)
                   && !(other.IndexRange is null)
                   && IndexRange.Contains(other.IndexRange);
        }

        private IndexRange? CreateIndexRange()
        {
            if (Start < 0 || End < 0)
                return null;

            return new IndexRange(Start, End, Step);
        }
    }
}