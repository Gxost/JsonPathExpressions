﻿#region License
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

    public class JsonPathPropertyElementTests
    {
        [Fact]
        public void IsStrict_ReturnsTrue()
        {
            var element = new JsonPathPropertyElement("name");

            element.IsStrict.Should().BeTrue();
        }

        [Fact]
        public void IsNormalized_ReturnsTrue()
        {
            var element = new JsonPathPropertyElement("name");

            element.IsNormalized.Should().BeTrue();
        }

        [Fact]
        public void GetNormalized_ReturnsSelf()
        {
            var element = new JsonPathPropertyElement("name");

            var actual = element.GetNormalized();

            actual.Should().Be(element);
        }

        [Fact]
        public void Matches_KnownPropertyName_ReturnsTrue()
        {
            var element = new JsonPathPropertyElement("name");
            var other = new JsonPathPropertyElement("name");

            bool? actual = element.Matches(other);

            actual.Should().BeTrue();
        }

        [Fact]
        public void Matches_KnownPropertyNameList_ReturnsTrue()
        {
            var element = new JsonPathPropertyElement("name");
            var other = new JsonPathPropertyListElement(new[] { "name" });

            bool? actual = element.Matches(other);

            actual.Should().BeTrue();
        }

        [Fact]
        public void Matches_UnknownPropertyNameList_ReturnsFalse()
        {
            var element = new JsonPathPropertyElement("name");
            var other = new JsonPathPropertyListElement(new[] { "name", "unknown" });

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
            var element = new JsonPathPropertyElement("name");
            var other = ElementCreator.CreateAny(type);

            bool? actual = element.Matches(other);

            actual.Should().BeFalse();
        }
    }
}