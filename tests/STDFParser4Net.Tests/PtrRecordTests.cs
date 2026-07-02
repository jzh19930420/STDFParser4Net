using System.IO;
using System.Linq;
using STDFParser4Net;
using STDFParser4Net.Enums;
using STDFParser4Net.Records;
using Xunit;

namespace STDFParser4Net.Tests
{
    public class PtrRecordTests
    {
        // FAR(LE) + PTR with all optional fields present.
        // OPT_FLAG=0x00 → RES_SCAL..C_LLMFMT all present, plus trailing C_HLMFMT/LO_SPEC/HI_SPEC.
        // Body = 4+1+1+1+1+4 + (1+3) + (1+2) + 1 + 1+1+1 + 4+4 + (1+1) + 1+1+1 + 4+4 = 44
        private static readonly byte[] FullFile =
        {
            0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,                       // FAR(LE)
            0x2C, 0x00, 0x0F, 0x0A,                                   // PTR: REC_LEN=44, TYP=15, SUB=10
            0xE8, 0x03, 0x00, 0x00,                                   // TEST_NUM = 1000
            0x01,                                                     // HEAD_NUM = 1
            0x01,                                                     // SITE_NUM = 1
            0x80,                                                     // TEST_FLG = 0x80 (alarm)
            0x00,                                                     // PARM_FLG = 0x00
            0x00, 0x00, 0xC0, 0x3F,                                   // RESULT = 1.5f
            0x03, 0x56, 0x44, 0x44,                                   // TEST_TXT = "VDD"
            0x02, 0x41, 0x31,                                         // ALARM_ID = "A1"
            0x00,                                                     // OPT_FLAG = 0x00 (all present)
            0xFD,                                                     // RES_SCAL = -3
            0xFD,                                                     // LLM_SCAL = -3
            0xFD,                                                     // HLM_SCAL = -3
            0x00, 0x00, 0x80, 0x3F,                                   // LO_LIMIT = 1.0f
            0x00, 0x00, 0x00, 0x40,                                   // HI_LIMIT = 2.0f
            0x01, 0x56,                                               // UNITS = "V"
            0x25,                                                     // C_RESFMT = '%'
            0x25,                                                     // C_LLMFMT = '%'
            0x25,                                                     // C_HLMFMT = '%'
            0x00, 0x00, 0x00, 0x3F,                                   // LO_SPEC = 0.5f
            0x00, 0x00, 0x20, 0x40                                    // HI_SPEC = 2.5f
        };

        [Fact]
        public void ParsesPtrAllFields()
        {
            var records = new StdfParser().ParseAll(new MemoryStream(FullFile));

            Assert.Equal(2, records.Count);
            var ptr = Assert.IsType<PtrRecord>(records[1]);
            Assert.Equal(RecordType.PTR, ptr.RecordType);
            Assert.Equal(1000u, ptr.TEST_NUM);
            Assert.Equal((byte)1, ptr.HEAD_NUM);
            Assert.Equal((byte)1, ptr.SITE_NUM);
            Assert.Equal(0x80, ptr.TEST_FLG);
            Assert.Equal(0x00, ptr.PARM_FLG);
            Assert.Equal(1.5f, ptr.RESULT);
            Assert.Equal("VDD", ptr.TEST_TXT!.Value.Text);
            Assert.Equal("A1", ptr.ALARM_ID!.Value.Text);
            Assert.Equal((byte?)0x00, ptr.OPT_FLAG);
            Assert.Equal((sbyte)-3, ptr.RES_SCAL);
            Assert.Equal((sbyte)-3, ptr.LLM_SCAL);
            Assert.Equal((sbyte)-3, ptr.HLM_SCAL);
            Assert.Equal(1.0f, ptr.LO_LIMIT);
            Assert.Equal(2.0f, ptr.HI_LIMIT);
            Assert.Equal("V", ptr.UNITS!.Value.Text);
            Assert.Equal('%', ptr.C_RESFMT);
            Assert.Equal('%', ptr.C_LLMFMT);
            Assert.Equal('%', ptr.C_HLMFMT);
            Assert.Equal(0.5f, ptr.LO_SPEC);
            Assert.Equal(2.5f, ptr.HI_SPEC);
        }

        [Fact]
        public void PtrFlagEnumsAreCorrect()
        {
            var records = new StdfParser().ParseAll(new MemoryStream(FullFile));
            var ptr = (PtrRecord)records[1];
            Assert.True(ptr.TestFlags.HasFlag(TestFlag.AlarmDetected));
            Assert.Equal(ParamFlag.None, ptr.ParmFlags);
            Assert.Equal(OptFlag.None, ptr.OptFlags);
        }

