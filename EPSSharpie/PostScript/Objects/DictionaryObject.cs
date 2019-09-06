using System;
using System.Collections.Generic;
using System.Text;

namespace EPSSharpie.PostScript.Objects
{
    public class DictionaryObject : ObjectBase
    {
        public IDictionary<LiteralName, ObjectBase> Value { get; private set; }

        public DictionaryObject(IDictionary<LiteralName, ObjectBase> value)
        {
            Value = value;
        }
    }
}
