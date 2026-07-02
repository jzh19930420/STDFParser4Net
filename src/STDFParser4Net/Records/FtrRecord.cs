using STDFParser4Net.Enums;

namespace STDFParser4Net.Records
{
    /// <summary>
    /// FTR — Functional Test Record (15,30). Result of a functional (digital)
    /// test. Carries cycle/address/fail counts, plus up to three pin-index/state
    /// arrays (RTN, PGM, FAIL). RTN_STAT is present only when TEST_FLG bit 3 is
    /// clear. VECT_NAM/ALARM_ID/PROG_TXT/RSLT_TXT and the SPIN_CNT/SPIN_INDX
    /// pair are trailing optionals.
    /// <para>
    /// OPT_FLAG is a fixed field for FTR (always present, unlike PTR/MPR). The
    /// bits of FTR's OPT_FLAG are not interpreted by this reader; the raw byte is
    /// exposed via <see cref="OPT_FLAG"/>.
    /// </para>
    /// </summary>
    public sealed class FtrRecord : StdfRecord
    {
        /// <summary>U4: Test number.</summary>
        public uint TEST_NUM { get; }

        /// <summary>U1: Test head number.</summary>
        public byte HEAD_NUM { get; }

        /// <summary>U1: Test site number.</summary>
        public byte SITE_NUM { get; }

        /// <summary>B1: Test flag. See <see cref="TestFlag"/>.</summary>
        public byte TEST_FLG { get; }

        /// <summary>B1: Optional flag (raw). FTR's OPT_FLAG has different bit semantics from PTR/MPR.</summary>
        public byte OPT_FLAG { get; }

        /// <summary>U4: Cycle count.</summary>
        public uint CYCL_CNT { get; }

        /// <summary>U4: Relative vector address.</summary>
        public uint REL_VADR { get; }

        /// <summary>U4: Repeat count.</summary>
        public uint REPT_CNT { get; }

        /// <summary>U4: Number of failures.</summary>
        public uint NUM_FAIL { get; }

        /// <summary>I4: X fail address.</summary>
        public int XFAIL_AD { get; }

        /// <summary>I4: Y fail address.</summary>
        public int YFAIL_AD { get; }

        /// <summary>I2: Vector offset.</summary>
        public short VECT_OFF { get; }

        /// <summary>U2: Return pin count (controls RTN_INDX and RTN_STAT array sizes).</summary>
        public ushort RTN_ICNT { get; }

        /// <summary>U2[k]: Return pin indices, length RTN_ICNT.</summary>
        public ushort[] RTN_INDX { get; }

        /// <summary>B1[k]: Return pin states, length RTN_ICNT. Null when TEST_FLG bit 3 is set.</summary>
        public byte[]? RTN_STAT { get; }

        /// <summary>U2: Programmed pin count (controls PGM_INDX and PGM_STAT array sizes).</summary>
        public ushort PGM_ICNT { get; }

        /// <summary>U2[k]: Programmed pin indices, length PGM_ICNT.</summary>
        public ushort[] PGM_INDX { get; }

        /// <summary>B1[k]: Programmed pin states, length PGM_ICNT.</summary>
        public byte[] PGM_STAT { get; }

        /// <summary>U2: Fail pin count (controls FAIL_INDX and FAIL_STAT array sizes).</summary>
        public ushort FAIL_ICNT { get; }

        /// <summary>U2[k]: Fail pin indices, length FAIL_ICNT.</summary>
        public ushort[] FAIL_INDX { get; }

        /// <summary>B1[k]: Fail pin states, length FAIL_ICNT.</summary>
        public byte[] FAIL_STAT { get; }

        /// <summary>Cn: Vector name. Null when absent.</summary>
        public StdfString? VECT_NAM { get; }

        /// <summary>Cn: Alarm identifier. Null when absent.</summary>
        public StdfString? ALARM_ID { get; }

        /// <summary>Cn: Program text. Null when absent.</summary>
        public StdfString? PROG_TXT { get; }

        /// <summary>Cn: Result text. Null when absent.</summary>
        public StdfString? RSLT_TXT { get; }

        /// <summary>U4: Scan pin count. Null when absent.</summary>
        public uint? SPIN_CNT { get; }

        /// <summary>U4[k]: Scan pin indices, length SPIN_CNT. Null when SPIN_CNT absent.</summary>
        public uint[]? SPIN_INDX { get; }

        /// <summary>Typed view of <see cref="TEST_FLG"/>.</summary>
        public TestFlag TestFlags => (TestFlag)TEST_FLG;

        /// <summary>True when RTN_STAT was present (TEST_FLG bit 3 clear).</summary>
        public bool HasReturnState => RTN_STAT != null;

        public FtrRecord(in StdfRecordHeader header,
            uint testNum, byte headNum, byte siteNum, byte testFlg, byte optFlag,
            uint cyclCnt, uint relVadr, uint rptCnt, uint numFail,
            int xfailAd, int yfailAd, short vectOff,
            ushort rtnIcnt, ushort[] rtnIndx, byte[]? rtnStat,
            ushort pgmIcnt, ushort[] pgmIndx, byte[] pgmStat,
            ushort failIcnt, ushort[] failIndx, byte[] failStat,
            StdfString? vectNam, StdfString? alarmId,
            StdfString? progTxt, StdfString? rsltTxt,
            uint? spinCnt, uint[]? spinIndx)
            : base(RecordType.FTR, header)
        {
            TEST_NUM = testNum;
            HEAD_NUM = headNum;
            SITE_NUM = siteNum;
            TEST_FLG = testFlg;
            OPT_FLAG = optFlag;
            CYCL_CNT = cyclCnt;
            REL_VADR = relVadr;
            REPT_CNT = rptCnt;
            NUM_FAIL = numFail;
            XFAIL_AD = xfailAd;
            YFAIL_AD = yfailAd;
            VECT_OFF = vectOff;
            RTN_ICNT = rtnIcnt;
            RTN_INDX = rtnIndx;
            RTN_STAT = rtnStat;
            PGM_ICNT = pgmIcnt;
            PGM_INDX = pgmIndx;
            PGM_STAT = pgmStat;
            FAIL_ICNT = failIcnt;
            FAIL_INDX = failIndx;
            FAIL_STAT = failStat;
            VECT_NAM = vectNam;
            ALARM_ID = alarmId;
            PROG_TXT = progTxt;
            RSLT_TXT = rsltTxt;
            SPIN_CNT = spinCnt;
            SPIN_INDX = spinIndx;
        }

        public override string ToString()
            => $"FTR TEST_NUM={TEST_NUM} HEAD={HEAD_NUM} SITE={SITE_NUM} CYCL_CNT={CYCL_CNT} NUM_FAIL={NUM_FAIL} TEST_FLG=0x{TEST_FLG:X2}";
    }
}
