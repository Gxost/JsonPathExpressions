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

namespace JsonPathExpressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Elements;
    using Utils;

    /// <summary>
    /// Extension methods for <see cref="JsonPathExpression"/> and its children
    /// </summary>
    public static class JsonPathExpressionExtensions
    {
        /// <summary>
        /// Check if current JsonPath expression starts with another JsonPath expression but is not equal to it
        /// </summary>
        /// <param name="path">JsonPath expression</param>
        /// <param name="prefix">JsonPath expression that is supposed to be a prefix of <paramref name="path"/></param>
        /// <returns>True if <paramref name="path"/> starts with <paramref name="prefix"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> or <paramref name="prefix"/> is null</exception>
        public static bool StartsWith(this JsonPathExpression path, JsonPathExpression prefix)
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));
            if (prefix is null)
                throw new ArgumentNullException(nameof(prefix));

            if (prefix.Elements.Count >= path.Elements.Count)
                return false;

            for (int i = 0; i < prefix.Elements.Count; ++i)
            {
                if (!path.Elements[i].Equals(prefix.Elements[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Tells if a set of JSON tree elements represented by current JsonPath expression contain JSON tree elements represented by another JsonPath expression
        /// </summary>
        /// <param name="path">JsonPath expression</param>
        /// <param name="other">JsonPath expression to check for matching</param>
        /// <returns>True if a set of JSON tree elements represented by current JsonPath expression contain JSON tree elements represented by <paramref name="other"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> or <paramref name="other"/> is null</exception>
        /// <remarks>Returns null if it's not possible to check if a set of JSON tree elements represented by the JsonPath expression contain JSON tree elements represented by another JsonPath expression</remarks>
        public static bool? Matches(this JsonPathExpression path, JsonPathExpression other)
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));
            if (other is null)
                throw new ArgumentNullException(nameof(other));

            return path.Elements[0].Matches(path.Elements, 0, other.Elements, 0);
        }

        /// <summary>
        /// Convert JsonPath expression to <see cref="AbsoluteJsonPathExpression"/>
        /// </summary>
        /// <param name="path">JsonPath expression to convert</param>
        /// <returns><see cref="AbsoluteJsonPathExpression"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null</exception>
        /// <remarks>
        /// <para>If <paramref name="path"/> is <see cref="AbsoluteJsonPathExpression"/>, <paramref name="path"/> is returned</para>
        /// <para>If <paramref name="path"/> is an absolute path, an absolute JsonPath expression with its elements is returned</para>
        /// <para>If <paramref name="path"/> is a relative path, an absolute JsonPath expression with its elements appended to root element is returned</para>
        /// </remarks>
        public static AbsoluteJsonPathExpression ToAbsolute(this JsonPathExpression path)
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));

            if (path is AbsoluteJsonPathExpression absolutePath)
                return absolutePath;

            var elements = path.IsAbsolute
                ? path.Elements
                : CollectionHelper.Concatenate(new JsonPathRootElement(), path.Elements);

            return new AbsoluteJsonPathExpression(elements);
        }

        /// <summary>
        /// Gets normalized JsonPath expression for passed one
        /// </summary>
        /// <typeparam name="TJsonPathExpression">JsonPath expression type</typeparam>
        /// <param name="path">JsonPath expression</param>
        /// <returns>Normalized JsonPath expression</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null</exception>
        /// <remarks>
        /// If the passed JsonPath expression is normalized, returns it
        /// </remarks>
        public static TJsonPathExpression GetNormalized<TJsonPathExpression>(this TJsonPathExpression path)
            where TJsonPathExpression : JsonPathExpression
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));

            if (path.IsNormalized)
                return path;

            var elements = path.Elements
                .Select(x => x.GetNormalized())
                .ToList();

            return (TJsonPathExpression) path.Create(elements);
        }

        /// <summary>
        /// Get parent JsonPath expression for current JsonPath expression and passed one
        /// </summary>
        /// <typeparam name="TJsonPathExpression">JsonPath expression type</typeparam>
        /// <param name="path">JsonPath expression</param>
        /// <param name="other">JsonPath expression to check</param>
        /// <returns>Parent JsonPath expression, or <paramref name="path"/>, or <paramref name="other"/>, or null</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> or <paramref name="other"/> is null</exception>
        /// <remarks>
        /// <para>Parent JsonPath expression is an expression which is prefix for both <paramref name="path"/> and <paramref name="other"/></para>
        /// <para>If <paramref name="path"/> starts with <paramref name="other"/>, <paramref name="other"/> is returned</para>
        /// <para>If <paramref name="other"/> starts with <paramref name="path"/>, <paramref name="path"/> is returned</para>
        /// <para>If there is no parent for both expressions, null is returned</para>
        /// </remarks>
        public static TJsonPathExpression? GetParentWith<TJsonPathExpression>(this TJsonPathExpression path, TJsonPathExpression other)
            where TJsonPathExpression : JsonPathExpression
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));
            if (other is null)
                throw new ArgumentNullException(nameof(other));

            int minLength = Math.Min(path.Elements.Count, other.Elements.Count);
            for (int i = 0; i < minLength; ++i)
            {
                if (path.Elements[i].Equals(other.Elements[i]))
                    continue;

                if (i == 0)
                    return null;

                var elements = path.Elements
                    .Take(i)
                    .ToList();

                return (TJsonPathExpression)path.Create(elements);
            }

            return minLength == path.Elements.Count ? path : other;
        }

        /// <summary>
        /// Get relative path from current JsonPath expression to child JsonPath expression
        /// </summary>
        /// <param name="path">JsonPath expression</param>
        /// <param name="childPath">Child JsonPath expression</param>
        /// <returns>Relative path from current JsonPath expression to child JsonPth expression, or null</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> or <paramref name="childPath"/> is null</exception>
        /// <remarks>
        /// If <paramref name="childPath"/> does not start with <paramref name="path"/>, or is equal to <paramref name="path"/>, null is returned
        /// </remarks>
        public static RelativeJsonPathExpression? GetRelativePathTo(this JsonPathExpression path, JsonPathExpression childPath)
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));
            if (childPath is null)
                throw new ArgumentNullException(nameof(childPath));

            if (path.Elements.Count >= childPath.Elements.Count)
                return null;
            if (!childPath.StartsWith(path))
                return null;

            var elements = childPath.Elements
                .Skip(path.Elements.Count)
                .ToList();

            return new RelativeJsonPathExpression(elements);
        }

        /// <summary>
        /// Append JsonPath elements to current JsonPath expression
        /// </summary>
        /// <typeparam name="TJsonPathExpression">JsonPath expression type</typeparam>
        /// <param name="path">JsonPath expression</param>
        /// <param name="elements">Collection of JsonPath elements to append</param>
        /// <returns>JsonPath expression starting with <paramref name="path"/> and ending with <paramref name="elements"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> or <paramref name="elements"/> is null</exception>
        /// <exception cref="ArgumentException">At least one JsonPath element is null</exception>
        /// <remarks>
        /// If <paramref name="elements"/> is empty, <paramref name="path"/> is returned
        /// </remarks>
        public static TJsonPathExpression Append<TJsonPathExpression>(this TJsonPathExpression path, params JsonPathElement[] elements)
            where TJsonPathExpression : JsonPathExpression
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));
            if (elements is null)
                throw new ArgumentNullException(nameof(elements));
            if (elements.Contains(null!))
                throw new ArgumentException("At least one element is null", nameof(elements));

            if (elements.Length == 0)
                return path;

            var resultElements = new List<JsonPathElement>(path.Elements);
            resultElements.AddRange(elements!);

            return (TJsonPathExpression) path.Create(resultElements);
        }

        /// <summary>
        /// Append relative JsonPath expression to current JsonPath expression
        /// </summary>
        /// <typeparam name="TJsonPathExpression">JsonPath expression type</typeparam>
        /// <param name="path">JsonPath expression</param>
        /// <param name="relativePath">Relative JsonPath expression to append</param>
        /// <returns>JsonPath expression starting with <paramref name="path"/> and ending with <paramref name="relativePath"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> or <paramref name="relativePath"/> is null</exception>
        public static TJsonPathExpression Append<TJsonPathExpression>(this TJsonPathExpression path, RelativeJsonPathExpression relativePath)
            where TJsonPathExpression : JsonPathExpression
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));
            if (relativePath is null)
                throw new ArgumentNullException(nameof(relativePath));

            var elements = new List<JsonPathElement>(path.Elements);
            elements.AddRange(relativePath.Elements);

            return (TJsonPathExpression)path.Create(elements);
        }

        /// <summary>
        /// Replace last JsonPath element in current JsonPath expression with another JsonPath element
        /// </summary>
        /// <typeparam name="TJsonPathExpression">JsonPath expression type</typeparam>
        /// <param name="path">JsonPath expression</param>
        /// <param name="element">New JsonPath element</param>
        /// <returns>JsonPath expression with replaced last JsonPath element</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> or <paramref name="element"/> is null</exception>
        public static TJsonPathExpression ReplaceLastWith<TJsonPathExpression>(this TJsonPathExpression path, JsonPathElement element)
            where TJsonPathExpression : JsonPathExpression
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));
            if (element is null)
                throw new ArgumentNullException(nameof(element));

            var elements = new List<JsonPathElement>(path.Elements) { [path.Elements.Count - 1] = element };

            return (TJsonPathExpression)path.Create(elements);
        }

        /// <summary>
        /// Remove last JsonPath elements from current JsonPath
        /// </summary>
        /// <typeparam name="TJsonPathExpression">JsonPath expression type</typeparam>
        /// <param name="path">JsonPath expression</param>
        /// <param name="count">Number of JsonPath elements to remove</param>
        /// <returns>JsonPath expression with removed last <paramref name="count"/> JsonPath elements, or null</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is negative</exception>
        /// <remarks>
        /// If <paramref name="count"/> is greater or equal to than <see cref="JsonPathExpression.Length"/>, null is returned
        /// </remarks>
        public static TJsonPathExpression? RemoveLast<TJsonPathExpression>(this TJsonPathExpression path, int count = 1)
            where TJsonPathExpression : JsonPathExpression
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), count, null);

            if (count == 0)
                return path;

            if (path.Elements.Count <= count)
                return null;

            var elements = path.Elements
                .Take(path.Elements.Count - count)
                .ToList();

            return (TJsonPathExpression)path.Create(elements);
        }
    }
}