using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CoDLUIDecompiler
{
    class LuaOpCode
    {
        public enum OpCodes
        {
            HKS_OPCODE_GETFIELD,
            HKS_OPCODE_TEST,
            HKS_OPCODE_CALL_I,
            HKS_OPCODE_CALL_C,
            HKS_OPCODE_EQ,
            HKS_OPCODE_EQ_BK,
            HKS_OPCODE_GETGLOBAL,
            HKS_OPCODE_MOVE,
            HKS_OPCODE_SELF,
            HKS_OPCODE_RETURN,
            HKS_OPCODE_GETTABLE_S,
            HKS_OPCODE_GETTABLE_N,
            HKS_OPCODE_GETTABLE,
            HKS_OPCODE_LOADBOOL,
            HKS_OPCODE_TFORLOOP,
            HKS_OPCODE_SETFIELD,
            HKS_OPCODE_SETTABLE_S,
            HKS_OPCODE_SETTABLE_S_BK,
            HKS_OPCODE_SETTABLE_N,
            HKS_OPCODE_SETTABLE_N_BK,
            HKS_OPCODE_SETTABLE,
            HKS_OPCODE_SETTABLE_BK,
            HKS_OPCODE_TAILCALL_I,
            HKS_OPCODE_TAILCALL_C,
            HKS_OPCODE_TAILCALL_M,
            HKS_OPCODE_LOADK,
            HKS_OPCODE_LOADNIL,
            HKS_OPCODE_SETGLOBAL,
            HKS_OPCODE_JMP,
            HKS_OPCODE_CALL_M,
            HKS_OPCODE_CALL,
            HKS_OPCODE_INTRINSIC_INDEX,
            HKS_OPCODE_INTRINSIC_NEWINDEX,
            HKS_OPCODE_INTRINSIC_SELF,
            HKS_OPCODE_INTRINSIC_INDEX_LITERAL,
            HKS_OPCODE_INTRINSIC_NEWINDEX_LITERAL,
            HKS_OPCODE_INTRINSIC_SELF_LITERAL,
            HKS_OPCODE_TAILCALL,
            HKS_OPCODE_GETUPVAL,
            HKS_OPCODE_SETUPVAL,
            HKS_OPCODE_ADD,
            HKS_OPCODE_ADD_BK,
            HKS_OPCODE_SUB,
            HKS_OPCODE_SUB_BK,
            HKS_OPCODE_MUL,
            HKS_OPCODE_MUL_BK,
            HKS_OPCODE_DIV,
            HKS_OPCODE_DIV_BK,
            HKS_OPCODE_MOD,
            HKS_OPCODE_MOD_BK,
            HKS_OPCODE_POW,
            HKS_OPCODE_POW_BK,
            HKS_OPCODE_NEWTABLE,
            HKS_OPCODE_UNM,
            HKS_OPCODE_NOT,
            HKS_OPCODE_LEN,
            HKS_OPCODE_LT,
            HKS_OPCODE_LT_BK,
            HKS_OPCODE_LE,
            HKS_OPCODE_LE_BK,
            HKS_OPCODE_SHIFT_LEFT,
            HKS_OPCODE_SHIFT_LEFT_BK,
            HKS_OPCODE_SHIFT_RIGHT,
            HKS_OPCODE_SHIFT_RIGHT_BK,
            HKS_OPCODE_BITWISE_AND,
            HKS_OPCODE_BITWISE_AND_BK,
            HKS_OPCODE_BITWISE_OR,
            HKS_OPCODE_BITWISE_OR_BK,
            HKS_OPCODE_CONCAT,
            HKS_OPCODE_TESTSET,
            HKS_OPCODE_FORPREP,
            HKS_OPCODE_FORLOOP,
            HKS_OPCODE_SETLIST,
            HKS_OPCODE_CLOSE,
            HKS_OPCODE_CLOSURE,
            HKS_OPCODE_VARARG,
            HKS_OPCODE_TAILCALL_I_R1,
            HKS_OPCODE_CALL_I_R1,
            HKS_OPCODE_SETUPVAL_R1,
            HKS_OPCODE_TEST_R1,
            HKS_OPCODE_NOT_R1,
            HKS_OPCODE_GETFIELD_R1,
            HKS_OPCODE_SETFIELD_R1,
            HKS_OPCODE_NEWSTRUCT,
            HKS_OPCODE_DATA,
            HKS_OPCODE_SETSLOTN,
            HKS_OPCODE_SETSLOTI,
            HKS_OPCODE_SETSLOT,
            HKS_OPCODE_SETSLOTS,
            HKS_OPCODE_SETSLOTMT,
            HKS_OPCODE_CHECKTYPE,
            HKS_OPCODE_CHECKTYPES,
            HKS_OPCODE_GETSLOT,
            HKS_OPCODE_GETSLOTMT,
            HKS_OPCODE_SELFSLOT,
            HKS_OPCODE_SELFSLOTMT,
            HKS_OPCODE_GETFIELD_MM,
            HKS_OPCODE_CHECKTYPE_D,
            HKS_OPCODE_GETSLOT_D,
            HKS_OPCODE_GETGLOBAL_MEM,
            HKS_OPCODE_MAX,
            HKS_OPCODE_DELETE,
            HKS_OPCODE_DELETE_BK,
            HKS_OPCODE_UNK,
        }
        

        public static Dictionary<LuaOpCode.OpCodes, Func<LuaFunction, string>> OPCodeFunctions = new Dictionary<LuaOpCode.OpCodes, Func<LuaFunction, string>>()
        {
            { LuaOpCode.OpCodes.HKS_OPCODE_GETFIELD, OP_GetField },
            { LuaOpCode.OpCodes.HKS_OPCODE_CALL_I, OP_Call_I },
            { LuaOpCode.OpCodes.HKS_OPCODE_EQ, OP_Eq },
            { LuaOpCode.OpCodes.HKS_OPCODE_GETGLOBAL, OP_GetGlobal },
            { LuaOpCode.OpCodes.HKS_OPCODE_MOVE, OP_Move },
            { LuaOpCode.OpCodes.HKS_OPCODE_SELF, OP_Self },
            { LuaOpCode.OpCodes.HKS_OPCODE_RETURN, OP_Return },
            { LuaOpCode.OpCodes.HKS_OPCODE_LOADBOOL, OP_LoadBool },
            { LuaOpCode.OpCodes.HKS_OPCODE_SETFIELD, OP_SetField },
            { LuaOpCode.OpCodes.HKS_OPCODE_LOADK, OP_LoadK },
            { LuaOpCode.OpCodes.HKS_OPCODE_LOADNIL, OP_LoadNil },
            { LuaOpCode.OpCodes.HKS_OPCODE_JMP, OP_Jmp },
            { LuaOpCode.OpCodes.HKS_OPCODE_ADD, OP_Add },
            { LuaOpCode.OpCodes.HKS_OPCODE_ADD_BK, OP_AddBk },
            { LuaOpCode.OpCodes.HKS_OPCODE_SUB, OP_Sub },
            { LuaOpCode.OpCodes.HKS_OPCODE_SUB_BK, OP_SubBk },
            { LuaOpCode.OpCodes.HKS_OPCODE_MUL, OP_Mul },
            { LuaOpCode.OpCodes.HKS_OPCODE_MUL_BK, OP_MulBk },
            { LuaOpCode.OpCodes.HKS_OPCODE_DIV, OP_Div },
            { LuaOpCode.OpCodes.HKS_OPCODE_DIV_BK, OP_DivBk },
            { LuaOpCode.OpCodes.HKS_OPCODE_MOD, OP_Mod },
            { LuaOpCode.OpCodes.HKS_OPCODE_MOD_BK, OP_ModBk },
            { LuaOpCode.OpCodes.HKS_OPCODE_POW, OP_Pow },
            { LuaOpCode.OpCodes.HKS_OPCODE_POW_BK, OP_PowBk },
            { LuaOpCode.OpCodes.HKS_OPCODE_NEWTABLE, OP_NewTable },
            { LuaOpCode.OpCodes.HKS_OPCODE_LT, OP_Lt },
            { LuaOpCode.OpCodes.HKS_OPCODE_LE, OP_Le },
            { LuaOpCode.OpCodes.HKS_OPCODE_CLOSURE, OP_Closure },
            { LuaOpCode.OpCodes.HKS_OPCODE_GETFIELD_R1, OP_GetFieldR1 },
            { LuaOpCode.OpCodes.HKS_OPCODE_DATA, OP_Data },
        };

        public static string OP_GetField(LuaFunction function)
        {
            function.Registers[function.currentInstruction.A].value = function.Registers[function.currentInstruction.B].value + "." + function.Constants[function.currentInstruction.C].value;
#if DEBUG
            function.writeLine(String.Format("-- r({0}) = r({1}).field({2}) // {3}",
                function.currentInstruction.A,
                function.currentInstruction.B,
                function.currentInstruction.C,
                function.Registers[function.currentInstruction.A].value));
#endif
            return "";
        }

        public static string OP_Call_I(LuaFunction function)
        {
            string funcName = function.Registers[function.currentInstruction.A].value;

            int parameterCount = function.currentInstruction.B - 1;
            int returnValueCount = function.currentInstruction.C - 1;
            string parametersString = "";

            // Setting up the parameter string
            // Check if b >= 1
            // if it is, there are b - 1 parameters
            if (parameterCount > 0)
            {
                byte startIndex = 1;
                // If the functions gets called on something, we want to use 1 parameter less
                if(funcName.Contains(":"))
                {
                    startIndex = 2;
                }
                parametersString += function.Registers[function.currentInstruction.A + startIndex].value;
                for (int j = function.currentInstruction.A + startIndex + 1; j <= function.currentInstruction.A + parameterCount; j++)
                {
                    parametersString += ", " + function.Registers[j].value;
                }
            }
            // If b is 0
            // parameters range from a + 1 to the top of the stack
            else if (parameterCount < 0)
            {
                byte startIndex = 1;
                if (funcName.Contains(":"))
                {
                    startIndex = 2;
                }
                parametersString += function.Registers[function.currentInstruction.A + startIndex].value;
                for (int j = function.currentInstruction.A + startIndex + 1; j <= function.Instructions[function.instructionPtr - 1].A; j++)
                {
                    parametersString += ", " + function.Registers[j].value;
                }
            }

            // Setting up the return values
            // Check if c >= 1
            // if it is, there are c - 1 return values
            if (returnValueCount > 0)
            {
                string returnVars = function.Registers[function.currentInstruction.A].makeLocalVariable();
                for (int i = function.currentInstruction.A + 1; i < function.currentInstruction.A + returnValueCount; i++)
                {
                    returnVars += ", " + function.Registers[i].makeLocalVariable();
                    function.Registers[i].isInitialized = true;
                }
                function.writeLine(String.Format("{0}{1} = {2}({3})",
                    (!function.Registers[function.currentInstruction.A].isInitialized) ? "local " : "",
                    returnVars,
                    funcName,
                    parametersString));
                function.Registers[function.currentInstruction.A].isInitialized = true;
                for (int i = function.currentInstruction.A + 1; i < function.currentInstruction.A + returnValueCount; i++)
                {
                    function.Registers[i].isInitialized = true;
                }
            }
            // We dont have return values
            else
            {
                function.writeLine(String.Format("{1}({2})",
                    (returnValueCount == -1) ? "return " : "", // if c = 0 it returns the value straight away
                    funcName,
                    parametersString));
            }
            return "";
        }

        public static string OP_Eq(LuaFunction function)
        {
            return DoCondition(function, "==", "~=");
        }

        private static string DoCondition(LuaFunction function, string oper, string operFalse)
        {
            string cValue = getCValue(function);
            return String.Format("{0} {1} {2}",
                    function.Registers[function.currentInstruction.B].value,
                    (function.currentInstruction.A == 0) ? oper : operFalse,
                    cValue);
        }

        public static string OP_GetGlobal(LuaFunction function)
        {
            function.Registers[function.currentInstruction.A].changeTo(function.Constants[function.currentInstruction.Bx], true);
            function.Registers[function.currentInstruction.A].globalValue = true;
#if DEBUG
            function.writeLine(String.Format("-- r({0}) = g[{1}] // {2}",
                function.currentInstruction.A,
                function.currentInstruction.Bx,
                function.Constants[function.currentInstruction.Bx].value));
#endif
            return "";
        }

        public static string OP_Move(LuaFunction function)
        {
            function.Registers[function.currentInstruction.A].value = function.Registers[function.currentInstruction.B].value;
#if DEBUG
            function.writeLine(String.Format("-- r({0}) = r({1}) // {2}",
                function.currentInstruction.A,
                function.currentInstruction.B,
                function.Registers[function.currentInstruction.A].value));
#endif
            return "";
        }

        public static string OP_Self(LuaFunction function)
        {
            string cValue;
            if (function.currentInstruction.C > 255)
            {
                function.Registers[function.currentInstruction.A].value = function.Registers[function.currentInstruction.B].value + ":" + function.Constants[function.currentInstruction.C - 256].value;
#if DEBUG
                function.writeLine(String.Format("-- r({0}) = r({1}):c[{2}] // {3}",
                    function.currentInstruction.A,
                    function.currentInstruction.B,
                    function.currentInstruction.C - 256,
                    function.Registers[function.currentInstruction.A].value));
#endif
            }
            else
            {
                function.Registers[function.currentInstruction.A].value = function.Registers[function.currentInstruction.B].value + ":" + function.Registers[function.currentInstruction.C].value;
#if DEBUG
                function.writeLine(String.Format("-- r({0}) = r({1}):r({2}) // {3}",
                    function.currentInstruction.A,
                    function.currentInstruction.B,
                    function.currentInstruction.C,
                    function.Registers[function.currentInstruction.A].value));
#endif
            }
            function.Registers[function.currentInstruction.A].type = Datatype.Type.Function;
            return "";
        }

        public static string OP_Return(LuaFunction function)
        {
            // we dont want to write return at the end of every function, only if it actually does something
            if (function.instructionPtr == function.instructionCount - 1)
            {
#if DEBUG
                function.writeLine("-- return");
#endif
                return "";
            }
            string returns = "";
            if (function.currentInstruction.B > 1)
            {
                returns += function.Registers[function.currentInstruction.A].value;
                for (int i = function.currentInstruction.A + 1; i <= function.currentInstruction.A + function.currentInstruction.B - 2; i++)
                {
                    returns += ", " + function.Registers[i];
                }
            }
            function.writeLine("return " + returns);
            return "";
        }

        public static string OP_LoadBool(LuaFunction function)
        {
            if (function.currentInstruction.B == 0)
                function.Registers[function.currentInstruction.A].value = "false";
            else
                function.Registers[function.currentInstruction.A].value = "true";
            function.Registers[function.currentInstruction.A].type = Datatype.Type.Boolean;
#if DEBUG
            function.writeLine(String.Format("-- r({0}) = {1}{2}",
                function.currentInstruction.A,
                function.Registers[function.currentInstruction.A].value,
                (function.currentInstruction.C == 1) ? " // skip next opcode" : ""));
#endif
            return "";
        }

        public static string OP_SetField(LuaFunction function)
        {
            string cValue = getCValue(function);
            function.writeLine(String.Format("{0}.{1} = {2}",
                function.Registers[function.currentInstruction.A].value,
                function.Constants[function.currentInstruction.B].value,
                cValue
            ));
            return "";
        }

        public static string OP_LoadK(LuaFunction function)
        {
            function.Registers[function.currentInstruction.A].changeTo(function.Constants[function.currentInstruction.Bx].getString());
#if DEBUG
            function.writeLine(String.Format("-- r({0}) = c[{1}] // {2}",
                function.currentInstruction.A,
                function.currentInstruction.Bx,
                function.Registers[function.currentInstruction.A].value));
#endif
            return "";
        }

        public static string OP_LoadNil(LuaFunction function)
        {
            for (int i = function.currentInstruction.A; i <= (function.currentInstruction.B); i++)
            {
                function.Registers[i].value = "nil";
                function.Registers[i].type = Datatype.Type.Nil;
            }
#if DEBUG
            if (function.currentInstruction.B > function.currentInstruction.A)
                function.writeLine(String.Format("-- r({0} to {1}) inclusive = nil", function.currentInstruction.A, function.currentInstruction.B));
            else
                function.writeLine(String.Format("-- r({0}) = nil", function.currentInstruction.A));
#endif
            return "";
        }

        public static string OP_Jmp(LuaFunction function)
        {
#if DEBUG
            function.writeLine(String.Format("-- skip the next [{0}] opcodes // advance {0} lines",
                function.currentInstruction.sBx));
#endif
            return "";
        }

        public static string OP_Add(LuaFunction function)
        {
            return DoOperator(function, "+");
        }

        public static string OP_AddBk(LuaFunction function)
        {
            return DoOperatorBK(function, "+");
        }

        public static string OP_Sub(LuaFunction function)
        {
            return DoOperator(function, "-");
        }

        public static string OP_SubBk(LuaFunction function)
        {
            return DoOperatorBK(function, "-");
        }

        public static string OP_Mul(LuaFunction function)
        {
            return DoOperator(function, "*");
        }

        public static string OP_MulBk(LuaFunction function)
        {
            return DoOperatorBK(function, "*");
        }

        public static string OP_Div(LuaFunction function)
        {
            return DoOperator(function, "/");
        }

        public static string OP_DivBk(LuaFunction function)
        {
            return DoOperatorBK(function, "/");
        }

        public static string OP_Mod(LuaFunction function)
        {
            return DoOperator(function, "%");
        }

        public static string OP_ModBk(LuaFunction function)
        {
            return DoOperatorBK(function, "%");
        }

        public static string OP_Pow(LuaFunction function)
        {
            return DoOperator(function, "^");
        }

        public static string OP_PowBk(LuaFunction function)
        {
            return DoOperatorBK(function, "^");
        }

        public static string OP_NewTable(LuaFunction function)
        {
            if(function.currentInstruction.B == 0 && function.currentInstruction.C == 0)
            {
                function.Registers[function.currentInstruction.A].value = "{}";
                function.Registers[function.currentInstruction.A].type = Datatype.Type.Table;
                return "";
            }
            function.Registers[function.currentInstruction.A].makeLocalVariable();
            function.writeLine(String.Format("{0}{1} = {2}",
                (!function.Registers[function.currentInstruction.A].isInitialized) ? "local " : "",
                function.Registers[function.currentInstruction.A].value, "{}"
            ));
            function.Registers[function.currentInstruction.A].isInitialized = true;
            function.Registers[function.currentInstruction.A].globalValue = false;
            return "";
        }

        public static string OP_Lt(LuaFunction function)
        {
            return DoCondition(function, "<", ">=");
        }

        public static string OP_Le(LuaFunction function)
        {
            return DoCondition(function, "<=", ">");
        }

        public static string OP_Closure(LuaFunction function)
        {
            function.Registers[function.currentInstruction.A].value = String.Format("__FUNC_{0:X}_", function.SubFunctions[function.currentInstruction.Bx].startPosition);
            function.Registers[function.currentInstruction.A].type = Datatype.Type.Function;
            long oldPos = function.inputReader.BaseStream.Position;
            function.inputReader.Seek(function.SubFunctions[function.currentInstruction.Bx].startPosition, SeekOrigin.Begin);
            function.SubFunctions[function.currentInstruction.Bx].decompile(function.tabLevel + function.endPositions.Count + function.tablePositions.Count + 1);
            function.inputReader.Seek(oldPos, SeekOrigin.Begin);
            return "";
        }

        public static string OP_GetFieldR1(LuaFunction function)
        {
            function.Registers[function.currentInstruction.A].value = function.Registers[function.currentInstruction.B].value + "." + function.Constants[function.currentInstruction.C].value;
            return "";
        }

        public static string OP_Data(LuaFunction function)
        {
            return "";
        }

        public static string getCValue(LuaFunction function)
        {
            if (function.currentInstruction.C > 255)
            {
                return function.Constants[function.currentInstruction.C - 256].getString();
            }
            else
            {
                return function.Registers[function.currentInstruction.C].value;
            }
        }

        public static string DoOperator(LuaFunction function, string oper)
        {
            string cValue = getCValue(function);
            function.Registers[function.currentInstruction.A].value = String.Format("({0} {1} {2})",
                function.Registers[function.currentInstruction.B].value,
                oper,
                cValue);
            return "";
        }

        public static string DoOperatorBK(LuaFunction function, string oper)
        {
            function.Registers[function.currentInstruction.A].value = String.Format("({0} {1} {2})",
                function.Constants[function.currentInstruction.B].value,
                oper,
                function.Registers[function.currentInstruction.C].value);
            return "";
        }

        public static bool isConditionOPCode(LuaFunction function, int ptr)
        {
            OpCodes OpCode = function.Instructions[ptr].OpCode;
            if (OpCode == OpCodes.HKS_OPCODE_EQ || OpCode == OpCodes.HKS_OPCODE_EQ_BK || OpCode == OpCodes.HKS_OPCODE_LE || OpCode == OpCodes.HKS_OPCODE_LE_BK ||
                OpCode == OpCodes.HKS_OPCODE_LT || OpCode == OpCodes.HKS_OPCODE_LT_BK)
                return true;
            return false;
        }
    }
}
