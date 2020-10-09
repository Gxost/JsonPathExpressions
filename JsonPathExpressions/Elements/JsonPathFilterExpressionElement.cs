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

namespace JsonPathExpressions.Elements
{
    using System;

    /// <summary>
    /// JsonPath element representing filter expression
    /// </summary>
    /// <remarks>
    /// Filer expressions are used to filter array items by their properties
    /// </remarks>
    public sealed class JsonPathFilterExpressionElement : JsonPathElement, IEquatable<JsonPathFilterExpressionElement>
    {
        /// <summary>
        /// Create <see cref="JsonPathFilterExpressionElement"/> instance
        /// </summary>
        /// <param name="expression">Filter expression</param>
        public JsonPathFilterExpressionElement(string expression)
        {
            if (string.IsNullOrEmpty(expression))
                throw new ArgumentNullException(nameof(expression));

            Expression = expression;
        }

        /// <inheritdoc />
        public override JsonPathElementType Type => JsonPathElementType.FilterExpression;

        /// <inheritdoc />
        public override bool IsStrict => false;

        /// <inheritdoc />
        public override bool IsNormalized => true;

        /// <summary>
        /// Gets filter expression
        /// </summary>
        public string Expression { get; }

        /// <inheritdoc />
        public override JsonPathElement GetNormalized()
        {
            return this;
        }

        /// <inheritdoc />
        public override bool? Matches(JsonPathElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            switch (element)
            {
                case JsonPathArrayIndexElement arrayIndexElement:
                case JsonPathAnyArrayIndexElement anyArrayIndexElement:
                case JsonPathArrayIndexListElement arrayIndexListElement:
                case JsonPathArraySliceElement arraySliceElement:
                case JsonPathExpressionElement expressionElement:
                    return null;
                case JsonPathFilterExpressionElement filterExpressionElement:
                    if (Equals(filterExpressionElement))
                        return true;
                    return null;
                default:
                    return false;
            }
        }

        /// <inheritdoc cref="IEquatable{T}"/>
        public bool Equals(JsonPathFilterExpressionElement other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Expression == other.Expression;
        }

        /// <inheritdoc />
        public override bool Equals(JsonPathElement other)
        {
            return Equals((object)other);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;

            return Equals((JsonPathFilterExpressionElement)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Expression.GetHashCode();
                hashCode = (hashCode * 397) ^ GetType().GetHashCode();

                return hashCode;
            }
        }
    }
}