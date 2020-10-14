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
    using System.Collections.Generic;

    internal class JsonPathExpressionBuilder : IFirstPathElementSyntax, INextPathElementSyntax
    {
        private readonly JsonPathElementsBuilder _elementsBuilder;

        private JsonPathExpressionBuilder()
        {
            _elementsBuilder = new JsonPathElementsBuilder();
        }

        public INextPathElementSyntax Root()
        {
            _elementsBuilder.Root();
            return this;
        }

        public IPathElementSyntax RecursiveDescentTo()
        {
            _elementsBuilder.RecursiveDescentTo();
            return this;
        }

        public INextPathElementSyntax Property(string name)
        {
            _elementsBuilder.Property(name);
            return this;
        }

        public INextPathElementSyntax AnyProperty()
        {
            _elementsBuilder.AnyProperty();
            return this;
        }

        public INextPathElementSyntax Properties(string firstName, params string[] names)
        {
            _elementsBuilder.Properties(firstName, names);
            return this;
        }

        public INextPathElementSyntax Properties(IReadOnlyCollection<string> names)
        {
            _elementsBuilder.Properties(names);
            return this;
        }

        public INextPathElementSyntax ArrayIndex(int index)
        {
            _elementsBuilder.ArrayIndex(index);
            return this;
        }

        public INextPathElementSyntax AnyArrayIndex()
        {
            _elementsBuilder.AnyArrayIndex();
            return this;
        }

        public INextPathElementSyntax ArrayIndexes(int firstIndex, params int[] indexes)
        {
            _elementsBuilder.ArrayIndexes(firstIndex, indexes);
            return this;
        }

        public INextPathElementSyntax ArrayIndexes(IReadOnlyCollection<int> indexes)
        {
            _elementsBuilder.ArrayIndexes(indexes);
            return this;
        }

        public INextPathElementSyntax ArraySlice(int? start, int? end, int step = 1)
        {
            _elementsBuilder.ArraySlice(start, end, step);
            return this;
        }

        public INextPathElementSyntax Expression(string expression)
        {
            _elementsBuilder.Expression(expression);
            return this;
        }

        public INextPathElementSyntax FilterExpression(string expression)
        {
            _elementsBuilder.FilterExpression(expression);
            return this;
        }

        public JsonPathExpression Build()
        {
            return new JsonPathExpression(_elementsBuilder.Build());
        }

        public static IFirstPathElementSyntax Create()
        {
            return new JsonPathExpressionBuilder();
        }
    }
}