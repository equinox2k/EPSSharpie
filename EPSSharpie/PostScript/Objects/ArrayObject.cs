using System;
using System.Collections.Generic;
using System.Text;

namespace EPSSharpie.PostScript.Objects
{
    public class ArrayObject : ObjectBase
    {
        public ObjectBase[] Value { get; private set; }

        public ArrayObject(ObjectBase[] value)
        {
            Value = value;
        }
    }
}
