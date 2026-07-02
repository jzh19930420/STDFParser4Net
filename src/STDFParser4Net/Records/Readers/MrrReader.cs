using STDFParser4Net.IO;
using STDFParser4Net.Records.Registry;

namespace STDFParser4Net.Records.Readers
{
    internal sealed class MrrReader : IRecordReader
    {
        public StdfRecord Read(StdfBinaryReader reader, in StdfRecordHeader header)
        {
            reader.BeginRecord(header);
            uint finishT = reader.ReadU4();
            // The last three fields (DISP_COD, USR_DESC, EXC_DESC) are optional.
            char dispCod = reader.HasRemaining(1) ? reader.ReadC1() : ' ';
            StdfString usrDesc = reader.HasRemaining(1) ? reader.ReadCn() : default;
            StdfString excDesc = reader.HasRemaining(1) ? reader.ReadCn() : default;
            return new MrrRecord(header, finishT, dispCod, usrDesc, excDesc);
        }
    }
}
