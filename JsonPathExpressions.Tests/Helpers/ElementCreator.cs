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

namespace JsonPathExpressions.Tests.Helpers
{
    using System;
    using JsonPathExpressions.Elements;

    public static class ElementCreator
    {
        public static JsonPathElement CreateAny(JsonPathElementType type)
        {
            switch (type)
            {
                case JsonPathElementType.Root:
                    return new JsonPathRootElement();
                case JsonPathElementType.RecursiveDescent:
                    return new JsonPathRecursiveDescentElement(new JsonPathPropertyElement("recursive~~~"));
                case JsonPathElementType.Property:
                    return new JsonPathPropertyElement("~~~");
                case JsonPathElementType.AnyProperty:
                    return new JsonPathAnyPropertyElement();
                case JsonPathElementType.PropertyList:
                    return new JsonPathPropertyListElement(new []{"~~~0", "~~~1"});
                case JsonPathElementType.ArrayIndex:
                    return new JsonPathArrayIndexElement(int.MaxValue);
                case JsonPathElementType.AnyArrayIndex:
                    return new JsonPathAnyArrayIndexElement();
                case JsonPathElementType.ArrayIndexList:
                    return new JsonPathArrayIndexListElement(new []{int.MaxValue - 1, int.MaxValue - 2});
                case JsonPathElementType.ArraySlice:
                    return new JsonPathArraySliceElement(int.MaxValue - 10, int.MaxValue - 9);
                case JsonPathElementType.Expression:
                    return new JsonPathExpressionElement("~~~expr~~~");
                case JsonPathElementType.FilterExpression:
                    return new JsonPathFilterExpressionElement("~~~filter-expr~~~");
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}