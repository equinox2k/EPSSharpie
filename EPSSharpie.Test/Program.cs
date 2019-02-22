using EPSSharpie.PostScript;
using System;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using Ionic.Zlib;

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

            var result = File.ReadAllText("DecodeTest.txt");
            result = result.Replace("\r", "");
            result = result.Replace("\n", "");
            var b = new Ascii85();
            b.EnforceMarks = false;
            var data = b.Decode(result);
            File.WriteAllBytes("test.zip", data);

            var input = new MemoryStream(data);
            var output = new MemoryStream();

            var a = new Ionic.Zlib.GZipStream(input, Ionic.Zlib.CompressionMode.Decompress);
            a.CopyTo(output);

            Console.WriteLine(epsDocument.PostScriptData);

            //https://stackoverflow.com/questions/52173621/decoding-and-decompressing-ai9-datastream-within-eps-files
            //https://www.complang.tuwien.ac.at/ulrich/PS/operators.html
        }
    }
}
