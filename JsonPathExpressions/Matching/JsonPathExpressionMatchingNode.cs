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
    using System.Collections.Generic;
    using Elements;

    internal class JsonPathExpressionMatchingNode
    {
        private static readonly IReadOnlyDictionary<JsonPathElement, JsonPathExpressionMatchingNode> Empty = new Dictionary<JsonPathElement, JsonPathExpressionMatchingNode>();

        private readonly Dictionary<JsonPathElement, JsonPathExpressionMatchingNode> _strict;
        private readonly Dictionary<JsonPathElement, JsonPathExpressionMatchingNode> _properties;
        private readonly Dictionary<JsonPathElement, JsonPathExpressionMatchingNode> _indexes;
        private readonly HashSet<JsonPathExpression> _recursiveDescents;
        private JsonPathExpression _current;

        public JsonPathExpressionMatchingNode(int index)
        {
            Index = index;

            _strict = new Dictionary<JsonPathElement, JsonPathExpressionMatchingNode>();
            _properties = new Dictionary<JsonPathElement, JsonPathExpressionMatchingNode>();
            _indexes = new Dictionary<JsonPathElement, JsonPathExpressionMatchingNode>();
            _recursiveDescents = new HashSet<JsonPathExpression>();
            _current = null;
        }

        public int Index { get; }

        public bool IsEmpty()
        {
            return _strict.Count == 0
                   && _properties.Count == 0
                   && _indexes.Count == 0
                   && _recursiveDescents.Count == 0
                   && _current == null;
        }

        public bool? Matches(JsonPathExpression jsonPath)
        {
            if (jsonPath == null)
                throw new ArgumentNullException(nameof(jsonPath));

            if (Index == jsonPath.Elements.Count)
                return _current != null;

            var element = jsonPath.Elements[Index];
            if (element.Type != JsonPathElementType.RecursiveDescent)
            {
                if (IsStrict(element) && Matches(jsonPath, _strict) == true)
                    return true;

                bool? result = false;
                var nonStrict = GetNonStrictNodes(element);
                foreach (var item in nonStrict)
                {
                    bool? elementMatches = item.Key.Matches(element);
                    if (elementMatches == false)
                        continue;

                    bool? nodeMatches = item.Value.Matches(jsonPath);
                    if (nodeMatches == false)
                        continue;

                    if (elementMatches == true && nodeMatches == true)
                        return true;

                    result = null;
                }

                if (result == null)
                    return null;
            }

            foreach (var item in _recursiveDescents)
            {
                bool? matches = item.Matches(jsonPath);
                if (matches != false)
                    return matches;
            }

            return false;
        }

        public bool Matches(JsonPathExpression jsonPath, List<JsonPathExpression> matchedBy)
        {
            if (jsonPath == null)
                throw new ArgumentNullException(nameof(jsonPath));
            if (matchedBy == null)
                throw new ArgumentNullException(nameof(matchedBy));

            if (Index == jsonPath.Elements.Count)
            {
                if (_current == null)
                    return false;

                matchedBy.Add(_current);
                return true;
            }

            bool result = false;

            var element = jsonPath.Elements[Index];
            if (element.Type != JsonPathElementType.RecursiveDescent)
            {
                if (IsStrict(element))
                    result = Matches(jsonPath, _strict, matchedBy);

                var nonStrict = GetNonStrictNodes(element);
                foreach (var item in nonStrict)
                {
                    if (item.Key.Matches(element) != true)
                        continue;

                    result |= item.Value.Matches(jsonPath, matchedBy);
                }
            }

            foreach (var item in _recursiveDescents)
            {
                if (item.Matches(jsonPath) != true)
                    continue;

                matchedBy.Add(item);
                result = true;
            }

            return result;
        }

        public bool Add(JsonPathExpression jsonPath)
        {
            if (jsonPath == null)
                throw new ArgumentNullException(nameof(jsonPath));

            if (Index == jsonPath.Elements.Count)
                return AddCurrent(jsonPath);

            var element = jsonPath.Elements[Index];
            if (element.Type == JsonPathElementType.RecursiveDescent)
                return _recursiveDescents.Add(jsonPath);

            return Add(jsonPath, GetNodes(element));
        }

        public bool Remove(JsonPathExpression jsonPath)
        {
            if (jsonPath == null)
                throw new ArgumentNullException(nameof(jsonPath));

            if (Index == jsonPath.Elements.Count)
                return RemoveCurrent();

            var element = jsonPath.Elements[Index];
            if (element.Type == JsonPathElementType.RecursiveDescent)
                return _recursiveDescents.Remove(jsonPath);

            return Remove(jsonPath, GetNodes(element));
        }

        public void Clear()
        {
            _strict.Clear();
            _properties.Clear();
            _indexes.Clear();
            _recursiveDescents.Clear();
            _current = null;
        }

        private static bool IsStrict(JsonPathElement element)
        {
            switch (element.Type)
            {
                case JsonPathElementType.Root:
                case JsonPathElementType.Property:
                case JsonPathElementType.ArrayIndex:
                    return true;
                case JsonPathElementType.AnyProperty:
                case JsonPathElementType.PropertyList:
                case JsonPathElementType.AnyArrayIndex:
                case JsonPathElementType.ArrayIndexList:
                case JsonPathElementType.ArraySlice:
                case JsonPathElementType.Expression:
                case JsonPathElementType.FilterExpression:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(element), element, $"Unknown JSON path element type: {element.GetType()}");
            }
        }

        private Dictionary<JsonPathElement, JsonPathExpressionMatchingNode> GetNodes(JsonPathElement element)
        {
            switch (element.Type)
            {
                case JsonPathElementType.Root:
                case JsonPathElementType.Property:
                case JsonPathElementType.ArrayIndex:
                    return _strict;
                case JsonPathElementType.AnyProperty:
                case JsonPathElementType.PropertyList:
                    return _properties;
                case JsonPathElementType.AnyArrayIndex:
                case JsonPathElementType.ArrayIndexList:
                case JsonPathElementType.ArraySlice:
                case JsonPathElementType.Expression:
                case JsonPathElementType.FilterExpression:
                    return _indexes;
                default:
                    throw new ArgumentOutOfRangeException(nameof(element), element, $"Unknown JSON path element type: {element.GetType()}");
            }
        }

        private IReadOnlyDictionary<JsonPathElement, JsonPathExpressionMatchingNode> GetNonStrictNodes(JsonPathElement element)
        {
            switch (element.Type)
            {
                case JsonPathElementType.Root:
                    return Empty;
                case JsonPathElementType.Property:
                case JsonPathElementType.AnyProperty:
                case JsonPathElementType.PropertyList:
                    return _properties;
                case JsonPathElementType.ArrayIndex:
                case JsonPathElementType.AnyArrayIndex:
                case JsonPathElementType.ArrayIndexList:
                case JsonPathElementType.ArraySlice:
                case JsonPathElementType.Expression:
                case JsonPathElementType.FilterExpression:
                    return _indexes;
                default:
                    throw new ArgumentOutOfRangeException(nameof(element), element, $"Unknown JSON path element type: {element.GetType()}");
            }
        }

        private bool? Matches(JsonPathExpression jsonPath, Dictionary<JsonPathElement, JsonPathExpressionMatchingNode> nodes)
        {
            var element = jsonPath.Elements[Index];
            if (!nodes.TryGetValue(element, out var node))
                return false;

            return node.Matches(jsonPath);
        }

        private bool Matches(JsonPathExpression jsonPath, Dictionary<JsonPathElement, JsonPathExpressionMatchingNode> nodes, List<JsonPathExpression> matchedBy)
        {
            var element = jsonPath.Elements[Index];
            if (!nodes.TryGetValue(element, out var node))
                return false;

            return node.Matches(jsonPath, matchedBy);
        }

        private bool Add(JsonPathExpression jsonPath, Dictionary<JsonPathElement, JsonPathExpressionMatchingNode> nodes)
        {
            var element = jsonPath.Elements[Index];
            if (!nodes.TryGetValue(element, out var node))
            {
                node = new JsonPathExpressionMatchingNode(Index + 1);
                nodes.Add(element, node);
            }

            return node.Add(jsonPath);
        }

        private bool Remove(JsonPathExpression jsonPath, Dictionary<JsonPathElement, JsonPathExpressionMatchingNode> nodes)
        {
            var element = jsonPath.Elements[Index];
            if (!nodes.TryGetValue(element, out var node))
                return false;

            if (!node.Remove(jsonPath))
                return false;

            if (node.IsEmpty())
                nodes.Remove(element);

            return true;
        }

        private bool AddCurrent(JsonPathExpression jsonPath)
        {
            if (_current != null)
                return false;

            _current = jsonPath;
            return true;
        }

        private bool RemoveCurrent()
        {
            if (_current == null)
                return false;

            _current = null;
            return true;
        }
    }
}