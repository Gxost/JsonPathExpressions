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

    internal class JsonPathExpressionMatchingNode<TJsonPathExpression>
        where TJsonPathExpression : JsonPathExpression
    {
        private static readonly IReadOnlyDictionary<JsonPathElement, JsonPathExpressionMatchingNode<TJsonPathExpression>> Empty = new Dictionary<JsonPathElement, JsonPathExpressionMatchingNode<TJsonPathExpression>>();

        private readonly Dictionary<JsonPathElement, JsonPathExpressionMatchingNode<TJsonPathExpression>> _strict;
        private readonly Dictionary<JsonPathElement, JsonPathExpressionMatchingNode<TJsonPathExpression>> _properties;
        private readonly Dictionary<JsonPathElement, JsonPathExpressionMatchingNode<TJsonPathExpression>> _indexes;
        private readonly HashSet<TJsonPathExpression> _recursiveDescents;
        private TJsonPathExpression? _current;

        public JsonPathExpressionMatchingNode(int index)
        {
            Index = index;

            _strict = new Dictionary<JsonPathElement, JsonPathExpressionMatchingNode<TJsonPathExpression>>();
            _properties = new Dictionary<JsonPathElement, JsonPathExpressionMatchingNode<TJsonPathExpression>>();
            _indexes = new Dictionary<JsonPathElement, JsonPathExpressionMatchingNode<TJsonPathExpression>>();
            _recursiveDescents = new HashSet<TJsonPathExpression>();
            _current = null;
        }

        public int Index { get; }

        public bool IsEmpty()
        {
            return _strict.Count == 0
                   && _properties.Count == 0
                   && _indexes.Count == 0
                   && _recursiveDescents.Count == 0
                   && _current is null;
        }

        public bool? Matches(TJsonPathExpression jsonPath)
        {
            if (Index == jsonPath.Elements.Count)
                return !(_current is null);

            bool? result = false;
            if (jsonPath.Elements[Index].Type != JsonPathElementType.RecursiveDescent)
            {
                result = MatchesNonRecursiveDescent(jsonPath);
                if (result == true)
                    return true;
            }

            foreach (var item in _recursiveDescents)
            {
                bool? matches = item.Matches(jsonPath);
                if (matches != false)
                    result = matches;

                if (result == true)
                    break;
            }

            return result;
        }

        public bool Matches(TJsonPathExpression jsonPath, List<TJsonPathExpression> matchedBy)
        {
            if (Index == jsonPath.Elements.Count)
            {
                if (_current is null)
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

        public bool Add(TJsonPathExpression jsonPath)
        {
            if (Index == jsonPath.Elements.Count)
                return AddCurrent(jsonPath);

            var element = jsonPath.Elements[Index];
            if (element.Type == JsonPathElementType.RecursiveDescent)
                return _recursiveDescents.Add(jsonPath);

            return Add(jsonPath, GetNodes(element));
        }

        public bool Remove(TJsonPathExpression jsonPath)
        {
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

        private bool? MatchesNonRecursiveDescent(TJsonPathExpression jsonPath)
        {
            var element = jsonPath.Elements[Index];
            bool? result = IsStrict(element)
                ? Matches(jsonPath, _strict)
                : false;

            if (result == true)
                return true;

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

            return result;
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

        private Dictionary<JsonPathElement, JsonPathExpressionMatchingNode<TJsonPathExpression>> GetNodes(JsonPathElement element)
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
                case JsonPathElementType.RecursiveDescent:
                    throw new ArgumentOutOfRangeException(nameof(element), element, "This should not happen: recursive descent is not allowed here");
                default:
                    throw new ArgumentOutOfRangeException(nameof(element), element, $"Unknown JSON path element type: {element.GetType()}");
            }
        }

        private IReadOnlyDictionary<JsonPathElement, JsonPathExpressionMatchingNode<TJsonPathExpression>> GetNonStrictNodes(JsonPathElement element)
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
                case JsonPathElementType.RecursiveDescent:
                    throw new ArgumentOutOfRangeException(nameof(element), element, "This should not happen: recursive descent is not allowed here");
                default:
                    throw new ArgumentOutOfRangeException(nameof(element), element, $"Unknown JSON path element type: {element.GetType()}");
            }
        }

        private bool? Matches(TJsonPathExpression jsonPath, Dictionary<JsonPathElement, JsonPathExpressionMatchingNode<TJsonPathExpression>> nodes)
        {
            var element = jsonPath.Elements[Index];
            if (!nodes.TryGetValue(element, out var node))
                return false;

            return node.Matches(jsonPath);
        }

        private bool Matches(TJsonPathExpression jsonPath, Dictionary<JsonPathElement, JsonPathExpressionMatchingNode<TJsonPathExpression>> nodes, List<TJsonPathExpression> matchedBy)
        {
            var element = jsonPath.Elements[Index];
            if (!nodes.TryGetValue(element, out var node))
                return false;

            return node.Matches(jsonPath, matchedBy);
        }

        private bool Add(TJsonPathExpression jsonPath, Dictionary<JsonPathElement, JsonPathExpressionMatchingNode<TJsonPathExpression>> nodes)
        {
            var element = jsonPath.Elements[Index];
            if (!nodes.TryGetValue(element, out var node))
            {
                node = new JsonPathExpressionMatchingNode<TJsonPathExpression>(Index + 1);
                nodes.Add(element, node);
            }

            return node.Add(jsonPath);
        }

        private bool Remove(TJsonPathExpression jsonPath, Dictionary<JsonPathElement, JsonPathExpressionMatchingNode<TJsonPathExpression>> nodes)
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

        private bool AddCurrent(TJsonPathExpression jsonPath)
        {
            if (!(_current is null))
                return false;

            _current = jsonPath;
            return true;
        }

        private bool RemoveCurrent()
        {
            if (_current is null)
                return false;

            _current = null;
            return true;
        }
    }
}