using System;
using System.Collections.Generic;
using System.Text;

namespace EPSSharpie.PostScript.Objects
{
    public class ProcedureObject : ObjectBase
    {
        public ObjectBase[] Value { get; private set; }

        public ProcedureObject(ObjectBase[] value)
        {
            Value = value;
        }
    }
}
