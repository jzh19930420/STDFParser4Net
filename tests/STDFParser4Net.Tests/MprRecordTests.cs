using System.IO;
using STDFParser4Net;
using STDFParser4Net.Enums;
using STDFParser4Net.Records;
using Xunit;

namespace STDFParser4Net.Tests
{
    public class MprRecordTests
    {
        // FAR(LE) + MPR with TEST_FLG=0x00 (bit3=0 → RTN_STAT present), 3 return results.
        private static readonly byte[] WithRtnStatFile =
        {
            0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,                       // FAR(LE)
            0x1D, 0x00, 0x0F, 0x0F,                                   // MPR: REC_LEN=29, TYP=15, SUB=15
            0xD0, 0x07, 0x00, 0x00,                                   // TEST_NUM = 2000
            0x01,                                                     // HEAD_NUM = 1
            0x01,                                                     // SITE_NUM = 1
            0x00,                                                     // TEST_FLG = 0x00 (bit3=0 → RTN_STAT present)
            0x00,                                                     // PARM_FLG = 0x00
            0x03, 0x00,                                               // RTN_ICNT = 3
            0x00, 0x00,                                               // RSLT_PCT = 0
            0x00, 0x00,                                               // NUM_CNT = 0
            0x00, 0x00, 0x80, 0x3F,                                   // RTN_RSLT[0] = 1.0f
            0x00, 0x00, 0x00, 0x40,                                   // RTN_RSLT[1] = 2.0f
            0x00, 0x00, 0x40, 0x40,                                   // RTN_RSLT[2] = 3.0f
            0x00,                                                     // RTN_STAT[0] = 0
            0x01,                                                     // RTN_STAT[1] = 1
            0x02                                                      // RTN_STAT[2] = 2
        };

        [Fact]
        public void ParsesMprWithReturnState()
        {
            var records = new StdfParser().ParseAll(new MemoryStream(WithRtnStatFile));

            Assert.Equal(2, records.Count);
            var mpr = Assert.IsType<MprRecord>(records[1]);
            Assert.Equal(RecordType.MPR, mpr.RecordType);
            Assert.Equal(2000u, mpr.TEST_NUM);
            Assert.Equal((byte)1, mpr.HEAD_NUM);
            Assert.Equal((byte)1, mpr.SITE_NUM);
            Assert.Equal(0x00, mpr.TEST_FLG);
            Assert.Equal((ushort)3, mpr.RTN_ICNT);
            Assert.Equal((ushort)0, mpr.RSLT_PCT);
            Assert.Equal((ushort)0, mpr.NUM_CNT);
            Assert.Equal(3, mpr.RTN_RSLT.Length);
            Assert.Equal(1.0f, mpr.RTN_RSLT[0]);
            Assert.Equal(2.0f, mpr.RTN_RSLT[1]);
            Assert.Equal(3.0f, mpr.RTN_RSLT[2]);
            Assert.True(mpr.HasReturnState);
            Assert.NotNull(mpr.RTN_STAT);
            Assert.Equal((byte)0, mpr.RTN_STAT![0]);
            Assert.Equal((byte)1, mpr.RTN_STAT[1]);
            Assert.Equal((byte)2, mpr.RTN_STAT[2]);
        }

        private static readonly byte[] WithoutRtnStatFile =
        {
            0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,                       // FAR(LE)
            0x16, 0x00, 0x0F, 0x0F,                                   // MPR: REC_LEN=22
            0xD0, 0x07, 0x00, 0x00,                                   // TEST_NUM = 2000
            0x01,                                                     // HEAD_NUM = 1
            0x01,                                                     // SITE_NUM = 1
            0x08,                                                     // TEST_FLG = 0x08 (bit3=1 → RTN_STAT absent)
            0x00,                                                     // PARM_FLG = 0x00
            0x02, 0x00,                                               // RTN_ICNT = 2
            0x00, 0x00,                                               // RSLT_PCT = 0
            0x00, 0x00,                                               // NUM_CNT = 0
            0x00, 0x00, 0x80, 0x3F,                                   // RTN_RSLT[0] = 1.0f
            0x00, 0x00, 0x00, 0x40                                    // RTN_RSLT[1] = 2.0f
        };

