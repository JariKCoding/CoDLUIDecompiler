using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoDLUIDecompiler
{
    class LuaCondition
    {
        public int line;

        public enum Type
        {
            If,
            For,
            Foreach,
            While,
            DoWhile,
            Else,
            End,
        }

        public enum Prefix
        {
            none,
            and,
            or
        }

        public Type type;

        public Prefix prefix;

        public LuaCondition master;

        public LuaCondition(int line, Type type, Prefix prefix = Prefix.none, LuaCondition master = null)
        {
            this.line = line;
            this.type = type;
            this.prefix = prefix;
            this.master = master;
        }
    }
}
