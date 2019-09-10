using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoDLUIDecompiler
{
    class Datatype
    {
        public string Name;
        public int Id;
        public int Unk;

        public enum Type
        {
            Nil,
            Boolean,
            LightUserData,
            Number,
            String,
            Table,
            Function,
            UserData,
            Thread,
            IFunction,
            CFunction,
            UI64,
            Struct,
            XHash
        }

        public Datatype(int ID, int Unk, string Name)
        {
            this.Name = Name;
            this.Id = ID;
            this.Unk = Unk;
        }
    }
}
