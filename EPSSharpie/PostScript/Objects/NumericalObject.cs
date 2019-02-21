using System;
using System.Collections.Generic;
using System.Text;

namespace EPSSharpie.PostScript.Objects
{
    public enum NumericalType
    {
        Integer,
        Float,
    }

    internal class NumericalObject : ObjectBase
    {        
        public NumericalType NumericalType { get; private set; }

        public double Double { get; private set; }

        public int Integer => (int)Double;

        public NumericalObject(int value)
        {
            NumericalType = NumericalType.Integer;
            Double = value;            
        }

        public NumericalObject(double value)
        {
            NumericalType = NumericalType.Float;
            Double = value;
        }
    }
}
