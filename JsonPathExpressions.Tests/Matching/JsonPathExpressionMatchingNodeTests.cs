namespace JsonPathExpressions.Tests.Matching
{
    using System.Collections.Generic;
    using FluentAssertions;
    using JsonPathExpressions.Matching;
    using Xunit;

    public class JsonPathExpressionMatchingNodeTests
    {
        [Theory]
        [InlineData("$.a.*.c[*]", "$.a.b.c[42]", true)]
        [InlineData("$.a.*.c[:42]", "$.a.b.c[42]", false)]
        [InlineData("$.a..[42]", "$.a.b.c[42]", true)]
        [InlineData("$.a..[:42]", "$.a.b.c[42]", false)]
        [InlineData("$.a.*.c[(@.length-1)]", "$.a.b.c[42]", null)]
        [InlineData("$.a..[(@.length-1)]", "$.a.b.c[42]", null)]
        public void Matches(string pathInSet, string path, bool? expected)
        {
            var matchingSet = new JsonPathExpressionMatchingNode<JsonPathExpression>(0);
            matchingSet.Add(new JsonPathExpression(pathInSet));

            bool? actual = matchingSet.Matches(new JsonPathExpression(path));

            actual.Should().Be(expected);
        }

        [Fact]
        public void Matches_ReturnsMatched()
        {
            var matchingSet = new JsonPathExpressionMatchingNode<JsonPathExpression>(0);
            matchingSet.Add(new JsonPathExpression("$.a.*.c[*]"));
            matchingSet.Add(new JsonPathExpression("$.*.b.c[:]"));
            matchingSet.Add(new JsonPathExpression("$.*.b.c[7]"));
            matchingSet.Add(new JsonPathExpression("$['c','d'].b.c[*]"));
            var expected = new List<JsonPathExpression>
            {
                new JsonPathExpression("$.a.*.c[*]"),
                new JsonPathExpression("$.*.b.c[:]")
            };

            var actual = new List<JsonPathExpression>();
            bool matched = matchingSet.Matches(new JsonPathExpression("$.a.b.c[42]"), actual);

            matched.Should().BeTrue();
            actual.Should().BeEquivalentTo(expected);
        }
    }
}