using STDFParser4Net.IO;
using STDFParser4Net.Records.Registry;

namespace STDFParser4Net.Records.Readers
{
    internal sealed class PmrReader : IRecordReader
    {
        public StdfRecord Read(StdfBinaryReader reader, in StdfRecordHeader header)
        {
            reader.BeginRecord(header);

            ushort pmrIndx = reader.ReadU2();
            ushort chanTyp = reader.ReadU2();

            StdfString? chanNam = null;
            if (reader.HasRemaining(1))
                chanNam = reader.ReadCn();

            StdfString? phyNam = null;
            if (reader.HasRemaining(1))
                phyNam = reader.ReadCn();

            StdfString? logNam = null;
            if (reader.HasRemaining(1))
                logNam = reader.ReadCn();

            return new PmrRecord(header, pmrIndx, chanTyp, chanNam, phyNam, logNam);
        }
    }
}
