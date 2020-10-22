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

    public class JsonPathExpressionTests
    {
        [Fact]
        public void IsAbsolute_StartsWithRootElement_ReturnsTrue()
        {
            var path = new JsonPathExpression(new JsonPathElement[]
            {
                new JsonPathRootElement(),
                new JsonPathPropertyElement("a"),
                new JsonPathArrayIndexElement(42)
            });

            path.IsAbsolute.Should().BeTrue();
        }

        [Fact]
        public void IsAbsolute_StartsWithNonRootElement_ReturnsFalse()
        {
            var path = new JsonPathExpression(new JsonPathElement[]
            {
                new JsonPathPropertyElement("a"),
                new JsonPathArrayIndexElement(42)
            });

            path.IsAbsolute.Should().BeFalse();
        }

        [Fact]
        public void IsStrict_StrictElements_ReturnsTrue()
        {
            var path = new JsonPathExpression(new JsonPathElement[]
            {
                new JsonPathRootElement(),
                new JsonPathPropertyElement("a"),
                new JsonPathArrayIndexElement(42)
            });

            path.IsStrict.Should().BeTrue();
        }

        [Fact]
        public void IsNormalized_NormalizedElements_ReturnsTrue()
        {
            var path = new JsonPathExpression(new JsonPathElement[]
            {
                new JsonPathRootElement(),
                new JsonPathPropertyElement("a"),
                new JsonPathArrayIndexElement(42)
            });

            path.IsNormalized.Should().BeTrue();
        }

        [Theory]
        [InlineData("$.a.*['b','c'][42][*][7,42][0:10]..[(@.length-1)][?(@.name = 'a')]", "$.a.*['b','c'][42][*][7,42][0:10]..[(@.length-1)][?(@.name = 'a')]", true)]
        [InlineData("[:]", "[::1]", true)]
        [InlineData("a", "b", false)]
        [InlineData("*", "b", false)]
        [InlineData("[7]", "[42]", false)]
        [InlineData("[*]", "[42]", false)]
        [InlineData("[7,42]", "[42]", false)]
        [InlineData("[7,42]", "[:]", false)]
        [InlineData("[0:10]", "[0:10:2]", false)]
        [InlineData("[0:10]", "[0:9]", false)]
        [InlineData("[:]", "[::2]", false)]
        [InlineData("[(@.length-1)]", "[(@.length-2)]", false)]
        [InlineData("[?(@.name = 'a')]", "[?(@.name = 'b')]", false)]
        [InlineData("$.a", "a", false)]
        [InlineData("$.a", "$..a", false)]
        public void Equals_ReturnsExpected(string first, string second, bool expected)
        {
            var firstPath = new JsonPathExpression(first);
            var secondPath = new JsonPathExpression(second);

            bool actual = firstPath.Equals(secondPath);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("a", "a", true)]
        [InlineData("a", "b", false)]
        [InlineData("a", null, false)]
        [InlineData(null, "a", false)]
        [InlineData(null, null, true)]
        public void EqualityOperator(string first, string second, bool expected)
        {
            var firstPath = first != null ? new JsonPathExpression(first) : null;
            var secondPath = second != null ? new JsonPathExpression(second) : null;

            bool actual = firstPath == secondPath;

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("a", "a", false)]
        [InlineData("a", "b", true)]
        [InlineData("a", null, true)]
        [InlineData(null, "a", true)]
        [InlineData(null, null, false)]
        public void InequalityOperator(string first, string second, bool expected)
        {
            var firstPath = first != null ? new JsonPathExpression(first) : null;
            var secondPath = second != null ? new JsonPathExpression(second) : null;

            bool actual = firstPath != secondPath;

            actual.Should().Be(expected);
        }

        [Fact]
        public void ExplicitOperator_Expression_ReturnsString()
        {
            var expression = JsonPathExpression.Builder.Root().Property("a").Build();
            string expected = "$.a";

            string actual = (string)expression;

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ExplicitOperator_NullExpression_ReturnsNull()
        {
            string actual = (string)(JsonPathExpression)null;

            actual.Should().BeNull();
        }

        [Fact]
        public void ExplicitOperator_String_ReturnsExpression()
        {
            var expected = JsonPathExpression.Builder.Root().Property("a").Build();

            var actual = (JsonPathExpression)"$.a";

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ExplicitOperator_NullString_ReturnsNull()
        {
            var actual = (JsonPathExpression)(string)null;

            actual.Should().BeNull();
        }

        [Fact]
        public void Create_ReturnsObjectOfTheSameType()
        {
            var path = new JsonPathExpression(new JsonPathElement[]
            {
                new JsonPathRootElement()
            });
            var elements = new JsonPathElement[]
            {
                new JsonPathRootElement(),
                new JsonPathPropertyElement("a"),
                new JsonPathArrayIndexElement(42)
            };

            var actual = path.Create(elements);

            actual.Elements.Should().BeEquivalentTo(elements);
            actual.GetType().Should().Be<JsonPathExpression>();
        }

        [Fact]
        public void Constructor_RootIsNotFirstElement_Throws()
        {
            var elements = new JsonPathElement[] { new JsonPathPropertyElement("a"), new JsonPathRootElement() };

            Action action = () => new JsonPathExpression(elements);

            action.Should().Throw<ArgumentException>();
        }
    }
}