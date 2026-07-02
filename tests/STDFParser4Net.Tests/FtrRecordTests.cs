using System.IO;
using STDFParser4Net;
using STDFParser4Net.Enums;
using STDFParser4Net.Records;
using Xunit;

namespace STDFParser4Net.Tests
{
    public class FtrRecordTests
    {
        // FAR(LE) + FTR with RTN/PGM/FAIL pin groups, TEST_FLG=0x00 (RTN_STAT present).
        // No trailing optional fields.
        // Body = 34 (fixed) + 8 (RTN) + 8 (PGM) + 5 (FAIL) = 55
        private static readonly byte[] BaseFile =
        {
            0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,                       // FAR(LE)
            0x37, 0x00, 0x0F, 0x1E,                                   // FTR: REC_LEN=55, TYP=15, SUB=30
            0xB8, 0x0B, 0x00, 0x00,                                   // TEST_NUM = 3000
            0x01,                                                     // HEAD_NUM = 1
            0x01,                                                     // SITE_NUM = 1
            0x00,                                                     // TEST_FLG = 0x00 (bit3=0 → RTN_STAT present)
            0x00,                                                     // OPT_FLAG = 0x00
            0x64, 0x00, 0x00, 0x00,                                   // CYCL_CNT = 100
            0x00, 0x00, 0x00, 0x00,                                   // REL_VADR = 0
            0x01, 0x00, 0x00, 0x00,                                   // REPT_CNT = 1
            0x00, 0x00, 0x00, 0x00,                                   // NUM_FAIL = 0
            0xFF, 0xFF, 0xFF, 0xFF,                                   // XFAIL_AD = -1
            0x00, 0x00, 0x00, 0x00,                                   // YFAIL_AD = 0
            0x00, 0x00,                                               // VECT_OFF = 0
            0x02, 0x00,                                               // RTN_ICNT = 2
            0x01, 0x00,                                               // RTN_INDX[0] = 1
            0x02, 0x00,                                               // RTN_INDX[1] = 2
            0x00,                                                     // RTN_STAT[0] = 0
            0x01,                                                     // RTN_STAT[1] = 1
            0x02, 0x00,                                               // PGM_ICNT = 2
            0x03, 0x00,                                               // PGM_INDX[0] = 3
            0x04, 0x00,                                               // PGM_INDX[1] = 4
            0x01,                                                     // PGM_STAT[0] = 1
            0x00,                                                     // PGM_STAT[1] = 0
            0x01, 0x00,                                               // FAIL_ICNT = 1
            0x05, 0x00,                                               // FAIL_INDX[0] = 5
            0x01                                                      // FAIL_STAT[0] = 1
        };

