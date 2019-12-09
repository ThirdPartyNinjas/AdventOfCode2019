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
        const int MemorySize = 10000;

        public delegate bool InputAction(out long input);

        public IntcodeMachine(long[] program, InputAction inputAction, Action<long> outputAction)
        {
            this.program = (long[])program.Clone();
            this.inputAction = inputAction;
            this.outputAction = outputAction;
            memory = new long[MemorySize];
            ResetProgram();
        }

        public void ResetProgram()
        {
            instructionPointer = 0;
            relativeBase = 0;
            Array.Copy(program, 0, memory, 0, program.Length);
            for (int i = program.Length; i < memory.Length; i++)
            {
                memory[i] = 0;
            }
        }

        public long GetMemoryValue(int index)
        {
            return memory[index];
        }

        public void SetMemoryValue(int index, long value)
        {
            memory[index] = value;
        }

        public void SetInputAction(InputAction inputAction)
        {
            this.inputAction = inputAction;
        }

        public void SetOutputAction(Action<long> outputAction)
        {
            this.outputAction = outputAction;
        }

        public IntcodeRunState RunProgram()
        {
            bool terminate = false;
            do
            {
                long instruction = memory[instructionPointer];
                long opCode = instruction % 100;
                instruction /= 100;
                long parameterModeA = instruction % 10;
                instruction /= 10;
                long parameterModeB = instruction % 10;
                instruction /= 10;
                long parameterModeC = instruction % 10;

                long parameterAddressA, parameterAddressB, parameterAddressC;

                switch (opCode)
                {
                    case 1: // Add
                        parameterAddressA = GetParameterAddress(parameterModeA, instructionPointer + 1);
                        parameterAddressB = GetParameterAddress(parameterModeB, instructionPointer + 2);
                        parameterAddressC = GetParameterAddress(parameterModeC, instructionPointer + 3);
                        memory[parameterAddressC] = memory[parameterAddressA] + memory[parameterAddressB];
                        instructionPointer += 4;
                        break;

                    case 2: // Multiply
                        parameterAddressA = GetParameterAddress(parameterModeA, instructionPointer + 1);
                        parameterAddressB = GetParameterAddress(parameterModeB, instructionPointer + 2);
                        parameterAddressC = GetParameterAddress(parameterModeC, instructionPointer + 3);
                        memory[parameterAddressC] = memory[parameterAddressA] * memory[parameterAddressB];
                        instructionPointer += 4;
                        break;

                    case 3: // Input
                        parameterAddressA = GetParameterAddress(parameterModeA, instructionPointer + 1);
                        if (!inputAction(out long input))
                            return IntcodeRunState.WaitingForInput;
                        memory[parameterAddressA] = input;
                        instructionPointer += 2;
                        break;

                    case 4: // Output
                        parameterAddressA = GetParameterAddress(parameterModeA, instructionPointer + 1);
                        outputAction(memory[parameterAddressA]);
                        instructionPointer += 2;
                        break;

                    case 5: // Jump if true
                        parameterAddressA = GetParameterAddress(parameterModeA, instructionPointer + 1);
                        parameterAddressB = GetParameterAddress(parameterModeB, instructionPointer + 2);
                        if (memory[parameterAddressA] != 0)
                            instructionPointer = memory[parameterAddressB];
                        else
                            instructionPointer += 3;
                        break;

                    case 6: // Jump if false
                        parameterAddressA = GetParameterAddress(parameterModeA, instructionPointer + 1);
                        parameterAddressB = GetParameterAddress(parameterModeB, instructionPointer + 2);
                        if (memory[parameterAddressA] == 0)
                            instructionPointer = memory[parameterAddressB];
                        else
                            instructionPointer += 3;
                        break;

                    case 7: // Less than
                        parameterAddressA = GetParameterAddress(parameterModeA, instructionPointer + 1);
                        parameterAddressB = GetParameterAddress(parameterModeB, instructionPointer + 2);
                        parameterAddressC = GetParameterAddress(parameterModeC, instructionPointer + 3);
                        memory[parameterAddressC] = (memory[parameterAddressA] < memory[parameterAddressB]) ? 1 : 0;
                        instructionPointer += 4;
                        break;

                    case 8: // Equals
                        parameterAddressA = GetParameterAddress(parameterModeA, instructionPointer + 1);
                        parameterAddressB = GetParameterAddress(parameterModeB, instructionPointer + 2);
                        parameterAddressC = GetParameterAddress(parameterModeC, instructionPointer + 3);
                        memory[parameterAddressC] = (memory[parameterAddressA] == memory[parameterAddressB]) ? 1 : 0;
                        instructionPointer += 4;
                        break;

                    case 9: // Adjust relative base
                        parameterAddressA = GetParameterAddress(parameterModeA, instructionPointer + 1);
                        relativeBase += memory[parameterAddressA];
                        instructionPointer += 2;
                        break;

                    case 99: // Terminate
                        terminate = true;
                        break;
                }
            } while (!terminate);

            return IntcodeRunState.Terminated;
        }

        private long GetParameterAddress(long parameterMode, long address)
        {
            switch(parameterMode)
            {
                case 0:
                    return memory[address];
                case 1:
                    return address;
                case 2:
                    return memory[address] + relativeBase;
                default:
                    throw new Exception("Unknown parameter mode");
            }
        }

        private long[] memory;
        private long[] program;
        private long instructionPointer;
        private long relativeBase;
        private InputAction inputAction;
        private Action<long> outputAction;
    }
}