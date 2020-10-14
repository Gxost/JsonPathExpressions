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
        public static IReadOnlyCollection<string> SplitQuoted(this string str, char delimiter, char quote)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            if (quote == delimiter)
                throw new ArgumentException("Quote character is equal to delimiter", nameof(quote));

            var parts = new List<string>();
            var delimiterAndQuote = new[] { delimiter, quote };

            for (int nextIndex = 0; nextIndex < str.Length;)
            {
                int index = str.IndexOfDelimiter(delimiter, quote, nextIndex, delimiterAndQuote);
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

        public static bool EndsWith(this string str, int position)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            return position == str.Length - 1;
        }

        private static int IndexOfDelimiter(this string str, char delimiter, char quote, int startIndex, char[] delimiterAndQuote)
        {
            while (startIndex < str.Length)
            {
                int index = str.IndexOfAny(delimiterAndQuote, startIndex);
                if (index == -1 || str[index] == delimiter)
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
    }
}