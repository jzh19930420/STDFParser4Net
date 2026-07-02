using STDFParser4Net.IO;
using STDFParser4Net.Records.Registry;

namespace STDFParser4Net.Records.Readers
{
    internal sealed class WirReader : IRecordReader
    {
        public StdfRecord Read(StdfBinaryReader reader, in StdfRecordHeader header)
        {
            reader.BeginRecord(header);
            byte headNum = reader.ReadU1();
            byte setupGrp = reader.ReadU1();
            uint startT = reader.ReadU4();

            uint? finishT = null;
            if (reader.HasRemaining(4))
                finishT = reader.ReadU4();

            StdfString? waferId = null;
            if (reader.HasRemaining(1))
                waferId = reader.ReadCn();

            return new WirRecord(header, headNum, setupGrp, startT, finishT, waferId);
        }
    }
}
