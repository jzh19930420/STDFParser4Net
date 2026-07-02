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
        // Body = 4+1+1+1+1+2+2+2 + 3*4 + 3*1 = 14+12+3 = 29
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

        // MPR with TEST_FLG=0x08 (bit3=1 → RTN_STAT absent), 2 return results.
        // Body = 14 + 2*4 = 22
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

        // MPR with OPT_FLAG and conditional fields.
        // TEST_FLG=0x00, RTN_ICNT=1, RTN_RSLT={1.0f}, RTN_STAT={0},
        // TEST_TXT="", ALARM_ID="" (empty Cn, present before OPT_FLAG),
        // OPT_FLAG=0x00 (all present), RES_SCAL=0, LLM_SCAL=0, HLM_SCAL=0,
        // LO_LIMIT=0.5f, HI_LIMIT=1.5f, UNITS="V", C_RESFMT='%', C_LLMFMT='%', C_HLMFMT='%'
        // Body = 14 + 4 + 1 + 1 + 1 + 1 + 1+1+1 + 4+4 + (1+1) + 1+1+1 = 38
        private static readonly byte[] WithOptFlagFile =
        {
            0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,                       // FAR(LE)
            0x26, 0x00, 0x0F, 0x0F,                                   // MPR: REC_LEN=38
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
            0x00,                                                     // OPT_FLAG = 0x00 (all present)
            0x00,                                                     // RES_SCAL = 0
            0x00,                                                     // LLM_SCAL = 0
            0x00,                                                     // HLM_SCAL = 0
            0x00, 0x00, 0x00, 0x3F,                                   // LO_LIMIT = 0.5f
            0x00, 0x00, 0xC0, 0x3F,                                   // HI_LIMIT = 1.5f
            0x01, 0x56,                                               // UNITS = "V"
            0x25,                                                     // C_RESFMT = '%'
            0x25,                                                     // C_LLMFMT = '%'
            0x25                                                      // C_HLMFMT = '%'
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
            Assert.Equal('%', mpr.C_RESFMT);
            Assert.Equal('%', mpr.C_LLMFMT);
            Assert.Equal('%', mpr.C_HLMFMT);
        }
    }
}
