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

namespace JsonPathExpressions.Tests
{
    using System;
    using FluentAssertions;
    using JsonPathExpressions.Elements;
    using Xunit;

    public class RelativeJsonPathExpressionTests
    {
        [Fact]
        public void Constructor_RelativePath_Succeeds()
        {
            var elements = new JsonPathElement[] { new JsonPathPropertyElement("a") };
            
            var actual = new RelativeJsonPathExpression(elements);

            actual.Elements.Should().BeEquivalentTo(elements);
        }

        [Fact]
        public void Constructor_AbsolutePath_Throws()
        {
            var elements = new JsonPathElement[] { new JsonPathRootElement(), new JsonPathPropertyElement("a") };

            Action action = () => new RelativeJsonPathExpression(elements);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Equals_EqualJsonPathExpression_ReturnsTrue()
        {
            var path = RelativeJsonPathExpression.Builder.Property("a").Build();
            var other = JsonPathExpression.Builder.Property("a").Build();

            bool actual = path.Equals(other);

            actual.Should().BeTrue();
        }

        [Fact]
        public void Equals_AbsoluteJsonPathExpression_ReturnsFalse()
        {
            var path = RelativeJsonPathExpression.Builder.Property("a").Build();
            var other = AbsoluteJsonPathExpression.Builder.Root().Property("a").Build();

            bool actual = path.Equals(other);

            actual.Should().BeFalse();
        }

        [Fact]
        public void ExplicitOperator_String_ReturnsExpression()
        {
            var expected = RelativeJsonPathExpression.Builder.Property("a").Build();

            var actual = (RelativeJsonPathExpression)"a";

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ExplicitOperator_Null_ReturnsNull()
        {
            var actual = (RelativeJsonPathExpression)(string)null;

            actual.Should().BeNull();
        }

        [Fact]
        public void Create_ReturnsObjectOfTheSameType()
        {
            var path = new RelativeJsonPathExpression(new JsonPathElement[]
            {
                new JsonPathPropertyElement("a")
            });
            var elements = new JsonPathElement[]
            {
                new JsonPathPropertyElement("a"),
                new JsonPathArrayIndexElement(42)
            };

            var actual = path.Create(elements);

            actual.Elements.Should().BeEquivalentTo(elements);
            actual.GetType().Should().Be<RelativeJsonPathExpression>();
        }
    }
}