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

namespace JsonPathExpressions.Matching
{
    using System.Collections.Generic;

    /// <summary>
    /// Allows to find JsonPath expressions that match given JsonPath expression
    /// </summary>
    public interface IReadOnlyJsonPathExpressionMatchingSet<TJsonPathExpression> : IReadOnlyCollection<TJsonPathExpression>
        where TJsonPathExpression : JsonPathExpression
    {
        /// <summary>
        /// Tells if a set of JSON tree elements represented by any JsonPath expression in the set contain JSON tree elements represented by passed JsonPath expression
        /// </summary>
        /// <param name="jsonPath">JsonPath expression to check for matching</param>
        /// <returns>True if a set of JSON tree elements represented by any JsonPath expression in the set contain JSON tree elements represented by <paramref name="jsonPath"/></returns>
        /// <remarks>Returns null if it's not possible to check if a set of JSON tree elements represented by any JsonPath expression in the set contain JSON tree elements represented by another JsonPath expression</remarks>
        bool? Matches(TJsonPathExpression jsonPath);

        /// <summary>
        /// Tells if a set of JSON tree elements represented by any JsonPath expression in the set contain JSON tree elements represented by passed JsonPath expression
        /// </summary>
        /// <param name="jsonPath">JsonPath expression to check for matching</param>
        /// <param name="matchedBy">List containing JsonPath expressions that matched <paramref name="jsonPath"/></param>
        /// <returns>True if a set of JSON tree elements represented by any JsonPath expression in the set contain JSON tree elements represented by <paramref name="jsonPath"/></returns>
        bool Matches(TJsonPathExpression jsonPath, out List<TJsonPathExpression> matchedBy);
    }
}