using STDFParser4Net.IO;
using STDFParser4Net.Records.Registry;

namespace STDFParser4Net.Records.Readers
{
    internal sealed class SdrReader : IRecordReader
    {
        public StdfRecord Read(StdfBinaryReader reader, in StdfRecordHeader header)
        {
            reader.BeginRecord(header);
            byte headNum = reader.ReadU1();
            byte siteGroup = reader.ReadU1();
            byte siteCnt = reader.ReadU1();
            byte[] siteNum = reader.ReadArray(siteCnt, () => reader.ReadU1());

            StdfString? handTyp = reader.HasRemaining(1) ? reader.ReadCn() : (StdfString?)null;
            StdfString? handId = reader.HasRemaining(1) ? reader.ReadCn() : (StdfString?)null;
            StdfString? cardTyp = reader.HasRemaining(1) ? reader.ReadCn() : (StdfString?)null;
            StdfString? cardId = reader.HasRemaining(1) ? reader.ReadCn() : (StdfString?)null;
            StdfString? loadTyp = reader.HasRemaining(1) ? reader.ReadCn() : (StdfString?)null;
            StdfString? loadId = reader.HasRemaining(1) ? reader.ReadCn() : (StdfString?)null;
            StdfString? dibTyp = reader.HasRemaining(1) ? reader.ReadCn() : (StdfString?)null;
            StdfString? dibId = reader.HasRemaining(1) ? reader.ReadCn() : (StdfString?)null;
            StdfString? cablTyp = reader.HasRemaining(1) ? reader.ReadCn() : (StdfString?)null;
            StdfString? cablId = reader.HasRemaining(1) ? reader.ReadCn() : (StdfString?)null;
            StdfString? contTyp = reader.HasRemaining(1) ? reader.ReadCn() : (StdfString?)null;
            StdfString? contId = reader.HasRemaining(1) ? reader.ReadCn() : (StdfString?)null;
            StdfString? lasrTyp = reader.HasRemaining(1) ? reader.ReadCn() : (StdfString?)null;
            StdfString? lasrId = reader.HasRemaining(1) ? reader.ReadCn() : (StdfString?)null;
            StdfString? extraTyp = reader.HasRemaining(1) ? reader.ReadCn() : (StdfString?)null;
            StdfString? extraId = reader.HasRemaining(1) ? reader.ReadCn() : (StdfString?)null;

            return new SdrRecord(header, headNum, siteGroup, siteCnt, siteNum,
                handTyp, handId, cardTyp, cardId, loadTyp, loadId,
                dibTyp, dibId, cablTyp, cablId, contTyp, contId,
                lasrTyp, lasrId, extraTyp, extraId);
        }
    }
}
