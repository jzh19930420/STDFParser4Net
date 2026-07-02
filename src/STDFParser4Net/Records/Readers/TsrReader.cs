using STDFParser4Net.IO;
using STDFParser4Net.Records.Registry;

namespace STDFParser4Net.Records.Readers
{
    internal sealed class TsrReader : IRecordReader
    {
        public StdfRecord Read(StdfBinaryReader reader, in StdfRecordHeader header)
        {
            reader.BeginRecord(header);

            // Fixed fields (7): HEAD_NUM, SITE_NUM, TEST_TYP, TEST_NUM, EXEC_CNT, FAIL_CNT, ALRM_CNT
            byte headNum = reader.ReadU1();
            byte siteNum = reader.ReadU1();
            char testTyp = reader.ReadC1();
            uint testNum = reader.ReadU4();
            uint execCnt = reader.ReadU4();
            uint failCnt = reader.ReadU4();
            uint alrmCnt = reader.ReadU4();

            // OPT_FLAG (B1, optional). When absent, all optional fields are null.
            byte? optFlag = null;
            if (reader.HasRemaining(1))
                optFlag = reader.ReadU1();

            // Conditional fields: present when OPT_FLAG exists AND the corresponding bit is 0.
            // Per STDF V4: bit=0 → field present, bit=1 → field omitted.
            // Note: OPT_FLAG is a single byte (B1), so only bits 0-7 can be explicitly set.
            // For bits 8-15 the bit check always evaluates to 0 (present); HasRemaining guards reads.

            uint? cyclCnt = null;
            if (optFlag.HasValue && (optFlag.Value & (1 << 0)) == 0 && reader.HasRemaining(4))
                cyclCnt = reader.ReadU4();

            uint? relVadr = null;
            if (optFlag.HasValue && (optFlag.Value & (1 << 1)) == 0 && reader.HasRemaining(4))
                relVadr = reader.ReadU4();

            uint? rptCnt = null;
            if (optFlag.HasValue && (optFlag.Value & (1 << 2)) == 0 && reader.HasRemaining(4))
                rptCnt = reader.ReadU4();

            uint? numFail = null;
            if (optFlag.HasValue && (optFlag.Value & (1 << 3)) == 0 && reader.HasRemaining(4))
                numFail = reader.ReadU4();

            int? xfailAd = null;
            if (optFlag.HasValue && (optFlag.Value & (1 << 4)) == 0 && reader.HasRemaining(4))
                xfailAd = reader.ReadI4();

            int? yfailAd = null;
            if (optFlag.HasValue && (optFlag.Value & (1 << 5)) == 0 && reader.HasRemaining(4))
                yfailAd = reader.ReadI4();

            short? vectOff = null;
            if (optFlag.HasValue && (optFlag.Value & (1 << 6)) == 0 && reader.HasRemaining(2))
                vectOff = reader.ReadI2();

            byte? rtnIcnd = null;
            if (optFlag.HasValue && (optFlag.Value & (1 << 7)) == 0 && reader.HasRemaining(1))
                rtnIcnd = reader.ReadU1();

            // Bits 8-15: a single byte cannot set these bits, so the bit test always
            // evaluates to "present" (0). The HasRemaining guard protects against
            // truncated records.
            byte? progIcnd = null;
            if (optFlag.HasValue && (optFlag.Value & (1 << 8)) == 0 && reader.HasRemaining(1))
                progIcnd = reader.ReadU1();

            byte? failIcnd = null;
            if (optFlag.HasValue && (optFlag.Value & (1 << 9)) == 0 && reader.HasRemaining(1))
                failIcnd = reader.ReadU1();

            byte[]? alrmCod = null;
            if (optFlag.HasValue && (optFlag.Value & (1 << 10)) == 0 && reader.HasRemaining(1))
                alrmCod = reader.ReadBn();

            StdfString? progNam = null;
            if (optFlag.HasValue && (optFlag.Value & (1 << 11)) == 0 && reader.HasRemaining(1))
                progNam = reader.ReadCn();

            StdfString? rsltNam = null;
            if (optFlag.HasValue && (optFlag.Value & (1 << 12)) == 0 && reader.HasRemaining(1))
                rsltNam = reader.ReadCn();

            uint? tstDur = null;
            if (optFlag.HasValue && (optFlag.Value & (1 << 13)) == 0 && reader.HasRemaining(4))
                tstDur = reader.ReadU4();

            uint? tstMin = null;
            if (optFlag.HasValue && (optFlag.Value & (1 << 14)) == 0 && reader.HasRemaining(4))
                tstMin = reader.ReadU4();

            uint? tstMax = null;
            if (optFlag.HasValue && (optFlag.Value & (1 << 15)) == 0 && reader.HasRemaining(4))
                tstMax = reader.ReadU4();

            return new TsrRecord(header, headNum, siteNum, testTyp, testNum,
                execCnt, failCnt, alrmCnt, optFlag,
                cyclCnt, relVadr, rptCnt, numFail,
                xfailAd, yfailAd, vectOff,
                rtnIcnd, progIcnd, failIcnd,
                alrmCod, progNam, rsltNam,
                tstDur, tstMin, tstMax);
        }
    }
}