        [Fact]
        public void ParsesFtrBaseFields()
        {
            var records = new StdfParser().ParseAll(new MemoryStream(BaseFile));

            Assert.Equal(2, records.Count);
            var ftr = Assert.IsType<FtrRecord>(records[1]);
            Assert.Equal(RecordType.FTR, ftr.RecordType);
            Assert.Equal(3000u, ftr.TEST_NUM);
            Assert.Equal((byte)1, ftr.HEAD_NUM);
            Assert.Equal((byte)1, ftr.SITE_NUM);
            Assert.Equal(0x00, ftr.TEST_FLG);
            Assert.Equal(0x00, ftr.OPT_FLAG);
            Assert.Equal(100u, ftr.CYCL_CNT);
            Assert.Equal(0u, ftr.REL_VADR);
            Assert.Equal(1u, ftr.REPT_CNT);
            Assert.Equal(0u, ftr.NUM_FAIL);
            Assert.Equal(-1, ftr.XFAIL_AD);
            Assert.Equal(0, ftr.YFAIL_AD);
            Assert.Equal((short)0, ftr.VECT_OFF);

            // RTN group
            Assert.Equal((ushort)2, ftr.RTN_ICNT);
            Assert.Equal(2, ftr.RTN_INDX.Length);
            Assert.Equal((ushort)1, ftr.RTN_INDX[0]);
            Assert.Equal((ushort)2, ftr.RTN_INDX[1]);
            Assert.True(ftr.HasReturnState);
            Assert.NotNull(ftr.RTN_STAT);
            Assert.Equal((byte)0, ftr.RTN_STAT![0]);
            Assert.Equal((byte)1, ftr.RTN_STAT[1]);

            // PGM group
            Assert.Equal((ushort)2, ftr.PGM_ICNT);
            Assert.Equal((ushort)3, ftr.PGM_INDX[0]);
            Assert.Equal((ushort)4, ftr.PGM_INDX[1]);
            Assert.Equal((byte)1, ftr.PGM_STAT[0]);
            Assert.Equal((byte)0, ftr.PGM_STAT[1]);

            // FAIL group
            Assert.Equal((ushort)1, ftr.FAIL_ICNT);
            Assert.Equal((ushort)5, ftr.FAIL_INDX[0]);
            Assert.Equal((byte)1, ftr.FAIL_STAT[0]);

            // No trailing optionals
            Assert.Null(ftr.VECT_NAM);
            Assert.Null(ftr.ALARM_ID);
            Assert.Null(ftr.SPIN_CNT);
            Assert.Null(ftr.SPIN_INDX);
        }

        // FTR with trailing optionals: VECT_NAM, empty Cn fields, SPIN_CNT=2 + SPIN_INDX.
        // Body = 55 + 3 + 1 + 1 + 1 + 4 + 8 = 73
        private static readonly byte[] WithTrailingFile =
        {
            0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,                       // FAR(LE)
            0x49, 0x00, 0x0F, 0x1E,                                   // FTR: REC_LEN=73
            0xB8, 0x0B, 0x00, 0x00,                                   // TEST_NUM = 3000
            0x01,                                                     // HEAD_NUM = 1
            0x01,                                                     // SITE_NUM = 1
            0x00,                                                     // TEST_FLG = 0x00
            0x00,                                                     // OPT_FLAG = 0x00
            0x64, 0x00, 0x00, 0x00,                                   // CYCL_CNT = 100
            0x00, 0x00, 0x00, 0x00,                                   // REL_VADR = 0
            0x01, 0x00, 0x00, 0x00,                                   // REPT_CNT = 1
            0x00, 0x00, 0x00, 0x00,                                   // NUM_FAIL = 0
            0xFF, 0xFF, 0xFF, 0xFF,                                   // XFAIL_AD = -1
            0x00, 0x00, 0x00, 0x00,                                   // YFAIL_AD = 0
            0x00, 0x00,                                               // VECT_OFF = 0
            0x02, 0x00,                                               // RTN_ICNT = 2
            0x01, 0x00,                                               // RTN_INDX[0] = 1
            0x02, 0x00,                                               // RTN_INDX[1] = 2
            0x00,                                                     // RTN_STAT[0] = 0
            0x01,                                                     // RTN_STAT[1] = 1
            0x02, 0x00,                                               // PGM_ICNT = 2
            0x03, 0x00,                                               // PGM_INDX[0] = 3
            0x04, 0x00,                                               // PGM_INDX[1] = 4
            0x01,                                                     // PGM_STAT[0] = 1
            0x00,                                                     // PGM_STAT[1] = 0
            0x01, 0x00,                                               // FAIL_ICNT = 1
            0x05, 0x00,                                               // FAIL_INDX[0] = 5
            0x01,                                                     // FAIL_STAT[0] = 1
            0x02, 0x56, 0x31,                                         // VECT_NAM = "V1"
            0x00,                                                     // ALARM_ID = "" (len=0)
            0x00,                                                     // PROG_TXT = "" (len=0)
            0x00,                                                     // RSLT_TXT = "" (len=0)
            0x02, 0x00, 0x00, 0x00,                                   // SPIN_CNT = 2
            0x64, 0x00, 0x00, 0x00,                                   // SPIN_INDX[0] = 100
            0xC8, 0x00, 0x00, 0x00                                    // SPIN_INDX[1] = 200
        };

