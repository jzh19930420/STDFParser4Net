using System.IO;
using System.Collections.Generic;
using STDFParser4Net.Records;
using Xunit;

namespace STDFParser4Net.Tests
{
    public class WcrRecordTests
    {
        [Fact]
        public void ParsesWcrAllFields()
        {
            // WCR (2,30):
            // WAFR_SIZ R4=8.0, DIE_HT R4=1.0, DIE_WID R4=1.0,
            // WF_UNITS U1=3, WF_FLAT C1='D', CENTER_X I2=5, CENTER_Y I2=-3,
            // POS_X C1='R', POS_Y C1='U'
            var body = new List<byte>();
            StdfTestBytes.R4(body, 8.0f);
            StdfTestBytes.R4(body, 1.0f);
            StdfTestBytes.R4(body, 1.0f);
            StdfTestBytes.U1(body, 3);
            StdfTestBytes.C1(body, 'D');
            StdfTestBytes.I2(body, 5);
            StdfTestBytes.I2(body, -3);
            StdfTestBytes.C1(body, 'R');
            StdfTestBytes.C1(body, 'U');
            byte[] file = StdfTestBytes.FarPlusRecord(2, 30, body.ToArray());

            var parser = new StdfParser();
            var records = parser.ParseAll(new MemoryStream(file));

            Assert.Equal(2, records.Count);
            var wcr = Assert.IsType<WcrRecord>(records[1]);
            Assert.Equal(8.0f, wcr.WAFR_SIZ);
            Assert.Equal(1.0f, wcr.DIE_HT);
            Assert.Equal(1.0f, wcr.DIE_WID);
            Assert.Equal(3, wcr.WF_UNITS);
            Assert.Equal('D', wcr.WF_FLAT);
            Assert.Equal(5, wcr.CENTER_X);
            Assert.Equal(-3, wcr.CENTER_Y);
            Assert.Equal('R', wcr.POS_X);
            Assert.Equal('U', wcr.POS_Y);
        }
    }
}
