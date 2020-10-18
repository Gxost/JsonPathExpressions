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

namespace JsonPathExpressions.Builders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Elements;
    using Utils;

    internal class JsonPathElementsBuilder
    {
        private readonly List<JsonPathElement> _elements;
        private bool _isRecursiveDescent;

        public JsonPathElementsBuilder()
        {
            _elements = new List<JsonPathElement>();
            _isRecursiveDescent = false;
        }

        public void Root()
        {
            if (_elements.Count > 0 || _isRecursiveDescent)
                throw new InvalidOperationException("Root element must be first element of the expression");

            _elements.Add(new JsonPathRootElement());
        }

        public void RecursiveDescentTo()
        {
            if (_isRecursiveDescent)
                throw new InvalidOperationException("Recursive descent must not follow recursive descent");

            _isRecursiveDescent = true;
        }

        public void Property(string name)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            AddElement(new JsonPathPropertyElement(name));
        }

        public void AnyProperty()
        {
            AddElement(new JsonPathAnyPropertyElement());
        }

        public void Properties(string firstName, params string[] names)
        {
            if (firstName is null)
                throw new ArgumentNullException(nameof(firstName));
            if (names is null)
                throw new ArgumentNullException(nameof(names));

            Properties(CollectionHelper.Concatenate(firstName, names));
        }

        public void Properties(IReadOnlyCollection<string> names)
        {
            if (names is null)
                throw new ArgumentNullException(nameof(names));
            if (names.Count == 0)
                throw new ArgumentException("No property names provided");

            if (names.Count == 1)
                AddElement(new JsonPathPropertyElement(names.First()));
            else
                AddElement(new JsonPathPropertyListElement(names));
        }

        public void ArrayIndex(int index)
        {
            AddElement(new JsonPathArrayIndexElement(index));
        }

        public void AnyArrayIndex()
        {
            AddElement(new JsonPathAnyArrayIndexElement());
        }

        public void ArrayIndexes(int firstIndex, params int[] indexes)
        {
            if (indexes is null)
                throw new ArgumentNullException(nameof(indexes));

            ArrayIndexes(CollectionHelper.Concatenate(firstIndex, indexes));
        }

        public void ArrayIndexes(IReadOnlyCollection<int> indexes)
        {
            if (indexes is null)
                throw new ArgumentNullException(nameof(indexes));
            if (indexes.Count == 0)
                throw new ArgumentException("No array indexes provided");

            if (indexes.Count == 1)
                AddElement(new JsonPathArrayIndexElement(indexes.First()));
            else
                AddElement(new JsonPathArrayIndexListElement(indexes));
        }

        public void ArraySlice(int? start, int? end, int step = 1)
        {
            AddElement(new JsonPathArraySliceElement(start, end, step));
        }

        public void Expression(string expression)
        {
            if (string.IsNullOrEmpty(expression))
                throw new ArgumentNullException(nameof(expression));

            AddElement(new JsonPathExpressionElement(expression));
        }

        public void FilterExpression(string expression)
        {
            if (string.IsNullOrEmpty(expression))
                throw new ArgumentNullException(nameof(expression));

            AddElement(new JsonPathFilterExpressionElement(expression));
        }

        public IReadOnlyCollection<JsonPathElement> Build()
        {
            if (_isRecursiveDescent)
                throw new InvalidOperationException("Element following recursive descent required");

            return _elements;
        }

        private void AddElement(JsonPathElement element)
        {
            if (_isRecursiveDescent)
                element = new JsonPathRecursiveDescentElement(element);

            _elements.Add(element);
            _isRecursiveDescent = false;
        }
    }
}