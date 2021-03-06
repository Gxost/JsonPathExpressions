﻿#region License
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

    internal class RelativeJsonPathExpressionBuilder : IFirstRelativePathElementSyntax, INextRelativePathElementSyntax
    {
        private readonly JsonPathElementsBuilder _elementsBuilder;

        private RelativeJsonPathExpressionBuilder()
        {
            _elementsBuilder = new JsonPathElementsBuilder();
        }

        public INextRelativePathElementSyntax this[string propertyName] => Property(propertyName);

        public INextRelativePathElementSyntax this[int index] => ArrayIndex(index);

        public IRelativePathElementSyntax RecursiveDescentTo()
        {
            _elementsBuilder.RecursiveDescentTo();
            return this;
        }

        public INextRelativePathElementSyntax Property(string name)
        {
            _elementsBuilder.Property(name);
            return this;
        }

        public INextRelativePathElementSyntax AnyProperty()
        {
            _elementsBuilder.AnyProperty();
            return this;
        }

        public INextRelativePathElementSyntax Properties(string firstName, params string[] names)
        {
            _elementsBuilder.Properties(firstName, names);
            return this;
        }

        public INextRelativePathElementSyntax Properties(IReadOnlyCollection<string> names)
        {
            _elementsBuilder.Properties(names);
            return this;
        }

        public INextRelativePathElementSyntax ArrayIndex(int index)
        {
            _elementsBuilder.ArrayIndex(index);
            return this;
        }

        public INextRelativePathElementSyntax AnyArrayIndex()
        {
            _elementsBuilder.AnyArrayIndex();
            return this;
        }

        public INextRelativePathElementSyntax ArrayIndexes(int firstIndex, params int[] indexes)
        {
            _elementsBuilder.ArrayIndexes(firstIndex, indexes);
            return this;
        }

        public INextRelativePathElementSyntax ArrayIndexes(IReadOnlyCollection<int> indexes)
        {
            _elementsBuilder.ArrayIndexes(indexes);
            return this;
        }

        public INextRelativePathElementSyntax ArraySlice(int? start, int? end, int step = 1)
        {
            _elementsBuilder.ArraySlice(start, end, step);
            return this;
        }

        public INextRelativePathElementSyntax Expression(string expression)
        {
            _elementsBuilder.Expression(expression);
            return this;
        }

        public INextRelativePathElementSyntax FilterExpression(string expression)
        {
            _elementsBuilder.FilterExpression(expression);
            return this;
        }

        public RelativeJsonPathExpression Build()
        {
            return new RelativeJsonPathExpression(_elementsBuilder.Build());
        }

        public static IFirstRelativePathElementSyntax Create()
        {
            return new RelativeJsonPathExpressionBuilder();
        }
    }
}