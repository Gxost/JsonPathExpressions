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
    using System.Linq;
    using FluentAssertions;
    using Helpers;
    using JsonPathExpressions.Elements;
    using Xunit;

    public class JsonPathExpressionElementTests
    {
        [Fact]
        public void IsStrict_ReturnsFalse()
        {
            var element = new JsonPathExpressionElement("@.length-1");

            element.IsStrict.Should().BeFalse();
        }

        [Fact]
        public void IsNormalized_ReturnsTrue()
        {
            var element = new JsonPathExpressionElement("@.length-1");

            element.IsNormalized.Should().BeTrue();
        }

        [Fact]
        public void GetNormalized_ReturnsSelf()
        {
            var element = new JsonPathExpressionElement("@.length-1");

            var actual = element.GetNormalized();

            actual.Should().Be(element);
        }

        [Theory]
        [InlineData("@.length-1", "@.length-1", true)]
        [InlineData("@.length-1", "@.length-2", false)]
        public void Matches_Expression(string expression, string otherExpression, bool expected)
        {
            var element = new JsonPathExpressionElement(expression);
            var other = new JsonPathExpressionElement(otherExpression);

            bool? actual = element.Matches(other);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("@.length-1", 1, null)]
        [InlineData("@.length-1", 2, false)]
        public void Matches_ArrayIndexList(string expression, int indexCount, bool? expected)
        {
            var element = new JsonPathExpressionElement(expression);
            var other = new JsonPathArrayIndexListElement(Enumerable.Range(0, indexCount).ToList());

            bool? actual = element.Matches(other);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("@.length-1", 0, 1, 1, null)]
        [InlineData("@.length-1", 0, 1, 2, null)]
        [InlineData("@.length-1", 100, 101, 1, null)]
        [InlineData("@.length-1", null, 1, 1, null)]
        [InlineData("@.length-1", 10, 9, -1, null)]
        [InlineData("@.length-1", -1, null, 1, null)]
        [InlineData("@.length-1", 0, 2, 1, false)]
        [InlineData("@.length-1", -2, null, 1, false)]
        public void Matches_ArraySlice(string expression, int? start, int? end, int step, bool? expected)
        {
            var element = new JsonPathExpressionElement(expression);
            var other = new JsonPathArraySliceElement(start, end, step);

            bool? actual = element.Matches(other);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(JsonPathElementType.Root, false)]
        [InlineData(JsonPathElementType.RecursiveDescent, false)]
        [InlineData(JsonPathElementType.Property, false)]
        [InlineData(JsonPathElementType.AnyProperty, false)]
        [InlineData(JsonPathElementType.PropertyList, false)]
        [InlineData(JsonPathElementType.ArrayIndex, null)]
        [InlineData(JsonPathElementType.AnyArrayIndex, false)]
        [InlineData(JsonPathElementType.FilterExpression, false)]
        public void Matches_Any(JsonPathElementType type, bool? expected)
        {
            var element = new JsonPathExpressionElement("@.length-1");
            var other = ElementCreator.CreateAny(type);

            bool? actual = element.Matches(other);

            actual.Should().Be(expected);
        }
    }
}