namespace JsonPathExpressions.Tests
{
    using System;
    using FluentAssertions;
    using JsonPathExpressions.Elements;
    using Xunit;

    public class AbsoluteJsonPathExpressionTests
    {
        [Fact]
        public void Constructor_AbsolutePath_Succeeds()
        {
            var elements = new JsonPathElement[] {new JsonPathRootElement(), new JsonPathPropertyElement("a")};

            var actual = new AbsoluteJsonPathExpression(elements);

            actual.Elements.Should().BeEquivalentTo(elements);
        }

        [Fact]
        public void Constructor_RelativePath_Throws()
        {
            var elements = new JsonPathElement[] { new JsonPathPropertyElement("a") };

            Action action = () => new AbsoluteJsonPathExpression(elements);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_ReturnsObjectOfTheSameType()
        {
            var path = new AbsoluteJsonPathExpression(new JsonPathElement[]
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
            actual.GetType().Should().Be<AbsoluteJsonPathExpression>();
        }
    }
}