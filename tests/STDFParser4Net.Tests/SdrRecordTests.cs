using System.IO;
using System.Collections.Generic;
using STDFParser4Net.Records;
using Xunit;

namespace STDFParser4Net.Tests
{
    public class SdrRecordTests
    {
        [Fact]
        public void ParsesSdrWithSiteArrayAndEquipmentFields()
        {
            // SDR (1,80): HEAD_NUM=1, SITE_GROUP=1, SITE_CNT=3, SITE_NUM=[1,2,4],
            // HAND_TYP="HT", HAND_ID="H1", CARD_TYP="CT", CARD_ID="C1"
            var body = new List<byte>();
            StdfTestBytes.U1(body, 1);       // HEAD_NUM
            StdfTestBytes.U1(body, 1);       // SITE_GROUP
            StdfTestBytes.U1(body, 3);       // SITE_CNT
            StdfTestBytes.U1(body, 1);       // SITE_NUM[0]
            StdfTestBytes.U1(body, 2);       // SITE_NUM[1]
            StdfTestBytes.U1(body, 4);       // SITE_NUM[2]
            StdfTestBytes.Cn(body, "HT");    // HAND_TYP
            StdfTestBytes.Cn(body, "H1");    // HAND_ID
            StdfTestBytes.Cn(body, "CT");    // CARD_TYP
            StdfTestBytes.Cn(body, "C1");    // CARD_ID
            byte[] file = StdfTestBytes.FarPlusRecord(1, 80, body.ToArray());

            var parser = new StdfParser();
            var records = parser.ParseAll(new MemoryStream(file));

            Assert.Equal(2, records.Count);
            var sdr = Assert.IsType<SdrRecord>(records[1]);
            Assert.Equal(1, sdr.HEAD_NUM);
            Assert.Equal(1, sdr.SITE_GROUP);
            Assert.Equal(3, sdr.SITE_CNT);
            Assert.Equal(new byte[] { 1, 2, 4 }, sdr.SITE_NUM);
            Assert.Equal("HT", sdr.HAND_TYP?.Text);
            Assert.Equal("H1", sdr.HAND_ID?.Text);
            Assert.Equal("CT", sdr.CARD_TYP?.Text);
            Assert.Equal("C1", sdr.CARD_ID?.Text);
            // Remaining optional fields absent
            Assert.Null(sdr.LOAD_TYP);
            Assert.Null(sdr.LOAD_ID);
            Assert.Null(sdr.DIB_TYP);
            Assert.Null(sdr.EXTRA_ID);
        }

        [Fact]
        public void ParsesSdrWithoutAnyOptionalFields()
        {
            // SDR (1,80): HEAD_NUM=2, SITE_GROUP=1, SITE_CNT=2, SITE_NUM=[1,2]
            var body = new List<byte>();
            StdfTestBytes.U1(body, 2);
            StdfTestBytes.U1(body, 1);
            StdfTestBytes.U1(body, 2);
            StdfTestBytes.U1(body, 1);
            StdfTestBytes.U1(body, 2);
            byte[] file = StdfTestBytes.FarPlusRecord(1, 80, body.ToArray());

            var parser = new StdfParser();
            var records = parser.ParseAll(new MemoryStream(file));

            Assert.Equal(2, records.Count);
            var sdr = Assert.IsType<SdrRecord>(records[1]);
            Assert.Equal(2, sdr.HEAD_NUM);
            Assert.Equal(2, sdr.SITE_CNT);
            Assert.Equal(new byte[] { 1, 2 }, sdr.SITE_NUM);
            Assert.Null(sdr.HAND_TYP);
            Assert.Null(sdr.HAND_ID);
            Assert.Null(sdr.EXTRA_ID);
        }

        [Fact]
        public void ParsesSdrWithSingleSite()
        {
            // SDR with SITE_CNT=1
            var body = new List<byte>();
            StdfTestBytes.U1(body, 1);
            StdfTestBytes.U1(body, 0);
            StdfTestBytes.U1(body, 1);
            StdfTestBytes.U1(body, 255);
            byte[] file = StdfTestBytes.FarPlusRecord(1, 80, body.ToArray());

            var parser = new StdfParser();
            var records = parser.ParseAll(new MemoryStream(file));

            var sdr = Assert.IsType<SdrRecord>(records[1]);
            Assert.Equal(1, sdr.SITE_CNT);
            Assert.Single(sdr.SITE_NUM);
            Assert.Equal(255, sdr.SITE_NUM[0]);
        }
    }
}
