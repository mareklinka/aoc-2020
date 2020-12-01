using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace _01
{
    public class UnitTest1
    {
        [Fact]
        public void Part1()
        {
            var input = File.ReadAllLines("input.txt").Select(int.Parse).ToList();
            var result = FindProduct(input, 0, 2020, 1);

            Assert.Equal(494475, result);
        }

        [Fact]
        public void Part2()
        {
            var input = File.ReadAllLines("input.txt").Select(int.Parse).ToList();
            var result = FindProduct(input, 0, 2020, 2);

            Assert.Equal(267520550, result);
        }

        [Theory]
        [InlineData("1721,979,366,299,675,1456", 1, 514579)]
        [InlineData("1721,979,366,299,675,1456", 2, 241861950)]
        public void Sample(string data, int depth, int expected)
        {
            var input = data.Split(",").Select(int.Parse).ToList();

            Assert.Equal(expected, FindProduct(input, 0, 2020, depth));
        }

        private int FindProduct(List<int> input, int startIndex, int targetSum, int depth)
        {
            for (var i = startIndex; i < input.Count; ++i)
            {
                var x = input[i];

                if (depth == 0)
                {
                    // no more recursion possible - targetSum must be an element in input
                    if (x == targetSum)
                    {
                        return x;
                    }
                }
                else
                {
                    // recursion possible - descend
                    var result = FindProduct(input, i + 1, targetSum - x, depth - 1);

                    if (result != 0)
                    {
                        return x * result;
                    }
                }
            }

            return 0;
        }
    }
}
