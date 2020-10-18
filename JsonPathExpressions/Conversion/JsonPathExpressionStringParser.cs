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
        private static readonly char[] Comma = {','};
        private static readonly char[] Colon = {':'};
        private static readonly char[] DotAndSquareBracket = {'.', '['};
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
                bool isFirstToken = nextIndex == 0;
                int index = jsonPath.IndexOfAny(DotAndSquareBracket, nextIndex);
                if (index == -1)
                {
                    elements.Add(ParseToken(jsonPath.Substring(nextIndex), isFirstToken, isRecursiveDescentApplied));
                    isRecursiveDescentApplied = false;
                    break;
                }

                if (jsonPath.EndsWith(index))
                    throw new JsonPathExpressionParsingException($"Expression ends with '{jsonPath[index]}'");

                if (index > nextIndex)
                {
                    elements.Add(ParseToken(jsonPath.Substring(nextIndex, index - nextIndex), isFirstToken, isRecursiveDescentApplied));
                    isRecursiveDescentApplied = false;
                }

                switch (jsonPath[index])
                {
                    case '.':
                        nextIndex = HandleDot(jsonPath, index, ref isRecursiveDescentApplied);
                        break;
                    case '[':
                        nextIndex = HandleSquareBracket(jsonPath, index, ref isRecursiveDescentApplied, elements);
                        break;
                    default:
                        throw new InvalidOperationException($"This should not happen: unknown character found: {jsonPath[index]}");
                }
            }

            if (isRecursiveDescentApplied)
                throw new JsonPathExpressionParsingException("Recursive descent must be followed by another expression element");

            return elements;
        }

        private static int HandleDot(string jsonPath, int index, ref bool isRecursiveDescentApplied)
        {
            if (isRecursiveDescentApplied)
                throw new JsonPathExpressionParsingException("Dot must not follow recursive descent");

            isRecursiveDescentApplied = jsonPath.ContainsAt('.', index + 1);
            int nextIndex = index + (isRecursiveDescentApplied ? 2 : 1);

            if (nextIndex >= jsonPath.Length)
            {
                string message = isRecursiveDescentApplied
                    ? "Recursive descent must be followed by another expression element"
                    : "Expression ends with dot";

                throw new JsonPathExpressionParsingException(message);
            }

            return nextIndex;
        }

        private static int HandleSquareBracket(string jsonPath, int index, ref bool isRecursiveDescentApplied, List<JsonPathElement> elements)
        {
            int closingIndex = jsonPath.IndexOfOutsideQuotes(']', '\'', index + 1);
            if (closingIndex == -1)
                throw new JsonPathExpressionParsingException("No matching closing square bracket found");

            elements.Add(ParseTokenInsideBrackets(jsonPath.Substring(index + 1, closingIndex - index - 1), isRecursiveDescentApplied));
            isRecursiveDescentApplied = false;
            return closingIndex + 1;
        }

        private static JsonPathElement ParseToken(string token, bool isFirstToken, bool isRecursiveDescentApplied)
        {
            if (isFirstToken && isRecursiveDescentApplied)
                throw new InvalidOperationException("This should not happen: recursive descent can not be applied to first token");
            if (token.IndexOfAny(ForbiddenCharactersOutsideBrackets) != -1)
                throw new JsonPathExpressionParsingException($"Forbidden characters found outside square brackets: {string.Join(" ", ForbiddenCharactersOutsideBrackets)}");

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
            if (string.IsNullOrEmpty(token))
                throw new JsonPathExpressionParsingException("No content inside square brackets");

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
            try
            {
                return new JsonPathArrayIndexElement(int.Parse(token));
            }
            catch (Exception e)
            {
                throw new JsonPathExpressionParsingException("Failed to parse expression", e);
            }
        }

        private static JsonPathElement ParseArrayIndexListToken(string token)
        {
            string[] parts = token.Split(Comma, StringSplitOptions.None);
            if (parts.Any(string.IsNullOrWhiteSpace))
                throw new JsonPathExpressionParsingException("Double commas found inside array index list element");

            try
            {
                var indexes = parts
                    .Select(x => int.Parse(x.Trim()))
                    .ToList();

                return new JsonPathArrayIndexListElement(indexes);
            }
            catch (Exception e)
            {
                throw new JsonPathExpressionParsingException("Failed to parse expression", e);
            }
        }

        private static JsonPathElement ParseArraySliceToken(string token)
        {
            string[] parts = token.Split(Colon, StringSplitOptions.None);
            if (parts.Length == 0 || parts.Length > 3)
                throw new JsonPathExpressionParsingException($"Array slice contains insufficient number of arguments: {parts.Length}");

            try
            {
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
            catch (Exception e)
            {
                throw new JsonPathExpressionParsingException("Failed to parse expression", e);
            }
        }

        private static JsonPathElement ParsePropertyNamesToken(string token)
        {
            if (!token.EndsWith("'", StringComparison.Ordinal))
                throw new JsonPathExpressionParsingException("No closing single quote found inside property list element");

            var parts = token
                .SplitQuoted(',', '\'')
                .Select(x => x.Trim())
                .ToList();

            if (parts.Any(string.IsNullOrWhiteSpace))
                throw new JsonPathExpressionParsingException("Double commas found inside array property list element");

            var names = parts
                .Select(x => x.Trim('\''))
                .ToList();

            if (names.Any(x => x.Contains('\'')))
                throw new JsonPathExpressionParsingException("Non-matching single quotes found inside array property list element");

            return names.Count == 1
                ? (JsonPathElement)new JsonPathPropertyElement(names[0])
                : (JsonPathElement)new JsonPathPropertyListElement(names);
        }

        private static JsonPathElement ParseExpressionToken(string token)
        {
            if (!token.EndsWith(")", StringComparison.Ordinal))
                throw new JsonPathExpressionParsingException("No closing brace found inside expression element");

            return new JsonPathExpressionElement(token.Substring(1, token.Length - 2));
        }

        private static JsonPathElement ParseFilterExpressionToken(string token)
        {
            if (!token.EndsWith(")", StringComparison.Ordinal))
                throw new JsonPathExpressionParsingException("No closing brace found inside filter expression element");

            return new JsonPathFilterExpressionElement(token.Substring(2, token.Length - 3));
        }
    }
}