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

    public class JsonPathPropertyListElementTests
    {
        [Theory]
        [InlineData(true, "a")]
        [InlineData(false, "a", "b")]
        public void IsStrict(bool expected, params string[] names)
        {
            var element = new JsonPathPropertyListElement(names);

            element.IsStrict.Should().Be(expected);
        }

        [Theory]
        [InlineData(false, "a")]
        [InlineData(true, "a", "b")]
        public void IsNormalized(bool expected, params string[] names)
        {
            var element = new JsonPathPropertyListElement(names);

            element.IsNormalized.Should().Be(expected);
        }

        [Fact]
        public void Matches_KnownPropertyName_ReturnsTrue()
        {
            var element = new JsonPathPropertyListElement(new[] { "name0", "name1", "name2" });
            var other = new JsonPathPropertyElement("name0");

            bool? actual = element.Matches(other);

            actual.Should().BeTrue();
        }

        [Fact]
        public void Matches_KnownPropertyNamesList_ReturnsTrue()
        {
            var element = new JsonPathPropertyListElement(new[] { "name0", "name1", "name2" });
            var other = new JsonPathPropertyListElement(new[] { "name0", "name1" });

            bool? actual = element.Matches(other);

            actual.Should().BeTrue();
        }

        [Fact]
        public void Matches_UnknownPropertyNamesList_ReturnsFalse()
        {
            var element = new JsonPathPropertyListElement(new[] { "name0", "name1", "name2" });
            var other = new JsonPathPropertyListElement(new[] { "name0", "name1", "name2", "name3" });

            bool? actual = element.Matches(other);

            actual.Should().BeFalse();
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
            var element = new JsonPathPropertyListElement(new[] { "name0", "name1" });
            var other = ElementCreator.CreateAny(type);

            bool? actual = element.Matches(other);

            actual.Should().BeFalse();
        }
    }
}