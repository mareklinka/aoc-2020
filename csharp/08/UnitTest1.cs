using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace _08
{
    public class UnitTest1
    {
        [Theory]
        [InlineData("sample.txt", 5)]
        [InlineData("input.txt", 1867)]
        public void Part1(string file, int expected)
        {
            var code = File.ReadAllLines(file).Select(ParseInstruction).ToArray();
            var visited = new HashSet<InstructionPointer>();

            var gameConsole = new HandheldGameConsole(stopFunction: ip => !visited.Add(ip) ? Termination.InfiniteLoop : Termination.None);
            var (result, _) = gameConsole.Run(code);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("sample.txt", 8)]
        [InlineData("input.txt", 1303)]
        public void Part2(string file, int expected)
        {
            var code = File.ReadAllLines(file).Select(ParseInstruction).ToArray();
            var visited = new HashSet<InstructionPointer>();

            var gameConsole = new HandheldGameConsole(stopFunction: ip => !visited.Add(ip) ? Termination.InfiniteLoop : Termination.None);
            var result = FindCorrectMutation(code);

            Assert.Equal(expected, result);
        }

        private static int FindCorrectMutation(Instruction[] code)
        {
            var visited = new HashSet<InstructionPointer>();
            var gameConsole = new HandheldGameConsole(stopFunction: ip => !visited.Add(ip) ? Termination.InfiniteLoop : Termination.None);

            for (var i = 0; i < code.Length; ++i)
            {
                if (code[i].Operation == Operation.Acc)
                {
                    continue;
                }

                visited.Clear();
                var originalInstruction = code[i];
                code[i] = SwapInstruction(originalInstruction);

                var (result, termination) = gameConsole.Run(code);

                if (termination == Termination.RanToCompletion)
                {
                    return result;
                }

                code[i] = originalInstruction;
            }

            throw new InvalidOperationException();
        }

        private static Instruction SwapInstruction(Instruction originalInstruction) =>
            originalInstruction switch
            {
                (Operation.Nop, var arg) => new(Operation.Jmp, arg),
                (Operation.Jmp, var arg) => new(Operation.Nop, arg),
                _ => throw new InvalidOperationException()
            };

        private static Instruction ParseInstruction(string loc) =>
            loc[0..3] switch
            {
                "nop" => new Instruction(Operation.Nop, 0),
                "acc" => new Instruction(Operation.Acc, int.Parse(loc[4..])),
                "jmp" => new Instruction(Operation.Jmp, int.Parse(loc[4..])),
                _ => throw new InvalidOperationException()
            };

        public class HandheldGameConsole
        {
            private InstructionPointer _ip = new(0);
            private int _accumulator;
            private readonly Func<InstructionPointer, Termination> _stopFunction;

            public HandheldGameConsole(Func<InstructionPointer, Termination> stopFunction)
            {
                _stopFunction = stopFunction;
            }

            public (int, Termination) Run(Instruction[] code)
            {
                _accumulator = 0;
                _ip = new(0);

                while (true)
                {
                    var termination = _stopFunction(_ip);

                    if (termination != Termination.None)
                    {
                        return (_accumulator, termination);
                    }

                    if (_ip.Value == code.Length)
                    {
                        return (_accumulator, Termination.RanToCompletion);
                    }

                    var currentInstruction = code[_ip.Value];

                    (_ip, _accumulator) = currentInstruction switch
                    {
                        (Operation.Nop, _) => (_ip with { Value = _ip.Value + 1 }, _accumulator),
                        (Operation.Acc, var arg) => (_ip with { Value = _ip.Value + 1 }, _accumulator + arg),
                        (Operation.Jmp, var arg) => (_ip with { Value = _ip.Value + arg }, _accumulator),
                        _ => throw new InvalidOperationException()
                    };
                }
            }
        }

        public enum Operation
        {
            Nop,
            Acc,
            Jmp
        }

        public enum Termination
        {
            None,
            InfiniteLoop,
            RanToCompletion
        }

        public record Instruction(Operation Operation, int Argument);
        public record InstructionPointer(int Value);
    }
}
