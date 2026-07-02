using STDFParser4Net.Enums;

namespace STDFParser4Net.Records
{
    /// <summary>
    /// PTR — Parametric Test Record (15,10). Result of a single parametric test.
    /// RESULT, LO_LIMIT, HI_LIMIT, RES_SCAL and all other numeric fields are
    /// returned raw — no scale adjustment, no unit conversion.
    /// <para>
    /// TEST_TXT and ALARM_ID are optional Cn fields that precede OPT_FLAG.
    /// OPT_FLAG (B1) is optional; when present, bits 0-7 control the presence of
    /// RES_SCAL through C_LLMFMT (bit=0 present, bit=1 absent). C_HLMFMT, LO_SPEC
    /// and HI_SPEC are trailing optionals guarded by remaining body bytes.
    /// </para>
    /// </summary>
    public sealed class PtrRecord : StdfRecord
    {
        /// <summary>U4: Test number.</summary>
        public uint TEST_NUM { get; }

        /// <summary>U1: Test head number.</summary>
        public byte HEAD_NUM { get; }

        /// <summary>U1: Test site number.</summary>
        public byte SITE_NUM { get; }

        /// <summary>B1: Test flag. See <see cref="TestFlag"/>.</summary>
        public byte TEST_FLG { get; }

        /// <summary>B1: Parametric flag. See <see cref="ParamFlag"/>.</summary>
        public byte PARM_FLG { get; }

        /// <summary>R4: Test result (raw, no scaling).</summary>
        public float RESULT { get; }

        /// <summary>Cn: Test description. Null when absent.</summary>
        public StdfString? TEST_TXT { get; }

        /// <summary>Cn: Alarm identifier. Null when absent.</summary>
        public StdfString? ALARM_ID { get; }

        /// <summary>B1: Optional flag. Null when absent. See <see cref="OptFlag"/>.</summary>
        public byte? OPT_FLAG { get; }

        /// <summary>I1 (OPT_FLAG bit0): result scale exponent. Null when omitted.</summary>
        public sbyte? RES_SCAL { get; }

        /// <summary>I1 (OPT_FLAG bit1): low-limit scale exponent. Null when omitted.</summary>
        public sbyte? LLM_SCAL { get; }

        /// <summary>I1 (OPT_FLAG bit2): high-limit scale exponent. Null when omitted.</summary>
        public sbyte? HLM_SCAL { get; }

        /// <summary>R4 (OPT_FLAG bit3): low limit (raw). Null when omitted.</summary>
        public float? LO_LIMIT { get; }

        /// <summary>R4 (OPT_FLAG bit4): high limit (raw). Null when omitted.</summary>
        public float? HI_LIMIT { get; }

        /// <summary>Cn (OPT_FLAG bit5): units string. Null when omitted.</summary>
        public StdfString? UNITS { get; }

        /// <summary>C1 (OPT_FLAG bit6): result format. Null when omitted.</summary>
        public char? C_RESFMT { get; }

        /// <summary>C1 (OPT_FLAG bit7): low-limit format. Null when omitted.</summary>
        public char? C_LLMFMT { get; }

        /// <summary>C1 (trailing optional): high-limit format. Null when omitted.</summary>
        public char? C_HLMFMT { get; }

        /// <summary>R4 (trailing optional): low spec limit (raw). Null when omitted.</summary>
        public float? LO_SPEC { get; }

        /// <summary>R4 (trailing optional): high spec limit (raw). Null when omitted.</summary>
        public float? HI_SPEC { get; }

        /// <summary>Typed view of <see cref="TEST_FLG"/>.</summary>
        public TestFlag TestFlags => (TestFlag)TEST_FLG;

        /// <summary>Typed view of <see cref="PARM_FLG"/>.</summary>
        public ParamFlag ParmFlags => (ParamFlag)PARM_FLG;

        /// <summary>Typed view of <see cref="OPT_FLAG"/> (None when OPT_FLAG absent).</summary>
        public OptFlag OptFlags => OPT_FLAG.HasValue ? (OptFlag)OPT_FLAG.Value : OptFlag.None;

        public PtrRecord(in StdfRecordHeader header,
            uint testNum, byte headNum, byte siteNum, byte testFlg, byte parmFlg, float result,
            StdfString? testTxt, StdfString? alarmId, byte? optFlag,
            sbyte? resScal, sbyte? llmScal, sbyte? hlmScal,
            float? loLimit, float? hiLimit, StdfString? units,
            char? cResfmt, char? cLlmfmt, char? cHlmfmt,
            float? loSpec, float? hiSpec)
            : base(RecordType.PTR, header)
        {
            TEST_NUM = testNum;
            HEAD_NUM = headNum;
            SITE_NUM = siteNum;
            TEST_FLG = testFlg;
            PARM_FLG = parmFlg;
            RESULT = result;
            TEST_TXT = testTxt;
            ALARM_ID = alarmId;
            OPT_FLAG = optFlag;
            RES_SCAL = resScal;
            LLM_SCAL = llmScal;
            HLM_SCAL = hlmScal;
            LO_LIMIT = loLimit;
            HI_LIMIT = hiLimit;
            UNITS = units;
            C_RESFMT = cResfmt;
            C_LLMFMT = cLlmfmt;
            C_HLMFMT = cHlmfmt;
            LO_SPEC = loSpec;
            HI_SPEC = hiSpec;
        }

        public override string ToString()
            => $"PTR TEST_NUM={TEST_NUM} HEAD={HEAD_NUM} SITE={SITE_NUM} RESULT={RESULT} TEST_FLG=0x{TEST_FLG:X2}";
    }
}
