#region License
// MIT License
//
// Copyright (c) 2026 Oleksandr Banakh
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

namespace JsonPathExpressions.Utils;

using System;
using System.Collections.Generic;

internal static class Collection
{
#if NET9_0_OR_GREATER
    public static IReadOnlyCollection<T> Concatenate<T>(T head, ReadOnlySpan<T> tail)
    {
        var result = new List<T>(tail.Length + 1) { head };
        result.AddRange(tail);

        return result;
    }
#endif

    // could be replaced with LINQ Prepend but Prepend is missing in .NET 4.6.1
    public static IReadOnlyCollection<T> Concatenate<T>(T head, IReadOnlyCollection<T> tail)
    {
        var result = new List<T>(tail.Count + 1) { head };
        result.AddRange(tail);

        return result;
    }

#if NET9_0_OR_GREATER
    public static IReadOnlyCollection<T> Concatenate<T>(IReadOnlyCollection<T> first, ReadOnlySpan<T> second)
    {
        var result = new List<T>(first.Count + second.Length);
        result.AddRange(first);
        result.AddRange(second);

        return result;
    }
#endif

    public static IReadOnlyCollection<T> Concatenate<T>(IReadOnlyCollection<T> first, IReadOnlyCollection<T> second)
    {
        var result = new List<T>(first.Count + second.Count);
        result.AddRange(first);
        result.AddRange(second);

        return result;
    }
}