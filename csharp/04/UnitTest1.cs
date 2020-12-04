using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace _04
{
    public class UnitTest1
    {
        private readonly string[] _eyeColors = new[] { "amb", "blu", "brn", "gry", "grn", "hzl", "oth" };
        private readonly string[] _requiredFields = new[] { "byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid", "cid" };

        [Fact]
        public void Part1()
        {
            var input = File.ReadAllText("input.txt");

            Assert.Equal(254, GetValidPasswordCount(input, IsPassportValid));
        }

        [Fact]
        public void Part2()
        {
            var input = File.ReadAllText("input.txt");

            Assert.Equal(184, GetValidPasswordCount(input, IsPassportValid2));
        }

        [Fact]
        public void Sample()
        {
            var input = File.ReadAllText("sample.txt");

            Assert.Equal(2, GetValidPasswordCount(input, IsPassportValid));
        }

        [Theory]
        [InlineData("eyr:1972 cid:100\nhcl:#18171d ecl:amb hgt:170 pid:186cm iyr:2018 byr:1926", false)]
        [InlineData("iyr:2019\nhcl:#602927 eyr:1967 hgt:170cm\necl:grn pid:012533040 byr:1946", false)]
        [InlineData("hcl:dab227 iyr:2012\necl:brn hgt:182cm pid:021572410 eyr:2020 byr:1992 cid:277", false)]
        [InlineData("hgt:59cm ecl:zzz\neyr:2038 hcl:74454a iyr:2023\npid:3556412378 byr:2007", false)]
        [InlineData("pid:087499704 hgt:74in ecl:grn iyr:2012 eyr:2030 byr:1980\nhcl:#623a2f", true)]
        [InlineData("eyr:2029 ecl:blu cid:129 byr:1989\niyr:2014 pid:896056539 hcl:#a97842 hgt:165cm", true)]
        [InlineData("hcl:#888785\nhgt:164cm byr:2001 iyr:2015 cid:88\npid:545766238 ecl:hzl\neyr:2022", true)]
        [InlineData("iyr:2010 hgt:158cm hcl:#b6652a ecl:blu byr:1944 eyr:2021 pid:093154719", true)]
        public void Sample2(string passportData, bool expected)
        {
            Assert.Equal(expected, IsPassportValid2(ParsePassport(passportData)));
        }

        private int GetValidPasswordCount(string input, Func<Passport, bool> validator)
        {
            var passportSegments = input.Split("\n\n");

            var passports = passportSegments.Select(ParsePassport);

            var valid = passports.Where(validator).ToList();

            return passports.Count(validator);
        }

        private Passport ParsePassport(string passportData)
        {
            var data = passportData.Split(new[] { " ", "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Split(':')).ToDictionary(_ => _[0], _ => _[1]);
            return new Passport(data);
        }

        private bool IsPassportValid(Passport p)
        {
            var diff = _requiredFields.Except(p.Data.Keys).ToList();

            return diff.Count == 0 || (diff.Count == 1 && diff[0] == "cid");
        }

        private bool IsPassportValid2(Passport p)
        {
            var diff = _requiredFields.Except(p.Data.Keys).ToList();

            if (diff.Count > 1 || (diff.Count == 1 && diff[0] != "cid"))
            {
                return false;
            }

            var byrValid = ValidateNumeric(p.Data["byr"], 4, 1920, 2002);
            var iyrValid = ValidateNumeric(p.Data["iyr"], 4, 2010, 2020);
            var eyrValid = ValidateNumeric(p.Data["eyr"], 4, 2020, 2030);
            var hgtValid = ValidateHeight(p.Data["hgt"]);
            var hclValid = Regex.IsMatch(p.Data["hcl"], "^#[0-9a-f]{6}$");
            var eclValid = ValidateEyes(p.Data["ecl"]);
            var pidValid = Regex.IsMatch(p.Data["pid"], "^[0-9]{9}$");

            return byrValid && iyrValid && eyrValid && hgtValid && hclValid && eclValid && pidValid;
        }

        private bool ValidateEyes(string data) => _eyeColors.Contains(data);

        private static bool ValidateHeight(string data) => data switch
        {
            string cms when cms.EndsWith("cm") => int.TryParse(cms[0..^2], out var c) && c is >= 150 and <= 193,
            string inches when inches.EndsWith("in") => int.TryParse(inches[0..^2], out var i) && i is >= 59 and <= 76,
            _ => false
        };

        private static bool ValidateNumeric(string data, int length, int min, int max) =>
            data.Length == length && int.TryParse(data, out var v) && min <= v && v <= max;

        public record Passport(Dictionary<string, string> Data);
    }
}
