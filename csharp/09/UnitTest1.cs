using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace _09
{
    public class UnitTest1
    {
        [Fact]
        public void Part1()
        {
            var input = File.ReadAllLines("input.txt").Select(long.Parse).ToArray();

            Assert.Equal(70639851, FindBrokenPoint(input, 25));
        }

        [Fact]
        public void Part2()
        {
            var input = File.ReadAllLines("input.txt").Select(long.Parse).ToArray();

            Assert.Equal(8249240, FindWeakPoint(input, 25));
        }

        [Fact]
        public void Sample1()
        {
            var input = File.ReadAllLines("sample.txt").Select(long.Parse).ToArray();

            Assert.Equal(127, FindBrokenPoint(input, 5));
        }

        [Fact]
        public void Sample2()
        {
            var input = File.ReadAllLines("sample.txt").Select(long.Parse).ToArray();

            Assert.Equal(62, FindWeakPoint(input, 5));
        }

        private static long FindWeakPoint(long[] input, int preamble)
        {
            var brokenPoint = FindBrokenPoint(input, preamble);
            var total = input.Sum();

            // for this task, we are looking to split the input list into three parts
            // these three parts must sum up to the list's total
            // to speed it up, we can precompute head- and tail-sums for the list
            // at index i the headSums list contains a sum of elements [0, i]
            // at index i the tailSums list contains a sum of elements [i, end]
            var headSums = new List<long> { input[0] };
            var tailSums = new List<long> { total };

            for (var i = 1; i < input.Length - 1; ++i)
            {
                var s = headSums[^1] + input[i];
                headSums.Add(s);
                tailSums.Add(total - s);
            }

            // one portion of the list must sum up to this number
            var breakPoint = FindBrokenPoint(input, preamble);

            // what we are looking for: indices i and j where sum([0, i]) + sum([j, end]) + breakPoint = total
            // at the same time, i + 1 < j => j - i > 1 (the middle element must have at least 1 element) =>
            // total - sum([0, i]) - sum([j, end]) = breakPoint
            for (var i = 0; i < headSums.Count; ++i)
            {
                for (var j = i + 2; j < tailSums.Count; ++j)
                {
                    if (total - headSums[i] - tailSums[j] == breakPoint)
                    {
                        var span = input[(i + 1)..(j + 1)];
                        return span.Min() + span.Max();
                    }
                }
            }

            throw new InvalidOperationException();
        }

        private static long FindBrokenPoint(long[] input, int preamble)
        {
            var inputBuffer = new LinkedList<long>(input[0..preamble]);

            var combinations = new Dictionary<long, int>();

            // the trick here is to maintain a cache of all the allowed values
            // first we compute the valid options for the preamble list
            // it is important to note that a combination might be given by multiple combinations
            // therefore we need a multi-set here (simulated via dictionary)
            for (var i = 0; i < inputBuffer.Count; ++i)
            {
                for (var j = i + 1; j < inputBuffer.Count; ++j)
                {
                    if (combinations.TryGetValue(input[i] + input[j], out var c))
                    {
                        combinations[input[i] + input[j]] = c + 1;
                    }
                    else
                    {
                        combinations.Add(input[i] + input[j], 1);
                    }
                }
            }

            // for every element after the preamble, we check whether it's valid and update the buffers
            foreach (var n in input.Skip(preamble))
            {
                if (!ValidateNext(inputBuffer, combinations, n))
                {
                    return n;
                }
            }

            throw new InvalidOperationException();
        }

        private static bool ValidateNext(LinkedList<long> inputBuffer, Dictionary<long, int> cache, long incomingNumber)
        {
            if (!cache.TryGetValue(incomingNumber, out var _))
            {
                // invalid element, we are done
                return false;
            }

            // this is the element that is leaving the buffer - we can remove all combinations created by it
            var dropped = inputBuffer.First.Value;
            foreach (var n in inputBuffer.Skip(1))
            {
                var c = cache[dropped + n];
                if (c == 1)
                {
                    cache.Remove(dropped + n);
                }
                else
                {
                    cache[dropped + n] = c - 1;
                }
            }

            // remove the old element and add the new one
            inputBuffer.RemoveFirst();
            inputBuffer.AddLast(incomingNumber);

            // add all combinations created by the new item into the cache
            foreach (var n in inputBuffer.Take(inputBuffer.Count - 1))
            {
                if (cache.TryGetValue(incomingNumber + n, out var c))
                {
                    cache[incomingNumber + n] = c + 1;
                }
                else
                {
                    cache.Add(incomingNumber + n, 1);
                }
            }

            return true;
        }
    }
}
