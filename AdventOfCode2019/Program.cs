using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2019
{
    public class Program
    {
        public static void Main(string[] _)
        {
            Day09();
        }

        private static void Day09()
        {
            long[] input;
            using (var file = File.OpenText("Input/day09.txt"))
            {
                string line = file.ReadLine();
                var strings = line.Split(',');
                input = new long[strings.Length];
                for (int i = 0; i < strings.Length; i++)
                    input[i] = long.Parse(strings[i]);
            }

            IntcodeMachine im = new IntcodeMachine(input,
                (out long value) =>
                {
                    value = 1;
                    return true;
                },
                (long value) =>
                {
                    Console.WriteLine("9a: " + value);
                });
            im.RunProgram();

            im.ResetProgram();
            im.SetInputAction(
                (out long value) =>
                {
                    value = 2;
                    return true;
                });
            im.SetOutputAction(
                (long value) =>
                {
                    Console.WriteLine("9b: " + value);
                });
            im.RunProgram();
        }

        private static void Day08a()
        {
            int minimumIndex;
            int minimumZeros = int.MaxValue;
            int minimumOnes = 0;
            int minimumTwos = 0;

            List<int[]> layers = new List<int[]>();

            using (var file = File.OpenText("Input/day08.txt"))
            {
                string line;
                int offset = 0;
                while ((line = file.ReadLine()) != null)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        int zeroes = 0;
                        int ones = 0;
                        int twos = 0;

                        int[] layer = new int[25 * 6];

                        for (int y = 0; y < 6; y++)
                        {
                            for (int x = 0; x < 25; x++)
                            {
                                int pixel = line[offset++] - '0';
                                layer[y * 25 + x] = pixel;
                                if (pixel == 0)
                                    zeroes++;
                                if (pixel == 1)
                                    ones++;
                                if (pixel == 2)
                                    twos++;
                            }
                        }

                        layers.Add(layer);

                        if (zeroes < minimumZeros)
                        {
                            minimumIndex = i;
                            minimumZeros = zeroes;
                            minimumOnes = ones;
                            minimumTwos = twos;
                        }
                    }
                }
            }

            Console.WriteLine("8a: " + (minimumOnes * minimumTwos));

            int[] final = new int[25 * 6];
            for (int y = 0; y < 6; y++)
            {
                for (int x = 0; x < 25; x++)
                {
                    final[y * 25 + x] = 2;
                }
            }

            for (int i = 0; i < layers.Count; i++)
            {
                for (int y = 0; y < 6; y++)
                {
                    for (int x = 0; x < 25; x++)
                    {
                        if (final[y * 25 + x] == 2)
                        {
                            final[y * 25 + x] = layers[i][y * 25 + x];
                        }
                    }
                }
            }

            for (int y = 0; y < 6; y++)
            {
                for (int x = 0; x < 25; x++)
                {
                    switch (final[y * 25 + x])
                    {
                        case 0:
                            Console.Write("X");
                            break;

                        case 1:
                            Console.Write(" ");
                            break;

                        case 2:
                            Console.Write(" ");
                            break;
                    }
                }
                Console.WriteLine("");
            }
        }

        private static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });

            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }

        private static void Day07b()
        {
            long[] input;
            using (var file = File.OpenText("Input/day07.txt"))
            {
                string line = file.ReadLine();
                var strings = line.Split(',');
                input = new long[strings.Length];
                for (int i = 0; i < strings.Length; i++)
                    input[i] = long.Parse(strings[i]);
            }

            List<int> set = new List<int> { 5, 6, 7, 8, 9 };
            var permutations = GetPermutations(set, 5);

            List<long> pipeAB = new List<long>();
            List<long> pipeBC = new List<long>();
            List<long> pipeCD = new List<long>();
            List<long> pipeDE = new List<long>();
            List<long> pipeEA = new List<long>();

            IntcodeMachine imA = new IntcodeMachine(input,
                (out long value) =>
                {
                    if (pipeEA.Count == 0)
                    {
                        value = default;
                        return false;
                    }
                    value = pipeEA[0];
                    pipeEA.RemoveAt(0);
                    return true;
                },
                (long value) =>
                {
                    pipeAB.Add(value);
                });
            IntcodeMachine imB = new IntcodeMachine(input,
                (out long value) =>
                {
                    if (pipeAB.Count == 0)
                    {
                        value = default;
                        return false;
                    }
                    value = pipeAB[0];
                    pipeAB.RemoveAt(0);
                    return true;
                },
                (long value) =>
                {
                    pipeBC.Add(value);
                });
            IntcodeMachine imC = new IntcodeMachine(input,
                (out long value) =>
                {
                    if (pipeBC.Count == 0)
                    {
                        value = default;
                        return false;
                    }
                    value = pipeBC[0];
                    pipeBC.RemoveAt(0);
                    return true;
                },
                (long value) =>
                {
                    pipeCD.Add(value);
                });
            IntcodeMachine imD = new IntcodeMachine(input,
                (out long value) =>
                {
                    if (pipeCD.Count == 0)
                    {
                        value = default;
                        return false;
                    }
                    value = pipeCD[0];
                    pipeCD.RemoveAt(0);
                    return true;
                },
                (long value) =>
                {
                    pipeDE.Add(value);
                });
            IntcodeMachine imE = new IntcodeMachine(input,
                (out long value) =>
                {
                    if (pipeDE.Count == 0)
                    {
                        value = default;
                        return false;
                    }
                    value = pipeDE[0];
                    pipeDE.RemoveAt(0);
                    return true;
                },
                (long value) =>
                {
                    pipeEA.Add(value);
                });

            long maxOutput = int.MinValue;
            foreach (var p in permutations)
            {
                List<int> permutationList = new List<int>(p);

                imA.ResetProgram();
                imB.ResetProgram();
                imC.ResetProgram();
                imD.ResetProgram();
                imE.ResetProgram();

                pipeAB.Add(permutationList[1]);
                pipeBC.Add(permutationList[2]);
                pipeCD.Add(permutationList[3]);
                pipeDE.Add(permutationList[4]);
                pipeEA.Add(permutationList[0]);
                pipeEA.Add(0);

                do
                {
                    imA.RunProgram();
                    imB.RunProgram();
                    imC.RunProgram();
                    imD.RunProgram();
                    if (imE.RunProgram() == IntcodeRunState.Terminated)
                    {
                        break;
                    }
                } while (true);

                if (pipeEA[0] > maxOutput)
                    maxOutput = pipeEA[0];
                pipeEA.Clear();
            }

            Console.WriteLine($"Day 7b: {maxOutput}");
        }

        private static void Day07a()
        {
            long[] input;
            using (var file = File.OpenText("Input/day07.txt"))
            {
                string line = file.ReadLine();
                var strings = line.Split(',');
                input = new long[strings.Length];
                for (int i = 0; i < strings.Length; i++)
                    input[i] = long.Parse(strings[i]);
            }

            List<int> set = new List<int> { 0, 1, 2, 3, 4 };
            var permutations = GetPermutations(set, 5);

            long maxOutput = long.MinValue;
            long output = 0;
            List<long> inputs = new List<long>();

            IntcodeMachine im = new IntcodeMachine(input,
                (out long value) =>
                {
                    value = inputs[0];
                    inputs.RemoveAt(0);
                    return true;
                },
                (long value) =>
                {
                    output = value;
                    if (output > maxOutput)
                        maxOutput = output;
                });

            foreach (var p in permutations)
            {
                List<int> permutationList = new List<int>(p);
                output = 0;
                for (int i = 0; i < 5; i++)
                {
                    int phase = permutationList[i];
                    im.ResetProgram();
                    inputs.Add(phase);
                    inputs.Add(output);
                    im.RunProgram();
                }
            }

            Console.WriteLine($"Day 7a: {maxOutput}");
        }

        public class OrbitNode
        {
            public OrbitNode DirectOrbit { get; set; }
            public string Name { get; set; }
        }

        private static void Day06()
        {
            Dictionary<string, OrbitNode> orbitNodes = new Dictionary<string, OrbitNode>();

            using (var file = File.OpenText("Input/day06.txt"))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    var strings = line.Split(')');

                    // if the named nodes aren't already in the dictionary, go ahead and create them
                    // then hook up the direct orbit we just learned about
                    if (!orbitNodes.ContainsKey(strings[0]))
                        orbitNodes[strings[0]] = new OrbitNode() { Name = strings[0] };
                    if (!orbitNodes.ContainsKey(strings[1]))
                        orbitNodes[strings[1]] = new OrbitNode() { Name = strings[1] };

                    orbitNodes[strings[1]].DirectOrbit = orbitNodes[strings[0]];
                }
            }

            int totalOrbits = 0;
            foreach (var node in orbitNodes.Values)
            {
                var currentNode = node;
                do
                {
                    currentNode = currentNode.DirectOrbit;
                    if (currentNode == null)
                        break;
                    totalOrbits++;
                } while (true);
            }

            Console.WriteLine($"Day 6a: {totalOrbits}");

            // we only have a one directional graph, there's no way to look up orbit chains
            // instead we're going to search from back the "YOU" node until we find a node that's in the "SAN"
            // node's path toward "COM" Once we're there, we just add up the number of jumps it took to reach
            // that shared node

            List<OrbitNode> youList = new List<OrbitNode>();
            {
                var currentNode = orbitNodes["YOU"];
                do
                {
                    currentNode = currentNode.DirectOrbit;
                    if (currentNode == null)
                        break;
                    youList.Add(currentNode);
                } while (true);
            }

            List<OrbitNode> santaList = new List<OrbitNode>();
            {
                var currentNode = orbitNodes["SAN"];
                do
                {
                    currentNode = currentNode.DirectOrbit;
                    if (currentNode == null)
                        break;
                    santaList.Add(currentNode);
                } while (true);
            }

            string sharedNode = "";
            int youCount = 0;
            foreach (var node in youList)
            {
                if (santaList.Find(n => n.Name == node.Name) != null)
                {
                    sharedNode = node.Name;
                    break;
                }
                youCount++;
            }

            int santaCount = 0;
            foreach (var node in santaList)
            {
                if (node.Name == sharedNode)
                {
                    break;
                }
                santaCount++;
            }

            Console.WriteLine($"Day 6b: {youCount + santaCount}");
        }

        private static void Day05()
        {
            long[] input;
            using (var file = File.OpenText("Input/day05.txt"))
            {
                string line = file.ReadLine();
                var strings = line.Split(',');
                input = new long[strings.Length];
                for (int i = 0; i < strings.Length; i++)
                    input[i] = long.Parse(strings[i]);
            }

            IntcodeMachine im = new IntcodeMachine(input,
                (out long value) =>
                {
                    value = 1;
                    return true;
                },
                (long value) =>
                {
                    Console.WriteLine("5a output value: " + value);
                });
            im.RunProgram();

            im.ResetProgram();
            im.SetInputAction(
                (out long value) =>
                {
                    value = 5;
                    return true;
                });
            im.SetOutputAction(
                (long value) =>
                {
                    Console.WriteLine("5b output value: " + value);
                });
            im.RunProgram();
        }

        private static void Day04b()
        {
            bool ContainsDouble(string s)
            {
                for (int i = 0; i < s.Length - 1; i++)
                {
                    if (s[i] == s[i + 1])
                    {
                        if (s.Length > i + 2)
                        {
                            if (s[i + 2] == s[i])
                                continue;
                        }
                        if (i > 0)
                        {
                            if (s[i - 1] == s[i])
                                continue;
                        }

                        return true;
                    }
                }
                return false;
            }

            bool Increasing(string s)
            {
                for (int i = 0; i < s.Length - 1; i++)
                {
                    if (s[i] > s[i + 1])
                        return false;
                }
                return true;
            }

            int count = 0;

            for (int i = 138307; i <= 654504; i++)
            {
                var s = i.ToString();
                if (ContainsDouble(s) && Increasing(s))
                {
                    count++;
                }
            }

            Console.WriteLine($"Day 4b: {count}");
        }

        private static void Day04a()
        {
            bool ContainsDouble(string s)
            {
                for (int i = 0; i < s.Length - 1; i++)
                {
                    if (s[i] == s[i + 1])
                        return true;
                }
                return false;
            }

            bool Increasing(string s)
            {
                for (int i = 0; i < s.Length - 1; i++)
                {
                    if (s[i] > s[i + 1])
                        return false;
                }
                return true;
            }

            int count = 0;

            for (int i = 138307; i <= 654504; i++)
            {
                var s = i.ToString();
                if (ContainsDouble(s) && Increasing(s))
                {
                    count++;
                }
            }

            Console.WriteLine($"Day 4a: {count}");
        }

        private static void Day03b()
        {
            List<(int x, int y, int steps)> GetPositions(string[] instructions)
            {
                int x = 0, y = 0, steps = 0;
                List<(int x, int y, int steps)> positions = new List<(int x, int y, int steps)>();

                foreach (var instruction in instructions)
                {
                    int distance = int.Parse(instruction.Substring(1));
                    int step = 1;
                    switch (instruction[0])
                    {
                        case 'U':
                            step = -1;
                            goto case 'D';
                        case 'D':
                            for (int i = 0; i < distance; i++)
                            {
                                y += step;
                                steps++;
                                positions.Add((x, y, steps));
                            }
                            break;

                        case 'L':
                            step *= -1;
                            goto case 'R';
                        case 'R':
                            for (int i = 0; i < distance; i++)
                            {
                                x += step;
                                steps++;
                                positions.Add((x, y, steps));
                            }
                            break;
                    }
                }
                return positions;
            }

            using (var file = File.OpenText("Input/day03.txt"))
            {
                var positionsA = GetPositions(file.ReadLine().Split(','));
                var positionsB = GetPositions(file.ReadLine().Split(','));

                int minimumSteps = int.MaxValue;
                foreach (var a in positionsA)
                {
                    foreach (var b in positionsB)
                    {
                        if (a.x == b.x && a.y == b.y)
                        {
                            int combinedSteps = a.steps + b.steps;
                            if (combinedSteps < minimumSteps)
                            {
                                minimumSteps = combinedSteps;
                            }
                        }
                    }
                }

                Console.WriteLine($"Day 3b: {minimumSteps}");
            }
        }

        private static void Day03a()
        {
            List<(int x, int y)> GetPositions(string[] instructions)
            {
                int x = 0, y = 0;
                List<(int x, int y)> positions = new List<(int x, int y)>();

                foreach (var instruction in instructions)
                {
                    int distance = int.Parse(instruction.Substring(1));
                    int step = 1;
                    switch (instruction[0])
                    {
                        case 'U':
                            step = -1;
                            goto case 'D';
                        case 'D':
                            for (int i = 0; i < distance; i++)
                            {
                                y += step;
                                positions.Add((x, y));
                            }
                            break;

                        case 'L':
                            step *= -1;
                            goto case 'R';
                        case 'R':
                            for (int i = 0; i < distance; i++)
                            {
                                x += step;
                                positions.Add((x, y));
                            }
                            break;
                    }
                }
                return positions;
            }

            using (var file = File.OpenText("Input/day03.txt"))
            {
                var positionsA = GetPositions(file.ReadLine().Split(','));
                var positionsB = GetPositions(file.ReadLine().Split(','));

                int minimumDistance = int.MaxValue;
                foreach (var a in positionsA)
                {
                    foreach (var b in positionsB)
                    {
                        if (a == b)
                        {
                            int distance = Math.Abs(a.x) + Math.Abs(a.y);
                            if (distance < minimumDistance)
                            {
                                minimumDistance = distance;
                            }
                        }
                    }
                }

                Console.WriteLine($"Day 3a: {minimumDistance}");
            }
        }

        private static void Day02b()
        {
            long[] input = {1,0,0,3,1,1,2,3,1,3,4,3,1,5,0,3,2,9,1,19,1,19,5,23,1,23,6,27,2,9,27,31,1,5,31,35,1,35,10,
                39,1,39,10,43,2,43,9,47,1,6,47,51,2,51,6,55,1,5,55,59,2,59,10,63,1,9,63,67,1,9,67,71,2,71,6,75,1,5,75,
                79,1,5,79,83,1,9,83,87,2,87,10,91,2,10,91,95,1,95,9,99,2,99,9,103,2,10,103,107,2,9,107,111,1,111,5,
                115,1,115,2,119,1,119,6,0,99,2,0,14,0};

            IntcodeMachine im = new IntcodeMachine(input, null, null);
            int noun = 0;
            int verb = 0;

            do
            {
                im.SetMemoryValue(1, noun);
                im.SetMemoryValue(2, verb);
                im.RunProgram();
                long result = im.GetMemoryValue(0);
                if (result == 19690720)
                    break;
                verb++;
                if (verb >= 100)
                {
                    noun++;
                    verb = 0;
                }
                im.ResetProgram();
            } while (true);

            Console.WriteLine($"Day 2b: {100 * noun + verb}");
        }

        private static void Day02a()
        {
            long[] input = {1,0,0,3,1,1,2,3,1,3,4,3,1,5,0,3,2,9,1,19,1,19,5,23,1,23,6,27,2,9,27,31,1,5,31,35,1,35,10,
                39,1,39,10,43,2,43,9,47,1,6,47,51,2,51,6,55,1,5,55,59,2,59,10,63,1,9,63,67,1,9,67,71,2,71,6,75,1,5,75,
                79,1,5,79,83,1,9,83,87,2,87,10,91,2,10,91,95,1,95,9,99,2,99,9,103,2,10,103,107,2,9,107,111,1,111,5,
                115,1,115,2,119,1,119,6,0,99,2,0,14,0};

            IntcodeMachine im = new IntcodeMachine(input, null, null);
            im.SetMemoryValue(1, 12);
            im.SetMemoryValue(2, 2);
            im.RunProgram();
            long result = im.GetMemoryValue(0);
            Console.WriteLine($"Day 2a: {result}");
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