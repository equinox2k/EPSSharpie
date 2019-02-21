using System;
using System.Collections.Generic;
using System.Text;

namespace EPSSharpie.PostScript.Objects
{
    internal class StringObject : ObjectBase
    {
        public string Value { get; internal set; }

        public StringObject(string value)
        {
            Value = value;
        }
    }
}
