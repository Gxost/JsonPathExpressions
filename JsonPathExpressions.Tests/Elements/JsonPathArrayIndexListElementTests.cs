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
    using Helpers;
    using JsonPathExpressions.Elements;
    using Xunit;

    public class JsonPathArrayIndexListElementTests
    {
        [Fact]
        public void Matches_KnownArrayIndex_ReturnsTrue()
        {
            var element = new JsonPathArrayIndexListElement(new []{ 0, 1, 2, 3 });
            var other = new JsonPathArrayIndexElement(0);

            bool? actual = element.Matches(other);

            actual.Should().BeTrue();
        }

        [Fact]
        public void Matches_KnownArrayIndexesList_ReturnsTrue()
        {
            var element = new JsonPathArrayIndexListElement(new [] { 0, 1, 2, 3 });
            var other = new JsonPathArrayIndexListElement(new [] { 1, 2 });

            bool? actual = element.Matches(other);

            actual.Should().BeTrue();
        }

        [Fact]
        public void Matches_UnknownArrayIndexesList_ReturnsFalse()
        {
            var element = new JsonPathArrayIndexListElement(new [] { 0, 1, 2, 3 });
            var other = new JsonPathArrayIndexListElement(new [] { 0, 1, 2, 3, 4 });

            bool? actual = element.Matches(other);

            actual.Should().BeFalse();
        }

        [Theory]
        // slice inside of index list
        [InlineData(0, 2, 1, true, 0, 1)]
        [InlineData(null, 2, 1, true, 0, 1)]
        [InlineData(0, 2, 1, true, 0, 1, 2)]
        [InlineData(null, 2, 1, true, 0, 1, 2)]
        // slice outside of index list
        [InlineData(0, 3, 1, false, 0, 1)]
        [InlineData(null, 3, 1, false, 0, 1)]
        // slice inside of index list
        [InlineData(0, 3, 2, true, 0, 2)]
        [InlineData(null, 3, 2, true, 0, 2)]
        [InlineData(0, 3, 2, true, 0, 1, 2)]
        [InlineData(null, 3, 2, true, 0, 1, 2)]
        // slice outside of index list
        [InlineData(0, 4, 2, false, 0, 1)]
        [InlineData(null, 4, 2, false, 0, 1)]
        // slice inside of index list
        [InlineData(2, 0, -1, true, 0, 1, 2)]
        [InlineData(2, null, -1, true, 0, 1, 2)]
        // slice outside of index list
        [InlineData(3, 0, -1, false, 0, 1, 2)]
        [InlineData(0, null, 2, false, 0, 1, 2)]
        // slice covers all indexes
        [InlineData(0, null, 1, false, 0)]
        [InlineData(null, null, 1, false, 0)]
        // impossible to check if slice result is in the index list
        [InlineData(-1, 1, 1, null, 0, 1)]
        [InlineData(0, -1, 2, null, 0, 1)]
        public void Matches_ArraySlice(int? start, int? end, int step, bool? expected, params int[] indexes)
        {
            var element = new JsonPathArrayIndexListElement(indexes);
            var other = new JsonPathArraySliceElement(start, end, step);

            bool? actual = element.Matches(other);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(JsonPathElementType.Root)]
        [InlineData(JsonPathElementType.RecursiveDescent)]
        [InlineData(JsonPathElementType.Property)]
        [InlineData(JsonPathElementType.AnyProperty)]
        [InlineData(JsonPathElementType.PropertyList)]
        [InlineData(JsonPathElementType.ArrayIndex)]
        [InlineData(JsonPathElementType.AnyArrayIndex)]
        [InlineData(JsonPathElementType.ArrayIndexList)]
        [InlineData(JsonPathElementType.ArraySlice)]
        [InlineData(JsonPathElementType.Expression)]
        [InlineData(JsonPathElementType.FilterExpression)]
        public void Matches_Any_ReturnsFalse(JsonPathElementType type)
        {
            var element = new JsonPathArrayIndexListElement(new []{0, 1});
            var other = ElementCreator.CreateAny(type);

            bool? actual = element.Matches(other);

            actual.Should().BeFalse();
        }
    }
}