using System;
using System.IO;
using System.Text;

namespace EPSSharpie
{
    public sealed class EpsDocument
    {
        public EpsHeader EpsHeader { get; private set; }

        public string PostScriptData { get; private set; }

        public byte[] WmfData { get; private set; }

        public byte[] TiffData { get; private set; }

        public static EpsDocument Parse(Stream stream) => new BinaryReader(stream).DisposeAfter(Parse);

        public static EpsDocument Parse(byte[] data) => new MemoryStream(data).DisposeAfter(Parse);



        private static EpsDocument Parse(BinaryReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            //var serializer = new XmlSerializer(typeof(SvgSvgElement));
            //var root = (SvgSvgElement)serializer.Deserialize(reader);
            //root.OnLoaded();

            var epsHeader = new EpsHeader(reader);
            var epsDocument = new EpsDocument
            {
                EpsHeader = epsHeader
            };

            reader.BaseStream.Position = epsHeader.PostScriptOffset;
            epsDocument.PostScriptData = Encoding.UTF8.GetString(reader.ReadBytes((int)epsHeader.PostScriptLength));

            reader.BaseStream.Position = epsHeader.WMFOffset;
            epsDocument.WmfData = reader.ReadBytes((int)epsHeader.WMFSize);

            reader.BaseStream.Position = epsHeader.TIFOffset;
            epsDocument.TiffData = reader.ReadBytes((int)epsHeader.TIFSize);

            return epsDocument;
        }
    }
}
