using System;
using System.Collections.Generic;
using System.Text;

namespace EPSSharpie.PostScript.Objects
{
    public enum ObjectFlag
    {
        Literal,
        Executable,
    }

    public enum ObjectAccess
    {
        Unlimited,
        ReadOnly,
        ExecuteOnly,
        None,
    }

    interface IObjectBase
    {

        ObjectFlag Flag { get; }
        ObjectAccess Access { get; }

        bool IsExecutable();
    }
}
