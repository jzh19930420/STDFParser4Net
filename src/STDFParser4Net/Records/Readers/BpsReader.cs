using STDFParser4Net.IO;
using STDFParser4Net.Records.Registry;

namespace STDFParser4Net.Records.Readers
{
    internal sealed class BpsReader : IRecordReader
    {
        public StdfRecord Read(StdfBinaryReader reader, in StdfRecordHeader header)
        {
            reader.BeginRecord(header);
            StdfString seqName = reader.HasRemaining(1) ? reader.ReadCn() : default;
            return new BpsRecord(header, seqName);
        }
    }
}
