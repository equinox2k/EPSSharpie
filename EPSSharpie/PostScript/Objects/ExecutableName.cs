using System;
using System.Collections.Generic;
using System.Text;

namespace EPSSharpie.PostScript.Objects
{
    public class ExecutableName : ObjectBase
    {
        public string Value { get; private set; }

        public ExecutableName(string value)
        {
            Value = value;
        }
    }
}
