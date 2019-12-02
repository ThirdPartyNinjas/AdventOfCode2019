using System;
using System.IO;

namespace AdventOfCode2019
{
    public class Program
    {
        public static void Main(string[] _)
        {
            Day02b();
        }

        private static void Day02b()
        {
            int[] input = {1,0,0,3,1,1,2,3,1,3,4,3,1,5,0,3,2,9,1,19,1,19,5,23,1,23,6,27,2,9,27,31,1,5,31,35,1,35,10,
                39,1,39,10,43,2,43,9,47,1,6,47,51,2,51,6,55,1,5,55,59,2,59,10,63,1,9,63,67,1,9,67,71,2,71,6,75,1,5,75,
                79,1,5,79,83,1,9,83,87,2,87,10,91,2,10,91,95,1,95,9,99,2,99,9,103,2,10,103,107,2,9,107,111,1,111,5,
                115,1,115,2,119,1,119,6,0,99,2,0,14,0};

            int[] program = new int[input.Length];
            int noun = 0;
            int verb = 0;

            do
            {
                input.CopyTo(program, 0);

                program[1] = noun;
                program[2] = verb;

                int currentPosition = 0;

                while (program[currentPosition] != 99)
                {
                    if (program[currentPosition] == 1)
                    {
                        program[program[currentPosition + 3]] = program[program[currentPosition + 1]] + program[program[currentPosition + 2]];
                    }
                    else if (program[currentPosition] == 2)
                    {
                        program[program[currentPosition + 3]] = program[program[currentPosition + 1]] * program[program[currentPosition + 2]];
                    }
                    currentPosition += 4;
                }

                if (program[0] == 19690720)
                {
                    break;
                }
                else
                {
                    noun++;
                    if(noun > 99)
                    {
                        noun = 0;
                        verb++;
                    }
                }
            } while (true);

            Console.WriteLine($"Day 2a: {100 * noun + verb}");
        }

        private static void Day02a()
        {
            int[] input = {1,0,0,3,1,1,2,3,1,3,4,3,1,5,0,3,2,9,1,19,1,19,5,23,1,23,6,27,2,9,27,31,1,5,31,35,1,35,10,
                39,1,39,10,43,2,43,9,47,1,6,47,51,2,51,6,55,1,5,55,59,2,59,10,63,1,9,63,67,1,9,67,71,2,71,6,75,1,5,75,
                79,1,5,79,83,1,9,83,87,2,87,10,91,2,10,91,95,1,95,9,99,2,99,9,103,2,10,103,107,2,9,107,111,1,111,5,
                115,1,115,2,119,1,119,6,0,99,2,0,14,0};

            input[1] = 12;
            input[2] = 2;

            int currentPosition = 0;

            while(input[currentPosition] != 99)
            {
                if(input[currentPosition] == 1)
                {
                    input[input[currentPosition + 3]] = input[input[currentPosition + 1]] + input[input[currentPosition + 2]];
                }
                else if(input[currentPosition] == 2)
                {
                    input[input[currentPosition + 3]] = input[input[currentPosition + 1]] * input[input[currentPosition + 2]];
                }
                currentPosition += 4;
            }
            Console.WriteLine($"Day 2a: {input[0]}");
        }

        private static void Day01b()
        {
            int CalculateFuel(int weight)
            {
                var fuel = weight / 3 - 2;

                if (fuel <= 0)
                    return 0;

                return fuel + CalculateFuel(fuel);
            }

            using (var file = File.OpenText("Input/day01.txt"))
            {
                string line;
                int total = 0;

                while ((line = file.ReadLine()) != null)
                {
                    if (int.TryParse(line, out int value))
                    {
                        total += CalculateFuel(value);
                    }
                }
                Console.WriteLine($"Day 1b: {total}");
            }
        }

        private static void Day01a()
        {
            using (var file = File.OpenText("Input/day01.txt"))
            {
                string line;
                int total = 0;

                while ((line = file.ReadLine()) != null)
                {
                    if (int.TryParse(line, out int value))
                    {
                        total += value / 3 - 2;
                    }
                }
                Console.WriteLine($"Day 1a: {total}");
            }
        }
    }
}