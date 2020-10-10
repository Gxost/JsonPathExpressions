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

    public class JsonPathRecursiveDescentElementTests
    {
        [Fact]
        public void IsStrict_ReturnsFalse()
        {
            var element = new JsonPathRecursiveDescentElement(new JsonPathPropertyElement("name"));

            element.IsStrict.Should().BeFalse();
        }

        [Fact]
        public void IsNormalized_AppliedToNormalized_ReturnsTrue()
        {
            var element = new JsonPathRecursiveDescentElement(new JsonPathPropertyElement("name"));

            element.IsNormalized.Should().BeTrue();
        }

        [Fact]
        public void IsNormalized_AppliedToNotNormalized_ReturnsFalse()
        {
            var element = new JsonPathRecursiveDescentElement(new JsonPathArraySliceElement(null, null));

            element.IsNormalized.Should().BeFalse();
        }

        [Fact]
        public void GetNormalized_AppliedToNormalized_ReturnsSelf()
        {
            var element = new JsonPathRecursiveDescentElement(new JsonPathPropertyElement("name"));

            var actual = element.GetNormalized();

            actual.Should().Be(element);
        }

        [Fact]
        public void GetNormalized_AppliedToNotNormalizedArraySlice_ReturnsAppliedToAnyArrayIndex()
        {
            var element = new JsonPathRecursiveDescentElement(new JsonPathArraySliceElement(null, null));
            var expected = new JsonPathRecursiveDescentElement(new JsonPathAnyArrayIndexElement());

            var actual = element.GetNormalized();

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData("name", "name", true)]
        [InlineData("name", "other-name", false)]
        public void Matches_RecursiveDescent(string propertyName, string otherPropertyName, bool expected)
        {
            var element = new JsonPathRecursiveDescentElement(new JsonPathPropertyElement(propertyName));
            var other = new JsonPathRecursiveDescentElement(new JsonPathPropertyElement(otherPropertyName));

            bool? actual = element.Matches(other);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(JsonPathElementType.Root, null)]
        [InlineData(JsonPathElementType.RecursiveDescent, false)]
        [InlineData(JsonPathElementType.Property, null)]
        [InlineData(JsonPathElementType.AnyProperty, null)]
        [InlineData(JsonPathElementType.PropertyList, null)]
        [InlineData(JsonPathElementType.ArrayIndex, null)]
        [InlineData(JsonPathElementType.AnyArrayIndex, null)]
        [InlineData(JsonPathElementType.ArrayIndexList, null)]
        [InlineData(JsonPathElementType.ArraySlice, null)]
        [InlineData(JsonPathElementType.Expression, null)]
        [InlineData(JsonPathElementType.FilterExpression, null)]
        public void Matches_Any(JsonPathElementType type, bool? expected)
        {
            var element = new JsonPathRecursiveDescentElement(new JsonPathArrayIndexElement(0));
            var other = ElementCreator.CreateAny(type);

            bool? actual = element.Matches(other);

            actual.Should().Be(expected);
        }
    }
}