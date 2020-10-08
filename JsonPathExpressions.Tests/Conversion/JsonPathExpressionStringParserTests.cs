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
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using JsonPathExpressions.Conversion;
    using JsonPathExpressions.Elements;
    using Xunit;

    public class JsonPathExpressionStringParserTests
    {
        [Fact]
        public void Parse_Root_Parses()
        {
            Parse_Worker("$", false, new JsonPathRootElement());
        }

        [Theory]
        [InlineData("..$", true, "$")]
        [InlineData("name", false, "name")]
        [InlineData("..name", true, "name")]
        [InlineData(" name ", false, " name ")]
        [InlineData(".. name ", true, " name ")]
        [InlineData("['']", false, "")]
        [InlineData("..['']", true, "")]
        [InlineData("['[']", false, "[")]
        [InlineData("..['[']", true, "[")]
        [InlineData("[']']", false, "]")]
        [InlineData("..[']']", true, "]")]
        [InlineData("['name']", false, "name")]
        [InlineData("..['name']", true, "name")]
        [InlineData("[' name ']", false, " name ")]
        [InlineData("..[' name ']", true, " name ")]
        [InlineData("['name.with.dots']", false, "name.with.dots")]
        [InlineData("..['name.with.dots']", true, "name.with.dots")]
        [InlineData("['name,with,commas']", false, "name,with,commas")]
        [InlineData("..['name,with,commas']", true, "name,with,commas")]
        public void Parse_Property_Parses(string expression, bool expectedRecursiveDescent, string expectedName)
        {
            Parse_Worker(expression, expectedRecursiveDescent, new JsonPathPropertyElement(expectedName));
        }

        [Theory]
        [InlineData("*", false)]
        [InlineData("..*", true)]
        public void Parse_AnyProperty_Parses(string expression, bool expectedRecursiveDescent)
        {
            Parse_Worker(expression, expectedRecursiveDescent, new JsonPathAnyPropertyElement());
        }

        [Theory]
        [InlineData("['', 'name']", false, "", "name")]
        [InlineData("..['', 'name']", true, "", "name")]
        [InlineData("['name1', 'name2']", false, "name1", "name2")]
        [InlineData("..['name1', 'name2']", true, "name1", "name2")]
        [InlineData("[' name1 ', ' name2 ']", false, " name1 ", " name2 ")]
        [InlineData("..[' name1 ', ' name2 ']", true, " name1 ", " name2 ")]
        [InlineData("['name.with.dots1', 'name.with.dots2']", false, "name.with.dots1", "name.with.dots2")]
        [InlineData("..['name.with.dots1', 'name.with.dots2']", true, "name.with.dots1", "name.with.dots2")]
        [InlineData("['name,with,commas1', 'name,with,commas2']", false, "name,with,commas1", "name,with,commas2")]
        [InlineData("..['name,with,commas1', 'name,with,commas2']", true, "name,with,commas1", "name,with,commas2")]
        public void Parse_PropertyList_Parses(string expression, bool expectedRecursiveDescent, params string[] expectedNames)
        {
            Parse_Worker(expression, expectedRecursiveDescent, new JsonPathPropertyListElement(expectedNames));
        }

        [Theory]
        [InlineData("[42]", false, 42)]
        [InlineData("..[42]", true, 42)]
        public void Parse_ArrayIndex_Parses(string expression, bool expectedRecursiveDescent, int expectedIndex)
        {
            Parse_Worker(expression, expectedRecursiveDescent, new JsonPathArrayIndexElement(expectedIndex));
        }

        [Theory]
        [InlineData("[*]", false)]
        [InlineData("..[*]", true)]
        public void Parse_AnyArrayIndex_Parses(string expression, bool expectedRecursiveDescent)
        {
            Parse_Worker(expression, expectedRecursiveDescent, new JsonPathAnyArrayIndexElement());
        }

        [Theory]
        [InlineData("[42, 777]", false, 42, 777)]
        [InlineData("..[42, 777]", true, 42, 777)]
        public void Parse_ArrayIndexList_Parses(string expression, bool expectedRecursiveDescent, params int[] expectedIndexes)
        {
            Parse_Worker(expression, expectedRecursiveDescent, new JsonPathArrayIndexListElement(expectedIndexes));
        }

        [Theory]
        [InlineData("[:]", false, null, null, 1)]
        [InlineData("..[:]", true, null, null, 1)]
        [InlineData("[-1:]", false, -1, null, 1)]
        [InlineData("..[-1:]", true, -1, null, 1)]
        [InlineData("[:2]", false, null, 2, 1)]
        [InlineData("..[:2]", true, null, 2, 1)]
        [InlineData("[::5]", false, null, null, 5)]
        [InlineData("..[::5]", true, null, null, 5)]
        [InlineData("[0:10]", false, 0, 10, 1)]
        [InlineData("..[0:10]", true, 0, 10, 1)]
        [InlineData("[0:10:5]", false, 0, 10, 5)]
        [InlineData("..[0:10:5]", true, 0, 10, 5)]
        [InlineData("[10:0:-5]", false, 10, 0, -5)]
        [InlineData("..[10:0:-5]", true, 10, 0, -5)]
        public void Parse_ArraySlice_Parses(string expression, bool expectedRecursiveDescent, int? expectedStart, int? expectedEnd, int expectedStep)
        {
            Parse_Worker(expression, expectedRecursiveDescent, new JsonPathArraySliceElement(expectedStart, expectedEnd, expectedStep));
        }

        [Theory]
        [InlineData("[(expr)]", false, "expr")]
        [InlineData("..[(expr)]", true, "expr")]
        public void Parse_Expression_Parses(string expression, bool expectedRecursiveDescent, string expectedExpression)
        {
            Parse_Worker(expression, expectedRecursiveDescent, new JsonPathExpressionElement(expectedExpression));
        }

        [Theory]
        [InlineData("[?(expr)]", false, "expr")]
        [InlineData("..[?(expr)]", true, "expr")]
        public void Parse_FilterExpression_Parses(string expression, bool expectedRecursiveDescent, string expectedExpression)
        {
            Parse_Worker(expression, expectedRecursiveDescent, new JsonPathFilterExpressionElement(expectedExpression));
        }

        [Fact]
        public void Parse_Properties_Parses()
        {
            string expression = "$.first.second..third['fourth'].['fifth'].$.*";
            var expected = new List<JsonPathElement>
            {
                new JsonPathRootElement(),
                new JsonPathPropertyElement("first"),
                new JsonPathPropertyElement("second"),
                new JsonPathRecursiveDescentElement(new JsonPathPropertyElement("third")),
                new JsonPathPropertyElement("fourth"),
                new JsonPathPropertyElement("fifth"),
                new JsonPathPropertyElement("$"),
                new JsonPathAnyPropertyElement()
            };

            var actual = JsonPathExpressionStringParser.Parse(expression);

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Parse_Indexes_Parses()
        {
            string expression = "$[0][1]..[2].[3][*]";
            var expected = new List<JsonPathElement>
            {
                new JsonPathRootElement(),
                new JsonPathArrayIndexElement(0),
                new JsonPathArrayIndexElement(1),
                new JsonPathRecursiveDescentElement(new JsonPathArrayIndexElement(2)),
                new JsonPathArrayIndexElement(3),
                new JsonPathAnyArrayIndexElement()
            };

            var actual = JsonPathExpressionStringParser.Parse(expression);

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData(".")]
        [InlineData("[")]
        [InlineData("]")]
        [InlineData("...name")]
        [InlineData("[]")]
        [InlineData("[,]")]
        [InlineData("[']")]
        [InlineData("['',']")]
        [InlineData("['abc',def]")]
        [InlineData("['abc'a,b'def']")]
        [InlineData("['',,'']")]
        [InlineData("[1,,2]")]
        [InlineData("[:::]")]
        public void Parse_ThrowsArgumentException(string expression)
        {
            Action action = () => JsonPathExpressionStringParser.Parse(expression);

            action.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData("[abc]")]
        [InlineData("[abc,def]")]
        [InlineData("[abc:def]")]
        public void Parse_ThrowsFormatException(string expression)
        {
            Action action = () => JsonPathExpressionStringParser.Parse(expression);

            action.Should().Throw<FormatException>();
        }

        private void Parse_Worker(string expression, bool expectedRecursiveDescent, JsonPathElement expectedElement)
        {
            var expected = expectedRecursiveDescent
                ? new JsonPathRecursiveDescentElement(expectedElement)
                : expectedElement;

            var actual = JsonPathExpressionStringParser.Parse(expression);

            actual.Should().BeEquivalentTo(expected);
        }
    }
}