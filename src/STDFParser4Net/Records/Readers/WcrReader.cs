using STDFParser4Net.IO;
using STDFParser4Net.Records.Registry;

namespace STDFParser4Net.Records.Readers
{
    internal sealed class WcrReader : IRecordReader
    {
        public StdfRecord Read(StdfBinaryReader reader, in StdfRecordHeader header)
        {
            reader.BeginRecord(header);
            float wafrSiz = reader.ReadR4();
            float dieHt = reader.ReadR4();
            float dieWid = reader.ReadR4();
            byte wfUnits = reader.ReadU1();
            char wfFlat = reader.ReadC1();
            short centerX = reader.ReadI2();
            short centerY = reader.ReadI2();
            char posX = reader.ReadC1();
            char posY = reader.ReadC1();
            return new WcrRecord(header, wafrSiz, dieHt, dieWid, wfUnits, wfFlat,
                centerX, centerY, posX, posY);
        }
    }
}
