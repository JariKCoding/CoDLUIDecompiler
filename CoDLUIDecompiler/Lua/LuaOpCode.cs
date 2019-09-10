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
            HKS_OPCODE_UNK,
        }
        

        public static Dictionary<LuaOpCode.OpCodes, Action<LuaFunction>> OPCodeFunctions = new Dictionary<LuaOpCode.OpCodes, Action<LuaFunction>>()
        {
            { LuaOpCode.OpCodes.HKS_OPCODE_GETGLOBAL, OP_GetGlobal },
            { LuaOpCode.OpCodes.HKS_OPCODE_MOVE, OP_Move },
            { LuaOpCode.OpCodes.HKS_OPCODE_LOADK, OP_LoadK },
            { LuaOpCode.OpCodes.HKS_OPCODE_CLOSURE, OP_Closure },
        };

        public static void OP_GetGlobal(LuaFunction function)
        {
            function.Registers[function.currentInstruction.A].changeTo(function.Constants[function.currentInstruction.Bx], true);
#if DEBUG
            function.writeLine(String.Format("-- r({0}) = g[{1}] // {2}",
                function.currentInstruction.A,
                function.currentInstruction.Bx,
                function.Constants[function.currentInstruction.Bx].value));
#endif
        }

        public static void OP_Move(LuaFunction function)
        {
            function.Registers[function.currentInstruction.A].value = function.Registers[function.currentInstruction.B].value;
#if DEBUG
            function.writeLine(String.Format("-- r({0}) = r({1}) // {2}",
                function.currentInstruction.A,
                function.currentInstruction.B,
                function.Registers[function.currentInstruction.A].value));
#endif
        }

        public static void OP_LoadK(LuaFunction function)
        {
            function.Registers[function.currentInstruction.A].changeTo(function.Constants[function.currentInstruction.Bx].getString());
#if DEBUG
            function.writeLine(String.Format("-- r({0}) = c[{1}] // {2}",
                function.currentInstruction.A,
                function.currentInstruction.Bx,
                function.Registers[function.currentInstruction.A]));
#endif
        }

        public static void OP_Closure(LuaFunction function)
        {
            long oldPos = function.inputReader.BaseStream.Position;
            function.inputReader.Seek(function.SubFunctions[function.currentInstruction.Bx].startPosition, SeekOrigin.Begin);
            function.SubFunctions[function.currentInstruction.Bx].decompile(function.tabLevel + function.endPositions.Count + function.tablePositions.Count + 1);
            function.inputReader.Seek(oldPos, SeekOrigin.Begin);
        }
    }
}
