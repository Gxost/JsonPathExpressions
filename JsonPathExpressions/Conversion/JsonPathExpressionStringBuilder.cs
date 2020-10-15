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

namespace JsonPathExpressions.Conversion
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Elements;

    internal static class JsonPathExpressionStringBuilder
    {
        private static readonly char[] ForbiddenCharactersForSimplePropertyName = { '[', ']', '.', '*' };

        public static string Build(IReadOnlyList<JsonPathElement> elements)
        {
            var builder = new StringBuilder(elements.Count * 5);
            for (int i = 0; i < elements.Count; ++i)
                builder.AppendElement(elements[i], i != 0);

            return builder.ToString();
        }

        public static string Build(JsonPathElement element)
        {
            var builder = new StringBuilder();
            builder.AppendElement(element, false);
            return builder.ToString();
        }

        private static void AppendElement(this StringBuilder builder, JsonPathElement element, bool useDot)
        {
            switch (element)
            {
                case JsonPathRootElement rootElement:
                    builder.AppendRoot();
                    break;
                case JsonPathRecursiveDescentElement recursiveDescentElement:
                    builder.AppendRecursiveDescent(recursiveDescentElement);
                    break;
                case JsonPathPropertyElement propertyElement:
                    builder.AppendProperty(propertyElement, useDot);
                    break;
                case JsonPathAnyPropertyElement anyPropertyElement:
                    builder.AppendAnyProperty(useDot);
                    break;
                case JsonPathPropertyListElement propertyListElement:
                    builder.AppendPropertyList(propertyListElement);
                    break;
                case JsonPathArrayIndexElement arrayIndexElement:
                    builder.AppendArrayIndex(arrayIndexElement);
                    break;
                case JsonPathAnyArrayIndexElement anyArrayIndexElement:
                    builder.AppendAnyArrayIndex();
                    break;
                case JsonPathArrayIndexListElement arrayIndexListElement:
                    builder.AppendArrayIndexList(arrayIndexListElement);
                    break;
                case JsonPathArraySliceElement arraySliceElement:
                    builder.AppendArraySlice(arraySliceElement);
                    break;
                case JsonPathExpressionElement expressionElement:
                    builder.AppendExpression(expressionElement);
                    break;
                case JsonPathFilterExpressionElement filterExpressionElement:
                    builder.AppendFilterExpression(filterExpressionElement);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(element), element, $"Unknown element type: {element.GetType()}");
            }
        }

        private static void AppendRoot(this StringBuilder builder)
        {
            builder.Append('$');
        }

        private static void AppendRecursiveDescent(this StringBuilder builder, JsonPathRecursiveDescentElement element)
        {
            builder.Append("..");
            builder.AppendElement(element.AppliedToElement, false);
        }

        private static void AppendProperty(this StringBuilder builder, JsonPathPropertyElement element, bool useDot)
        {
            bool isSimple = element.Name.Length != 0 && element.Name.IndexOfAny(ForbiddenCharactersForSimplePropertyName) == -1;
            if (isSimple)
            {
                if (useDot)
                    builder.Append('.');

                builder.Append(element.Name);
            }
            else
            {
                builder.Append("['");
                builder.Append(element.Name);
                builder.Append("']");
            }
        }

        private static void AppendAnyProperty(this StringBuilder builder, bool useDot)
        {
            if (useDot)
                builder.Append(".*");
            else
                builder.Append('*');
        }

        private static void AppendPropertyList(this StringBuilder builder, JsonPathPropertyListElement element)
        {
            builder.Append('[');

            bool isFirst = true;
            foreach (string name in element.Names)
            {
                if (isFirst)
                    isFirst = false;
                else
                    builder.Append(',');

                builder.Append('\'');
                builder.Append(name);
                builder.Append('\'');
            }

            builder.Append(']');
        }

        private static void AppendArrayIndex(this StringBuilder builder, JsonPathArrayIndexElement element)
        {
            builder.Append('[');
            builder.Append(element.Index);
            builder.Append(']');
        }

        private static void AppendAnyArrayIndex(this StringBuilder builder)
        {
            builder.Append("[*]");
        }

        private static void AppendArrayIndexList(this StringBuilder builder, JsonPathArrayIndexListElement element)
        {
            builder.Append('[');

            bool isFirst = true;
            foreach (int index in element.Indexes)
            {
                if (isFirst)
                    isFirst = false;
                else
                    builder.Append(',');

                builder.Append(index);
            }

            builder.Append(']');
        }

        private static void AppendArraySlice(this StringBuilder builder, JsonPathArraySliceElement element)
        {
            builder.Append('[');

            builder.AppendNullable(element.Start);
            builder.Append(':');
            builder.AppendNullable(element.End);
            if (element.Step != 1)
            {
                builder.Append(':');
                builder.Append(element.Step);
            }

            builder.Append(']');
        }

        private static void AppendExpression(this StringBuilder builder, JsonPathExpressionElement element)
        {
            builder.Append("[(");
            builder.Append(element.Expression);
            builder.Append(")]");
        }

        private static void AppendFilterExpression(this StringBuilder builder, JsonPathFilterExpressionElement element)
        {
            builder.Append("[?(");
            builder.Append(element.Expression);
            builder.Append(")]");
        }

        private static void AppendNullable(this StringBuilder builder, int? value)
        {
            if (value.HasValue)
                builder.Append(value.Value);
        }
    }
}