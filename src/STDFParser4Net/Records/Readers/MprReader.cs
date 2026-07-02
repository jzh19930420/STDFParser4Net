using STDFParser4Net.IO;
using STDFParser4Net.Records.Registry;

namespace STDFParser4Net.Records.Readers
{
    internal sealed class MprReader : IRecordReader
    {
        public StdfRecord Read(StdfBinaryReader reader, in StdfRecordHeader header)
        {
            reader.BeginRecord(header);

            // Fixed fields: TEST_NUM, HEAD_NUM, SITE_NUM, TEST_FLG, PARM_FLG, RTN_ICNT, RSLT_PCT, NUM_CNT
            uint testNum = reader.ReadU4();
            byte headNum = reader.ReadU1();
            byte siteNum = reader.ReadU1();
            byte testFlg = reader.ReadU1();
            byte parmFlg = reader.ReadU1();
            ushort rtnIcnt = reader.ReadU2();
            ushort rsltPct = reader.ReadU2();
            ushort numCnt = reader.ReadU2();

            // RTN_RSLT: kxR4, RTN_ICNT entries
            float[] rtnRslt = reader.ReadArray(rtnIcnt, () => reader.ReadR4());

            // RTN_STAT: kxB1, RTN_ICNT entries — present only when TEST_FLG bit 3 is clear.
            byte[]? rtnStat = null;
            if ((testFlg & 0x08) == 0)
                rtnStat = reader.ReadArray(rtnIcnt, () => reader.ReadU1());

            // Optional TEST_TXT, ALARM_ID
            StdfString? testTxt = null;
            if (reader.HasRemaining(1))
                testTxt = reader.ReadCn();

            StdfString? alarmId = null;
            if (reader.HasRemaining(1))
                alarmId = reader.ReadCn();

            // Optional OPT_FLAG and conditional fields (same layout as PTR)
            byte? optFlag = null;
            if (reader.HasRemaining(1))
                optFlag = reader.ReadU1();

            sbyte? resScal = null;
            if (optFlag.HasValue && (optFlag.Value & (1 << 0)) == 0 && reader.HasRemaining(1))
                resScal = reader.ReadI1();

            sbyte? llmScal = null;
            if (optFlag.HasValue && (optFlag.Value & (1 << 1)) == 0 && reader.HasRemaining(1))
                llmScal = reader.ReadI1();

            sbyte? hlmScal = null;
            if (optFlag.HasValue && (optFlag.Value & (1 << 2)) == 0 && reader.HasRemaining(1))
                hlmScal = reader.ReadI1();

            float? loLimit = null;
            if (optFlag.HasValue && (optFlag.Value & (1 << 3)) == 0 && reader.HasRemaining(4))
                loLimit = reader.ReadR4();

            float? hiLimit = null;
            if (optFlag.HasValue && (optFlag.Value & (1 << 4)) == 0 && reader.HasRemaining(4))
                hiLimit = reader.ReadR4();

            StdfString? units = null;
            if (optFlag.HasValue && (optFlag.Value & (1 << 5)) == 0 && reader.HasRemaining(1))
                units = reader.ReadCn();

            char? cResfmt = null;
            if (optFlag.HasValue && (optFlag.Value & (1 << 6)) == 0 && reader.HasRemaining(1))
                cResfmt = reader.ReadC1();

            char? cLlmfmt = null;
            if (optFlag.HasValue && (optFlag.Value & (1 << 7)) == 0 && reader.HasRemaining(1))
                cLlmfmt = reader.ReadC1();

            char? cHlmfmt = null;
            if (optFlag.HasValue && (optFlag.Value & (1 << 8)) == 0 && reader.HasRemaining(1))
                cHlmfmt = reader.ReadC1();

            float? loSpec = null;
            if (optFlag.HasValue && (optFlag.Value & (1 << 9)) == 0 && reader.HasRemaining(4))
                loSpec = reader.ReadR4();

            float? hiSpec = null;
            if (optFlag.HasValue && (optFlag.Value & (1 << 10)) == 0 && reader.HasRemaining(4))
                hiSpec = reader.ReadR4();

            return new MprRecord(header, testNum, headNum, siteNum, testFlg, parmFlg,
                rtnIcnt, rsltPct, numCnt, rtnRslt, rtnStat,
                testTxt, alarmId, optFlag,
                resScal, llmScal, hlmScal, loLimit, hiLimit, units,
                cResfmt, cLlmfmt, cHlmfmt, loSpec, hiSpec);
        }
    }
}
