namespace JsonPathExpressions.Tests.Elements
{
    using FluentAssertions;
    using JsonPathExpressions.Elements;
    using Xunit;

    public class IndexRangeTests
    {
        [Theory]
        [InlineData(0, 0, 1, 0)]
        [InlineData(0, 1, 1, 1)]
        [InlineData(0, 1, 2, 1)]
        [InlineData(0, 10, 3, 4)]
        [InlineData(1, 0, -1, 1)]
        [InlineData(10, 0, -3, 4)]
        public void IndexCount(int? start, int? end, int step, int expected)
        {
            var range = new IndexRange(start, end, step);

            range.IndexCount.Should().Be(expected);
        }
    }
}