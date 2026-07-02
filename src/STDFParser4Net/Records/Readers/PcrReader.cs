using STDFParser4Net.IO;
using STDFParser4Net.Records.Registry;

namespace STDFParser4Net.Records.Readers
{
    internal sealed class PcrReader : IRecordReader
    {
        public StdfRecord Read(StdfBinaryReader reader, in StdfRecordHeader header)
        {
            reader.BeginRecord(header);
            byte headNum = reader.ReadU1();
            byte siteNum = reader.ReadU1();
            uint partCnt = reader.ReadU4();
            uint rtstCnt = reader.ReadU4();
            uint abrtCnt = reader.ReadU4();
            uint goodCnt = reader.ReadU4();
            uint funcCnt = reader.ReadU4();
            return new PcrRecord(header, headNum, siteNum, partCnt, rtstCnt, abrtCnt, goodCnt, funcCnt);
        }
    }
}
