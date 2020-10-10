namespace JsonPathExpressions.Tests
{
    using FluentAssertions;
    using Xunit;

    public class JsonPathExpressionExtensionsTests
    {
        [Theory]
        [InlineData("$..a[7,7]", "$..a[7]")]
        [InlineData("$..a[7:8]", "$..a[7]")]
        [InlineData("$..a[:]", "$..a[*]")]
        [InlineData("$.a[0:42]", "$.a[:42]")]
        [InlineData("$.a[42:42]", "$.a[:0]")]
        [InlineData("..a['b','b']", "..a.b")]
        public void GetNormalized(string path, string expected)
        {
            var pathExpr = new JsonPathExpression(path);
            var expectedExpr = new JsonPathExpression(expected);

            var actual = pathExpr.GetNormalized();

            actual.Should().BeEquivalentTo(expectedExpr);
        }
    }
}