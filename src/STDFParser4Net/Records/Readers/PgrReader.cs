using STDFParser4Net.IO;
using STDFParser4Net.Records.Registry;

namespace STDFParser4Net.Records.Readers
{
    internal sealed class PgrReader : IRecordReader
    {
        public StdfRecord Read(StdfBinaryReader reader, in StdfRecordHeader header)
        {
            reader.BeginRecord(header);

            ushort grpIndx = reader.ReadU2();
            StdfString grpNam = reader.ReadCn();
            ushort indxCnt = reader.ReadU2();
            ushort[] pmrIndx = reader.ReadArray(indxCnt, () => reader.ReadU2());

            return new PgrRecord(header, grpIndx, grpNam, indxCnt, pmrIndx);
        }
    }
}
