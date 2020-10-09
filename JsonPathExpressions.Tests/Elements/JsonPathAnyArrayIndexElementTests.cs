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

    public class JsonPathAnyArrayIndexElementTests
    {
        [Fact]
        public void IsStrict_ReturnsFalse()
        {
            var element = new JsonPathAnyArrayIndexElement();

            element.IsStrict.Should().BeFalse();
        }

        [Theory]
        [InlineData(JsonPathElementType.Root, false)]
        [InlineData(JsonPathElementType.RecursiveDescent, false)]
        [InlineData(JsonPathElementType.Property, false)]
        [InlineData(JsonPathElementType.AnyProperty, false)]
        [InlineData(JsonPathElementType.PropertyList, false)]
        [InlineData(JsonPathElementType.ArrayIndex, true)]
        [InlineData(JsonPathElementType.AnyArrayIndex, true)]
        [InlineData(JsonPathElementType.ArrayIndexList, true)]
        [InlineData(JsonPathElementType.ArraySlice, true)]
        [InlineData(JsonPathElementType.Expression, true)]
        [InlineData(JsonPathElementType.FilterExpression, false)]
        public void Matches(JsonPathElementType type, bool? expected)
        {
            var element = new JsonPathAnyArrayIndexElement();
            var other = ElementCreator.CreateAny(type);

            bool? actual = element.Matches(other);

            actual.Should().Be(expected);
        }
    }
}