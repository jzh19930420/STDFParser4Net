using STDFParser4Net.IO;
using STDFParser4Net.Records.Registry;

namespace STDFParser4Net.Records.Readers
{
    internal sealed class SbrReader : IRecordReader
    {
        public StdfRecord Read(StdfBinaryReader reader, in StdfRecordHeader header)
        {
            reader.BeginRecord(header);
            byte headNum = reader.ReadU1();
            byte siteNum = reader.ReadU1();
            ushort sbinNum = reader.ReadU2();
            uint sbinCnt = reader.ReadU4();
            char sbinPf = reader.ReadC1();
            StdfString sbinNam = reader.HasRemaining(1) ? reader.ReadCn() : default;
            return new SbrRecord(header, headNum, siteNum, sbinNum, sbinCnt, sbinPf, sbinNam);
        }
    }
}
