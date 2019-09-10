using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CoDLUIDecompiler
{
    class LuaFunction
    {
        public int upValsCount;
        public int parameterCount;
        public int registerCount;
        public int instructionCount;
        public int constantCount;
        public int subFunctionCount;

        public bool usesVarArg;

        private LuaFile luaFile;

        public LuaInstruction[] Instructions;
        public List<string> Upvalues;
        public LuaConstant[] Constants;
        public LuaRegister[] Registers;
        public LuaFunction[] SubFunctions;

        public List<int> tablePositions;

        public List<int> endPositions;
        public List<int> elsePositions;
        public string orCondition;

        public bool isInitFunction;
        public LuaFunction superFunction;
        public int tabLevel;
        public int instructionPtr;
        public LuaInstruction currentInstruction;

        public string newName;

        public long startPosition;
        public long endPosition;
        public BinaryReader inputReader;
        public StreamWriter outputWriter;

        public LuaFunction(LuaFile luaFile, int tabLevel = 0, bool IsInitFunction = false, LuaFunction SuperFunction = null)
        {
            this.inputReader = luaFile.inputReader;
            this.outputWriter = luaFile.outputWriter;
            this.luaFile = luaFile;
            this.tabLevel = tabLevel;
            this.isInitFunction = IsInitFunction;
            this.superFunction = SuperFunction;
            this.startPosition = this.inputReader.BaseStream.Position;
            // Read the header
            this.readHeader();
            // Read all instructions
            this.readInstructions();
            // Read all constants
            this.readConstants();
            // read the footer
            this.readFooter();
            // read the sub functions
            this.readSubFunctions();
        }

        private void readHeader()
        {
            this.upValsCount = this.inputReader.ReadInt32();
            this.Upvalues = new List<string>();
            this.parameterCount = this.inputReader.ReadInt32();
            int flags = inputReader.ReadByte();
            if (flags != 0)
            {
                this.usesVarArg = true;
            }
            this.registerCount = this.inputReader.ReadInt32();
            this.instructionCount = this.inputReader.ReadInt32();

            // Some unknown bytes (BO3 and BO4) almost 0
            if(this.luaFile.Game != LuaFile.SupportedGames.BlackOps2)
            {
                int weirdint = this.inputReader.ReadInt32();
            }

            // Add extra bytes to make it line up
            int extra = 4 - ((int)this.inputReader.BaseStream.Position % 4);
            if (extra > 0 && extra < 4)
                this.inputReader.ReadBytes(extra);

            this.tablePositions = new List<int>();
            this.endPositions = new List<int>();
            this.elsePositions = new List<int>();
            this.orCondition = "";
        }

        private void readFooter()
        {
            int unkInt = this.inputReader.ReadInt32();
            if (luaFile.Game != LuaFile.SupportedGames.WorldWar2)
            {
                float unkFloat = this.inputReader.ReadSingle();
            }
            
            this.subFunctionCount = this.inputReader.ReadInt32();
            this.endPosition = this.inputReader.BaseStream.Position;
        }

        private void readInstructions()
        {
            // Create array for instructions
            this.Instructions = new LuaInstruction[this.instructionCount];
            // Go through all instructions
            for (int i = 0; i < this.instructionCount; i++)
            {
                // Making a new instruction
                LuaInstruction instruction = new LuaInstruction(this);
                // Reading the values attached to the instruction
                // A = 8 bits
                // B = 9 bits
                // C = 8 bits
                // OpCode = 7 bits
                instruction.A = this.inputReader.ReadByte();

                int cValue = this.inputReader.ReadByte();
                byte bValue = this.inputReader.ReadByte();
                if (GetBit(bValue, 0) == 1)
                    cValue += 256;
                instruction.C = cValue;

                instruction.B = bValue >> 1;
                byte flagsB = this.inputReader.ReadByte();
                if (GetBit(flagsB, 0) == 1)
                    instruction.B += 128;

                instruction.Bx = (instruction.B * 512) + instruction.C;
                instruction.sBx = instruction.Bx - 65536 + 1;

                if(this.luaFile.OPCodeTable.TryGetValue(flagsB >> 1, out LuaOpCode.OpCodes opcode))
                {
                    instruction.OpCode = opcode;
                }
                else
                {
                    instruction.OpCode = LuaOpCode.OpCodes.HKS_OPCODE_UNK;
                }

                // Add to array
                this.Instructions[i] = instruction;
            }
        }

        private void readConstants()
        {
            this.constantCount = this.inputReader.ReadInt32();
            // Create array for constants
            this.Constants = new LuaConstant[this.constantCount];
            // Go through all constants
            for (int i = 0; i < this.constantCount; i++)
            {
                // Make a new constant object
                LuaConstant constant = new LuaConstant();
                // Read the type of constant
                constant.type = (Datatype.Type)this.inputReader.ReadByte();
                // Check if its a constant we can read
                switch (constant.type)
                {
                    case Datatype.Type.Nil:
                        constant.value = "nil";
                        break;
                    case Datatype.Type.Boolean:
                        constant.value = (this.inputReader.ReadByte() == 1) ? "true" : "false";
                        break;
                    case Datatype.Type.Number:
                        constant.value = this.inputReader.ReadSingle().ToString("0.000000");
                        break;
                    case Datatype.Type.String:
                        int stringLength = this.inputReader.ReadInt32();
                        if(this.luaFile.Game != LuaFile.SupportedGames.BlackOps2)
                        {
                            int unkInt = this.inputReader.ReadInt32();
                        }
                        constant.value = this.inputReader.ReadNullTerminatedString();
                        break;
                    case Datatype.Type.XHash:
                        // TODO : Add support for switch to string
                        constant.value = String.Format("0x{0:X}", this.inputReader.ReadUInt64() & 0xFFFFFFFFFFFFFFF);
                        break;
                    default:
                        constant.value = "Unknown constant type";
                        break;
                }
                // Add it to the array
                this.Constants[i] = constant;
            }
        }

        private void readSubFunctions()
        {
            Console.WriteLine(this.subFunctionCount);
            this.SubFunctions = new LuaFunction[this.subFunctionCount];
            for (int i = 0; i < this.subFunctionCount; i++)
            {
                SubFunctions[i] = new LuaFunction(this.luaFile, this.tabLevel + 1, false, this);
            }
        }

        public void decompile(int tabLevel = 0, bool inline = false)
        {
#if DEBUG
            this.writeLine(String.Format("-- OP Count: 0x{0:X}", this.instructionCount));
            this.writeLine(String.Format("-- Constant Count: 0x{0:X}", this.constantCount));
            this.writeLine(String.Format("-- Registers Count: 0x{0:X}", this.registerCount));
            this.writeLine(String.Format("-- UpValue Count: 0x{0:X}", this.upValsCount));
            this.writeLine(String.Format("-- SubFuncs Count: 0x{0:X}", this.subFunctionCount));
#endif
            this.tabLevel = tabLevel;
            this.Registers = new LuaRegister[this.registerCount];
            for (int i = 0; i < this.registerCount; i++)
            {
                this.Registers[i] = new LuaRegister(i);
            }

            // Write the function header if it isnt the init function
            if (!this.isInitFunction)
            {
                // Look if we have a inline function
                if (inline)
                {
                    outputWriter.Write("function (");
                }
                else if (this.newName == null)
                {
                    write("local function " + String.Format("__FUNC_{0:X}_", this.startPosition) + "(", -1);
                }
                else
                {
                    write("function " + this.newName + "(", -1);
                }
                int argIndex = 0, fixedIndex = 0;
                while (argIndex < this.parameterCount)
                {
                    if (this.Upvalues.Contains("arg" + fixedIndex))
                    {
                        fixedIndex++;
                        continue;
                    }
                    this.Registers[argIndex].initialze("arg" + fixedIndex);
                    outputWriter.Write(((argIndex > 0) ? ", " : "") + "arg" + fixedIndex++);
                    argIndex++;
                }
                if (this.usesVarArg)
                {
                    if (this.parameterCount > 0)
                        outputWriter.Write(", ");
                    outputWriter.Write("...");
                }
                outputWriter.Write(")\n");
            }

            this.instructionPtr = 0;
            while (this.instructionPtr < this.instructionCount)
            {
                try
                {
                    currentInstruction = this.Instructions[instructionPtr];
                    if(LuaOpCode.OPCodeFunctions.TryGetValue(currentInstruction.OpCode, out Action<LuaFunction> func))
                    {
                        func(this);
                    }
                    else
                    {
                        this.writeLine(String.Format("Unhandled OpCode: {0} ({1}, {2}, {3}, 0x{4:X})",
                            currentInstruction.OpCode,
                            currentInstruction.A,
                            currentInstruction.B,
                            currentInstruction.C,
                            (int)currentInstruction.OpCode));
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine("Error @ " + this.instructionPtr + ": " + e.Message);
                }
                nextInstruction();
            }

            // Add an ending to the function
            if (!this.isInitFunction)
            {
                writeLine("end", -1);
                this.outputWriter.WriteLine();
            }
        }

        public void nextInstruction()
        {
            this.instructionPtr++;
        }

        public void writeLine(string text, int extra = 0)
        {
            for (int i = 0; i < this.tabLevel + extra + this.endPositions.Count + this.tablePositions.Count; i++)
            {
                this.outputWriter.Write("\t");
            }
            this.outputWriter.Write(text + "\n");
        }

        public void write(string text, int extra = 0)
        {
            for (int i = 0; i < this.tabLevel + extra + this.endPositions.Count + this.tablePositions.Count; i++)
            {
                this.outputWriter.Write("\t");
            }
            this.outputWriter.Write(text);
        }

        private static byte GetBit(long input, int bit)
        {
            return (byte)((input >> bit) & 1);
        }
    }
}
