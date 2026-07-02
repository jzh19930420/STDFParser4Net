using System.IO;
using System.Collections.Generic;
using STDFParser4Net.Records;
using Xunit;

namespace STDFParser4Net.Tests
{
    public class TsrRecordTests
    {
        private static List<byte> TsrFixedBody(byte head, byte site, char testTyp, uint testNum,
            uint exec, uint fail, uint alrm)
        {
            var b = new List<byte>();
            StdfTestBytes.U1(b, head);
            StdfTestBytes.U1(b, site);
            StdfTestBytes.C1(b, testTyp);
            StdfTestBytes.U4(b, testNum);
            StdfTestBytes.U4(b, exec);
            StdfTestBytes.U4(b, fail);
            StdfTestBytes.U4(b, alrm);
            return b;
        }

        [Fact]
        public void ParsesTsrWithoutOptFlag()
        {
            // TSR (10,30) with only the 7 fixed fields — no OPT_FLAG.
            var body = TsrFixedBody(1, 1, 'P', 1000, 500, 10, 2);
            byte[] file = StdfTestBytes.FarPlusRecord(10, 30, body.ToArray());

            var parser = new StdfParser();
            var records = parser.ParseAll(new MemoryStream(file));

            Assert.Equal(2, records.Count);
            var tsr = Assert.IsType<TsrRecord>(records[1]);
            Assert.Equal(1, tsr.HEAD_NUM);
            Assert.Equal(1, tsr.SITE_NUM);
            Assert.Equal('P', tsr.TEST_TYP);
            Assert.Equal(1000u, tsr.TEST_NUM);
            Assert.Equal(500u, tsr.EXEC_CNT);
            Assert.Equal(10u, tsr.FAIL_CNT);
            Assert.Equal(2u, tsr.ALRM_CNT);
            Assert.Null(tsr.OPT_FLAG);
            Assert.Null(tsr.CYCL_CNT);
            Assert.Null(tsr.REL_VADR);
            Assert.Null(tsr.TST_MAX);
        }

        [Fact]
        public void ParsesTsrWithOptFlagZero_AllFieldsPresent()
        {
            // OPT_FLAG=0x00 → all conditional fields present.
            var body = TsrFixedBody(1, 2, 'F', 2000, 100, 5, 1);
            StdfTestBytes.U1(body, 0x00);  // OPT_FLAG
            StdfTestBytes.U4(body, 300);   // CYCL_CNT
            StdfTestBytes.U4(body, 301);   // REL_VADR
            StdfTestBytes.U4(body, 302);   // REPT_CNT
            StdfTestBytes.U4(body, 303);   // NUM_FAIL
            StdfTestBytes.I4(body, -10);   // XFAIL_AD
            StdfTestBytes.I4(body, 20);    // YFAIL_AD
            StdfTestBytes.I2(body, 7);     // VECT_OFF
            StdfTestBytes.U1(body, 1);     // RTN_ICND
            StdfTestBytes.U1(body, 2);     // PROG_ICND
            StdfTestBytes.U1(body, 3);     // FAIL_ICND
            StdfTestBytes.Bn(body, new byte[] { 0xAA });  // ALRM_COD
            StdfTestBytes.Cn(body, "PROG");  // PROG_NAM
            StdfTestBytes.Cn(body, "RSLT");  // RSLT_NAM
            StdfTestBytes.U4(body, 999);    // TST_DUR
            StdfTestBytes.U4(body, 1);      // TST_MIN
            StdfTestBytes.U4(body, 2);      // TST_MAX
            byte[] file = StdfTestBytes.FarPlusRecord(10, 30, body.ToArray());

            var parser = new StdfParser();
            var records = parser.ParseAll(new MemoryStream(file));

            Assert.Equal(2, records.Count);
            var tsr = Assert.IsType<TsrRecord>(records[1]);
            Assert.Equal('F', tsr.TEST_TYP);
            Assert.Equal((byte?)0x00, tsr.OPT_FLAG);
            Assert.Equal((uint?)300, tsr.CYCL_CNT);
            Assert.Equal((uint?)301, tsr.REL_VADR);
            Assert.Equal((uint?)302, tsr.REPT_CNT);
            Assert.Equal((uint?)303, tsr.NUM_FAIL);
            Assert.Equal((int?)(-10), tsr.XFAIL_AD);
            Assert.Equal((int?)20, tsr.YFAIL_AD);
            Assert.Equal((short?)7, tsr.VECT_OFF);
            Assert.Equal((byte?)1, tsr.RTN_ICND);
            Assert.Equal((byte?)2, tsr.PROG_ICND);
            Assert.Equal((byte?)3, tsr.FAIL_ICND);
            Assert.Equal(new byte[] { 0xAA }, tsr.ALRM_COD);
            Assert.Equal("PROG", tsr.PROG_NAM?.Text);
            Assert.Equal("RSLT", tsr.RSLT_NAM?.Text);
            Assert.Equal((uint?)999, tsr.TST_DUR);
            Assert.Equal((uint?)1, tsr.TST_MIN);
            Assert.Equal((uint?)2, tsr.TST_MAX);
        }

