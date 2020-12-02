using System;
using System.IO;
using System.Linq;
using Xunit;

namespace _02
{
    public class UnitTest1
    {
        [Fact]
        public void Part1()
        {
            var entries = File.ReadAllLines("input.txt")
                              .Select(_ => _.Split(" "))
                              .Select(_ => new Entry(int.Parse(_[0].Split("-")[0]), int.Parse(_[0].Split("-")[1]), _[1][0], _[2]));

            var validCount = entries.Count(IsPasswordValid1);

            Assert.Equal(493, validCount);
        }

        [Fact]
        public void Part2()
        {
            var entries = File.ReadAllLines("input.txt")
                              .Select(_ => _.Split(" "))
                              .Select(_ => new Entry(int.Parse(_[0].Split("-")[0]), int.Parse(_[0].Split("-")[1]), _[1][0], _[2]));

            var validCount = entries.Count(IsPasswordValid2);

            Assert.Equal(593, validCount);
        }

        [Theory]
        [InlineData("1-3 a: abcde", true)]
        [InlineData("1-3 b: cdefg", false)]
        [InlineData("2-9 c: ccccccccc", true)]
        public void Sample1(string input, bool expectedResult)
        {
            var entries = new[] { input }.Select(_ => _.Split(" "))
                                         .Select(_ => new Entry(int.Parse(_[0].Split("-")[0]), int.Parse(_[0].Split("-")[1]), _[1][0], _[2]));

            var validCount = entries.Count(IsPasswordValid1);

            Assert.Equal(expectedResult ? 1 : 0, validCount);
        }

        [Theory]
        [InlineData("1-3 a: abcde", true)]
        [InlineData("1-3 b: cdefg", false)]
        [InlineData("2-9 c: ccccccccc", false)]
        public void Sample2(string input, bool expectedResult)
        {
            var entries = new[] { input }.Select(_ => _.Split(" "))
                                         .Select(_ => new Entry(int.Parse(_[0].Split("-")[0]), int.Parse(_[0].Split("-")[1]), _[1][0], _[2]));

            var validCount = entries.Count(IsPasswordValid2);

            Assert.Equal(expectedResult ? 1 : 0, validCount);
        }

        private bool IsPasswordValid1(Entry e)
        {
            var count = e.Password.Count(_ => _ == e.C);

            return e.Min <= count && count <= e.Max;
        }

        private bool IsPasswordValid2(Entry e) => (e.Password[e.Min - 1] == e.C) ^ (e.Password[e.Max - 1] == e.C);

        private record Entry(int Min, int Max, char C, string Password);
    }
}
