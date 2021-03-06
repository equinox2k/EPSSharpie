﻿using System;
using System.Collections.Generic;
using System.Text;

namespace EPSSharpie.PostScript.Objects
{
    public class StringObject : ObjectBase
    {
        public string Value { get; private set; }

        public StringObject(string value)
        {
            Value = value;
        }
    }
}
