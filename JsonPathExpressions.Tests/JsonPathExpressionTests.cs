namespace JsonPathExpressions.Tests
{
    using System;
    using FluentAssertions;
    using JsonPathExpressions.Elements;
    using Xunit;

    public class JsonPathExpressionTests
    {
        [Fact]
        public void Constructor_RootIsNotFirstElement_Throws()
        {
            var elements = new JsonPathElement[] { new JsonPathPropertyElement("a"), new JsonPathRootElement() };

            Action action = () => new JsonPathExpression(elements);

            action.Should().Throw<ArgumentException>();
        }
    }
}