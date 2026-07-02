using STDFParser4Net.IO;
using STDFParser4Net.Records.Registry;

namespace STDFParser4Net.Records.Readers
{
    internal sealed class HbrReader : IRecordReader
    {
        public StdfRecord Read(StdfBinaryReader reader, in StdfRecordHeader header)
        {
            reader.BeginRecord(header);
            byte headNum = reader.ReadU1();
            byte siteNum = reader.ReadU1();
            ushort hbinNum = reader.ReadU2();
            uint hbinCnt = reader.ReadU4();
            char hbinPf = reader.ReadC1();
            StdfString hbinNam = reader.HasRemaining(1) ? reader.ReadCn() : default;
            return new HbrRecord(header, headNum, siteNum, hbinNum, hbinCnt, hbinPf, hbinNam);
        }
    }
}
