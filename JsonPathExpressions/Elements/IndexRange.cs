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
    using System.Collections.Generic;

    internal class IndexRange
    {
        public IndexRange(int? start, int? end, int step)
        {
            if (start < 0)
                throw new ArgumentOutOfRangeException(nameof(start), start, "Start is negative");
            if (end < 0)
                throw new ArgumentOutOfRangeException(nameof(end), end, "End is negative");
            if (step == 0)
                throw new ArgumentOutOfRangeException(nameof(step), step, "Step is zero");

            if (step > 0)
            {
                Left = start ?? 0;
                Right = end - 1 ?? int.MaxValue;
            }
            else
            {
                Left = end + 1 ?? 0;
                Right = start ?? 0;
            }

            Step = step;
        }

        public int Left { get; }
        public int Right { get; }
        public int Step { get; }

        public int Start => Step > 0 ? Left : Right;
        public bool ContainsAllIndexes => Left == 0 && Right == int.MaxValue && Step == 1;
        public int IndexCount => Right >= Left
            ? (Right - Left) / Math.Abs(Step) + 1
            : 0;

        public bool Contains(int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), index, "Index must not be negative");

            return index >= Left
                   && index <= Right
                   && (index - Start) % Step == 0;
        }

        public bool Contains(IndexRange other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            // check steps
            if (Math.Abs(Step) != Math.Abs(other.Step))
            {
                if (Math.Abs(Step) > Math.Abs(other.Step))
                {
                    if (Step % other.Step != 0)
                        return false;
                }
                else
                {
                    if (other.Step % Step != 0)
                        return false;
                }
            }

            // check left
            if (other.Left < Left)
            {
                if (other.Step > 0)
                    return false;
                if (other.Left - Left <= other.Step)
                    return false;
                for (int i = other.Left; i < Left; ++i)
                {
                    if ((other.Right - i) % other.Step == 0)
                        return false;
                }
            }

            // check right
            if (other.Right > Right)
            {
                if (other.Step < 0)
                    return false;

                if (other.Right - Right >= other.Step)
                    return false;
                for (int i = other.Right; i > Right; --i)
                {
                    if ((i - other.Left) % other.Step == 0)
                        return false;
                }
            }

            return (other.Start - Start) % Step == 0;
        }

        public IEnumerable<int> GetIndexes()
        {
            if (Step > 0)
            {
                for (int i = Left; i <= Right; i += Step)
                    yield return i;
            }
            else
            {
                for (int i = Right; i >= Left; i += Step)
                    yield return i;
            }
        }
    }
}