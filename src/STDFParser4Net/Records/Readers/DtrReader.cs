using STDFParser4Net.IO;
using STDFParser4Net.Records.Registry;

namespace STDFParser4Net.Records.Readers
{
    internal sealed class DtrReader : IRecordReader
    {
        public StdfRecord Read(StdfBinaryReader reader, in StdfRecordHeader header)
        {
            reader.BeginRecord(header);
            StdfString textDat = reader.HasRemaining(1) ? reader.ReadCn() : default;
            return new DtrRecord(header, textDat);
        }
    }
}
