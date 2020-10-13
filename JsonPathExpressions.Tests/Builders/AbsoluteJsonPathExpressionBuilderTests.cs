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

namespace JsonPathExpressions.Tests.Builders
{
    using FluentAssertions;
    using JsonPathExpressions.Builders;
    using JsonPathExpressions.Elements;
    using Xunit;

    public class AbsoluteJsonPathExpressionBuilderTests
    {
        [Fact]
        public void Build_Builds()
        {
            var expected = new AbsoluteJsonPathExpression(new JsonPathElement[]
            {
                new JsonPathRootElement(),
                new JsonPathRecursiveDescentElement(new JsonPathPropertyElement("a")),
                new JsonPathAnyPropertyElement(),
                new JsonPathPropertyListElement(new [] {"b", "c"}),
                new JsonPathArrayIndexElement(42),
                new JsonPathAnyArrayIndexElement(),
                new JsonPathArrayIndexListElement(new [] {7, 42}),
                new JsonPathArraySliceElement(0, 42, 2),
                new JsonPathExpressionElement("@.length-1"),
                new JsonPathFilterExpressionElement("@.name = 'a'")
            });

            var actual = AbsoluteJsonPathExpressionBuilder.Create()
                .Root()
                .RecursiveDescentTo().Property("a")
                .AnyProperty()
                .Properties("b", "c")
                .ArrayIndex(42)
                .AnyArrayIndex()
                .ArrayIndexes(7, 42)
                .ArraySlice(0, 42, 2)
                .Expression("@.length-1")
                .FilterExpression("@.name = 'a'")
                .Build();

            actual.Should().BeEquivalentTo(expected);
        }
    }
}