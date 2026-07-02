namespace STDFParser4Net.Records
{
    /// <summary>
    /// TSR — Test Synopsis Record (10,30). Contains per-test summary statistics.
    /// After the 7 fixed fields, an optional OPT_FLAG (B1) may appear. When
    /// present, subsequent fields are conditionally included: a field is present
    /// when its corresponding OPT_FLAG bit is 0, and omitted (null) when the bit
    /// is 1.
    /// </summary>
    public sealed class TsrRecord : StdfRecord
    {
        /// <summary>U1: Test head number.</summary>
        public byte HEAD_NUM { get; }

        /// <summary>U1: Test site number.</summary>
        public byte SITE_NUM { get; }

        /// <summary>C1: Test type ('P'=parametric, 'F'=functional, 'M'=multi-result, ' '=unknown).</summary>
        public char TEST_TYP { get; }

        /// <summary>U4: Test number.</summary>
        public uint TEST_NUM { get; }

        /// <summary>U4: Number of times the test was executed.</summary>
        public uint EXEC_CNT { get; }

        /// <summary>U4: Number of times the test failed.</summary>
        public uint FAIL_CNT { get; }

        /// <summary>U4: Number of times the test alarmed.</summary>
        public uint ALRM_CNT { get; }

        /// <summary>
        /// B1: Optional flag. Null when OPT_FLAG is absent from the record. When
        /// present, controls which optional fields follow: bit=0 means the field is
        /// present, bit=1 means it is omitted.
        /// </summary>
        public byte? OPT_FLAG { get; }

        /// <summary>U4 (OPT_FLAG bit0). Null when omitted or OPT_FLAG absent.</summary>
        public uint? CYCL_CNT { get; }

        /// <summary>U4 (OPT_FLAG bit1). Null when omitted or OPT_FLAG absent.</summary>
        public uint? REL_VADR { get; }

        /// <summary>U4 (OPT_FLAG bit2). Null when omitted or OPT_FLAG absent.</summary>
        public uint? REPT_CNT { get; }

        /// <summary>U4 (OPT_FLAG bit3). Null when omitted or OPT_FLAG absent.</summary>
        public uint? NUM_FAIL { get; }

        /// <summary>I4 (OPT_FLAG bit4). Null when omitted or OPT_FLAG absent.</summary>
        public int? XFAIL_AD { get; }

        /// <summary>I4 (OPT_FLAG bit5). Null when omitted or OPT_FLAG absent.</summary>
        public int? YFAIL_AD { get; }

        /// <summary>I2 (OPT_FLAG bit6). Null when omitted or OPT_FLAG absent.</summary>
        public short? VECT_OFF { get; }

        /// <summary>U1 (OPT_FLAG bit7). Null when omitted or OPT_FLAG absent.</summary>
        public byte? RTN_ICND { get; }

        /// <summary>U1 (OPT_FLAG bit8). Null when omitted or OPT_FLAG absent.</summary>
        public byte? PROG_ICND { get; }

        /// <summary>U1 (OPT_FLAG bit9). Null when omitted or OPT_FLAG absent.</summary>
        public byte? FAIL_ICND { get; }

        /// <summary>Bn (OPT_FLAG bit10). Null when omitted or OPT_FLAG absent.</summary>
        public byte[]? ALRM_COD { get; }

        /// <summary>Cn (OPT_FLAG bit11). Null when omitted or OPT_FLAG absent.</summary>
        public StdfString? PROG_NAM { get; }

        /// <summary>Cn (OPT_FLAG bit12). Null when omitted or OPT_FLAG absent.</summary>
        public StdfString? RSLT_NAM { get; }

        /// <summary>U4 (OPT_FLAG bit13). Null when omitted or OPT_FLAG absent.</summary>
        public uint? TST_DUR { get; }

        /// <summary>U4 (OPT_FLAG bit14). Null when omitted or OPT_FLAG absent.</summary>
        public uint? TST_MIN { get; }

        /// <summary>U4 (OPT_FLAG bit15). Null when omitted or OPT_FLAG absent.</summary>
        public uint? TST_MAX { get; }

        public TsrRecord(in StdfRecordHeader header,
            byte headNum, byte siteNum, char testTyp, uint testNum,
            uint execCnt, uint failCnt, uint alrmCnt, byte? optFlag,
            uint? cyclCnt, uint? relVadr, uint? rptCnt, uint? numFail,
            int? xfailAd, int? yfailAd, short? vectOff,
            byte? rtnIcnd, byte? progIcnd, byte? failIcnd,
            byte[]? alrmCod, StdfString? progNam, StdfString? rsltNam,
            uint? tstDur, uint? tstMin, uint? tstMax)
            : base(RecordType.TSR, header)
        {
            HEAD_NUM = headNum;
            SITE_NUM = siteNum;
            TEST_TYP = testTyp;
            TEST_NUM = testNum;
            EXEC_CNT = execCnt;
            FAIL_CNT = failCnt;
            ALRM_CNT = alrmCnt;
            OPT_FLAG = optFlag;
            CYCL_CNT = cyclCnt;
            REL_VADR = relVadr;
            REPT_CNT = rptCnt;
            NUM_FAIL = numFail;
            XFAIL_AD = xfailAd;
            YFAIL_AD = yfailAd;
            VECT_OFF = vectOff;
            RTN_ICND = rtnIcnd;
            PROG_ICND = progIcnd;
            FAIL_ICND = failIcnd;
            ALRM_COD = alrmCod;
            PROG_NAM = progNam;
            RSLT_NAM = rsltNam;
            TST_DUR = tstDur;
            TST_MIN = tstMin;
            TST_MAX = tstMax;
        }

        public override string ToString()
            => $"TSR HEAD_NUM={HEAD_NUM} SITE_NUM={SITE_NUM} TEST_TYP='{TEST_TYP}' TEST_NUM={TEST_NUM} EXEC_CNT={EXEC_CNT} FAIL_CNT={FAIL_CNT} ALRM_CNT={ALRM_CNT}";
    }
}
