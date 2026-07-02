using System.IO;
using System.Collections.Generic;
using STDFParser4Net.Records;
using Xunit;

namespace STDFParser4Net.Tests
{
    public class PirRecordTests
    {
        [Fact]
        public void ParsesPirWithHeadAndSite()
        {
            // PIR (5,10): HEAD_NUM=1, SITE_NUM=2
            var body = new byte[] { 0x01, 0x02 };
            byte[] file = StdfTestBytes.FarPlusRecord(5, 10, body);

            var parser = new StdfParser();
            var records = parser.ParseAll(new MemoryStream(file));

            Assert.Equal(2, records.Count);
            var pir = Assert.IsType<PirRecord>(records[1]);
            Assert.Equal(1, pir.HEAD_NUM);
            Assert.Equal(2, pir.SITE_NUM);
        }
    }
}
