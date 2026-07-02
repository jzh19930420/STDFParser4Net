using STDFParser4Net.IO;
using STDFParser4Net.Records.Registry;

namespace STDFParser4Net.Records.Readers
{
    internal sealed class RdrReader : IRecordReader
    {
        public StdfRecord Read(StdfBinaryReader reader, in StdfRecordHeader header)
        {
            reader.BeginRecord(header);
            byte headNum = reader.ReadU1();
            byte siteNum = reader.ReadU1();
            ushort numBins = reader.ReadU2();
            ushort[] rtstBin = reader.ReadArray(numBins, () => reader.ReadU2());
            return new RdrRecord(header, headNum, siteNum, numBins, rtstBin);
        }
    }
}
