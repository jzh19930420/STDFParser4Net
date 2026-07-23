using STDFParser4Net.IO;
using STDFParser4Net.Records.Registry;

namespace STDFParser4Net.Records.Readers
{
    internal sealed class PtrReader : IRecordReader
    {
        public StdfRecord Read(StdfBinaryReader reader, in StdfRecordHeader header)
        {
            reader.BeginRecord(header);

            // Fixed fields (12 bytes): TEST_NUM, HEAD_NUM, SITE_NUM, TEST_FLG, PARM_FLG, RESULT
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

            // Optional OPT_FLAG (B1). When present, the following fields are read in fixed
            // order using remaining body bytes — OPT bits are stored but not used to skip
            // fields (production equipment commonly writes all scales/limits regardless of bits).
            byte? optFlag = null;
            if (reader.HasRemaining(1))
                optFlag = reader.ReadU1();

            sbyte? resScal = null;
            sbyte? llmScal = null;
            sbyte? hlmScal = null;
            float? loLimit = null;
            float? hiLimit = null;
            StdfString? units = null;
            StdfString? cResfmt = null;
            StdfString? cLlmfmt = null;
            StdfString? cHlmfmt = null;
            float? loSpec = null;
            float? hiSpec = null;

            if (optFlag.HasValue)
            {
                if (reader.HasRemaining(1)) resScal = reader.ReadI1();
                if (reader.HasRemaining(1)) llmScal = reader.ReadI1();
                if (reader.HasRemaining(1)) hlmScal = reader.ReadI1();
                if (reader.HasRemaining(4)) loLimit = reader.ReadR4();
                if (reader.HasRemaining(4)) hiLimit = reader.ReadR4();
                if (reader.HasRemaining(1)) units = reader.ReadCn();
                if (reader.HasRemaining(1)) cResfmt = reader.ReadCn();
                if (reader.HasRemaining(1)) cLlmfmt = reader.ReadCn();
                if (reader.HasRemaining(1)) cHlmfmt = reader.ReadCn();
                if (reader.HasRemaining(4)) loSpec = reader.ReadR4();
                if (reader.HasRemaining(4)) hiSpec = reader.ReadR4();
            }

            return new PtrRecord(header, testNum, headNum, siteNum, testFlg, parmFlg, result,
                testTxt, alarmId, optFlag,
                resScal, llmScal, hlmScal, loLimit, hiLimit, units,
                cResfmt, cLlmfmt, cHlmfmt, loSpec, hiSpec);
        }
    }
}
