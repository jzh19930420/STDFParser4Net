using STDFParser4Net.IO;
using STDFParser4Net.Records.Registry;

namespace STDFParser4Net.Records.Readers
{
    internal sealed class AtrReader : IRecordReader
    {
        public StdfRecord Read(StdfBinaryReader reader, in StdfRecordHeader header)
        {
            reader.BeginRecord(header);
            uint modTim = reader.ReadU4();
            StdfString cmdLine = reader.HasRemaining(1) ? reader.ReadCn() : default;
            return new AtrRecord(header, modTim, cmdLine);
        }
    }
}