        // PTR with OPT_FLAG=0x18 → LO_LIMIT (bit3) and HI_LIMIT (bit4) absent.
        // Body = 12 + 4 + 3 + 1 + 1+1+1 + (skip 4+4) + 2 + 1+1+1 + 4+4 = 36
        private static readonly byte[] PartialFile =
        {
            0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,                       // FAR(LE)
            0x24, 0x00, 0x0F, 0x0A,                                   // PTR: REC_LEN=36
            0xE8, 0x03, 0x00, 0x00,                                   // TEST_NUM = 1000
            0x01,                                                     // HEAD_NUM = 1
            0x01,                                                     // SITE_NUM = 1
            0x10,                                                     // TEST_FLG = 0x10 (failed)
            0x40,                                                     // PARM_FLG = 0x40 (optional low limit failed)
            0x00, 0x00, 0x80, 0x3F,                                   // RESULT = 1.0f
            0x03, 0x49, 0x44, 0x44,                                   // TEST_TXT = "IDD"
            0x00,                                                     // ALARM_ID = "" (len=0)
            0x18,                                                     // OPT_FLAG = 0x18 (LO_LIMIT + HI_LIMIT absent)
            0xFD,                                                     // RES_SCAL = -3
            0xFD,                                                     // LLM_SCAL = -3
            0xFD,                                                     // HLM_SCAL = -3
            // LO_LIMIT absent (bit3=1)
            // HI_LIMIT absent (bit4=1)
            0x01, 0x41,                                               // UNITS = "A"
            0x25,                                                     // C_RESFMT = '%'
            0x25,                                                     // C_LLMFMT = '%'
            0x25,                                                     // C_HLMFMT = '%'
            0x00, 0x00, 0x00, 0x3F,                                   // LO_SPEC = 0.5f
            0x00, 0x00, 0x20, 0x40                                    // HI_SPEC = 2.5f
        };

        [Fact]
        public void ParsesPtrWithOptFlagSkippingLimits()
        {
            var records = new StdfParser().ParseAll(new MemoryStream(PartialFile));

            var ptr = Assert.IsType<PtrRecord>(records[1]);
            Assert.Equal((byte?)0x18, ptr.OPT_FLAG);
            Assert.Equal((sbyte)-3, ptr.RES_SCAL);
            Assert.Null(ptr.LO_LIMIT);
            Assert.Null(ptr.HI_LIMIT);
            Assert.Equal("A", ptr.UNITS!.Value.Text);
            Assert.Equal(0.5f, ptr.LO_SPEC);
            Assert.Equal(2.5f, ptr.HI_SPEC);
            Assert.True(ptr.TestFlags.HasFlag(TestFlag.TestFailed));
            Assert.True(ptr.ParmFlags.HasFlag(ParamFlag.OptionalLowLimitFailed));
            Assert.True(ptr.OptFlags.HasFlag(OptFlag.LoLimitAbsent));
            Assert.True(ptr.OptFlags.HasFlag(OptFlag.HiLimitAbsent));
        }

        // Minimal PTR: only the 6 fixed fields, no optional data.
        // Body = 4+1+1+1+1+4 = 12
        private static readonly byte[] MinimalFile =
        {
            0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,                       // FAR(LE)
            0x0C, 0x00, 0x0F, 0x0A,                                   // PTR: REC_LEN=12
            0x01, 0x00, 0x00, 0x00,                                   // TEST_NUM = 1
            0x01,                                                     // HEAD_NUM = 1
            0x01,                                                     // SITE_NUM = 1
            0x00,                                                     // TEST_FLG = 0x00
            0x00,                                                     // PARM_FLG = 0x00
            0x00, 0x00, 0x80, 0x3F                                    // RESULT = 1.0f
        };

        [Fact]
        public void ParsesPtrMinimalNoOptional()
        {
            var records = new StdfParser().ParseAll(new MemoryStream(MinimalFile));

            var ptr = Assert.IsType<PtrRecord>(records[1]);
            Assert.Equal(1u, ptr.TEST_NUM);
            Assert.Equal(1.0f, ptr.RESULT);
            Assert.Null(ptr.TEST_TXT);
            Assert.Null(ptr.ALARM_ID);
            Assert.Null(ptr.OPT_FLAG);
            Assert.Null(ptr.RES_SCAL);
            Assert.Null(ptr.LO_LIMIT);
            Assert.Null(ptr.HI_LIMIT);
            Assert.Null(ptr.LO_SPEC);
            Assert.Null(ptr.HI_SPEC);
        }
    }
}
