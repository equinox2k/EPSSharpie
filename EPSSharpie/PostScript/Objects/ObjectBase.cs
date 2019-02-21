using System;
using System.Collections.Generic;
using System.Text;

namespace EPSSharpie.PostScript.Objects
{
    internal abstract class ObjectBase : IObjectBase
    {        

        public ObjectFlag Flag { get; internal set; }
        public ObjectAccess Access { get; internal set; }

        public ObjectBase()
        {
            Flag = ObjectFlag.Literal;
            Access = ObjectAccess.Unlimited;
        }

        public bool IsExecutable()
        {
            return Flag == ObjectFlag.Executable;
        }
    }
}
