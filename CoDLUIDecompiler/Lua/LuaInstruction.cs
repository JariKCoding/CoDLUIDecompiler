using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoDLUIDecompiler
{
    class LuaInstruction
    {
        public byte A;
        public int B;
        public int C;
        public LuaOpCode.OpCodes OpCode;
        public int Bx;
        public int sBx;
        public bool visited = false;

        private LuaFunction function;

        public LuaInstruction(LuaFunction function)
        {
            this.function = function;
        }
    }
}
