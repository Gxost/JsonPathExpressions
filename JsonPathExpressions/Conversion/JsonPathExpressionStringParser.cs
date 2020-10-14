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

namespace JsonPathExpressions.Conversion
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Elements;
    using Utils;

    internal static class JsonPathExpressionStringParser
    {
        private static readonly char[] StopCharsOutsideBrackets = {'.', '['};
        private static readonly char[] StopCharsInsideBrackets = {'\'', ']'};
        private static readonly char[] ForbiddenCharactersOutsideBrackets = {'[', ']'};

        public static List<JsonPathElement> Parse(string jsonPath)
        {
            if (string.IsNullOrEmpty(jsonPath))
                throw new ArgumentNullException(nameof(jsonPath));

            var elements = new List<JsonPathElement>();

            int nextIndex = 0;
            bool isRecursiveDescentApplied = false;
            while (nextIndex < jsonPath.Length)
            {
                int index = jsonPath.IndexOfAny(StopCharsOutsideBrackets, nextIndex);
                if (index == -1)
                {
                    elements.Add(ParseToken(jsonPath.Substring(nextIndex), nextIndex == 0, isRecursiveDescentApplied));
                    isRecursiveDescentApplied = false;
                    break;
                }

                if (jsonPath.EndsWith(index))
                    throw new ArgumentException($"Expression ends with '{jsonPath[index]}'", nameof(jsonPath));

                if (index > nextIndex)
                {
                    elements.Add(ParseToken(jsonPath.Substring(nextIndex, index - nextIndex), nextIndex == 0, isRecursiveDescentApplied));
                    isRecursiveDescentApplied = false;
                }

                switch (jsonPath[index])
                {
                    case '.':
                        if (isRecursiveDescentApplied)
                            throw new ArgumentException("Dot must not follow recursive descent", nameof(jsonPath));

                        if (jsonPath.ContainsAt('.', index + 1))
                        {
                            isRecursiveDescentApplied = true;
                            nextIndex = index + 2;
                        }
                        else
                        {
                            nextIndex = index + 1;
                        }

                        if (nextIndex >= jsonPath.Length)
                            throw new ArgumentException("Path ends with dot", nameof(jsonPath));
                        break;
                    case '[':
                        int closingIndex = GetClosingBracketIndex(jsonPath, index + 1);
                        if (closingIndex == -1)
                            throw new ArgumentException("No matching closing bracket found", nameof(jsonPath));
                        if (closingIndex - index == 1)
                            throw new ArgumentException("No content inside brackets", nameof(jsonPath));

                        elements.Add(ParseTokenInsideBrackets(jsonPath.Substring(index + 1, closingIndex - index - 1), isRecursiveDescentApplied));
                        isRecursiveDescentApplied = false;
                        nextIndex = closingIndex + 1;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (isRecursiveDescentApplied)
                throw new ArgumentException("Recursive descent must be applied", nameof(jsonPath));

            return elements;
        }

        private static JsonPathElement ParseToken(string token, bool isFirstToken, bool isRecursiveDescentApplied)
        {
            if (isFirstToken && isRecursiveDescentApplied)
                throw new ArgumentException("Recursive descent can not be applied to first token", nameof(isRecursiveDescentApplied));
            if (token.IndexOfAny(ForbiddenCharactersOutsideBrackets) != -1)
                throw new ArgumentException($"Forbidden characters found outside brackets: {string.Join(" ", ForbiddenCharactersOutsideBrackets)}", nameof(token));

            if (isFirstToken && token == "$")
                return new JsonPathRootElement();

            var element = token == "*"
                ? (JsonPathElement)new JsonPathAnyPropertyElement()
                : (JsonPathElement)new JsonPathPropertyElement(token);

            return isRecursiveDescentApplied
                ? new JsonPathRecursiveDescentElement(element)
                : element;
        }

        private static JsonPathElement ParseTokenInsideBrackets(string token, bool isRecursiveDescentApplied)
        {
            var element = ParseTokenInsideBrackets(token.Trim());
            return isRecursiveDescentApplied
                ? new JsonPathRecursiveDescentElement(element)
                : element;
        }

        private static JsonPathElement ParseTokenInsideBrackets(string token)
        {
            if (token == "*")
                return new JsonPathAnyArrayIndexElement();
            if (token.StartsWith("\'", StringComparison.Ordinal))
                return ParsePropertyNamesToken(token);
            if (token.StartsWith("(", StringComparison.Ordinal))
                return ParseExpressionToken(token);
            if (token.StartsWith("?(", StringComparison.Ordinal))
                return ParseFilterExpressionToken(token);
            if (token.IndexOf(':') != -1)
                return ParseArraySliceToken(token);
            if (token.IndexOf(',') != -1)
                return ParseArrayIndexListToken(token);

            return ParseArrayIndexToken(token);
        }

        private static JsonPathElement ParseArrayIndexToken(string token)
        {
            return new JsonPathArrayIndexElement(int.Parse(token));
        }

        private static JsonPathElement ParseArrayIndexListToken(string token)
        {
            string[] parts = token.Split(new[] { ',' }, StringSplitOptions.None);
            if (parts.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException("Double commas found", nameof(token));

            var indexes = parts
                .Select(x => int.Parse(x.Trim()))
                .ToList();

            return new JsonPathArrayIndexListElement(indexes);
        }

        private static JsonPathElement ParseArraySliceToken(string token)
        {
            string[] parts = token.Split(new[] { ':' }, StringSplitOptions.None);
            if (parts.Length == 0 || parts.Length > 3)
                throw new ArgumentException("Array slice contains insufficient number of arguments", nameof(token));

            int? start = parts[0] == ""
                ? default(int?)
                : int.Parse(parts[0]);
            int? end = parts.Length < 2 || parts[1] == ""
                ? default(int?)
                : int.Parse(parts[1]);
            int step = parts.Length < 3 || parts[2] == ""
                ? 1
                : int.Parse(parts[2]);

            return new JsonPathArraySliceElement(start, end, step);
        }

        private static JsonPathElement ParsePropertyNamesToken(string token)
        {
            if (!token.EndsWith("'", StringComparison.Ordinal))
                throw new ArgumentException("No closing single quote found", nameof(token));

            var parts = token
                .SplitQuoted(',', '\'')
                .Select(x => x.Trim())
                .ToList();

            if (parts.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException("Double commas found", nameof(token));

            var names = parts
                .Select(x => x.Trim('\''))
                .ToList();

            if (names.Any(x => x.Contains('\'')))
                throw new ArgumentException("Non-matching single quotes found", nameof(token));

            return names.Count == 1
                ? (JsonPathElement)new JsonPathPropertyElement(names[0])
                : (JsonPathElement)new JsonPathPropertyListElement(names);
        }

        private static JsonPathElement ParseExpressionToken(string token)
        {
            if (!token.EndsWith(")", StringComparison.Ordinal))
                throw new ArgumentException("No closing brace found", nameof(token));

            return new JsonPathExpressionElement(token.Substring(1, token.Length - 2));
        }

        private static JsonPathElement ParseFilterExpressionToken(string token)
        {
            if (!token.EndsWith(")", StringComparison.Ordinal))
                throw new ArgumentException("No closing brace found", nameof(token));

            return new JsonPathFilterExpressionElement(token.Substring(2, token.Length - 3));
        }

        private static int GetClosingBracketIndex(string expression, int startIndex)
        {
            bool isInsideLiteral = false;
            for (int index = startIndex; index < expression.Length; ++index)
            {
                index = expression.IndexOfAny(StopCharsInsideBrackets, index);
                if (index == -1)
                    break;

                switch (expression[index])
                {
                    case '\'':
                        isInsideLiteral = !isInsideLiteral;
                        break;
                    case ']':
                        if (!isInsideLiteral)
                            return index;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return -1;
        }

        private static bool ContainsAt(this string inString, char character, int atPosition)
        {
            return atPosition < inString.Length && inString[atPosition] == character;
        }
    }
}