        [Fact]
        public void ParsesTsrWithOptFlag0xFF_Bits0to7Omitted()
        {
            // OPT_FLAG=0xFF → all bit0-bit7 fields omitted.
            // Bits 8-15 of a single byte are always 0, so those fields are still present.
            var body = TsrFixedBody(1, 1, 'M', 3000, 50, 0, 0);
            StdfTestBytes.U1(body, 0xFF);  // OPT_FLAG — all bits 0-7 set
            // CYCL_CNT through RTN_ICND: all omitted
            StdfTestBytes.U1(body, 11);    // PROG_ICND (bit8=0, present)
            StdfTestBytes.U1(body, 22);    // FAIL_ICND (bit9=0, present)
            StdfTestBytes.Bn(body, new byte[] { 0x01 });  // ALRM_COD (bit10=0)
            StdfTestBytes.Cn(body, "PN");  // PROG_NAM (bit11=0)
            StdfTestBytes.Cn(body, "RN");  // RSLT_NAM (bit12=0)
            StdfTestBytes.U4(body, 800);   // TST_DUR (bit13=0)
            StdfTestBytes.U4(body, 5);     // TST_MIN (bit14=0)
            StdfTestBytes.U4(body, 9);     // TST_MAX (bit15=0)
            byte[] file = StdfTestBytes.FarPlusRecord(10, 30, body.ToArray());

            var parser = new StdfParser();
            var records = parser.ParseAll(new MemoryStream(file));

            Assert.Equal(2, records.Count);
            var tsr = Assert.IsType<TsrRecord>(records[1]);
            Assert.Equal((byte?)0xFF, tsr.OPT_FLAG);
            // Bits 0-7 fields: all null
            Assert.Null(tsr.CYCL_CNT);
            Assert.Null(tsr.REL_VADR);
            Assert.Null(tsr.REPT_CNT);
            Assert.Null(tsr.NUM_FAIL);
            Assert.Null(tsr.XFAIL_AD);
            Assert.Null(tsr.YFAIL_AD);
            Assert.Null(tsr.VECT_OFF);
            Assert.Null(tsr.RTN_ICND);
            // Bits 8-15 fields: present
            Assert.Equal((byte?)11, tsr.PROG_ICND);
            Assert.Equal((byte?)22, tsr.FAIL_ICND);
            Assert.Equal(new byte[] { 0x01 }, tsr.ALRM_COD);
            Assert.Equal("PN", tsr.PROG_NAM?.Text);
            Assert.Equal("RN", tsr.RSLT_NAM?.Text);
            Assert.Equal((uint?)800, tsr.TST_DUR);
            Assert.Equal((uint?)5, tsr.TST_MIN);
            Assert.Equal((uint?)9, tsr.TST_MAX);
        }

        [Fact]
        public void ParsesTsrWithOptFlag0x01_OnlyCyclCntOmitted()
        {
            // OPT_FLAG=0x01 → only bit0 set, CYCL_CNT omitted; all others present.
            var body = TsrFixedBody(2, 3, 'P', 4000, 200, 8, 0);
            StdfTestBytes.U1(body, 0x01);  // OPT_FLAG — bit0=1
            // CYCL_CNT omitted
            StdfTestBytes.U4(body, 410);   // REL_VADR (bit1=0, present)
            StdfTestBytes.U4(body, 420);   // REPT_CNT (bit2=0, present)
            StdfTestBytes.U4(body, 430);   // NUM_FAIL (bit3=0, present)
            StdfTestBytes.I4(body, -1);    // XFAIL_AD (bit4=0, present)
            StdfTestBytes.I4(body, 1);     // YFAIL_AD (bit5=0, present)
            StdfTestBytes.I2(body, 0);     // VECT_OFF (bit6=0, present)
            StdfTestBytes.U1(body, 9);     // RTN_ICND (bit7=0, present)
            // Bits 8-15: present
            StdfTestBytes.U1(body, 5);     // PROG_ICND
            StdfTestBytes.U1(body, 6);     // FAIL_ICND
            StdfTestBytes.Bn(body, new byte[0]);  // ALRM_COD (empty)
            StdfTestBytes.Cn(body, "");    // PROG_NAM (empty)
            StdfTestBytes.Cn(body, "");    // RSLT_NAM (empty)
            StdfTestBytes.U4(body, 111);   // TST_DUR
            StdfTestBytes.U4(body, 222);   // TST_MIN
            StdfTestBytes.U4(body, 333);   // TST_MAX
            byte[] file = StdfTestBytes.FarPlusRecord(10, 30, body.ToArray());

            var parser = new StdfParser();
            var records = parser.ParseAll(new MemoryStream(file));

            Assert.Equal(2, records.Count);
            var tsr = Assert.IsType<TsrRecord>(records[1]);
            Assert.Equal((byte?)0x01, tsr.OPT_FLAG);
            Assert.Null(tsr.CYCL_CNT);             // bit0=1 → omitted
            Assert.Equal((uint?)410, tsr.REL_VADR); // bit1=0 → present
            Assert.Equal((uint?)420, tsr.REPT_CNT);
            Assert.Equal((uint?)430, tsr.NUM_FAIL);
            Assert.Equal((int?)(-1), tsr.XFAIL_AD);
            Assert.Equal((int?)1, tsr.YFAIL_AD);
            Assert.Equal((short?)0, tsr.VECT_OFF);
            Assert.Equal((byte?)9, tsr.RTN_ICND);
            Assert.Equal((byte?)5, tsr.PROG_ICND);
            Assert.Equal((byte?)6, tsr.FAIL_ICND);
            Assert.Equal((uint?)111, tsr.TST_DUR);
            Assert.Equal((uint?)222, tsr.TST_MIN);
            Assert.Equal((uint?)333, tsr.TST_MAX);
        }
    }
}
