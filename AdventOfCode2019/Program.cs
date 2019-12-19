using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode2019
{
    public class Program
    {
        public static void Main(string[] _)
        {
            Day17b();
        }

        public static void Day17b()
        {
            long[] input;
            using (var file = File.OpenText("Input/day17.txt"))
            {
                string line = file.ReadLine();
                var strings = line.Split(',');
                input = new long[strings.Length];
                for (int i = 0; i < strings.Length; i++)
                    input[i] = long.Parse(strings[i]);
            }

            string[] inputs = { "B,A,B,C,A,C,B,C,A,C\n", "R,10,L,8,L,8,L,10\n", "L,8,R,10,L,10\n", "L,4,L,6,L,8,L,8\n", "n\n" };
            List<Byte[]> asciiInputs = new List<byte[]>();

            foreach(var s in inputs)
            {
                asciiInputs.Add(ASCIIEncoding.ASCII.GetBytes(s));
            }

            {
                string line = "";
                int inputCount = 0;
                int inputPosition = 0;

                IntcodeMachine im = new IntcodeMachine(input,
                    (out long value) =>
                    {
                        value = asciiInputs[inputCount][inputPosition++];
                        if(inputPosition >= asciiInputs[inputCount].Length)
                        {
                            inputPosition = 0;
                            inputCount++;
                        }
                        Console.Write((char)(value));
                        return true;
                    },
                    (long value) =>
                    {
                        if(value > 127)
                        {
                            Console.WriteLine("17b Program Output: " + value);
                        }
                        else if(value == 10)
                        {
                            Console.WriteLine(line);
                            line = "";
                        }
                        else
                        {
                            line += (char)(value);
                        }
                    });
                im.SetMemoryValue(0, 2);
                im.RunProgram();
            }
        }

        public static void Day17a()
        {
            long[] input;
            using (var file = File.OpenText("Input/day17.txt"))
            {
                string line = file.ReadLine();
                var strings = line.Split(',');
                input = new long[strings.Length];
                for (int i = 0; i < strings.Length; i++)
                    input[i] = long.Parse(strings[i]);
            }

            List<string> lines = new List<string>();
            {
                string line = "";

                IntcodeMachine im = new IntcodeMachine(input, null,
                    (long value) =>
                    {
                        if (value == 10 && line.Length > 0)
                        {
                            Console.WriteLine(line);
                            lines.Add(line);
                            line = "";
                        }
                        else
                        {
                            line += (char)(value);
                        }
                    });

                im.RunProgram();
            }

            bool[,] scaffolding = new bool[lines.Count, lines[0].Length];
            for (int y = 0; y < lines.Count; y++)
            {
                for (int x = 0; x < lines[0].Length; x++)
                {
                    scaffolding[y, x] = lines[y][x] != '.';
                }
            }

            int sum = 0;

            for (int y = 1; y < lines.Count - 1; y++)
            {
                for (int x = 1; x < lines[0].Length - 1; x++)
                {
                    if (scaffolding[y - 1, x] && scaffolding[y + 1, x] && scaffolding[y, x - 1] && scaffolding[y, x + 1])
                    {
                        sum += x * y;
                    }
                }
            }

            Console.WriteLine("17a: " + sum);
        }

        public static void Day16b()
        {
            List<int> input = new List<int>();

            using (var file = File.OpenText("Input/day16.txt"))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    foreach (var c in line)
                    {
                        input.Add(c - '0');
                    }
                }
            }

            // samples:
            //input = new List<int>() { 0, 3, 0, 3, 6, 7, 3, 2, 5, 7, 7, 2, 1, 2, 9, 4, 4, 0, 6, 3, 4, 9, 1, 5, 6, 5, 4, 7, 4, 6, 6, 4 };
            //input = new List<int>() { 0, 2, 9, 3, 5, 1, 0, 9, 6, 9, 9, 9, 4, 0, 8, 0, 7, 4, 0, 7, 5, 8, 5, 4, 4, 7, 0, 3, 4, 3, 2, 3 };
            //input = new List<int>() { 0, 3, 0, 8, 1, 7, 7, 0, 8, 8, 4, 9, 2, 1, 9, 5, 9, 7, 3, 1, 1, 6, 5, 4, 4, 6, 8, 5, 0, 5, 1, 7 };

            int outputOffset = 0;
            for (int i = 0; i < 7; i++)
            {
                outputOffset *= 10;
                outputOffset += input[i];
            }

            if (outputOffset <= input.Count / 2)
            {
                throw new Exception("This code only works for output offsets over half.");
            }

            List<int> signal = new List<int>();
            for (int i = 0; i < 10000; i++)
            {
                signal.AddRange(input);
            }
            List<int> signalCopy = new List<int>(signal);

            const int totalPhases = 100;
            int length = signal.Count;

            for (int phase = 0; phase < totalPhases; phase++)
            {
                List<int> phaseInput = signal;
                List<int> phaseOutput = signalCopy;
                int previousSum = 0;

                for (int i = length - 1; i >= outputOffset; i--)
                {
                    int sum = previousSum + phaseInput[i];
                    phaseOutput[i] = sum % 10;
                    previousSum = sum;
                }

                signal = phaseOutput;
                signalCopy = phaseInput;
            }

            Console.Write("Day 16b: ");
            for (int i = 0; i < 8; i++)
            {
                Console.Write(signal[i + outputOffset]);
            }
            Console.WriteLine("");
        }

        public static void Day16a()
        {
            List<int> signal = new List<int>();

            using (var file = File.OpenText("Input/day16.txt"))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    foreach (var c in line)
                    {
                        signal.Add(c - '0');
                    }
                }
            }

            const int totalPhases = 100;
            List<int> signalCopy = new List<int>(signal);

            int[] pattern = { 0, 1, 0, -1 };

            for (int phase = 0; phase < totalPhases; phase++)
            {
                List<int> phaseInput = signal;
                List<int> phaseOutput = signalCopy;

                for (int i = 0; i < phaseInput.Count; i++)
                {
                    int sum = 0;
                    int offset = 1;
                    int scale = i + 1;

                    for (int j = 0; j < phaseInput.Count; j++)
                    {
                        sum += phaseInput[j] * pattern[offset / scale % 4];
                        offset = (offset + 1) % (scale * 4);
                    }

                    phaseOutput[i] = Math.Abs(sum) % 10;
                }

                signal = phaseOutput;
                signalCopy = phaseInput;
            }

            Console.Write("Day 16a: ");
            for (int i = 0; i < 8; i++)
            {
                Console.Write(signal[i]);
            }
            Console.WriteLine("");
        }

        public static void Day15b()
        {
            List<List<int>> data = new List<List<int>>();

            using (var file = File.OpenText("Input/day15b.txt"))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    List<int> lineData = new List<int>();

                    foreach (var c in line)
                    {
                        switch (c)
                        {
                            case ' ':
                            case '#':
                                lineData.Add(1);
                                break;

                            case 'S':
                            case '.':
                                lineData.Add(0);
                                break;

                            case 'O':
                                lineData.Add(10);
                                break;
                        }
                    }
                    data.Add(lineData);
                }
            }

            int current = 11;
            int time = 0;
            do
            {
                int count = 0;
                for (int y = 0; y < data.Count; y++)
                {
                    for (int x = 0; x < data[0].Count; x++)
                    {
                        if (data[y][x] >= 10 && data[y][x] != current)
                        {
                            if (x > 0 && data[y][x - 1] == 0)
                            {
                                data[y][x - 1] = current;
                                count++;
                            }
                            if (x < data[0].Count - 1 && data[y][x + 1] == 0)
                            {
                                data[y][x + 1] = current;
                                count++;
                            }
                            if (y > 0 && data[y - 1][x] == 0)
                            {
                                data[y - 1][x] = current;
                                count++;
                            }
                            if (y < data.Count - 1 && data[y + 1][x] == 0)
                            {
                                data[y + 1][x] = current;
                                count++;
                            }
                        }
                    }
                }
                if (count == 0)
                {
                    break;
                }
                else
                {
                    current++;
                    time++;
                }
            } while (true);

            Console.WriteLine("15b: " + time);
        }

        public static void Day15a()
        {
            Dictionary<(int x, int y), bool> wallMap = new Dictionary<(int, int), bool>();
            int droidX = 0, droidY = 0;
            int oxygenX = int.MaxValue, oxygenY = int.MaxValue;
            int droidDirection = 0;
            bool manualMode = false;
            Random random = new Random();

            List<(int x, int y)> offsets = new List<(int x, int y)>()
            {
                (0, 0),
                (0, -1),
                (0, 1),
                (-1, 0),
                (1, 0)
            };

            wallMap[(0, 0)] = false;

            void DrawMap()
            {
                Console.SetCursorPosition(0, 0);

                int minimumX = int.MaxValue, maximumX = int.MinValue;
                int minimumY = int.MaxValue, maximumY = int.MinValue;

                foreach (var key in wallMap.Keys)
                {
                    if (key.x < minimumX)
                        minimumX = key.x;
                    if (key.x > maximumX)
                        maximumX = key.x;
                    if (key.y < minimumY)
                        minimumY = key.y;
                    if (key.y > maximumY)
                        maximumY = key.y;
                }

                if (minimumX > -20)
                    minimumX = -20;
                if (minimumY > -20)
                    minimumY = -20;

                for (int dy = minimumY; dy <= maximumY; dy++)
                {
                    for (int dx = minimumX; dx <= maximumX; dx++)
                    {
                        if (dx == droidX && dy == droidY)
                        {
                            Console.Write("D");
                        }
                        else if (dx == oxygenX && dy == oxygenY)
                        {
                            Console.Write("O");
                        }
                        else if (dx == 0 && dy == 0)
                        {
                            Console.Write("S");
                        }
                        else
                        {
                            if (wallMap.ContainsKey((dx, dy)))
                            {
                                if (wallMap[(dx, dy)] == true)
                                {
                                    Console.Write("#");
                                }
                                else
                                {
                                    Console.Write(".");
                                }
                            }
                            else
                            {
                                Console.Write(" ");
                            }
                        }
                    }
                    Console.WriteLine("");
                }
            }

            long[] input;
            using (var file = File.OpenText("Input/day15.txt"))
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
                    value = 0;

                    if (!manualMode && Console.KeyAvailable)
                    {
                        var key = Console.ReadKey();
                        if (key.Key == ConsoleKey.Spacebar)
                            manualMode = true;
                    }

                    if (manualMode)
                    {
                        do
                        {
                            Console.WriteLine("Hit an arrow key:");
                            var key = Console.ReadKey();
                            if (key.Key == ConsoleKey.UpArrow)
                                value = droidDirection = 1;
                            else if (key.Key == ConsoleKey.DownArrow)
                                value = droidDirection = 2;
                            else if (key.Key == ConsoleKey.LeftArrow)
                                value = droidDirection = 3;
                            else if (key.Key == ConsoleKey.RightArrow)
                                value = droidDirection = 4;
                            else if (key.Key == ConsoleKey.Enter)
                                manualMode = false;
                            else
                                continue;
                            break;
                        } while (true);
                    }

                    if (!manualMode)
                    {
                        if (!wallMap.ContainsKey((droidX + offsets[1].x, droidY + offsets[1].y)))
                        {
                            value = droidDirection = 1;
                        }
                        else if (!wallMap.ContainsKey((droidX + offsets[2].x, droidY + offsets[2].y)))
                        {
                            value = droidDirection = 2;
                        }
                        else if (!wallMap.ContainsKey((droidX + offsets[3].x, droidY + offsets[3].y)))
                        {
                            value = droidDirection = 3;
                        }
                        else if (!wallMap.ContainsKey((droidX + offsets[4].x, droidY + offsets[4].y)))
                        {
                            value = droidDirection = 4;
                        }
                        else
                        {
                            value = droidDirection = random.Next(1, 5);
                        }
                    }
                    return true;
                },
                (long value) =>
                {
                    if (value == 0)
                    {
                        wallMap[(droidX + offsets[droidDirection].x, droidY + offsets[droidDirection].y)] = true;
                    }
                    else if (value == 1)
                    {
                        wallMap[(droidX + offsets[droidDirection].x, droidY + offsets[droidDirection].y)] = false;
                        droidX += offsets[droidDirection].x;
                        droidY += offsets[droidDirection].y;
                    }
                    else if (value == 2)
                    {
                        wallMap[(droidX + offsets[droidDirection].x, droidY + offsets[droidDirection].y)] = false;
                        droidX += offsets[droidDirection].x;
                        droidY += offsets[droidDirection].y;
                        oxygenX = droidX;
                        oxygenY = droidY;
                    }
                    DrawMap();
                });

            DrawMap();
            im.RunProgram();
        }

        private class Formula
        {
            public string Product { get; set; }
            public int Amount { get; set; }
            public Dictionary<string, int> Ingredients = new Dictionary<string, int>();
        }

        private static void Day14()
        {
            // Note: day 14 part 2 won't work with any other input
            // There's a magic number that would need to be removed

            Dictionary<string, Formula> formulas = new Dictionary<string, Formula>();

            using (var file = File.OpenText("Input/day14.txt"))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    line = line.Replace(" =>", ",");
                    line = line.Replace(", ", ",");
                    var strings = line.Split(',');

                    var product = strings[strings.Length - 1].Split(' ');
                    Formula f = new Formula()
                    {
                        Product = product[1],
                        Amount = int.Parse(product[0]),
                    };
                    for (int i = 0; i < strings.Length - 1; i++)
                    {
                        var ingredient = strings[i].Split(' ');
                        f.Ingredients[ingredient[1]] = int.Parse(ingredient[0]);
                    }
                    formulas[f.Product] = f;
                }
            }

            Dictionary<string, ulong> resourcesAvailable = new Dictionary<string, ulong>();
            resourcesAvailable["ORE"] = 1000000;

            ulong fuelCrafted = 0;

            void CraftResource(string target, ulong amount)
            {
                if (resourcesAvailable.ContainsKey(target) && resourcesAvailable[target] > 0)
                {
                    if (resourcesAvailable[target] >= amount)
                    {
                        return;
                    }
                    else
                    {
                        amount -= resourcesAvailable[target];
                    }
                }

                if (target == "ORE")
                {
                    Console.WriteLine("14b Fuel Crafted: " + fuelCrafted);
                    Environment.Exit(0);
                }

                ulong batchesRequired = amount / (ulong)(formulas[target].Amount) + ((amount % (ulong)(formulas[target].Amount)) == 0ul ? 0ul : 1ul);

                foreach (var kvp in formulas[target].Ingredients)
                {
                    CraftResource(kvp.Key, (ulong)kvp.Value * batchesRequired);
                    resourcesAvailable[kvp.Key] -= (ulong)kvp.Value * batchesRequired;
                }
                if (resourcesAvailable.ContainsKey(target))
                {
                    resourcesAvailable[target] += (ulong)formulas[target].Amount * batchesRequired;
                }
                else
                {
                    resourcesAvailable[target] = (ulong)formulas[target].Amount * batchesRequired;
                }
            }

            CraftResource("FUEL", 1);

            Console.WriteLine("14a :" + (1000000 - resourcesAvailable["ORE"]));

            resourcesAvailable.Clear();
            resourcesAvailable["ORE"] = 1000000000000;

            // This magic number was stumbled upon experimentally.
            // if you want to run with other inputs, delete these three lines,
            // but expect the program to take awhile
            CraftResource("FUEL", 3209250);
            fuelCrafted += 3209250;
            resourcesAvailable["FUEL"] -= 3209250;

            while (true)
            {
                CraftResource("FUEL", 1);
                fuelCrafted += 1;
                resourcesAvailable["FUEL"] -= 1;
            }
        }

        private static void Day13()
        {
            Dictionary<(int x, int y), int> tiles = new Dictionary<(int x, int y), int>();
            int x = 0, y = 0;
            int count = 0;
            int ballx = 0, bally = 0;
            int paddlex = 0, paddley = 0;

            long[] input;
            using (var file = File.OpenText("Input/day13.txt"))
            {
                string line = file.ReadLine();
                var strings = line.Split(',');
                input = new long[strings.Length];
                for (int i = 0; i < strings.Length; i++)
                    input[i] = long.Parse(strings[i]);
            }

            IntcodeMachine im = new IntcodeMachine(input, null,
                (long value) =>
                {
                    if (count == 0)
                    {
                        count++;
                        x = (int)value;
                    }
                    else if (count == 1)
                    {
                        count++;
                        y = (int)value;
                    }
                    else
                    {
                        count = 0;
                        tiles[(x, y)] = (int)value;

                        if (value == 4)
                        {
                            ballx = x;
                            bally = y;
                        }
                        else if (value == 3)
                        {
                            paddlex = x;
                            paddley = y;
                        }
                    }
                });
            im.RunProgram();

            int minimumX = int.MaxValue, maximumX = int.MinValue;
            int minimumY = int.MaxValue, maximumY = int.MinValue;
            foreach (var k in tiles.Keys)
            {
                if (k.x < minimumX)
                    minimumX = k.x;
                if (k.x > maximumX)
                    maximumX = k.x;
                if (k.y < minimumY)
                    minimumY = k.y;
                if (k.y > maximumY)
                    maximumY = k.y;
            }

            int wallCount = 0;

            for (int dy = minimumY; dy <= maximumY; dy++)
            {
                for (int dx = minimumX; dx <= maximumX; dx++)
                {
                    if (tiles.ContainsKey((dx, dy)))
                    {
                        Console.Write(tiles[(dx, dy)].ToString());

                        if (tiles[(dx, dy)] == 2)
                            wallCount++;
                    }
                    else
                    {
                        Console.Write(".");
                    }
                }
                Console.WriteLine("");
            }

            Console.WriteLine("10a: " + wallCount);

            long score = 0;

            void PrintScreen()
            {
                Console.SetCursorPosition(0, 0);
                for (int dy = minimumY; dy <= maximumY; dy++)
                {
                    for (int dx = minimumX; dx <= maximumX; dx++)
                    {
                        if (tiles.ContainsKey((dx, dy)))
                        {
                            if (tiles[(dx, dy)] == 0)
                            {
                                Console.Write(" ");
                            }
                            else
                            {
                                Console.Write(tiles[(dx, dy)].ToString());
                            }
                        }
                        else
                        {
                            Console.Write(".");
                        }
                    }
                    Console.WriteLine("");
                }

                Console.WriteLine("Score: " + score);
            }

            im.ResetProgram();
            im.SetMemoryValue(0, 2);
            im.SetInputAction(
                (out long value) =>
                {
                    if (ballx < paddlex)
                        value = -1;
                    else if (ballx > paddlex)
                        value = 1;
                    else
                        value = 0;
                    return true;
                });
            im.SetOutputAction(
                (long value) =>
                {
                    if (count == 0)
                    {
                        count++;
                        x = (int)value;
                    }
                    else if (count == 1)
                    {
                        count++;
                        y = (int)value;
                    }
                    else
                    {
                        count = 0;
                        if (x == -1 && y == 0)
                        {
                            score = value;
                            PrintScreen();
                        }
                        else
                        {
                            tiles[(x, y)] = (int)value;

                            PrintScreen();

                            if (value == 4)
                            {
                                ballx = x;
                                bally = y;
                            }
                            else if (value == 3)
                            {
                                paddlex = x;
                                paddley = y;
                            }
                        }
                    }
                });
            im.RunProgram();
        }

        private class V3
        {
            public int X = 0, Y = 0, Z = 0;

            public V3()
            {
            }

            public V3(int a, int b, int c)
            {
                X = a;
                Y = b;
                Z = c;
            }
        }

        private static long LCM(long a, long b)
        {
            return (a / GCD(a, b)) * b;
        }

        private static void Day12b()
        {
            List<V3> positions = new List<V3>() { new V3(-4, -14, 8), new V3(1, -8, 10), new V3(-15, 2, 1), new V3(-17, -17, 16) };
            List<V3> velocities = new List<V3>() { new V3(), new V3(), new V3(), new V3() };
            var initialPositionX = (-4, 1, -15, -17);
            var initialPositionY = (-14, -8, 2, -17);
            var initialPositionZ = (8, 10, 1, 16);

            void UpdateVelocities()
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = i + 1; j < 4; j++)
                    {
                        if (i == j)
                            continue;

                        if (positions[i].X > positions[j].X)
                        {
                            velocities[i].X--;
                            velocities[j].X++;
                        }
                        else if (positions[i].X < positions[j].X)
                        {
                            velocities[i].X++;
                            velocities[j].X--;
                        }

                        if (positions[i].Y > positions[j].Y)
                        {
                            velocities[i].Y--;
                            velocities[j].Y++;
                        }
                        else if (positions[i].Y < positions[j].Y)
                        {
                            velocities[i].Y++;
                            velocities[j].Y--;
                        }

                        if (positions[i].Z > positions[j].Z)
                        {
                            velocities[i].Z--;
                            velocities[j].Z++;
                        }
                        else if (positions[i].Z < positions[j].Z)
                        {
                            velocities[i].Z++;
                            velocities[j].Z--;
                        }
                    }
                }
            }

            void UpdatePositions()
            {
                for (int i = 0; i < 4; i++)
                {
                    positions[i].X += velocities[i].X;
                    positions[i].Y += velocities[i].Y;
                    positions[i].Z += velocities[i].Z;
                }
            }

            int xcount = 0;
            int ycount = 0;
            int zcount = 0;

            bool xdone = false;
            bool ydone = false;
            bool zdone = false;

            do
            {
                UpdateVelocities();
                UpdatePositions();

                if (!xdone)
                {
                    xcount++;
                    var b1 = (positions[0].X, positions[1].X, positions[2].X, positions[3].X) == initialPositionX;
                    var b2 = (velocities[0].X, velocities[1].X, velocities[2].X, velocities[3].X) == (0, 0, 0, 0);
                    if (b1 && b2)
                        xdone = true;
                }

                if (!ydone)
                {
                    ycount++;
                    var b1 = (positions[0].Y, positions[1].Y, positions[2].Y, positions[3].Y) == initialPositionY;
                    var b2 = (velocities[0].Y, velocities[1].Y, velocities[2].Y, velocities[3].Y) == (0, 0, 0, 0);
                    if (b1 && b2)
                        ydone = true;
                }

                if (!zdone)
                {
                    zcount++;
                    var b1 = (positions[0].Z, positions[1].Z, positions[2].Z, positions[3].Z) == initialPositionZ;
                    var b2 = (velocities[0].Z, velocities[1].Z, velocities[2].Z, velocities[3].Z) == (0, 0, 0, 0);
                    if (b1 && b2)
                        zdone = true;
                }
            } while (!xdone || !ydone || !zdone);

            Console.WriteLine($"12b {LCM(xcount, LCM(ycount, zcount))}");
        }

        private static void Day12a()
        {
            List<V3> positions = new List<V3>() { new V3(-4, -14, 8), new V3(1, -8, 10), new V3(-15, 2, 1), new V3(-17, -17, 16) };
            List<V3> velocities = new List<V3>() { new V3(), new V3(), new V3(), new V3() };

            void UpdateVelocities()
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = i + 1; j < 4; j++)
                    {
                        if (i == j)
                            continue;

                        if (positions[i].X > positions[j].X)
                        {
                            velocities[i].X--;
                            velocities[j].X++;
                        }
                        else if (positions[i].X < positions[j].X)
                        {
                            velocities[i].X++;
                            velocities[j].X--;
                        }

                        if (positions[i].Y > positions[j].Y)
                        {
                            velocities[i].Y--;
                            velocities[j].Y++;
                        }
                        else if (positions[i].Y < positions[j].Y)
                        {
                            velocities[i].Y++;
                            velocities[j].Y--;
                        }

                        if (positions[i].Z > positions[j].Z)
                        {
                            velocities[i].Z--;
                            velocities[j].Z++;
                        }
                        else if (positions[i].Z < positions[j].Z)
                        {
                            velocities[i].Z++;
                            velocities[j].Z--;
                        }
                    }
                }
            }

            void UpdatePositions()
            {
                for (int i = 0; i < 4; i++)
                {
                    positions[i].X += velocities[i].X;
                    positions[i].Y += velocities[i].Y;
                    positions[i].Z += velocities[i].Z;
                }
            }

            for (int i = 0; i < 1000; i++)
            {
                UpdateVelocities();
                UpdatePositions();
            }

            int energy = 0;

            for (int i = 0; i < 4; i++)
            {
                int potential = 0;
                potential += Math.Abs(positions[i].X);
                potential += Math.Abs(positions[i].Y);
                potential += Math.Abs(positions[i].Z);

                int kinetic = 0;
                kinetic += Math.Abs(velocities[i].X);
                kinetic += Math.Abs(velocities[i].Y);
                kinetic += Math.Abs(velocities[i].Z);

                energy += potential * kinetic;
            }

            Console.WriteLine("12a " + energy);
        }

        private static void Day11()
        {
            Dictionary<(int x, int y), (int color, bool painted)> panels = new Dictionary<(int x, int y), (int color, bool painted)>();
            int x = 0, y = 0;
            int direction = 0;
            bool waitingForColor = true;

            long[] input;
            using (var file = File.OpenText("Input/day11.txt"))
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
                    if (panels.ContainsKey((x, y)))
                    {
                        value = panels[(x, y)].color;
                    }
                    else
                    {
                        value = 0;
                    }
                    return true;
                },
                (long value) =>
                {
                    if (waitingForColor)
                    {
                        panels[(x, y)] = ((int)value, true);
                        waitingForColor = false;
                    }
                    else
                    {
                        direction += value == 0 ? -1 : 1;
                        if (direction < 0)
                            direction = 3;
                        if (direction > 3)
                            direction = 0;
                        waitingForColor = true;
                        switch (direction)
                        {
                            case 0:
                                y--;
                                break;

                            case 1:
                                x++;
                                break;

                            case 2:
                                y++;
                                break;

                            case 3:
                                x--;
                                break;
                        }
                    }
                });
            im.RunProgram();

            int paintedCount = 0;
            foreach (var v in panels.Values)
            {
                if (v.painted)
                {
                    paintedCount++;
                }
            }

            Console.WriteLine("10a: " + paintedCount);

            panels.Clear();
            panels[(0, 0)] = (1, false);
            x = y = 0;
            direction = 0;
            waitingForColor = true;
            im.ResetProgram();
            im.RunProgram();

            int minimumX = int.MaxValue, maximumX = int.MinValue;
            int minimumY = int.MaxValue, maximumY = int.MinValue;
            foreach (var k in panels.Keys)
            {
                if (k.x < minimumX)
                    minimumX = k.x;
                if (k.x > maximumX)
                    maximumX = k.x;
                if (k.y < minimumY)
                    minimumY = k.y;
                if (k.y > maximumY)
                    maximumY = k.y;
            }

            for (int dy = minimumY; dy <= maximumY; dy++)
            {
                for (int dx = minimumX; dx <= maximumX; dx++)
                {
                    if (panels.ContainsKey((dx, dy)))
                    {
                        if (panels[(dx, dy)].color == 1)
                            Console.Write("#");
                        else
                            Console.Write(".");
                    }
                    else
                    {
                        Console.Write(".");
                    }
                }
                Console.WriteLine("");
            }
        }

        private static long GCD(long a, long b)
        {
            while (a != 0 && b != 0)
            {
                if (a > b)
                    a %= b;
                else
                    b %= a;
            }

            return a == 0 ? b : a;
        }

        private static double GetAngle(int x, int y)
        {
            return Math.Atan2(y, x) + Math.PI / 2.0;
        }

        public class Asteroid
        {
            public bool Present { get; set; }
            public int ViewCount { get; set; }
            public double Angle { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
        }

        private static void Day10()
        {
            List<List<Asteroid>> asteroidMatrix = new List<List<Asteroid>>();

            using (var file = File.OpenText("Input/day10.txt"))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    List<Asteroid> current = new List<Asteroid>();
                    foreach (var c in line)
                    {
                        if (c == '#')
                            current.Add(new Asteroid() { Present = true, ViewCount = 0 });
                        else
                            current.Add(new Asteroid() { Present = false, ViewCount = 0 });
                    }
                    asteroidMatrix.Add(current);
                }
            }

            // This is some ugly brute force action
            // I look through all of the asteroids, then for each asteroid, I look through all of them again,
            // and compare if the asteroid is the closest one in that direction.
            // I determined the direction by using the Euclidean algorithm to find the greatest common divisor,
            // dividing the distance by that, then stepping along the direction.

            int maxViewCount = 0;
            int baseX = 0, baseY = 0;

            for (int thisY = 0; thisY < asteroidMatrix.Count; thisY++)
            {
                for (int thisX = 0; thisX < asteroidMatrix[thisY].Count; thisX++)
                {
                    if (asteroidMatrix[thisY][thisX].Present)
                    {
                        for (int y = 0; y < asteroidMatrix.Count; y++)
                        {
                            for (int x = 0; x < asteroidMatrix[y].Count; x++)
                            {
                                asteroidMatrix[y][x].X = x;
                                asteroidMatrix[y][x].Y = y;

                                if (x == thisX && y == thisY)
                                    continue;

                                if (asteroidMatrix[y][x].Present)
                                {
                                    int gcd = (int)GCD(Math.Abs(y - thisY), Math.Abs(x - thisX));
                                    if (gcd == 1)
                                    {
                                        asteroidMatrix[thisY][thisX].ViewCount++;
                                    }
                                    else
                                    {
                                        int xOffset = (x - thisX) / gcd;
                                        int yOffset = (y - thisY) / gcd;
                                        int tempX = thisX;
                                        int tempY = thisY;
                                        while (tempX + xOffset >= 0 && tempX + xOffset < asteroidMatrix[y].Count
                                            && tempY + yOffset >= 0 && tempY + yOffset < asteroidMatrix.Count)
                                        {
                                            tempX += xOffset;
                                            tempY += yOffset;
                                            if (tempX == x && tempY == y)
                                            {
                                                asteroidMatrix[thisY][thisX].ViewCount++;
                                                break;
                                            }
                                            else if (asteroidMatrix[tempY][tempX].Present)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (asteroidMatrix[thisY][thisX].ViewCount > maxViewCount)
                        {
                            maxViewCount = asteroidMatrix[thisY][thisX].ViewCount;
                            baseX = thisX;
                            baseY = thisY;
                        }
                    }
                }
            }

            Console.WriteLine("10a: " + maxViewCount);

            // More brute force nonsense
            // I calculate the angle from our base asteroid to every other asteroid
            // Then I sort the list of asteroids by their angle,
            // Starting at angle zero, I find all the matching asteroids, pick the closest one
            // and blow it up. Then I look for the next highest angle in the list and start again
            // if I don't find a next angle, then loop back to the beginning.

            List<Asteroid> asteroidList = new List<Asteroid>();

            for (int y = 0; y < asteroidMatrix.Count; y++)
            {
                for (int x = 0; x < asteroidMatrix[y].Count; x++)
                {
                    if (x == baseX && y == baseY)
                        continue;

                    if (asteroidMatrix[y][x].Present)
                    {
                        asteroidMatrix[y][x].Angle = GetAngle(x - baseX, y - baseY);
                        if (asteroidMatrix[y][x].Angle < 0)
                            asteroidMatrix[y][x].Angle += Math.PI * 2;
                        asteroidList.Add(asteroidMatrix[y][x]);
                    }
                }
            }

            asteroidList.Sort((a, b) => (a.Angle.CompareTo(b.Angle)));

            double currentAngle = 0.0;
            int laserCount = 0;
            int foundX = 0, foundY = 0;

            do
            {
                var matchList = asteroidList.FindAll((asteroid) => (Math.Abs(asteroid.Angle - currentAngle) < 0.0001));
                Asteroid closest = null;
                double closestDistance = double.MaxValue;
                for (int i = 0; i < matchList.Count; i++)
                {
                    double distance = Math.Sqrt(Math.Pow(matchList[i].X - baseX, 2) + Math.Pow(matchList[i].Y - baseY, 2));
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closest = matchList[i];
                    }
                }

                laserCount++;
                if (laserCount == 200)
                {
                    foundX = closest.X;
                    foundY = closest.Y;
                    break;
                }

                asteroidList.Remove(closest);

                bool foundNewAngle = false;
                foreach (var a in asteroidList)
                {
                    if (a.Angle > currentAngle + 0.0001)
                    {
                        currentAngle = a.Angle;
                        foundNewAngle = true;
                        break;
                    }
                }
                if (!foundNewAngle)
                    currentAngle = asteroidList[0].Angle;
            } while (true);

            Console.WriteLine($"10b: {foundX * 100 + foundY}");
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