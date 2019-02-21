using System;
using System.Collections.Generic;
using System.Text;

namespace EPSSharpie.PostScript.Objects
{
    internal class NameObject : ObjectBase
    {
        public string Value { get; internal set; }

        public NameObject(string value, bool executable = true)
        {
            Value = value;
            Flag = ObjectFlag.Executable;
        }
    }
}
