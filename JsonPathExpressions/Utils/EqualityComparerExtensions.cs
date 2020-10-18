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
    using System.Collections.Generic;

    internal static class EqualityComparerExtensions
    {
        public static bool CollectionsEqual<T>(this IEqualityComparer<T> equalityComparer, IReadOnlyCollection<T> first, IReadOnlyCollection<T> second)
            where T : notnull
        {
            int firstCount = first.Count;
            int secondCount = second.Count;
            if (firstCount != secondCount)
                return false;

            if (firstCount == 0 && secondCount == 0)
                return true;

            using var firstEnumerator = first.GetEnumerator();
            using var secondEnumerator = second.GetEnumerator();

            while (firstEnumerator.MoveNext() && secondEnumerator.MoveNext())
            {
                var firstItem = firstEnumerator.Current;
                var secondItem = secondEnumerator.Current;

                bool itemsEqual = ReferenceEquals(firstItem, secondItem)
                                  || equalityComparer.Equals(firstItem, secondItem);

                if (!itemsEqual)
                    return false;
            }

            return true;
        }

        public static int GetCollectionHashCode<T>(this IEqualityComparer<T> equalityComparer, IEnumerable<T> collection)
        {
            unchecked
            {
                int hashCode = 0;

                int count = 0;
                foreach (var item in collection)
                {
                    ++count;
                    hashCode = (hashCode * 397) ^ equalityComparer.GetHashCode(item);
                }

                hashCode = (hashCode * 397) ^ count;
                return hashCode;
            }
        }
    }
}