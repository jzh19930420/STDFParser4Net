using STDFParser4Net.Enums;

namespace STDFParser4Net.Records
{
    /// <summary>
    /// PRR — Part Results Record (5,20). Contains the results for a single tested
    /// part. PART_ID, PART_TXT and PART_FIX are optional.
    /// </summary>
    public sealed class PrrRecord : StdfRecord
    {
        /// <summary>U1: Test head number.</summary>
        public byte HEAD_NUM { get; }

        /// <summary>U1: Test site number.</summary>
        public byte SITE_NUM { get; }

        /// <summary>U4: Part number.</summary>
        public uint PART_NUM { get; }

        /// <summary>U2: Number of tests executed.</summary>
        public ushort NUM_TEST { get; }

        /// <summary>U2: Hardware bin number.</summary>
        public ushort HARD_BIN { get; }

        /// <summary>U2: Software bin number.</summary>
        public ushort SOFT_BIN { get; }

        /// <summary>I2: X coordinate of the part on the wafer.</summary>
        public short X_COORD { get; }

        /// <summary>I2: Y coordinate of the part on the wafer.</summary>
        public short Y_COORD { get; }

        /// <summary>U4: Part testing time in milliseconds.</summary>
        public uint TEST_T { get; }

        /// <summary>B1: Part information flags. See <see cref="PartFlag"/>.</summary>
        public byte PART_FLG { get; }

        /// <summary>U2: Number of hardware bins.</summary>
        public ushort NUM_HARD { get; }

        /// <summary>U2: Number of software bins.</summary>
        public ushort NUM_SOFT { get; }

        /// <summary>Cn: Part identifier. Null when absent.</summary>
        public StdfString? PART_ID { get; }

        /// <summary>Cn: Part description text. Null when absent.</summary>
        public StdfString? PART_TXT { get; }

        /// <summary>Bn: Part fix data. Null when absent.</summary>
        public byte[]? PART_FIX { get; }

        /// <summary>
        /// Convenience: true when the part passed. Uses the Loader convention where
        /// bit 3 (0x08) of PART_FLG is the pass/fail control bit.
        /// </summary>
        public bool IsPass => (PART_FLG & (byte)PartFlag.Failed) == 0;

        public PrrRecord(in StdfRecordHeader header,
            byte headNum, byte siteNum, uint partNum, ushort numTest,
            ushort hardBin, ushort softBin, short xCoord, short yCoord,
            uint testT, byte partFlg, ushort numHard, ushort numSoft,
            StdfString? partId, StdfString? partTxt, byte[]? partFix)
            : base(RecordType.PRR, header)
        {
            HEAD_NUM = headNum;
            SITE_NUM = siteNum;
            PART_NUM = partNum;
            NUM_TEST = numTest;
            HARD_BIN = hardBin;
            SOFT_BIN = softBin;
            X_COORD = xCoord;
            Y_COORD = yCoord;
            TEST_T = testT;
            PART_FLG = partFlg;
            NUM_HARD = numHard;
            NUM_SOFT = numSoft;
            PART_ID = partId;
            PART_TXT = partTxt;
            PART_FIX = partFix;
        }

        public override string ToString()
            => $"PRR HEAD_NUM={HEAD_NUM} SITE_NUM={SITE_NUM} PART_FLG=0x{PART_FLG:X2} HARD_BIN={HARD_BIN} SOFT_BIN={SOFT_BIN} IsPass={IsPass}";
    }
}
