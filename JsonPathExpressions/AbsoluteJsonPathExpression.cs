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
    using Builders;
    using Conversion;
    using Elements;

    /// <summary>
    /// JsonPath expression starting with <see cref="JsonPathRootElement"/>
    /// </summary>
    public sealed class AbsoluteJsonPathExpression : JsonPathExpression, IEquatable<AbsoluteJsonPathExpression>
    {
        /// <summary>
        /// Create <see cref="AbsoluteJsonPathExpression"/> instance from string presentation
        /// </summary>
        /// <param name="jsonPath">JsonPath expression string</param>
        public AbsoluteJsonPathExpression(string jsonPath)
            : this(JsonPathExpressionStringParser.Parse(jsonPath))
        {
        }

        /// <summary>
        /// Create <see cref="AbsoluteJsonPathExpression"/> instance with passed elements
        /// </summary>
        /// <param name="elements">Collection of JsonPath elements</param>
        /// <exception cref="ArgumentException">Empty elements collection provided</exception>
        public AbsoluteJsonPathExpression(IReadOnlyCollection<JsonPathElement> elements)
            : base(elements, true)
        {
        }

        /// <summary>
        /// Gets absolute JsonPath expression builder
        /// </summary>
        public static new IFirstAbsolutePathElementSyntax Builder => AbsoluteJsonPathExpressionBuilder.Create();

        /// <inheritdoc cref="IEquatable{T}"/>
        public bool Equals(AbsoluteJsonPathExpression other)
        {
            return Equals((JsonPathExpression)other);
        }

        /// <summary>
        /// Convert <see cref="string"/> to <see cref="AbsoluteJsonPathExpression"/>
        /// </summary>
        /// <param name="path">String representing JsonPath expression</param>
        /// <remarks>
        /// Null <see cref="string"/> is converted to null <see cref="AbsoluteJsonPathExpression"/>
        /// </remarks>
        public static explicit operator AbsoluteJsonPathExpression(string path)
        {
            if (path == null)
                return null;

            return new AbsoluteJsonPathExpression(path);
        }

        /// <inheritdoc />
        protected internal override JsonPathExpression Create(IReadOnlyCollection<JsonPathElement> elements)
        {
            return new AbsoluteJsonPathExpression(elements);
        }
    }
}