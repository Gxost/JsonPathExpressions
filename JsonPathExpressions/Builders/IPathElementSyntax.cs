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

    /// <summary>
    /// Fluent syntax allowing to add JsonPath elements
    /// </summary>
    public interface IPathElementSyntax
    {
        /// <summary>
        /// Add property element
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <returns><see cref="INextPathElementSyntax"/></returns>
        INextPathElementSyntax this[string propertyName] { get; }

        /// <summary>
        /// Add array index element
        /// </summary>
        /// <param name="index">Array index</param>
        /// <returns><see cref="INextPathElementSyntax"/></returns>
        INextPathElementSyntax this[int index] { get; }

        /// <summary>
        /// Add property element
        /// </summary>
        /// <param name="name">Property name</param>
        /// <returns><see cref="INextPathElementSyntax"/></returns>
        INextPathElementSyntax Property(string name);

        /// <summary>
        /// Add any property element
        /// </summary>
        /// <returns><see cref="INextPathElementSyntax"/></returns>
        INextPathElementSyntax AnyProperty();

        /// <summary>
        /// Add one or multiple properties element
        /// </summary>
        /// <param name="firstName">First property name</param>
        /// <param name="names">Collection of property names</param>
        /// <returns><see cref="INextPathElementSyntax"/></returns>
        INextPathElementSyntax Properties(string firstName, params string[] names);

        /// <summary>
        /// Add one or multiple properties element
        /// </summary>
        /// <param name="names">Collection of property names</param>
        /// <returns><see cref="INextPathElementSyntax"/></returns>
        INextPathElementSyntax Properties(IReadOnlyCollection<string> names);

        /// <summary>
        /// Add array index element
        /// </summary>
        /// <param name="index">Array index</param>
        /// <returns><see cref="INextPathElementSyntax"/></returns>
        INextPathElementSyntax ArrayIndex(int index);

        /// <summary>
        /// Add any array index element
        /// </summary>
        /// <returns><see cref="INextPathElementSyntax"/></returns>
        INextPathElementSyntax AnyArrayIndex();

        /// <summary>
        /// Add one or multiple array indexes element
        /// </summary>
        /// <param name="firstIndex">First array index</param>
        /// <param name="indexes">Collection of array indexes</param>
        /// <returns><see cref="INextPathElementSyntax"/></returns>
        INextPathElementSyntax ArrayIndexes(int firstIndex, params int[] indexes);

        /// <summary>
        /// Add one or multiple array indexes element
        /// </summary>
        /// <param name="indexes">Collection of array indexes</param>
        /// <returns><see cref="INextPathElementSyntax"/></returns>
        INextPathElementSyntax ArrayIndexes(IReadOnlyCollection<int> indexes);

        /// <summary>
        /// Add array slice element
        /// </summary>
        /// <param name="start">Slice start</param>
        /// <param name="end">Slice end</param>
        /// <param name="step">Slice step</param>
        /// <returns><see cref="INextPathElementSyntax"/></returns>
        INextPathElementSyntax ArraySlice(int? start, int? end, int step = 1);

        /// <summary>
        /// Add expression element
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <returns><see cref="INextPathElementSyntax"/></returns>
        INextPathElementSyntax Expression(string expression);

        /// <summary>
        /// Add filter expression element
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <returns><see cref="INextPathElementSyntax"/></returns>
        INextPathElementSyntax FilterExpression(string expression);
    }
}