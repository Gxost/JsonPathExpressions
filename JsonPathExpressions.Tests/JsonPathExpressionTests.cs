namespace JsonPathExpressions.Tests
{
    using System;
    using FluentAssertions;
    using JsonPathExpressions.Elements;
    using Xunit;

    public class JsonPathExpressionTests
    {
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