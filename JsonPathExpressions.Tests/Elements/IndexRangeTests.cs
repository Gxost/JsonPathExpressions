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

namespace JsonPathExpressions.Tests.Elements
{
    using FluentAssertions;
    using JsonPathExpressions.Elements;
    using Xunit;

    public class IndexRangeTests
    {
        [Theory]
        [InlineData(null, 0, 1, 0)]
        [InlineData(42, 42, 1, 0)]
        [InlineData(42, 0, 1, 0)]
        [InlineData(null, 1, 1, 1)]
        [InlineData(null, 1, 2, 1)]
        [InlineData(10, 20, 1, 10)]
        [InlineData(10, 20, 2, 5)]
        [InlineData(10, 0, -1, 10)]
        [InlineData(10, 0, -2, 5)]
        public void IndexCount(int? start, int? end, int step, int expected)
        {
            var range = new IndexRange(start, end, step);

            range.IndexCount.Should().Be(expected);
        }

        [Theory]
        [InlineData(null, null, 1, true)]
        [InlineData(0, null, 1, true)]
        [InlineData(null, null, 2, false)]
        [InlineData(0, null, 2, false)]
        [InlineData(1, null, 1, false)]
        public void ContainsAllIndexes(int? start, int? end, int step, bool expected)
        {
            var range = new IndexRange(start, end, step);

            range.ContainsAllIndexes.Should().Be(expected);
        }

        [Theory]
        [InlineData(null, null, 1, 42, true)]
        [InlineData(0, null, 1, 42, true)]
        [InlineData(null, 43, 1, 42, true)]
        [InlineData(0, 43, 1, 42, true)]
        [InlineData(0, null, 2, 42, true)]
        [InlineData(0, null, 3, 42, true)]
        [InlineData(0, null, 4, 42, false)]
        [InlineData(0, 42, 1, 42, false)]
        [InlineData(10, 0, -1, 7, true)]
        [InlineData(10, 0, -2, 7, false)]
        public void Contains_Index(int? start, int? end, int step, int index, bool? expected)
        {
            var range = new IndexRange(start, end, step);

            bool? actual = range.Contains(index);

            actual.Should().Be(expected);
        }

        [Theory]
        // positive steps, start and end are set
        // other range is inside
        [InlineData(0, 10, 1, 0, 9, 1, true)]
        [InlineData(0, 10, 2, 0, 9, 2, true)]
        [InlineData(0, 11, 2, 0, 12, 2, true)]
        [InlineData(0, 10, 1, 3, 9, 1, true)]
        [InlineData(0, 10, 2, 4, 9, 2, true)]
        // other range is inside but items are different because of different start
        [InlineData(0, 10, 2, 5, 9, 2, false)]
        // other range is outside
        [InlineData(1, 10, 1, 0, 10, 1, false)]
        [InlineData(1, 10, 1, 0, 10, 2, false)]
        [InlineData(0, 10, 1, 0, 11, 1, false)]
        [InlineData(0, 10, 1, 0, 11, 2, false)]
        // different steps
        [InlineData(0, 10, 2, 0, 10, 3, false)]
        // other range step is divisible by step
        [InlineData(0, 10, 1, 0, 10, 2, true)]
        [InlineData(0, 10, 2, 0, 10, 4, true)]
        [InlineData(0, 10, 3, 0, 10, 6, true)]
        // positive steps, no end set
        // other range is inside
        [InlineData(0, null, 1, 0, null, 1, true)]
        [InlineData(0, null, 2, 0, null, 2, true)]
        [InlineData(0, null, 1, 3, null, 1, true)]
        [InlineData(0, null, 2, 4, null, 2, true)]
        // other range is inside but items are different because of different start
        [InlineData(0, null, 2, 5, null, 2, false)]
        // different steps
        [InlineData(0, null, 2, 0, null, 3, false)]
        // other range step is divisible by step
        [InlineData(0, null, 1, 0, null, 2, true)]
        [InlineData(0, null, 2, 0, null, 4, true)]
        [InlineData(0, null, 3, 0, null, 6, true)]
        // negative steps, start and end are set
        // other range is inside
        [InlineData(10, 0, -1, 10, 1, -1, true)]
        [InlineData(10, 0, -2, 10, 1, -2, true)]
        [InlineData(10, 3, -2, 10, 2, -2, true)]
        [InlineData(10, 0, -1, 6, 1, -1, true)]
        [InlineData(10, 0, -2, 6, 1, -2, true)]
        // other range is inside but items are different because of different start
        [InlineData(10, 0, -2, 7, 1, -2, false)]
        // other range is outside
        [InlineData(9, 1, -1, 9, 0, -1, false)]
        [InlineData(9, 1, -1, 9, 0, -2, false)]
        [InlineData(10, 0, -1, 11, 0, -1, false)]
        [InlineData(10, 0, -1, 11, 0, -2, false)]
        // different steps
        [InlineData(10, 0, -2, 10, 0, -3, false)]
        // other range step is divisible by step
        [InlineData(10, 0, -1, 10, 0, -2, true)]
        [InlineData(10, 0, -2, 10, 0, -4, true)]
        [InlineData(10, 0, -2, 10, 0, -6, true)]
        // negative steps, no end set
        // other range is inside
        [InlineData(10, null, -1, 10, null, -1, true)]
        [InlineData(10, null, -2, 10, null, -2, true)]
        [InlineData(10, null, -1, 6, null, -1, true)]
        [InlineData(10, null, -2, 6, null, -2, true)]
        // other range is inside but items are different because of different start
        [InlineData(10, null, -2, 7, null, -2, false)]
        // different steps
        [InlineData(10, null, -2, 10, null, -3, false)]
        // other range step is divisible by step
        [InlineData(10, null, -1, 10, null, -2, true)]
        [InlineData(10, null, -2, 10, null, -4, true)]
        [InlineData(10, null, -2, 10, null, -6, true)]
        // different step signs
        [InlineData(2, 11, 1, 10, 1, -1, true)]
        public void Contains_IndexRange(int? start, int? end, int step, int? otherStart, int? otherEnd, int otherStep, bool? expected)
        {
            var range = new IndexRange(start, end, step);
            var other = new IndexRange(otherStart, otherEnd, otherStep);

            bool? actual = range.Contains(other);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(null, 5, 1, 0, 1, 2, 3, 4)]
        [InlineData(null, 5, 2, 0, 2, 4)]
        [InlineData(5, 0, -1, 5, 4, 3, 2, 1)]
        [InlineData(5, 0, -2, 5, 3, 1)]
        [InlineData(5, null, -1, 5, 4, 3, 2, 1, 0)]
        [InlineData(5, null, -2, 5, 3, 1)]
        public void GetIndexes(int? start, int? end, int step, params int[] expected)
        {
            var range = new IndexRange(start, end, step);

            var actual = range.GetIndexes();

            actual.Should().BeEquivalentTo(expected);
        }
    }
}