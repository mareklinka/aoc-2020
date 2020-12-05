using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace _05
{
    public class UnitTest1
    {
        [Fact]
        public void Part1()
        {
            Assert.Equal(901, ReadBoardingPasses().Max(s => FindSeatId(s)));
        }

        [Fact]
        public void Part2()
        {
            var existingSeats = new SortedList<int, int>();

            foreach (var s in ReadBoardingPasses())
            {
                var rc = FindSeatId(s);

                existingSeats.Add(rc, rc);
            }

            var previous = existingSeats.Values[0];
            for (var i = 1; i < existingSeats.Count - 1; ++i)
            {
                var current = existingSeats.Values[i];
                if (current != previous + 1)
                {
                    previous = current - 1;
                    break;
                }
                previous = current;
            }

            Assert.Equal(661, previous);
        }

        [Theory]
        [InlineData("FBFBBFFRLR", 357)]
        [InlineData("BFFFBBFRRR", 567)]
        [InlineData("FFFBBBFRRR", 119)]
        [InlineData("BBFFBBFRLL", 820)]
        public void Sample(string seat, int id)
        {
            var result = FindSeatId(seat);
            Assert.Equal(id, result);
        }

        private static IEnumerable<char[]> ReadBoardingPasses()
        {
            using var stream = File.OpenRead("input.txt");
            using var reader = new StreamReader(stream);

            var buffer = new char[10];

            while (!reader.EndOfStream)
            {
                reader.ReadBlock(buffer); // seat definition without the new line
                yield return buffer;
                reader.ReadBlock(buffer, 0, 1); // the new line
            }
        }

        private static int FindSeatId(ReadOnlySpan<char> seat)
        {
            var (row, col) = FindSeat(seat);

            return CalculateSeatId(row, col);
        }

        private static int CalculateSeatId(int row, int col) => (row * 8) + col;

        private static (int Row, int Column) FindSeat(ReadOnlySpan<char> seat) => (FindRowId(0, 127, seat[0..7]), FindColId(0, 7, seat[7..]));

        private static int FindRowId(int start, int end, ReadOnlySpan<char> seat) =>
            seat switch
            {
                ReadOnlySpan<char> s when s.IsEmpty => start,
                ReadOnlySpan<char> s when s[0] == 'F' => FindRowId(start, end - ((end - start) / 2) - 1, seat[1..]),
                ReadOnlySpan<char> s when s[0] == 'B' => FindRowId(start + ((end - start) / 2) + 1, end, seat[1..]),
                _ => throw new InvalidOperationException()
            };

        private static int FindColId(int start, int end, ReadOnlySpan<char> seat) =>
            seat switch
            {
                ReadOnlySpan<char> s when s.IsEmpty => start,
                ReadOnlySpan<char> s when s[0] == 'L' => FindColId(start, end - ((end - start) / 2) - 1, seat[1..]),
                ReadOnlySpan<char> s when s[0] == 'R' => FindColId(start + ((end - start) / 2) + 1, end, seat[1..]),
                _ => throw new InvalidOperationException()
            };
    }
}