        [Fact]
        public void ParsesMprWithoutReturnState()
        {
            var records = new StdfParser().ParseAll(new MemoryStream(WithoutRtnStatFile));

            var mpr = Assert.IsType<MprRecord>(records[1]);
            Assert.Equal(0x08, mpr.TEST_FLG);
            Assert.True(mpr.TestFlags.HasFlag(TestFlag.TestAborted));
            Assert.False(mpr.HasReturnState);
            Assert.Null(mpr.RTN_STAT);
            Assert.Equal(2, mpr.RTN_RSLT.Length);
            Assert.Equal(1.0f, mpr.RTN_RSLT[0]);
            Assert.Equal(2.0f, mpr.RTN_RSLT[1]);
        }

        // Formats as Cn (length-prefixed).
        // Fixed: 4+1+1+1+1+2+2+2 = 14
        // RTN_RSLT: 4, RTN_STAT: 1, TEST_TXT:1, ALARM:1, OPT:1
        // RES/LLM/HLM:3, LO/HI:8, UNITS:2, formats: 2*3=6 → total 14+4+1+1+1+1+3+8+2+6 = 41
        private static readonly byte[] WithOptFlagFile =
        {
            0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,                       // FAR(LE)
            0x29, 0x00, 0x0F, 0x0F,                                   // MPR: REC_LEN=41
            0xD0, 0x07, 0x00, 0x00,                                   // TEST_NUM = 2000
            0x01,                                                     // HEAD_NUM = 1
            0x01,                                                     // SITE_NUM = 1
            0x00,                                                     // TEST_FLG = 0x00
            0x00,                                                     // PARM_FLG = 0x00
            0x01, 0x00,                                               // RTN_ICNT = 1
            0x00, 0x00,                                               // RSLT_PCT = 0
            0x00, 0x00,                                               // NUM_CNT = 0
            0x00, 0x00, 0x80, 0x3F,                                   // RTN_RSLT[0] = 1.0f
            0x00,                                                     // RTN_STAT[0] = 0
            0x00,                                                     // TEST_TXT = "" (len=0)
            0x00,                                                     // ALARM_ID = "" (len=0)
            0x00,                                                     // OPT_FLAG = 0x00
            0x00,                                                     // RES_SCAL = 0
            0x00,                                                     // LLM_SCAL = 0
            0x00,                                                     // HLM_SCAL = 0
            0x00, 0x00, 0x00, 0x3F,                                   // LO_LIMIT = 0.5f
            0x00, 0x00, 0xC0, 0x3F,                                   // HI_LIMIT = 1.5f
            0x01, 0x56,                                               // UNITS = "V"
            0x01, 0x25,                                               // C_RESFMT = "%"
            0x01, 0x25,                                               // C_LLMFMT = "%"
            0x01, 0x25                                                // C_HLMFMT = "%"
        };

        [Fact]
        public void ParsesMprWithOptFlagConditionalFields()
        {
            var records = new StdfParser().ParseAll(new MemoryStream(WithOptFlagFile));

            var mpr = Assert.IsType<MprRecord>(records[1]);
            Assert.Equal((byte?)0x00, mpr.OPT_FLAG);
            Assert.Equal((sbyte)0, mpr.RES_SCAL);
            Assert.Equal((sbyte)0, mpr.LLM_SCAL);
            Assert.Equal((sbyte)0, mpr.HLM_SCAL);
            Assert.Equal(0.5f, mpr.LO_LIMIT);
            Assert.Equal(1.5f, mpr.HI_LIMIT);
            Assert.Equal("V", mpr.UNITS!.Value.Text);
            Assert.Equal("%", mpr.C_RESFMT!.Value.Text);
            Assert.Equal("%", mpr.C_LLMFMT!.Value.Text);
            Assert.Equal("%", mpr.C_HLMFMT!.Value.Text);
        }
    }
}
