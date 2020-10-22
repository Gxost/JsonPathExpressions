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
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Allows to find JsonPath expressions that match given JsonPath expression
    /// </summary>
    public sealed class JsonPathExpressionMatchingSet<TJsonPathExpression> : IReadOnlyJsonPathExpressionMatchingSet<TJsonPathExpression>, ICollection<TJsonPathExpression>
        where TJsonPathExpression : JsonPathExpression
    {
        private readonly HashSet<TJsonPathExpression> _hashSet;
        private readonly JsonPathExpressionMatchingNode<TJsonPathExpression> _matchingNode;

        /// <summary>
        /// Create <see cref="JsonPathExpressionMatchingSet{TJsonPathExpression}"/> instance
        /// </summary>
        public JsonPathExpressionMatchingSet()
        {
            _hashSet = new HashSet<TJsonPathExpression>();
            _matchingNode = new JsonPathExpressionMatchingNode<TJsonPathExpression>(0);
        }

        /// <summary>
        /// Create <see cref="JsonPathExpressionMatchingSet{TJsonPathExpression}"/> instance
        /// </summary>
        /// <param name="jsonPaths">Collection of JsonPath expression to add</param>
        /// <exception cref="ArgumentNullException"><paramref name="jsonPaths"/> or its item is null</exception>
        public JsonPathExpressionMatchingSet(IEnumerable<TJsonPathExpression> jsonPaths)
            : this()
        {
            if (jsonPaths is null)
                throw new ArgumentNullException(nameof(jsonPaths));

            foreach (var path in jsonPaths)
                Add(path);
        }

        /// <inheritdoc cref="ICollection{T}" />
        public int Count => _hashSet.Count;

        /// <inheritdoc cref="ICollection{T}" />
        public bool IsReadOnly => false;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc cref="ICollection{T}" />
        public IEnumerator<TJsonPathExpression> GetEnumerator()
        {
            return _hashSet.GetEnumerator();
        }

        /// <inheritdoc cref="ICollection{T}" />
        public bool Contains(TJsonPathExpression item)
        {
            return _hashSet.Contains(item);
        }

        /// <inheritdoc cref="ICollection{T}" />
        public void CopyTo(TJsonPathExpression[] array, int arrayIndex)
        {
            _hashSet.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public bool? Matches(TJsonPathExpression jsonPath)
        {
            if (jsonPath is null)
                throw new ArgumentNullException(nameof(jsonPath));

            return _matchingNode.Matches(jsonPath);
        }

        /// <inheritdoc />
        public bool Matches(TJsonPathExpression jsonPath, out List<TJsonPathExpression> matchedBy)
        {
            if (jsonPath is null)
                throw new ArgumentNullException(nameof(jsonPath));

            matchedBy = new List<TJsonPathExpression>();
            return _matchingNode.Matches(jsonPath, matchedBy);
        }

        void ICollection<TJsonPathExpression>.Add(TJsonPathExpression item)
        {
            Add(item);
        }

        /// <summary>
        /// Add JsonPath expression to the set
        /// </summary>
        /// <param name="jsonPath">JsonPath expression to add</param>
        /// <returns>True if <paramref name="jsonPath"/> is added to the set</returns>
        /// <exception cref="ArgumentNullException"><paramref name="jsonPath"/> is null</exception>
        public bool Add(TJsonPathExpression jsonPath)
        {
            if (jsonPath is null)
                throw new ArgumentNullException(nameof(jsonPath));

            if (!_hashSet.Add(jsonPath))
                return false;
            
            if (!_matchingNode.Add(jsonPath))
                throw new InvalidOperationException("This should not happen: JsonPath was added to hash set but not to matching tree");

            return true;
        }

        /// <inheritdoc />
        public bool Remove(TJsonPathExpression item)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));

            if (!_hashSet.Remove(item))
                return false;

            if (!_matchingNode.Remove(item))
                throw new InvalidOperationException("This should not happen: JsonPath was removed from hash set but not from matching tree");

            return true;
        }

        /// <inheritdoc />
        public void Clear()
        {
            _hashSet.Clear();
            _matchingNode.Clear();
        }
    }
}