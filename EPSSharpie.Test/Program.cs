using System;
using System.IO;

namespace EPSSharpie.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var epsData = File.ReadAllBytes("Example.eps");
            var epsDocument = EpsDocument.Parse(epsData);

            Console.WriteLine(epsDocument.PostScriptData);
        }
    }
}
