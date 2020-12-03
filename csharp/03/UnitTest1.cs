using System.IO;
using Xunit;

namespace _03
{
    public class UnitTest1
    {
        [Fact]
        public void Part1()
        {
            var input = File.ReadAllLines("input.txt");

            var count = CountTrees(input, 3, 1);

            Assert.Equal(153, count);
        }

        [Fact]
        public void Part2()
        {
            var input = File.ReadAllLines("input.txt");

            var count = CountTrees(input, 1, 1)
                        * CountTrees(input, 3, 1)
                        * CountTrees(input, 5, 1)
                        * CountTrees(input, 7, 1)
                        * CountTrees(input, 1, 2);

            Assert.Equal(0, count);
        }

        [Theory]
        [InlineData(1, 1, 2)]
        [InlineData(3, 1, 7)]
        [InlineData(5, 1, 3)]
        [InlineData(7, 1, 4)]
        [InlineData(1, 2, 2)]
        public void Sample(int slopeX, int slopeY, int expected)
        {
            var input = File.ReadAllLines("sample.txt");

            var count = CountTrees(input, slopeX, slopeY);

            Assert.Equal(expected, count);
        }

        private static long CountTrees(string[] input, int slopeX, int slopeY)
        {
            var width = input[0].Length;
            var height = input.Length;

            var x = 0;
            var y = 0;
            var count = 0L;

            while (y < height - 1)
            {
                x = (x + slopeX) % width;
                y += slopeY;

                if (input[y][x] == '#')
                {
                    ++count;
                }
            }

            return count;
        }
    }
}
