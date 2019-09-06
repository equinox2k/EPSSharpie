using EPSSharpie.PostScript;
using EPSSharpie.PostScript.Objects;
using System;
using System.IO;
using System.Text;
using Xunit;
using EPSSharpie;

namespace EPSSharpie.Tests
{
    public class UnitTest1
    {
        //https://stackoverflow.com/questions/52173621/decoding-and-decompressing-ai9-datastream-within-eps-files
        //https://www.complang.tuwien.ac.at/ulrich/PS/operators.html
        //https://www.complang.tuwien.ac.at/ulrich/PS/postscript.html
        //https://www-cdf.fnal.gov/offline/PostScript/PLRM3.pdf

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


        //var testData1 = "abc %123123e32323\n123";
        //interpreter.Load(Encoding.ASCII.GetBytes(testData1));

        //var testData2 = "(Strings may contain special characters *!&}^% and balanced parentheses ( ) (and so on).)";
        //interpreter.Load(Encoding.ASCII.GetBytes(testData2));

        //var testData3 = "(Strings may contain octal e.g. \\11065)";
        //interpreter.Load(Encoding.ASCII.GetBytes(testData3));

        [Fact]
        public void Test1()
        {
            var psData = ResourceLoader.GetEmbeddedResourceBytes("Test001.ps");
            var interpreter = new Interpreter();
            interpreter.Load(psData);
            Assert.Single(interpreter.OperandStack);
            var value = interpreter.OperandStack.Pop();
            Assert.IsAssignableFrom<NumericalObject>(value);
            var numericalObject = value as NumericalObject;
            Assert.Equal(50, numericalObject.Value);
        }

        [Fact]
        public void Test2()
        {
            var psData = ResourceLoader.GetEmbeddedResourceBytes("Test002.ps");
            var interpreter = new Interpreter();
            interpreter.Load(psData);
            Assert.Single(interpreter.OperandStack);
            var value = interpreter.OperandStack.Pop();
            Assert.IsAssignableFrom<NumericalObject>(value);
            var numericalObject = value as NumericalObject;
            Assert.Equal(50, (int)numericalObject.Value);
        }

        [Fact]
        public void Test3()
        {
            var psData = ResourceLoader.GetEmbeddedResourceBytes("Test003.ps");
            var interpreter = new Interpreter();
            interpreter.Load(psData);
            Assert.Single(interpreter.OperandStack);
            var value = interpreter.OperandStack.Pop();
            Assert.IsAssignableFrom<NumericalObject>(value);
            var numericalObject = value as NumericalObject;
            Assert.Equal(10, (int)numericalObject.Value);
        }

        [Fact]
        public void Test99()
        {
            var psData = ResourceLoader.GetEmbeddedResourceBytes("Test099.ps");
            var interpreter = new Interpreter();
            interpreter.Load(psData);
        }


        [Fact]
        public void Test100()
        {
            var epsData = ResourceLoader.GetEmbeddedResourceBytes("Test100.eps");
            var epsDocument = EpsDocument.Parse(epsData);
            if (epsDocument.TiffData.Length > 0)
            {
                File.WriteAllBytes("Test1.tif", epsDocument.TiffData);
            }

            var interpreter = new Interpreter();
            interpreter.Load(epsDocument.PostScriptData);
        }




    }
}
