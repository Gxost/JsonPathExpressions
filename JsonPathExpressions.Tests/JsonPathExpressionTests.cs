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