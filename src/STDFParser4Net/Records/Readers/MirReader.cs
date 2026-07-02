using STDFParser4Net.IO;
using STDFParser4Net.Records.Registry;

namespace STDFParser4Net.Records.Readers
{
    internal sealed class MirReader : IRecordReader
    {
        public StdfRecord Read(StdfBinaryReader reader, in StdfRecordHeader header)
        {
            reader.BeginRecord(header);

            // 8 fixed fields (always present)
            uint setupT = reader.ReadU4();
            uint startT = reader.ReadU4();
            byte statNum = reader.ReadU1();
            char modeCod = reader.ReadC1();
            char rtstCod = reader.ReadC1();
            char protCod = reader.ReadC1();
            ushort burnTim = reader.ReadU2();
            char cmodCod = reader.ReadC1();

            // 30 optional Cn fields, in spec order. Each is preceded by a HasRemaining(1)
            // check for the 1-byte length prefix; absent fields stay at default(StdfString).
            StdfString lotId    = reader.HasRemaining(1) ? reader.ReadCn() : default;
            StdfString partTyp  = reader.HasRemaining(1) ? reader.ReadCn() : default;
            StdfString nodeNam  = reader.HasRemaining(1) ? reader.ReadCn() : default;
            StdfString tstrTyp  = reader.HasRemaining(1) ? reader.ReadCn() : default;
            StdfString jobNam   = reader.HasRemaining(1) ? reader.ReadCn() : default;
            StdfString jobRev   = reader.HasRemaining(1) ? reader.ReadCn() : default;
            StdfString sblotId  = reader.HasRemaining(1) ? reader.ReadCn() : default;
            StdfString operNam  = reader.HasRemaining(1) ? reader.ReadCn() : default;
            StdfString execTyp  = reader.HasRemaining(1) ? reader.ReadCn() : default;
            StdfString execVer  = reader.HasRemaining(1) ? reader.ReadCn() : default;
            StdfString testCod  = reader.HasRemaining(1) ? reader.ReadCn() : default;
            StdfString tstTemp  = reader.HasRemaining(1) ? reader.ReadCn() : default;
            StdfString userTxt  = reader.HasRemaining(1) ? reader.ReadCn() : default;
            StdfString auxFile  = reader.HasRemaining(1) ? reader.ReadCn() : default;
            StdfString pkgTyp   = reader.HasRemaining(1) ? reader.ReadCn() : default;
            StdfString famlyId  = reader.HasRemaining(1) ? reader.ReadCn() : default;
            StdfString dateCod  = reader.HasRemaining(1) ? reader.ReadCn() : default;
            StdfString facilId  = reader.HasRemaining(1) ? reader.ReadCn() : default;
            StdfString floorId  = reader.HasRemaining(1) ? reader.ReadCn() : default;
            StdfString procId   = reader.HasRemaining(1) ? reader.ReadCn() : default;
            StdfString operFrq  = reader.HasRemaining(1) ? reader.ReadCn() : default;
            StdfString specNam  = reader.HasRemaining(1) ? reader.ReadCn() : default;
            StdfString specVer  = reader.HasRemaining(1) ? reader.ReadCn() : default;
            StdfString flowId   = reader.HasRemaining(1) ? reader.ReadCn() : default;
            StdfString setupId  = reader.HasRemaining(1) ? reader.ReadCn() : default;
            StdfString dsgnRev  = reader.HasRemaining(1) ? reader.ReadCn() : default;
            StdfString engId    = reader.HasRemaining(1) ? reader.ReadCn() : default;
            StdfString romCod   = reader.HasRemaining(1) ? reader.ReadCn() : default;
            StdfString serlNum  = reader.HasRemaining(1) ? reader.ReadCn() : default;
            StdfString suprNam  = reader.HasRemaining(1) ? reader.ReadCn() : default;

            return new MirRecord(header,
                setupT, startT, statNum,
                modeCod, rtstCod, protCod, burnTim, cmodCod,
                lotId, partTyp, nodeNam, tstrTyp,
                jobNam, jobRev, sblotId, operNam,
                execTyp, execVer, testCod, tstTemp,
                userTxt, auxFile, pkgTyp, famlyId,
                dateCod, facilId, floorId, procId,
                operFrq, specNam, specVer, flowId,
                setupId, dsgnRev, engId, romCod,
                serlNum, suprNam);
        }
    }
}
