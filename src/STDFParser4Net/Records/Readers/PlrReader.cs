using STDFParser4Net.IO;
using STDFParser4Net.Records.Registry;

namespace STDFParser4Net.Records.Readers
{
    internal sealed class PlrReader : IRecordReader
    {
        public StdfRecord Read(StdfBinaryReader reader, in StdfRecordHeader header)
        {
            reader.BeginRecord(header);

            ushort grpCnt = reader.ReadU2();
            ushort[] grpIndx = reader.ReadArray(grpCnt, () => reader.ReadU2());
            ushort[] grpMode = reader.ReadArray(grpCnt, () => reader.ReadU2());
            byte[] grpRadix = reader.ReadArray(grpCnt, () => reader.ReadU1());
            StdfString[] pgmChar = reader.ReadArray(grpCnt, () => reader.ReadCn());
            StdfString[] pgmChal = reader.ReadArray(grpCnt, () => reader.ReadCn());
            StdfString[] rslChar = reader.ReadArray(grpCnt, () => reader.ReadCn());
            StdfString[] rslChal = reader.ReadArray(grpCnt, () => reader.ReadCn());

            return new PlrRecord(header, grpCnt, grpIndx, grpMode, grpRadix,
                pgmChar, pgmChal, rslChar, rslChal);
        }
    }
}
