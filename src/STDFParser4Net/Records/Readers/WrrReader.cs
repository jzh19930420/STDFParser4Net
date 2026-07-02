using STDFParser4Net.IO;
using STDFParser4Net.Records.Registry;

namespace STDFParser4Net.Records.Readers
{
    internal sealed class WrrReader : IRecordReader
    {
        public StdfRecord Read(StdfBinaryReader reader, in StdfRecordHeader header)
        {
            reader.BeginRecord(header);
            byte headNum = reader.ReadU1();
            uint partCnt = reader.ReadU4();
            uint rtstCnt = reader.ReadU4();
            uint abrtCnt = reader.ReadU4();
            uint goodCnt = reader.ReadU4();
            uint funcCnt = reader.ReadU4();

            StdfString? waferId = null;
            if (reader.HasRemaining(1))
                waferId = reader.ReadCn();

            StdfString? fabwfId = null;
            if (reader.HasRemaining(1))
                fabwfId = reader.ReadCn();

            StdfString? wafrNote = null;
            if (reader.HasRemaining(1))
                wafrNote = reader.ReadCn();

            return new WrrRecord(header, headNum, partCnt, rtstCnt, abrtCnt, goodCnt, funcCnt,
                waferId, fabwfId, wafrNote);
        }
    }
}
