using System;
using System.Collections.Generic;
using System.Text;

namespace EPSSharpie.PostScript.Objects
{
    public class NumericalObject : ObjectBase
    {
        public double Value { get; private set; }

        public NumericalObject(double value)
        {
            Value = value;
        }

        public NumericalObject(long value)
        {
            Value = value;
        }
    }
}
