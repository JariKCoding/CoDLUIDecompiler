using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoDLUIDecompiler
{
    class LuaConstant
    {
        public Datatype.Type type;
        public string value;


        public string getString()
        {
            if (this.type == Datatype.Type.String && this.value != "nil")
                return "\"" + this.value + "\"";
            else
                return this.value;
        }
    }
}
