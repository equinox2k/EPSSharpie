using System;
using System.Collections.Generic;
using System.Text;

namespace EPSSharpie.PostScript.Objects
{
    public class OperandObject : ObjectBase
    {
        public Action Value { get; private set; }

        public OperandObject(Action value)
        {
            Value = value;
        }
    }
}
