using STDFParser4Net.IO;
using STDFParser4Net.Records.Registry;

namespace STDFParser4Net.Records.Readers
{
    internal sealed class PtrReader : IRecordReader
    {
        public StdfRecord Read(StdfBinaryReader reader, in StdfRecordHeader header)
        {
            reader.BeginRecord(header);

            // Fixed fields (10 bytes): TEST_NUM, HEAD_NUM, SITE_NUM, TEST_FLG, PARM_FLG, RESULT
            uint testNum = reader.ReadU4();
            byte headNum = reader.ReadU1();
            byte siteNum = reader.ReadU1();
            byte testFlg = reader.ReadU1();
            byte parmFlg = reader.ReadU1();
            float result = reader.ReadR4();

            // Optional TEST_TXT, ALARM_ID (precede OPT_FLAG)
            StdfString? testTxt = null;
            if (reader.HasRemaining(1))
                testTxt = reader.ReadCn();

            StdfString? alarmId = null;
            if (reader.HasRemaining(1))
                alarmId = reader.ReadCn();

            // Optional OPT_FLAG (B1)
            byte? optFlag = null;
            if (reader.HasRemaining(1))
                optFlag = reader.ReadU1();

            // Conditional fields: present when OPT_FLAG exists AND the corresponding bit is 0.
            // Per STDF V4: bit=0 -> field present, bit=1 -> field omitted.
            // OPT_FLAG is B1 (1 byte), so only bits 0-7 can be explicitly set. Bits 8-10
            // always test as 0 (present); HasRemaining guards against truncated records.

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

            // Trailing optionals (bits 8-10 cannot be set in B1; guarded by HasRemaining)
            char? cHlmfmt = null;
            if (optFlag.HasValue && (optFlag.Value & (1 << 8)) == 0 && reader.HasRemaining(1))
                cHlmfmt = reader.ReadC1();

            float? loSpec = null;
            if (optFlag.HasValue && (optFlag.Value & (1 << 9)) == 0 && reader.HasRemaining(4))
                loSpec = reader.ReadR4();

            float? hiSpec = null;
            if (optFlag.HasValue && (optFlag.Value & (1 << 10)) == 0 && reader.HasRemaining(4))
                hiSpec = reader.ReadR4();

            return new PtrRecord(header, testNum, headNum, siteNum, testFlg, parmFlg, result,
                testTxt, alarmId, optFlag,
                resScal, llmScal, hlmScal, loLimit, hiLimit, units,
                cResfmt, cLlmfmt, cHlmfmt, loSpec, hiSpec);
        }
    }
}
