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

namespace JsonPathExpressions.Utils
{
    using System;
    using System.Collections.Generic;

    internal static class StringExtensions
    {
        private static readonly char[] CommaWithSingleQuote = {',', '\''};
        private static readonly char[] ClosingSquareBracketWithSingleQuote = {']', '\''};

        public static IReadOnlyCollection<string> SplitQuoted(this string str, char delimiter, char quote)
        {
            if (quote == delimiter)
                throw new ArgumentException("Quote character is equal to delimiter", nameof(quote));

            var parts = new List<string>();
            var valueWithQuote = GetValueWithQuote(delimiter, quote);

            for (int nextIndex = 0; nextIndex < str.Length;)
            {
                int index = str.IndexOfOutsideQuotes(delimiter, quote, nextIndex, valueWithQuote);
                if (index == -1)
                {
                    parts.Add(str.Substring(nextIndex));
                    break;
                }

                parts.Add(str.Substring(nextIndex, index - nextIndex));
                nextIndex = index + 1;
            }

            return parts;
        }

        public static int IndexOfOutsideQuotes(this string str, char value, char quote, int startIndex)
        {
            if (quote == value)
                throw new ArgumentException("Quote character is equal to searched value", nameof(quote));

            return str.IndexOfOutsideQuotes(value, quote, startIndex, GetValueWithQuote(value, quote));
        }

        public static bool EndsWith(this string str, int position)
        {
            return position == str.Length - 1;
        }

        public static bool ContainsAt(this string str, char value, int atPosition)
        {
            return atPosition < str.Length && str[atPosition] == value;
        }

        private static int IndexOfOutsideQuotes(this string str, char value, char quote, int startIndex, char[] valueWithQuote)
        {
            while (startIndex < str.Length)
            {
                int index = str.IndexOfAny(valueWithQuote, startIndex);
                if (index == -1 || str[index] == value)
                    return index;
                if (str.EndsWith(index))
                    return -1;

                index = str.IndexOf(quote, index + 1);
                if (index == -1)
                    return -1;

                startIndex = index + 1;
            }

            return -1;
        }

        private static char[] GetValueWithQuote(char value, char quote)
        {
            if (quote == '\'')
            {
                switch (value)
                {
                    case ',':
                        return CommaWithSingleQuote;
                    case ']':
                        return ClosingSquareBracketWithSingleQuote;
                }
            }

            return new[] { value, quote };
        }
    }
}