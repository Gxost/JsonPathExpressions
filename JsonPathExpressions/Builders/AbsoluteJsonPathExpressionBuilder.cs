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

    internal class AbsoluteJsonPathExpressionBuilder : IFirstAbsolutePathElementSyntax, INextAbsolutePathElementSyntax
    {
        private readonly JsonPathElementsBuilder _elementsBuilder;

        private AbsoluteJsonPathExpressionBuilder()
        {
            _elementsBuilder = new JsonPathElementsBuilder();
        }

        public INextAbsolutePathElementSyntax this[string propertyName] => Property(propertyName);

        public INextAbsolutePathElementSyntax this[int index] => ArrayIndex(index);

        public INextAbsolutePathElementSyntax Root()
        {
            _elementsBuilder.Root();
            return this;
        }

        public IAbsolutePathElementSyntax RecursiveDescentTo()
        {
            _elementsBuilder.RecursiveDescentTo();
            return this;
        }

        public INextAbsolutePathElementSyntax Property(string name)
        {
            _elementsBuilder.Property(name);
            return this;
        }

        public INextAbsolutePathElementSyntax AnyProperty()
        {
            _elementsBuilder.AnyProperty();
            return this;
        }

        public INextAbsolutePathElementSyntax Properties(string firstName, params string[] names)
        {
            _elementsBuilder.Properties(firstName, names);
            return this;
        }

        public INextAbsolutePathElementSyntax Properties(IReadOnlyCollection<string> names)
        {
            _elementsBuilder.Properties(names);
            return this;
        }

        public INextAbsolutePathElementSyntax ArrayIndex(int index)
        {
            _elementsBuilder.ArrayIndex(index);
            return this;
        }

        public INextAbsolutePathElementSyntax AnyArrayIndex()
        {
            _elementsBuilder.AnyArrayIndex();
            return this;
        }

        public INextAbsolutePathElementSyntax ArrayIndexes(int firstIndex, params int[] indexes)
        {
            _elementsBuilder.ArrayIndexes(firstIndex, indexes);
            return this;
        }

        public INextAbsolutePathElementSyntax ArrayIndexes(IReadOnlyCollection<int> indexes)
        {
            _elementsBuilder.ArrayIndexes(indexes);
            return this;
        }

        public INextAbsolutePathElementSyntax ArraySlice(int? start, int? end, int step = 1)
        {
            _elementsBuilder.ArraySlice(start, end, step);
            return this;
        }

        public INextAbsolutePathElementSyntax Expression(string expression)
        {
            _elementsBuilder.Expression(expression);
            return this;
        }

        public INextAbsolutePathElementSyntax FilterExpression(string expression)
        {
            _elementsBuilder.FilterExpression(expression);
            return this;
        }

        public AbsoluteJsonPathExpression Build()
        {
            return new AbsoluteJsonPathExpression(_elementsBuilder.Build());
        }

        public static IFirstAbsolutePathElementSyntax Create()
        {
            return new AbsoluteJsonPathExpressionBuilder();
        }
    }
}