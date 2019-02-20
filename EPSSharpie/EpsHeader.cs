using System;
using System.IO;

namespace EPSSharpie
{
    public sealed class EpsHeader
    {
        public uint Id { get; private set; }
        public uint PostScriptOffset { get; private set; }
        public uint PostScriptLength { get; private set; }
        public uint WMFOffset { get; private set; }
        public uint WMFSize { get; private set; }
        public uint TIFOffset { get; private set; }
        public uint TIFSize { get; private set; }
        public uint CheckSum { get; private set; }

        internal EpsHeader(BinaryReader reader)
        {
            Id = reader.ReadUInt32();
            PostScriptOffset = reader.ReadUInt32();
            PostScriptLength = reader.ReadUInt32();
            WMFOffset = reader.ReadUInt32();
            WMFSize = reader.ReadUInt32();
            TIFOffset = reader.ReadUInt32();
            TIFSize = reader.ReadUInt32();
            CheckSum = reader.ReadUInt32();
        }
    }
}
