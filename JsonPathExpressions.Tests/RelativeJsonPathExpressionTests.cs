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
    }
}