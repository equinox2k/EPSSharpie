using System;
using System.Collections.Generic;
using System.Text;

namespace EPSSharpie.PostScript.Objects
{
    public class BooleanObject : ObjectBase
    {
        public bool Value { get; private set; }

        public BooleanObject(bool value)
        {
            Value = value;
        }
    }
}
