using System;

namespace AdventOfCode2019
{
    public enum IntcodeRunState
    {
        WaitingForInput,
        Terminated
    }

    public class IntcodeMachine
    {
        public delegate bool InputAction(out int input);

        public IntcodeMachine(int[] program, InputAction inputAction, Action<int> outputAction)
        {
            this.program = (int[])program.Clone();
            this.inputAction = inputAction;
            this.outputAction = outputAction;
            ResetProgram();
        }

        public void ResetProgram()
        {
            instructionPointer = 0;
            memory = (int[])program.Clone();
        }

        public int ReadMemoryValue(int index)
        {
            return memory[index];
        }

        public void WriteMemoryValue(int index, int value)
        {
            memory[index] = value;
        }

        public IntcodeRunState RunProgram()
        {
            bool terminate = false;
            do
            {
                int instruction = memory[instructionPointer];
                int opCode = instruction % 100;
                instruction /= 100;
                int parameterModeA = instruction % 10;
                instruction /= 10;
                int parameterModeB = instruction % 10;
                instruction /= 10;
                int parameterModeC = instruction % 10;

                int parameterA, parameterB, parameterC;

                switch (opCode)
                {
                    case 1: // Add
                        parameterA = (parameterModeA == 1) ? memory[instructionPointer + 1] : memory[memory[instructionPointer + 1]];
                        parameterB = (parameterModeB == 1) ? memory[instructionPointer + 2] : memory[memory[instructionPointer + 2]];
                        parameterC = memory[instructionPointer + 3];
                        memory[parameterC] = parameterA + parameterB;
                        instructionPointer += 4;
                        break;

                    case 2: // Multiply
                        parameterA = (parameterModeA == 1) ? memory[instructionPointer + 1] : memory[memory[instructionPointer + 1]];
                        parameterB = (parameterModeB == 1) ? memory[instructionPointer + 2] : memory[memory[instructionPointer + 2]];
                        parameterC = memory[instructionPointer + 3];
                        memory[parameterC] = parameterA * parameterB;
                        instructionPointer += 4;
                        break;

                    case 3: // Input
                        parameterA = memory[instructionPointer + 1];
                        if (!inputAction(out int input))
                            return IntcodeRunState.WaitingForInput;
                        memory[parameterA] = input;
                        instructionPointer += 2;
                        break;

                    case 4: // Output
                        parameterA = (parameterModeA == 1) ? memory[instructionPointer + 1] : memory[memory[instructionPointer + 1]];
                        outputAction(parameterA);
                        instructionPointer += 2;
                        break;

                    case 5: // Jump if true
                        parameterA = (parameterModeA == 1) ? memory[instructionPointer + 1] : memory[memory[instructionPointer + 1]];
                        parameterB = (parameterModeB == 1) ? memory[instructionPointer + 2] : memory[memory[instructionPointer + 2]];
                        if (parameterA != 0)
                            instructionPointer = parameterB;
                        else
                            instructionPointer += 3;
                        break;

                    case 6: // Jump if false
                        parameterA = (parameterModeA == 1) ? memory[instructionPointer + 1] : memory[memory[instructionPointer + 1]];
                        parameterB = (parameterModeB == 1) ? memory[instructionPointer + 2] : memory[memory[instructionPointer + 2]];
                        if (parameterA == 0)
                            instructionPointer = parameterB;
                        else
                            instructionPointer += 3;
                        break;

                    case 7: // Less than
                        parameterA = (parameterModeA == 1) ? memory[instructionPointer + 1] : memory[memory[instructionPointer + 1]];
                        parameterB = (parameterModeB == 1) ? memory[instructionPointer + 2] : memory[memory[instructionPointer + 2]];
                        parameterC = memory[instructionPointer + 3];
                        memory[parameterC] = (parameterA < parameterB) ? 1 : 0;
                        instructionPointer += 4;
                        break;

                    case 8: // Equals
                        parameterA = (parameterModeA == 1) ? memory[instructionPointer + 1] : memory[memory[instructionPointer + 1]];
                        parameterB = (parameterModeB == 1) ? memory[instructionPointer + 2] : memory[memory[instructionPointer + 2]];
                        parameterC = memory[instructionPointer + 3];
                        memory[parameterC] = (parameterA == parameterB) ? 1 : 0;
                        instructionPointer += 4;
                        break;

                    case 99: // Terminate
                        terminate = true;
                        break;
                }
            } while (!terminate);

            return IntcodeRunState.Terminated;
        }

        private int[] memory;
        private int[] program;
        private int instructionPointer;
        private InputAction inputAction;
        private Action<int> outputAction;
    }
}