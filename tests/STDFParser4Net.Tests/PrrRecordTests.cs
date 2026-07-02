using System.IO;
using System.Collections.Generic;
using STDFParser4Net.Enums;
using STDFParser4Net.Records;
using Xunit;

namespace STDFParser4Net.Tests
{
    public class PrrRecordTests
    {
        [Fact]
        public void ParsesPrrWithAllFields()
        {
            // PRR (5,20) per task field order:
            // HEAD_NUM=1, SITE_NUM=2, PART_NUM=10, NUM_TEST=50, HARD_BIN=1, SOFT_BIN=1,
            // X_COORD=5, Y_COORD=-3, TEST_T=1234, PART_FLG=0x00(pass), NUM_HARD=3, NUM_SOFT=3,
            // PART_ID="P1", PART_TXT="T1", PART_FIX=[0xAB,0xCD]
            var body = new List<byte>();
            StdfTestBytes.U1(body, 1);       // HEAD_NUM
            StdfTestBytes.U1(body, 2);       // SITE_NUM
            StdfTestBytes.U4(body, 10);      // PART_NUM
            StdfTestBytes.U2(body, 50);      // NUM_TEST
            StdfTestBytes.U2(body, 1);       // HARD_BIN
            StdfTestBytes.U2(body, 1);       // SOFT_BIN
            StdfTestBytes.I2(body, 5);       // X_COORD
            StdfTestBytes.I2(body, -3);      // Y_COORD
            StdfTestBytes.U4(body, 1234);    // TEST_T
            StdfTestBytes.U1(body, 0x00);    // PART_FLG (pass)
            StdfTestBytes.U2(body, 3);       // NUM_HARD
            StdfTestBytes.U2(body, 3);       // NUM_SOFT
            StdfTestBytes.Cn(body, "P1");    // PART_ID
            StdfTestBytes.Cn(body, "T1");    // PART_TXT
            StdfTestBytes.Bn(body, new byte[] { 0xAB, 0xCD }); // PART_FIX
            byte[] file = StdfTestBytes.FarPlusRecord(5, 20, body.ToArray());

            var parser = new StdfParser();
            var records = parser.ParseAll(new MemoryStream(file));

            Assert.Equal(2, records.Count);
            var prr = Assert.IsType<PrrRecord>(records[1]);
            Assert.Equal(1, prr.HEAD_NUM);
            Assert.Equal(2, prr.SITE_NUM);
            Assert.Equal(10u, prr.PART_NUM);
            Assert.Equal(50, prr.NUM_TEST);
            Assert.Equal(1, prr.HARD_BIN);
            Assert.Equal(1, prr.SOFT_BIN);
            Assert.Equal(5, prr.X_COORD);
            Assert.Equal(-3, prr.Y_COORD);
            Assert.Equal(1234u, prr.TEST_T);
            Assert.Equal(0x00, prr.PART_FLG);
            Assert.Equal(3, prr.NUM_HARD);
            Assert.Equal(3, prr.NUM_SOFT);
            Assert.Equal("P1", prr.PART_ID?.Text);
            Assert.Equal("T1", prr.PART_TXT?.Text);
            Assert.Equal(new byte[] { 0xAB, 0xCD }, prr.PART_FIX);
            Assert.True(prr.IsPass);
        }

        [Fact]
        public void ParsesPrrWithoutOptionalFields()
        {
            // PRR (5,20): only fixed fields, PART_FLG=0x08 (failed)
            var body = new List<byte>();
            StdfTestBytes.U1(body, 1);
            StdfTestBytes.U1(body, 1);
            StdfTestBytes.U4(body, 1);
            StdfTestBytes.U2(body, 10);
            StdfTestBytes.U2(body, 2);
            StdfTestBytes.U2(body, 2);
            StdfTestBytes.I2(body, 0);
            StdfTestBytes.I2(body, 0);
            StdfTestBytes.U4(body, 100);
            StdfTestBytes.U1(body, 0x08);  // PART_FLG = Failed
            StdfTestBytes.U2(body, 1);
            StdfTestBytes.U2(body, 1);
            byte[] file = StdfTestBytes.FarPlusRecord(5, 20, body.ToArray());

            var parser = new StdfParser();
            var records = parser.ParseAll(new MemoryStream(file));

            Assert.Equal(2, records.Count);
            var prr = Assert.IsType<PrrRecord>(records[1]);
            Assert.Equal(0x08, prr.PART_FLG);
            Assert.False(prr.IsPass);
            Assert.Null(prr.PART_ID);
            Assert.Null(prr.PART_TXT);
            Assert.Null(prr.PART_FIX);
        }

        [Fact]
        public void PartFlagEnumHasCorrectValues()
        {
            Assert.Equal(0x01, (byte)PartFlag.NoPassFail);
            Assert.Equal(0x02, (byte)PartFlag.Retested);
            Assert.Equal(0x04, (byte)PartFlag.Abnormal);
            Assert.Equal(0x08, (byte)PartFlag.Failed);
        }
    }
}
