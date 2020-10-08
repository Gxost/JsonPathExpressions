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

namespace JsonPathExpressions.Tests.Conversion
{
    using System.Collections.Generic;
    using FluentAssertions;
    using JsonPathExpressions.Conversion;
    using JsonPathExpressions.Elements;
    using Xunit;

    public class JsonPathExpressionStringBuilderTests
    {
        [Fact]
        public void Build_Root_Builds()
        {
            Build_Worker(new JsonPathRootElement(), false, "$");
        }

        [Theory]
        [InlineData("abc", false, "abc")]
        [InlineData("abc", true, "..abc")]
        [InlineData("$", false, "$")]
        [InlineData("$", true, "..$")]
        [InlineData("", false, "['']")]
        [InlineData("", true, "..['']")]
        [InlineData("*", false, "['*']")]
        [InlineData("*", true, "..['*']")]
        [InlineData("[", false, "['[']")]
        [InlineData("[", true, "..['[']")]
        [InlineData("]", false, "[']']")]
        [InlineData("]", true, "..[']']")]
        [InlineData(".", false, "['.']")]
        [InlineData(".", true, "..['.']")]
        public void Build_Property_Builds(string name, bool recursiveDescent, string expected)
        {
            Build_Worker(new JsonPathPropertyElement(name), recursiveDescent, expected);
        }

        [Theory]
        [InlineData(false, "*")]
        [InlineData(true, "..*")]
        public void Build_AnyProperty_Builds(bool recursiveDescent, string expected)
        {
            Build_Worker(new JsonPathAnyPropertyElement(), recursiveDescent, expected);
        }

        [Theory]
        [InlineData("['abc','def']", false, "abc", "def")]
        [InlineData("..['abc','def']", true, "abc", "def")]
        public void Build_PropertyList_Builds(string expected, bool recursiveDescent, params string[] names)
        {
            Build_Worker(new JsonPathPropertyListElement(names), recursiveDescent, expected);
        }

        [Theory]
        [InlineData(42, false, "[42]")]
        [InlineData(42, true, "..[42]")]
        public void Build_ArrayIndex_Builds(int index, bool recursiveDescent, string expected)
        {
            Build_Worker(new JsonPathArrayIndexElement(index), recursiveDescent, expected);
        }

        [Theory]
        [InlineData(false, "[*]")]
        [InlineData(true, "..[*]")]
        public void Build_AnyArrayIndex_Builds(bool recursiveDescent, string expected)
        {
            Build_Worker(new JsonPathAnyArrayIndexElement(), recursiveDescent, expected);
        }

        [Theory]
        [InlineData("[7,42]", false,7, 42)]
        [InlineData("..[7,42]", true, 7, 42)]
        public void Build_ArrayIndexList_Builds(string expected, bool recursiveDescent, params int[] indexes)
        {
            Build_Worker(new JsonPathArrayIndexListElement(indexes), recursiveDescent, expected);
        }

        [Theory]
        [InlineData(null, null, 1, false, "[:]")]
        [InlineData(null, null, 1, true, "..[:]")]
        [InlineData(null, null, 2, false, "[::2]")]
        [InlineData(null, null, 2, true, "..[::2]")]
        [InlineData(-1, null, 1, false, "[-1:]")]
        [InlineData(-1, null, 1, true, "..[-1:]")]
        [InlineData(-1, null, 2, false, "[-1::2]")]
        [InlineData(-1, null, 2, true, "..[-1::2]")]
        [InlineData(null, 7, 1, false, "[:7]")]
        [InlineData(null, 7, 1, true, "..[:7]")]
        [InlineData(null, 7, 2, false, "[:7:2]")]
        [InlineData(null, 7, 2, true, "..[:7:2]")]
        public void Build_ArraySlice_Builds(int? start, int? end, int step, bool recursiveDescent, string expected)
        {
            Build_Worker(new JsonPathArraySliceElement(start, end, step), recursiveDescent, expected);
        }

        [Theory]
        [InlineData("expr", false, "[(expr)]")]
        [InlineData("expr", true, "..[(expr)]")]
        public void Build_Expression_Builds(string expression, bool recursiveDescent, string expected)
        {
            Build_Worker(new JsonPathExpressionElement(expression), recursiveDescent, expected);
        }

        [Theory]
        [InlineData("expr", false, "[?(expr)]")]
        [InlineData("expr", true, "..[?(expr)]")]
        public void Build_FilterExpression_Builds(string expression, bool recursiveDescent, string expected)
        {
            Build_Worker(new JsonPathFilterExpressionElement(expression), recursiveDescent, expected);
        }

        private void Build_Worker(JsonPathElement element, bool recursiveDescent, string expected)
        {
            var elements = new List<JsonPathElement>
            {
                recursiveDescent
                    ? new JsonPathRecursiveDescentElement(element)
                    : element
            };

            string actual = JsonPathExpressionStringBuilder.Build(elements);

            actual.Should().Be(expected);
        }
    }
}