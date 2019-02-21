using EPSSharpie.PostScript;
using System;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;

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

            //var a = new Interpreter();
            //a.Load(epsDocument.PostScriptData);
            Regex rx = new Regex(@"(%AI9_DataStream)([\s\S]*?)(~>)",    RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var result = rx.Match(epsDocument.PostScriptData).ToString();
            result = result.Replace("%AI9_DataStream", "");
            result = result.Replace("\r\n", "");

            var a = new Ascii85();
            var b = a.Decode(result);
            File.WriteAllBytes("test.zip", b);

            var output = new MemoryStream();
                using (DeflateStream decompressionStream = new DeflateStream(new MemoryStream(b), CompressionMode.Decompress))
                {
                decompressionStream.CopyTo(output);
                }
            File.WriteAllBytes("test2.txt", output.ToArray());

            Console.WriteLine(epsDocument.PostScriptData);

            //https://stackoverflow.com/questions/52173621/decoding-and-decompressing-ai9-datastream-within-eps-files
        }
    }
}
