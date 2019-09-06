using System;
using System.Collections.Generic;
using System.Text;

namespace EPSSharpie.PostScript.Objects
{
    public class LiteralName : ObjectBase
    {
        public string Value { get; private set; }

        public LiteralName(string value)
        {
            Value = value;
        }
    }
}