        [Fact]
        public void ParsesFtrWithTrailingAndSpinIndx()
        {
            var records = new StdfParser().ParseAll(new MemoryStream(WithTrailingFile));

            var ftr = Assert.IsType<FtrRecord>(records[1]);
            Assert.Equal("V1", ftr.VECT_NAM!.Value.Text);
            Assert.Equal("", ftr.ALARM_ID!.Value.Text);
            Assert.Equal("", ftr.PROG_TXT!.Value.Text);
            Assert.Equal("", ftr.RSLT_TXT!.Value.Text);
            Assert.Equal(2u, ftr.SPIN_CNT);
            Assert.NotNull(ftr.SPIN_INDX);
            Assert.Equal(2, ftr.SPIN_INDX!.Length);
            Assert.Equal(100u, ftr.SPIN_INDX[0]);
            Assert.Equal(200u, ftr.SPIN_INDX[1]);
        }

        // FTR with TEST_FLG=0x08 (bit3=1 → RTN_STAT absent), PGM_ICNT=0, FAIL_ICNT=0.
        // Body = 34 + (2+4+0) + 2 + 2 = 44
        private static readonly byte[] NoRtnStatFile =
        {
            0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,                       // FAR(LE)
            0x2C, 0x00, 0x0F, 0x1E,                                   // FTR: REC_LEN=44
            0xB8, 0x0B, 0x00, 0x00,                                   // TEST_NUM = 3000
            0x01,                                                     // HEAD_NUM = 1
            0x01,                                                     // SITE_NUM = 1
            0x08,                                                     // TEST_FLG = 0x08 (bit3=1 → RTN_STAT absent)
            0x00,                                                     // OPT_FLAG = 0x00
            0x64, 0x00, 0x00, 0x00,                                   // CYCL_CNT = 100
            0x00, 0x00, 0x00, 0x00,                                   // REL_VADR = 0
            0x01, 0x00, 0x00, 0x00,                                   // REPT_CNT = 1
            0x00, 0x00, 0x00, 0x00,                                   // NUM_FAIL = 0
            0xFF, 0xFF, 0xFF, 0xFF,                                   // XFAIL_AD = -1
            0x00, 0x00, 0x00, 0x00,                                   // YFAIL_AD = 0
            0x00, 0x00,                                               // VECT_OFF = 0
            0x02, 0x00,                                               // RTN_ICNT = 2
            0x01, 0x00,                                               // RTN_INDX[0] = 1
            0x02, 0x00,                                               // RTN_INDX[1] = 2
            // RTN_STAT absent
            0x00, 0x00,                                               // PGM_ICNT = 0
            // PGM_INDX empty, PGM_STAT empty
            0x00, 0x00,                                               // FAIL_ICNT = 0
            // FAIL_INDX empty, FAIL_STAT empty
        };

        [Fact]
        public void ParsesFtrWithoutReturnState()
        {
            var records = new StdfParser().ParseAll(new MemoryStream(NoRtnStatFile));

            var ftr = Assert.IsType<FtrRecord>(records[1]);
            Assert.Equal(0x08, ftr.TEST_FLG);
            Assert.True(ftr.TestFlags.HasFlag(TestFlag.TestAborted));
            Assert.False(ftr.HasReturnState);
            Assert.Null(ftr.RTN_STAT);
            Assert.Equal(2, ftr.RTN_INDX.Length);
            Assert.Equal((ushort)0, ftr.PGM_ICNT);
            Assert.Empty(ftr.PGM_STAT);
            Assert.Equal((ushort)0, ftr.FAIL_ICNT);
            Assert.Empty(ftr.FAIL_STAT);
        }
    }
}
