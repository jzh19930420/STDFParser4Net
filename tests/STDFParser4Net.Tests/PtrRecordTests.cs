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
        // Formats are Cn: "%" is 1-byte string → length prefix 0x01 + 0x25.
        // Body = 12 + (1+3) + (1+2) + 1 + 1+1+1 + 4+4 + (1+1) + (1+1)+(1+1)+(1+1) + 4+4
        //      = 12 + 4 + 3 + 1 + 3 + 8 + 2 + 6 + 8 = 47
        private static readonly byte[] FullFile =
        {
            0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,                       // FAR(LE)
            0x2F, 0x00, 0x0F, 0x0A,                                   // PTR: REC_LEN=47, TYP=15, SUB=10
            0xE8, 0x03, 0x00, 0x00,                                   // TEST_NUM = 1000
            0x01,                                                     // HEAD_NUM = 1
            0x01,                                                     // SITE_NUM = 1
            0x80,                                                     // TEST_FLG = 0x80 (alarm)
            0x00,                                                     // PARM_FLG = 0x00
            0x00, 0x00, 0xC0, 0x3F,                                   // RESULT = 1.5f
            0x03, 0x56, 0x44, 0x44,                                   // TEST_TXT = "VDD"
            0x02, 0x41, 0x31,                                         // ALARM_ID = "A1"
            0x00,                                                     // OPT_FLAG = 0x00
            0xFD,                                                     // RES_SCAL = -3
            0xFD,                                                     // LLM_SCAL = -3
            0xFD,                                                     // HLM_SCAL = -3
            0x00, 0x00, 0x80, 0x3F,                                   // LO_LIMIT = 1.0f
            0x00, 0x00, 0x00, 0x40,                                   // HI_LIMIT = 2.0f
            0x01, 0x56,                                               // UNITS = "V"
            0x01, 0x25,                                               // C_RESFMT = "%"
            0x01, 0x25,                                               // C_LLMFMT = "%"
            0x01, 0x25,                                               // C_HLMFMT = "%"
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
            Assert.Equal("%", ptr.C_RESFMT!.Value.Text);
            Assert.Equal("%", ptr.C_LLMFMT!.Value.Text);
            Assert.Equal("%", ptr.C_HLMFMT!.Value.Text);
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

        // Production-style: OPT_FLAG=0x0E claims LLM/HLM/LO absent, but all scales/limits
        // are still written in fixed order (matches real PowerTech / WJS files).
        // Body = 12 + (1+21) + 1 + 1 + 1+1+1 + 4+4 + (1+1) + 0+0+0 + 4+4
        // TEST_TXT="Test:2 VFEC IAK +5 mA" len=21
        // REST after OPT: RES=0 LLM=0 HLM=0 LO=0.15 HI=1.5 UNITS="V" empty fmts + LO/HI_SPEC=0
        // = 12 + 22 + 1 + 1 + 3 + 8 + 2 + 3 + 8 = 60
        private static readonly byte[] ProductionStyleFile =
        {
            0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,                       // FAR(LE)
            0x3C, 0x00, 0x0F, 0x0A,                                   // PTR: REC_LEN=60
            0x02, 0x00, 0x00, 0x00,                                   // TEST_NUM = 2
            0x01,                                                     // HEAD_NUM
            0x01,                                                     // SITE_NUM
            0x00,                                                     // TEST_FLG
            0xC0,                                                     // PARM_FLG
            0x48, 0x57, 0xCF, 0x3E,                                   // RESULT ≈ 0.40496
            0x15,                                                     // TEST_TXT len=21
            0x54, 0x65, 0x73, 0x74, 0x3A, 0x32, 0x20, 0x56, 0x46, 0x45, 0x43, 0x20,
            0x49, 0x41, 0x4B, 0x20, 0x2B, 0x35, 0x20, 0x6D, 0x41,   // "Test:2 VFEC IAK +5 mA"
            0x00,                                                     // ALARM_ID = ""
            0x0E,                                                     // OPT_FLAG (bits claim LLM/HLM/LO absent)
            0x00,                                                     // RES_SCAL
            0x00,                                                     // LLM_SCAL (still present)
            0x00,                                                     // HLM_SCAL (still present)
            0x9A, 0x99, 0x19, 0x3E,                                   // LO_LIMIT = 0.15f
            0x00, 0x00, 0xC0, 0x3F,                                   // HI_LIMIT = 1.5f
            0x01, 0x56,                                               // UNITS = "V"
            0x00,                                                     // C_RESFMT = ""
            0x00,                                                     // C_LLMFMT = ""
            0x00,                                                     // C_HLMFMT = ""
            0x00, 0x00, 0x00, 0x00,                                   // LO_SPEC = 0
            0x00, 0x00, 0x00, 0x00                                    // HI_SPEC = 0
        };

        [Fact]
        public void ParsesProductionStyleIgnoringOptBits()
        {
            var records = new StdfParser().ParseAll(new MemoryStream(ProductionStyleFile));

            var ptr = Assert.IsType<PtrRecord>(records[1]);
            Assert.Equal(2u, ptr.TEST_NUM);
            Assert.Equal("Test:2 VFEC IAK +5 mA", ptr.TEST_TXT!.Value.Text);
            Assert.Equal((byte?)0x0E, ptr.OPT_FLAG);
            Assert.Equal((sbyte)0, ptr.RES_SCAL);
            Assert.Equal((sbyte)0, ptr.LLM_SCAL);
            Assert.Equal((sbyte)0, ptr.HLM_SCAL);
            Assert.Equal(0.15f, ptr.LO_LIMIT);
            Assert.Equal(1.5f, ptr.HI_LIMIT);
            Assert.Equal("V", ptr.UNITS!.Value.Text);
            Assert.Equal("", ptr.C_RESFMT!.Value.Text);
            Assert.Equal(0f, ptr.LO_SPEC);
            Assert.Equal(0f, ptr.HI_SPEC);
            // OPT bits still exposed for callers
            Assert.True(ptr.OptFlags.HasFlag(OptFlag.LlmScalAbsent));
            Assert.True(ptr.OptFlags.HasFlag(OptFlag.HlmScalAbsent));
            Assert.True(ptr.OptFlags.HasFlag(OptFlag.LoLimitAbsent));
        }

        // Two PTRs back-to-back: first uses production layout; second is minimal fixed-only.
        // Ensures first record does not desync the stream.
        [Fact]
        public void ProductionStylePtrDoesNotDesyncNextRecord()
        {
            var body2 = new byte[]
            {
                0x03, 0x00, 0x00, 0x00, // TEST_NUM=3
                0x01, 0x01, 0x00, 0x00,
                0x00, 0x00, 0x80, 0x3F  // RESULT=1.0
            };
            var file = new System.Collections.Generic.List<byte>();
            // reuse ProductionStyleFile without its FAR, rebuild FAR + two records
            file.AddRange(StdfTestBytes.FarLe());
            // first PTR body from ProductionStyleFile (skip FAR 6 + header 4)
            var firstBody = ProductionStyleFile.Skip(10).ToArray();
            StdfTestBytes.Header(file, (ushort)firstBody.Length, 15, 10);
            file.AddRange(firstBody);
            StdfTestBytes.Header(file, (ushort)body2.Length, 15, 10);
            file.AddRange(body2);

            var records = new StdfParser().ParseAll(new MemoryStream(file.ToArray()));
            Assert.Equal(3, records.Count);
            var ptr1 = Assert.IsType<PtrRecord>(records[1]);
            var ptr2 = Assert.IsType<PtrRecord>(records[2]);
            Assert.Equal(2u, ptr1.TEST_NUM);
            Assert.Equal("V", ptr1.UNITS!.Value.Text);
            Assert.Equal(3u, ptr2.TEST_NUM);
            Assert.Equal(1.0f, ptr2.RESULT);
            Assert.Null(ptr2.OPT_FLAG);
        }

        // Minimal PTR: only the 6 fixed fields, no optional data.
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

        [Fact]
        public void ParsesCnFormatStringPercent5Dot3f()
        {
            // OPT present + scales/limits + UNITS + C_RESFMT="%5.3f"
            var body = new System.Collections.Generic.List<byte>();
            StdfTestBytes.U4(body, 10010);
            StdfTestBytes.U1(body, 1);
            StdfTestBytes.U1(body, 1);
            StdfTestBytes.U1(body, 0);
            StdfTestBytes.U1(body, 0);
            StdfTestBytes.R4(body, 0.1f);
            StdfTestBytes.Cn(body, "Kelvin");
            StdfTestBytes.Cn(body, "");
            StdfTestBytes.U1(body, 0x0F);
            StdfTestBytes.U1(body, 0); // RES
            StdfTestBytes.U1(body, 0); // LLM
            StdfTestBytes.U1(body, 0); // HLM
            StdfTestBytes.R4(body, 0f);
            StdfTestBytes.R4(body, 0.2f);
            StdfTestBytes.Cn(body, "V");
            StdfTestBytes.Cn(body, "%5.3f");
            StdfTestBytes.Cn(body, "%5.3f");
            StdfTestBytes.Cn(body, "%5.3f");
            StdfTestBytes.R4(body, 0f);
            StdfTestBytes.R4(body, 0f);

            byte[] file = StdfTestBytes.FarPlusRecord(15, 10, body.ToArray());
            var ptr = Assert.IsType<PtrRecord>(new StdfParser().ParseAll(new MemoryStream(file))[1]);
            Assert.Equal("Kelvin", ptr.TEST_TXT!.Value.Text);
            Assert.Equal(0.2f, ptr.HI_LIMIT);
            Assert.Equal("V", ptr.UNITS!.Value.Text);
            Assert.Equal("%5.3f", ptr.C_RESFMT!.Value.Text);
            Assert.Equal("%5.3f", ptr.C_LLMFMT!.Value.Text);
            Assert.Equal("%5.3f", ptr.C_HLMFMT!.Value.Text);
        }
    }
}
