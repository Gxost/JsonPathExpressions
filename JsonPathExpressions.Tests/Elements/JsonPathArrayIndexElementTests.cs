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

    public class JsonPathArrayIndexElementTests
    {
        [Fact]
        public void IsStrict_ReturnsTrue()
        {
            var element = new JsonPathArrayIndexElement(7);

            element.IsStrict.Should().BeTrue();
        }

        [Fact]
        public void IsNormalized_ReturnsTrue()
        {
            var element = new JsonPathArrayIndexElement(7);

            element.IsNormalized.Should().BeTrue();
        }

        [Fact]
        public void GetNormalized_ReturnsSelf()
        {
            var element = new JsonPathArrayIndexElement(7);

            var actual = element.GetNormalized();

            actual.Should().Be(element);
        }

        [Fact]
        public void Matches_KnownArrayIndex_ReturnsTrue()
        {
            var element = new JsonPathArrayIndexElement(7);
            var other = new JsonPathArrayIndexElement(7);

            bool? actual = element.Matches(other);

            actual.Should().BeTrue();
        }

        [Fact]
        public void Matches_KnownArrayIndexList_ReturnsTrue()
        {
            var element = new JsonPathArrayIndexElement(7);
            var other = new JsonPathArrayIndexListElement(new []{7});

            bool? actual = element.Matches(other);

            actual.Should().BeTrue();
        }

        [Fact]
        public void Matches_UnknownArrayIndexList_ReturnsFalse()
        {
            var element = new JsonPathArrayIndexElement(7);
            var other = new JsonPathArrayIndexListElement(new[] { 0, 7 });

            bool? actual = element.Matches(other);

            actual.Should().BeFalse();
        }

        [Theory]
        // single index
        [InlineData(7, 7, 8, 1, true)]
        [InlineData(7, 7, 9, 2, true)]
        // multiple indexes
        [InlineData(7, 7, 9, 1, false)]
        [InlineData(7, 7, 10, 2, false)]
        // single index
        [InlineData(7, 7, 6, -1, true)]
        [InlineData(7, 7, 6, -2, true)]
        // multiple indexes
        [InlineData(7, 7, 5, -1, false)]
        [InlineData(7, 7, 4, -2, false)]
        public void Matches_ArraySlice(int index, int? start, int? end, int step, bool? expected)
        {
            var element = new JsonPathArrayIndexElement(index);
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
            var element = new JsonPathArrayIndexElement(7);
            var other = ElementCreator.CreateAny(type);

            bool? actual = element.Matches(other);

            actual.Should().BeFalse();
        }
    }
}