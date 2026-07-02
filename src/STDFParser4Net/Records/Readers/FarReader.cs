using STDFParser4Net.IO;
using STDFParser4Net.Records.Registry;

namespace STDFParser4Net.Records.Readers
{
    internal sealed class FarReader : IRecordReader
    {
        public StdfRecord Read(StdfBinaryReader reader, in StdfRecordHeader header)
        {
            reader.BeginRecord(header);
            byte cpuType = reader.ReadU1();
            byte stdfVer = reader.ReadU1();
            return new FarRecord(header, cpuType, stdfVer);
        }
    }
}
