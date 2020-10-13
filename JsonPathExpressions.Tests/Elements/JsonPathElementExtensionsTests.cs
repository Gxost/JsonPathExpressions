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
    using System;
    using FluentAssertions;
    using Helpers;
    using JsonPathExpressions.Elements;
    using Xunit;

    public class JsonPathElementExtensionsTests
    {
        [Theory]
        [InlineData(JsonPathElementType.ArrayIndex, true, JsonPathElementType.ArrayIndex, true)]
        [InlineData(JsonPathElementType.ArrayIndex, false, JsonPathElementType.ArrayIndex, true)]
        [InlineData(JsonPathElementType.ArrayIndex, true, JsonPathElementType.AnyArrayIndex, false)]
        [InlineData(JsonPathElementType.ArrayIndex, false, JsonPathElementType.AnyArrayIndex, false)]
        public void IsOfType_SingleType(JsonPathElementType elementType, bool isRecursiveDescent, JsonPathElementType typeArg, bool expected)
        {
            var element = isRecursiveDescent
                ? new JsonPathRecursiveDescentElement(ElementCreator.CreateAny(elementType))
                : ElementCreator.CreateAny(elementType);

            bool actual = element.IsOfType(typeArg);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(true, JsonPathElementType.ArrayIndex, true, JsonPathElementType.ArrayIndex, JsonPathElementType.AnyArrayIndex)]
        [InlineData(true, JsonPathElementType.ArrayIndex, false, JsonPathElementType.ArrayIndex, JsonPathElementType.AnyArrayIndex)]
        [InlineData(true, JsonPathElementType.AnyArrayIndex, true, JsonPathElementType.ArrayIndex, JsonPathElementType.AnyArrayIndex)]
        [InlineData(true, JsonPathElementType.AnyArrayIndex, false, JsonPathElementType.ArrayIndex, JsonPathElementType.AnyArrayIndex)]
        [InlineData(false, JsonPathElementType.Property, true, JsonPathElementType.ArrayIndex, JsonPathElementType.AnyArrayIndex)]
        [InlineData(false, JsonPathElementType.Property, false, JsonPathElementType.ArrayIndex, JsonPathElementType.AnyArrayIndex)]
        public void IsOfType_MultipleTypes(bool expected, JsonPathElementType elementType, bool isRecursiveDescent, params JsonPathElementType[] typeArgs)
        {
            var element = isRecursiveDescent
                ? new JsonPathRecursiveDescentElement(ElementCreator.CreateAny(elementType))
                : ElementCreator.CreateAny(elementType);

            bool actual = element.IsOfType(typeArgs);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(true, JsonPathElementType.ArrayIndex, true, JsonPathElementType.ArrayIndex, JsonPathElementType.AnyArrayIndex)]
        [InlineData(true, JsonPathElementType.ArrayIndex, false, JsonPathElementType.ArrayIndex, JsonPathElementType.AnyArrayIndex)]
        [InlineData(true, JsonPathElementType.AnyArrayIndex, true, JsonPathElementType.ArrayIndex, JsonPathElementType.AnyArrayIndex)]
        [InlineData(true, JsonPathElementType.AnyArrayIndex, false, JsonPathElementType.ArrayIndex, JsonPathElementType.AnyArrayIndex)]
        [InlineData(false, JsonPathElementType.Property, true, JsonPathElementType.ArrayIndex, JsonPathElementType.AnyArrayIndex)]
        [InlineData(false, JsonPathElementType.Property, false, JsonPathElementType.ArrayIndex, JsonPathElementType.AnyArrayIndex)]
        public void IsOfTypeInRange(bool expected, JsonPathElementType elementType, bool isRecursiveDescent, JsonPathElementType firstType, JsonPathElementType lastType)
        {
            var element = isRecursiveDescent
                ? new JsonPathRecursiveDescentElement(ElementCreator.CreateAny(elementType))
                : ElementCreator.CreateAny(elementType);

            bool actual = element.IsOfTypeInRange(firstType, lastType);

            actual.Should().Be(expected);
        }

        [Fact]
        public void GetUnderlyingElement_Property_ReturnsProperty()
        {
            var element = ElementCreator.CreateAny(JsonPathElementType.Property);

            var actual = element.GetUnderlyingElement();

            actual.Should().Be(element);
        }
        [Fact]
        public void GetUnderlyingElement_RecursiveDescentAppliedToProperty_ReturnsProperty()
        {
            var property = ElementCreator.CreateAny(JsonPathElementType.Property);
            JsonPathElement element = new JsonPathRecursiveDescentElement(property);

            var actual = element.GetUnderlyingElement();

            actual.Should().Be(property);
        }

        [Fact]
        public void CastTo_PropertyToProperty_Casts()
        {
            var element = ElementCreator.CreateAny(JsonPathElementType.Property);

            var actual = element.CastTo<JsonPathPropertyElement>();

            actual.Should().Be(element);
        }

        [Fact]
        public void CastTo_RecursiveDescentAppliedToPropertyToProperty_Casts()
        {
            var property = ElementCreator.CreateAny(JsonPathElementType.Property);
            JsonPathElement element = new JsonPathRecursiveDescentElement(property);

            var actual = element.CastTo<JsonPathPropertyElement>();

            actual.Should().Be(property);
        }

        [Fact]
        public void CastTo_PropertyToArrayIndex_Throws()
        {
            var element = ElementCreator.CreateAny(JsonPathElementType.Property);

            Action action = () => element.CastTo<JsonPathArrayIndexElement>();

            action.Should().Throw<InvalidCastException>();
        }

        [Fact]
        public void CastTo_RecursiveDescentAppliedToPropertyToArrayIndex_Throws()
        {
            JsonPathElement element = new JsonPathRecursiveDescentElement(ElementCreator.CreateAny(JsonPathElementType.Property));

            Action action = () => element.CastTo<JsonPathArrayIndexElement>();

            action.Should().Throw<InvalidCastException>();
        }

        [Fact]
        public void As_PropertyToProperty_Casts()
        {
            var element = ElementCreator.CreateAny(JsonPathElementType.Property);

            var actual = element.As<JsonPathPropertyElement>();

            actual.Should().Be(element);
        }

        [Fact]
        public void As_RecursiveDescentAppliedToPropertyToProperty_Casts()
        {
            var property = ElementCreator.CreateAny(JsonPathElementType.Property);
            JsonPathElement element = new JsonPathRecursiveDescentElement(property);

            var actual = element.As<JsonPathPropertyElement>();

            actual.Should().Be(property);
        }

        [Fact]
        public void As_PropertyToArrayIndex_ReturnsNull()
        {
            var element = ElementCreator.CreateAny(JsonPathElementType.Property);

            var actual = element.As<JsonPathArrayIndexElement>();

            actual.Should().BeNull();
        }

        [Fact]
        public void As_RecursiveDescentAppliedToPropertyToArrayIndex_ReturnsNull()
        {
            JsonPathElement element = new JsonPathRecursiveDescentElement(ElementCreator.CreateAny(JsonPathElementType.Property));

            var actual = element.As<JsonPathArrayIndexElement>();

            actual.Should().BeNull();
        }
    }
}