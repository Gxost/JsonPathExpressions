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
    using System.Linq;

    /// <summary>
    /// Extension methods for <see cref="JsonPathElement"/>
    /// </summary>
    public static class JsonPathElementExtensions
    {
        /// <summary>
        /// Check if JsonPath element has same type as passed or element is a recursive descent applied to JsonPath element with same type as passed
        /// </summary>
        /// <param name="element">JsonPath element</param>
        /// <param name="type">JsonPath element type to check</param>
        /// <returns>True if JsonPath element has same type as passed or element is a recursive descent applied to JsonPath element with same type as passed</returns>
        public static bool IsOfType(this JsonPathElement element, JsonPathElementType type)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            while (true)
            {
                if (element.Type == type)
                    return true;

                if (!(element is JsonPathRecursiveDescentElement recursiveDescentElement))
                    return false;

                element = recursiveDescentElement.AppliedToElement;
            }
        }

        /// <summary>
        /// Check if JsonPath element has same type as passed or element is a recursive descent applied to JsonPath element with same type as passed
        /// </summary>
        /// <param name="element">JsonPath element</param>
        /// <param name="types">Array of JsonPath element types to check</param>
        /// <returns>True if JsonPath element has same type as passed or element is a recursive descent applied to JsonPath element with same type as passed</returns>
        /// <exception cref="ArgumentException">No element types provided</exception>
        public static bool IsOfType(this JsonPathElement element, params JsonPathElementType[] types)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            if (types == null)
                throw new ArgumentNullException(nameof(types));
            if (types.Length == 0)
                throw new ArgumentException("No element types provided", nameof(types));

            while (true)
            {
                if (types.Contains(element.Type))
                    return true;

                if (!(element is JsonPathRecursiveDescentElement recursiveDescentElement))
                    return false;

                element = recursiveDescentElement.AppliedToElement;
            }
        }

        /// <summary>
        /// Check if JsonPath element has type in a passed range or element is a recursive descent applied to JsonPath element with type in a passed range
        /// </summary>
        /// <param name="element">JsonPath element</param>
        /// <param name="firstType">First JsonPath element type in the range</param>
        /// <param name="lastType">Last JsonPath element type in the range</param>
        /// <returns>True if JsonPath element has type in a passed range or element is a recursive descent applied to element with JsonPath type in a passed range</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="lastType"/> is less than <paramref name="firstType"/></exception>
        public static bool IsOfTypeInRange(this JsonPathElement element, JsonPathElementType firstType, JsonPathElementType lastType)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            if (lastType < firstType)
                throw new ArgumentOutOfRangeException(nameof(lastType), lastType, "Last type is less than first type");

            while (true)
            {
                if (element.Type >= firstType && element.Type <= lastType)
                    return true;

                if (!(element is JsonPathRecursiveDescentElement recursiveDescentElement))
                    return false;

                element = recursiveDescentElement.AppliedToElement;
            }
        }

        /// <summary>
        /// Get non-recursive-descent JsonPath element for passed JsonPath element
        /// </summary>
        /// <param name="element">JsonPath element</param>
        /// <returns>Non-recursive-descent JsonPath element</returns>
        /// <remarks>
        /// If <paramref name="element"/> is <see cref="JsonPathRecursiveDescentElement"/> <see cref="JsonPathRecursiveDescentElement.AppliedToElement"/> is returned
        /// </remarks>
        public static JsonPathElement GetUnderlyingElement(this JsonPathElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            return element.Type == JsonPathElementType.RecursiveDescent
                ? ((JsonPathRecursiveDescentElement)element).AppliedToElement
                : element;
        }

        /// <summary>
        /// Cast JsonPath element or element to which recursive descent is applied to specified type
        /// </summary>
        /// <typeparam name="TJsonPathElement">Derived JsonPath element type</typeparam>
        /// <param name="element">JsonPath element</param>
        /// <returns>JsonPath element or element to which recursive descent is applied cast to derived type</returns>
        /// <exception cref="InvalidCastException">Invalid casting from JsonPathElement to <typeparamref name="TJsonPathElement"/></exception>
        public static TJsonPathElement CastTo<TJsonPathElement>(this JsonPathElement element)
            where TJsonPathElement : JsonPathElement
        {
            return typeof(TJsonPathElement) != typeof(JsonPathRecursiveDescentElement) && element is JsonPathRecursiveDescentElement recursiveDescentElement
                ? (TJsonPathElement)recursiveDescentElement.AppliedToElement
                : (TJsonPathElement)element;
        }

        /// <summary>
        /// Try to cast JsonPath element or element to which recursive descent is applied to specified type
        /// </summary>
        /// <typeparam name="TJsonPathElement">Derived JsonPath element type</typeparam>
        /// <param name="element">JsonPath element</param>
        /// <returns>JsonPath element or element to which recursive descent is applied cast to derived type, or null</returns>
        public static TJsonPathElement As<TJsonPathElement>(this JsonPathElement element)
            where TJsonPathElement : JsonPathElement
        {
            return typeof(TJsonPathElement) != typeof(JsonPathRecursiveDescentElement) && element is JsonPathRecursiveDescentElement recursiveDescentElement
                ? recursiveDescentElement.AppliedToElement as TJsonPathElement
                : element as TJsonPathElement;
        }
    }
}