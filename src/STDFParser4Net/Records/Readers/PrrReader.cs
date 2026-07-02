using STDFParser4Net.IO;
using STDFParser4Net.Records.Registry;

namespace STDFParser4Net.Records.Readers
{
    internal sealed class PrrReader : IRecordReader
    {
        public StdfRecord Read(StdfBinaryReader reader, in StdfRecordHeader header)
        {
            reader.BeginRecord(header);
            byte headNum = reader.ReadU1();
            byte siteNum = reader.ReadU1();
            uint partNum = reader.ReadU4();
            ushort numTest = reader.ReadU2();
            ushort hardBin = reader.ReadU2();
            ushort softBin = reader.ReadU2();
            short xCoord = reader.ReadI2();
            short yCoord = reader.ReadI2();
            uint testT = reader.ReadU4();
            byte partFlg = reader.ReadU1();
            ushort numHard = reader.ReadU2();
            ushort numSoft = reader.ReadU2();

            StdfString? partId = null;
            if (reader.HasRemaining(1))
                partId = reader.ReadCn();

            StdfString? partTxt = null;
            if (reader.HasRemaining(1))
                partTxt = reader.ReadCn();

            byte[]? partFix = null;
            if (reader.HasRemaining(1))
                partFix = reader.ReadBn();

            return new PrrRecord(header, headNum, siteNum, partNum, numTest,
                hardBin, softBin, xCoord, yCoord, testT, partFlg, numHard, numSoft,
                partId, partTxt, partFix);
        }
    }
}
