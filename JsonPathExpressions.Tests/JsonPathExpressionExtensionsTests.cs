namespace JsonPathExpressions.Tests
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using JsonPathExpressions.Conversion;
    using JsonPathExpressions.Elements;
    using Xunit;

    public class JsonPathExpressionExtensionsTests
    {
        [Theory]
        [InlineData("$.a.b", "$['a']", true)]
        [InlineData("$.a.b", "$['b']", false)]
        public void StartsWith(string path, string prefixPath, bool expected)
        {
            var pathExpr = new JsonPathExpression(path);
            var prefixPathExpr = new JsonPathExpression(prefixPath);

            bool actual = pathExpr.StartsWith(prefixPathExpr);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("$.a.b", "$['a']['b']", true)]
        [InlineData("$.a.b", "$['b']", false)]
        [InlineData("$.a.*.c[:]", "$.a.b.c[42]", true)]
        [InlineData("$..b", "$.a.b", true)]
        [InlineData("$..b[*]", "$.a.b.c.d.b[42]", true)]
        [InlineData("$..b[(@.length-1)]", "$.a.b.c.d.b[42]", null)]
        [InlineData("$..b[(@.length-1)]", "$.a.b.c.d.b", false)]
        public void Matches(string path, string other, bool? expected)
        {
            var pathExpr = new JsonPathExpression(path);
            var otherExpr = new JsonPathExpression(other);

            bool? actual = pathExpr.Matches(otherExpr);

            actual.Should().Be(expected);
        }

        [Fact]
        public void ToAbsolute_AbsoluteJsonPathExpression_ReturnsPassedExpression()
        {
            var path = new AbsoluteJsonPathExpression(new JsonPathElement[]
            {
                new JsonPathRootElement(), new JsonPathPropertyElement("a")
            });

            var actual = path.ToAbsolute();

            actual.Should().Be(path);
        }

        [Theory]
        [InlineData("$.a", "$.a")]
        [InlineData("a", "$.a")]
        public void ToAbsolute(string path, string expected)
        {
            var pathExpr = new JsonPathExpression(path);
            var expectedExpr = new AbsoluteJsonPathExpression(expected);

            var actual = pathExpr.ToAbsolute();

            actual.Should().BeEquivalentTo(expectedExpr);
        }

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

        [Theory]
        [InlineData("$.a.b", "$.a.c", "$.a")]
        [InlineData("$.a.b", "$.c", "$")]
        [InlineData("$.a.b", "$.a.b.c", "$.a.b")]
        [InlineData("$.a.b.c", "$.a.b", "$.a.b")]
        [InlineData("a.b", "c", null)]
        public void GetParentWith(string path, string other, string expected)
        {
            var pathExpr = new JsonPathExpression(path);
            var otherExpr = new JsonPathExpression(other);
            var expectedExpr = expected != null ? new JsonPathExpression(expected) : null;

            var actual = pathExpr.GetParentWith(otherExpr);

            actual.Should().BeEquivalentTo(expectedExpr);
        }

        [Theory]
        [InlineData("$.a", "$.a.b.c", "b.c")]
        [InlineData("$.a", "$.a", null)]
        [InlineData("$.a.b", "$.a", null)]
        public void GetRelativePathTo(string path, string child, string expected)
        {
            var pathExpr = new JsonPathExpression(path);
            var childExpr = new JsonPathExpression(child);
            var expectedExpr = expected != null ? new RelativeJsonPathExpression(expected) : null;

            var actual = pathExpr.GetRelativePathTo(childExpr);

            actual.Should().BeEquivalentTo(expectedExpr);
        }

        [Theory]
        [InlineData("$.a", "$.a")]
        [InlineData("$.a", "$.a.b", "b")]
        [InlineData("$.a", "$.a.b[42]", "b", "[42]")]
        public void Append_Elements(string path, string expected, params string[] elementsToAppend)
        {
            var pathExpr = new JsonPathExpression(path);
            var expectedExpr = new JsonPathExpression(expected);
            var elements = elementsToAppend
                .Select(x => JsonPathExpressionStringParser.Parse(x).First())
                .ToArray();

            var actual = pathExpr.Append(elements);

            actual.Should().BeEquivalentTo(expectedExpr);
        }

        [Theory]
        [InlineData("$.a", "b", "$.a.b")]
        [InlineData("$.a", "b[42]", "$.a.b[42]")]
        public void Append_RelativeJsonPathExpression(string path, string relativePath, string expected)
        {
            var pathExpr = new JsonPathExpression(path);
            var relativePathExpr = new RelativeJsonPathExpression(relativePath);
            var expectedExpr = new JsonPathExpression(expected);

            var actual = pathExpr.Append(relativePathExpr);

            actual.Should().BeEquivalentTo(expectedExpr);
        }

        [Fact]
        public void ReplaceLastWith_Replaces()
        {
            var path = new JsonPathExpression(new JsonPathElement[] {new JsonPathRootElement(), new JsonPathPropertyElement("a") });
            var element = new JsonPathPropertyElement("b");
            var expected = new JsonPathExpression(new JsonPathElement[] { new JsonPathRootElement(), element });

            var actual = path.ReplaceLastWith(element);

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ReplaceLastWith_RootElementInAbsolutePath_Throws()
        {
            var path = new AbsoluteJsonPathExpression(new JsonPathElement[] { new JsonPathRootElement() });

            Action action = () => path.ReplaceLastWith(new JsonPathPropertyElement("a"));

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ReplaceLastWith_WithRootElementInRelativePath_Throws()
        {
            var path = new RelativeJsonPathExpression(new JsonPathElement[] { new JsonPathPropertyElement("a") });

            Action action = () => path.ReplaceLastWith(new JsonPathRootElement());

            action.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData("$.a.b", 0, "$.a.b")]
        [InlineData("$.a.b", 1, "$.a")]
        [InlineData("$.a.b", 2, "$")]
        public void RemoveLast(string path, int count, string expected)
        {
            var pathExpr = new JsonPathExpression(path);
            var expectedExpr = new JsonPathExpression(expected);

            var actual = pathExpr.RemoveLast(count);

            actual.Should().BeEquivalentTo(expectedExpr);
        }
    }
}