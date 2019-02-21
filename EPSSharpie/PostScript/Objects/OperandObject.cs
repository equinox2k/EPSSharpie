using System;
using System.Collections.Generic;
using System.Text;

namespace EPSSharpie.PostScript.Objects
{
    internal class OperandObject : ObjectBase
    {
        public Action Value { get; internal set; }

        public OperandObject(Action value)
        {
            Value = value;
        }
    }
}
