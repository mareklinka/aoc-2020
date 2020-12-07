using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace _07
{
    public class UnitTest1
    {
        private readonly Regex _r = new("(?<count>[0-9]+) (?<color>.+?) bag(s)?", RegexOptions.Compiled);

        [Fact]
        public void Part1()
        {
            var input = File.ReadAllLines("input.txt");

            Assert.Equal(278, CountContainingBags(ParseParentTree(input), "shiny gold"));
        }

        [Fact]
        public void Part2()
        {
            var input = File.ReadAllLines("input.txt");

            Assert.Equal(45157, CountContainedBags(ParseChildrenTree(input), "shiny gold"));
        }

        [Fact]
        public void Sample1()
        {
            var input = File.ReadAllLines("sample.txt");

            Assert.Equal(4, CountContainingBags(ParseParentTree(input), "shiny gold"));
        }

        [Fact]
        public void Sample2()
        {
            var input = File.ReadAllLines("sample.txt");

            Assert.Equal(32, CountContainedBags(ParseChildrenTree(input), "shiny gold"));
        }

        private int CountContainedBags(Dictionary<string, List<(int, string)>> tree, string startingColor)
        {
            if (tree.TryGetValue(startingColor, out var list))
            {
                return list.Sum(_ => _.Item1 + (_.Item1 * CountContainedBags(tree, _.Item2)));
            }

            return 0;
        }

        private static int CountContainingBags(Dictionary<string, List<string>> tree, string startingColor)
        {
            var q = new Queue<string>(tree[startingColor]);
            var visited = new HashSet<string>(tree[startingColor]);
            var count = 0;

            while (q.Count > 0)
            {
                var color = q.Dequeue();

                ++count;

                if (tree.TryGetValue(color, out var list))
                {
                    foreach (var parent in list.Where(_ => !visited.Contains(_)))
                    {
                        visited.Add(parent);
                        q.Enqueue(parent);
                    }
                }
            }

            return count;
        }

        private Dictionary<string, List<string>> ParseParentTree(string[] lines)
        {
            var mapToParent = new Dictionary<string, List<string>>();

            foreach (var line in lines)
            {
                var outerColor = GetOuterColor(line);
                var innerColors = GetInnerColors(line);

                foreach (var (_, color) in innerColors)
                {
                    if (mapToParent.TryGetValue(color, out var list))
                    {
                        list.Add(outerColor);
                    }
                    else
                    {
                        mapToParent.Add(color, new List<string> { outerColor });
                    }
                }
            }

            return mapToParent;
        }

        private Dictionary<string, List<(int, string)>> ParseChildrenTree(string[] lines)
        {
            var mapToParent = new Dictionary<string, List<(int, string)>>();

            foreach (var line in lines)
            {
                var outerColor = GetOuterColor(line);
                var innerColors = GetInnerColors(line);

                foreach (var (count, color) in innerColors)
                {
                    if (mapToParent.TryGetValue(outerColor, out var list))
                    {
                        list.Add((count, color));
                    }
                    else
                    {
                        mapToParent.Add(outerColor, new List<(int, string)> { (count, color) });
                    }
                }
            }

            return mapToParent;
        }

        private (int, string)[] GetInnerColors(string line)
        {
            var matches = _r.Matches(line);

            return matches.Cast<Match>().Select(_ => (int.Parse(_.Groups["count"].Value), _.Groups["color"].Value)).ToArray();
        }

        private static string GetOuterColor(string line)
        {
            var endIndex = 0;
            var spaces = 0;
            while (spaces < 2)
            {
                if (line[endIndex] == ' ')
                {
                    ++spaces;
                }

                ++endIndex;
            }

            return line[0..(endIndex - 1)];
        }
    }
}
