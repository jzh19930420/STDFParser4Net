using System;
using STDFParser4Net.IO;
using STDFParser4Net.Records.Registry;

namespace STDFParser4Net.Records.Readers
{
    internal sealed class GdrReader : IRecordReader
    {
        public StdfRecord Read(StdfBinaryReader reader, in StdfRecordHeader header)
        {
            reader.BeginRecord(header);

            byte genFlg = reader.ReadU1();
            ushort fldCnt = reader.ReadU2();

            var fields = new GdrField[fldCnt];
            for (int i = 0; i < fldCnt; i++)
                fields[i] = ReadGdrField(reader);

            return new GdrRecord(header, genFlg, fldCnt, fields);
        }

        /// <summary>
        /// Read a single Vn field: a U1 type code followed by the typed value.
        /// Codes follow STDF V4: 0=U1, 1=U2, 2=U4, 3=I1, 4=I2, 5=I4, 6=R4, 7=R8,
        /// 8=B1, 10=Cn, 11=Bn, 12=Dn, 13=N1. Unknown codes store null.
        /// </summary>
        private static GdrField ReadGdrField(StdfBinaryReader reader)
        {
            byte code = reader.ReadU1();
            object? value;
            switch (code)
            {
                case 0:  value = reader.ReadU1();  break;  // U1
                case 1:  value = reader.ReadU2();  break;  // U2
                case 2:  value = reader.ReadU4();  break;  // U4
                case 3:  value = reader.ReadI1();  break;  // I1
                case 4:  value = reader.ReadI2();  break;  // I2
                case 5:  value = reader.ReadI4();  break;  // I4
                case 6:  value = reader.ReadR4();  break;  // R4
                case 7:  value = reader.ReadR8();  break;  // R8
                case 8:  value = reader.ReadU1();  break;  // B1 (single byte)
                case 10: value = reader.ReadCn();  break;  // Cn
                case 11: value = reader.ReadBn();  break;  // Bn
                case 12: value = reader.ReadDn();  break;  // Dn
                case 13: value = reader.ReadN1();  break;  // N1 (single nibble)
                default: value = null;             break;  // unknown / reserved (9)
            }
            return new GdrField(code, value);
        }
    }
}
