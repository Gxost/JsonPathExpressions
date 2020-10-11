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

namespace JsonPathExpressions.Tests.Matching
{
    using System.Collections.Generic;
    using FluentAssertions;
    using JsonPathExpressions.Matching;
    using Xunit;

    public class JsonPathExpressionMatchingSetTests
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
            var matchingSet = new JsonPathExpressionMatchingSet();
            matchingSet.Add(new JsonPathExpression(pathInSet));

            bool? actual = matchingSet.Matches(new JsonPathExpression(path));

            actual.Should().Be(expected);
        }

        [Fact]
        public void Matches_ReturnsMatched()
        {
            var matchingSet = new JsonPathExpressionMatchingSet();
            matchingSet.Add(new JsonPathExpression("$.a.*.c[*]"));
            matchingSet.Add(new JsonPathExpression("$.*.b.c[:]"));
            matchingSet.Add(new JsonPathExpression("$.*.b.c[7]"));
            matchingSet.Add(new JsonPathExpression("$['c','d'].b.c[*]"));
            var expected = new List<JsonPathExpression>
            {
                new JsonPathExpression("$.a.*.c[*]"),
                new JsonPathExpression("$.*.b.c[:]")
            };

            bool matched = matchingSet.Matches(new JsonPathExpression("$.a.b.c[42]"), out var actual);

            matched.Should().BeTrue();
            actual.Should().BeEquivalentTo(expected);
        }
    }
}