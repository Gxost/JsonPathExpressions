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
    /// JsonPath element representing any array index
    /// </summary>
    public sealed class JsonPathAnyArrayIndexElement : JsonPathElement, IEquatable<JsonPathAnyArrayIndexElement>
    {
        /// <inheritdoc />
        public override JsonPathElementType Type => JsonPathElementType.AnyArrayIndex;

        /// <inheritdoc />
        public override bool IsStrict => false;

        /// <inheritdoc />
        public override bool IsNormalized => true;

        /// <inheritdoc />
        public override JsonPathElement GetNormalized()
        {
            return this;
        }

        /// <inheritdoc />
        public override bool? Matches(JsonPathElement element)
        {
            if (element is null)
                throw new ArgumentNullException(nameof(element));

            switch (element.Type)
            {
                case JsonPathElementType.AnyArrayIndex:
                case JsonPathElementType.ArrayIndex:
                case JsonPathElementType.ArrayIndexList:
                case JsonPathElementType.ArraySlice:
                case JsonPathElementType.Expression:
                    return true;
                default:
                    return false;
            }
        }

        /// <inheritdoc cref="IEquatable{T}"/>
        public bool Equals(JsonPathAnyArrayIndexElement other)
        {
            return !(other is null);
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

            return Equals((JsonPathAnyArrayIndexElement) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }
    }
}