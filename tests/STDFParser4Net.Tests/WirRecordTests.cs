using System.IO;
using System.Collections.Generic;
using STDFParser4Net.Records;
using Xunit;

namespace STDFParser4Net.Tests
{
    public class WirRecordTests
    {
        [Fact]
        public void ParsesWirWithAllFields()
        {
            // WIR (2,10): HEAD_NUM=1, SETUP_GRP=2, START_T=1000, FINISH_T=2000, WAFER_ID="W01"
            var body = new List<byte>();
            StdfTestBytes.U1(body, 1);
            StdfTestBytes.U1(body, 2);
            StdfTestBytes.U4(body, 1000);
            StdfTestBytes.U4(body, 2000);
            StdfTestBytes.Cn(body, "W01");
            byte[] file = StdfTestBytes.FarPlusRecord(2, 10, body.ToArray());

            var parser = new StdfParser();
            var records = parser.ParseAll(new MemoryStream(file));

            Assert.Equal(2, records.Count);
            var wir = Assert.IsType<WirRecord>(records[1]);
            Assert.Equal(1, wir.HEAD_NUM);
            Assert.Equal(2, wir.SETUP_GRP);
            Assert.Equal(1000u, wir.START_T);
            Assert.Equal(2000u, wir.FINISH_T);
            Assert.Equal("W01", wir.WAFER_ID?.Text);
        }

        [Fact]
        public void ParsesWirWithoutOptionalFields()
        {
            // WIR (2,10): HEAD_NUM=1, SETUP_GRP=1, START_T=500 — no FINISH_T, no WAFER_ID
            var body = new List<byte>();
            StdfTestBytes.U1(body, 1);
            StdfTestBytes.U1(body, 1);
            StdfTestBytes.U4(body, 500);
            byte[] file = StdfTestBytes.FarPlusRecord(2, 10, body.ToArray());

            var parser = new StdfParser();
            var records = parser.ParseAll(new MemoryStream(file));

            Assert.Equal(2, records.Count);
            var wir = Assert.IsType<WirRecord>(records[1]);
            Assert.Equal(1, wir.HEAD_NUM);
            Assert.Equal(500u, wir.START_T);
            Assert.Null(wir.FINISH_T);
            Assert.Null(wir.WAFER_ID);
        }
    }
}
