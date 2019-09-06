using System;
using System.Collections.Generic;
using System.Text;

namespace EPSSharpie.PostScript.Objects
{
    public enum SymbolEnum
    {
        Procedure,
        Array,
        Dictionary
    }

    public class SymbolObject : ObjectBase
    {
        public SymbolEnum Value { get; private set; }

        public SymbolObject(SymbolEnum value)
        {
            Value = value;
        }
}
}
