using EPSSharpie.PostScript;
using System;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using Ionic.Zlib;
using System.Text;

namespace EPSSharpie.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var epsData = File.ReadAllBytes("Example.eps");
            var epsDocument = EpsDocument.Parse(epsData);
            if (epsDocument.TiffData.Length > 0)
            {
                File.WriteAllBytes("Example.tif", epsDocument.TiffData);
            }

            //var result = File.ReadAllText("DecodeTest.txt");
            //result = result.Replace("\r", "");
            //result = result.Replace("\n", "");
            //var b = new Ascii85();
            //b.EnforceMarks = false;
            //var data = b.Decode(result);

            //var input = new MemoryStream(data);
            //var output = new MemoryStream();
            //var a = new Ionic.Zlib.DeflateStream(input, Ionic.Zlib.CompressionMode.Decompress);
            //a.CopyTo(output);

            //Console.WriteLine(epsDocument.PostScriptData);

            var interpreter = new Interpreter();

            //var testData1 = "abc %123123e32323\n123";
            //interpreter.Load(Encoding.ASCII.GetBytes(testData1));

            //var testData2 = "(Strings may contain special characters *!&}^% and balanced parentheses ( ) (and so on).)";
            //interpreter.Load(Encoding.ASCII.GetBytes(testData2));

            var testData3 = "(Strings may contain octal e.g. \\11065)";
            interpreter.Load(Encoding.ASCII.GetBytes(testData3));

            interpreter.Load(epsDocument.PostScriptData);


            //https://stackoverflow.com/questions/52173621/decoding-and-decompressing-ai9-datastream-within-eps-files
            //https://www.complang.tuwien.ac.at/ulrich/PS/operators.html
            //https://www.complang.tuwien.ac.at/ulrich/PS/postscript.html
            //https://www-cdf.fnal.gov/offline/PostScript/PLRM3.pdf
        }
    }
}
