using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace _06
{
    public class UnitTest1
    {
        [Fact]
        public void Part1()
        {
            var result = File.ReadAllText("input.txt").Split("\n\n").Sum(CountGroupAny);

            Assert.Equal(6291, result);
        }

        [Fact]
        public void Part2()
        {
            var result = File.ReadAllText("input.txt").Split("\n\n").Sum(CountGroupAll);

            Assert.Equal(3052, result);
        }

        [Theory]
        [InlineData("abc\n\na\nb\nc\n\nab\nac\n\na\na\na\na\n\nb", 11)]
        public void Sample1(string input, int expected)
        {
            var result = input.Split("\n\n").Sum(CountGroupAny);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("abc\n\na\nb\nc\n\nab\nac\n\na\na\na\na\n\nb", 6)]
        public void Sample2(string input, int expected)
        {
            var result = input.Split("\n\n").Sum(CountGroupAll);

            Assert.Equal(expected, result);
        }

        private int CountGroupAny(string group) =>
            group
                .Split("\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(_ => _.ToCharArray())
                .Aggregate(new HashSet<char>(), (acc, c) =>
                {
                    acc.UnionWith(c);
                    return acc;
                })
                .Count;

        private int CountGroupAll(string group) =>
            group
                .Split("\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(_ => _.ToCharArray())
                .Aggregate(
                    new HashSet<char>(Enumerable.Range('a', 26).Cast<char>()),
                    (acc, c) =>
                    {
                        acc.IntersectWith(c);
                        return acc;
                    })
                .Count;
    }
}
