using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoDLUIDecompiler
{
    class LuaRegister
    {
        public string value = "";
        public int index;
        public Datatype.Type type;
        public bool globalValue = false;
        public bool isInitialized = false;

        public LuaRegister(int index)
        {
            this.isInitialized = false;
            this.index = index;
            this.globalValue = false;
        }

        public void changeTo(LuaConstant constant, bool isGlobal = false)
        {
            this.globalValue = isGlobal;
            this.value = constant.value;

            this.type = constant.type;
        }

        public void changeTo(string value)
        {
            this.value = value;
        }

        public string makeLocalVariable()
        {
            this.value = "registerVal" + this.index;
            return this.value;
        }

        public void initialze(string value)
        {
            this.value = value;
            this.isInitialized = true;
        }
    }
}
