using System.IO;
using System.Collections.Generic;
using STDFParser4Net.Records;
using Xunit;

namespace STDFParser4Net.Tests
{
    public class WrrRecordTests
    {
        [Fact]
        public void ParsesWrrWithAllOptionalCnFields()
        {
            // WRR (2,20): HEAD_NUM=1, PART_CNT=100, RTST_CNT=2, ABRT_CNT=1, GOOD_CNT=95, FUNC_CNT=90,
            // WAFER_ID="W01", FABWF_ID="F1", WAFR_NOTE="NOTE"
            var body = new List<byte>();
            StdfTestBytes.U1(body, 1);
            StdfTestBytes.U4(body, 100);
            StdfTestBytes.U4(body, 2);
            StdfTestBytes.U4(body, 1);
            StdfTestBytes.U4(body, 95);
            StdfTestBytes.U4(body, 90);
            StdfTestBytes.Cn(body, "W01");
            StdfTestBytes.Cn(body, "F1");
            StdfTestBytes.Cn(body, "NOTE");
            byte[] file = StdfTestBytes.FarPlusRecord(2, 20, body.ToArray());

            var parser = new StdfParser();
            var records = parser.ParseAll(new MemoryStream(file));

            Assert.Equal(2, records.Count);
            var wrr = Assert.IsType<WrrRecord>(records[1]);
            Assert.Equal(1, wrr.HEAD_NUM);
            Assert.Equal(100u, wrr.PART_CNT);
            Assert.Equal(2u, wrr.RTST_CNT);
            Assert.Equal(1u, wrr.ABRT_CNT);
            Assert.Equal(95u, wrr.GOOD_CNT);
            Assert.Equal(90u, wrr.FUNC_CNT);
            Assert.Equal("W01", wrr.WAFER_ID?.Text);
            Assert.Equal("F1", wrr.FABWF_ID?.Text);
            Assert.Equal("NOTE", wrr.WAFR_NOTE?.Text);
        }

        [Fact]
        public void ParsesWrrWithoutOptionalFields()
        {
            // WRR (2,20): only fixed fields
            var body = new List<byte>();
            StdfTestBytes.U1(body, 1);
            StdfTestBytes.U4(body, 50);
            StdfTestBytes.U4(body, 0);
            StdfTestBytes.U4(body, 0);
            StdfTestBytes.U4(body, 48);
            StdfTestBytes.U4(body, 48);
            byte[] file = StdfTestBytes.FarPlusRecord(2, 20, body.ToArray());

            var parser = new StdfParser();
            var records = parser.ParseAll(new MemoryStream(file));

            Assert.Equal(2, records.Count);
            var wrr = Assert.IsType<WrrRecord>(records[1]);
            Assert.Equal(50u, wrr.PART_CNT);
            Assert.Equal(48u, wrr.GOOD_CNT);
            Assert.Null(wrr.WAFER_ID);
            Assert.Null(wrr.FABWF_ID);
            Assert.Null(wrr.WAFR_NOTE);
        }
    }
}
