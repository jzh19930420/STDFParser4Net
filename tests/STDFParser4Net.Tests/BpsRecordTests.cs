using System.IO;
using STDFParser4Net;
using STDFParser4Net.Records;
using Xunit;

namespace STDFParser4Net.Tests
{
    public class BpsRecordTests
    {
        // FAR(LE) + BPS: SEQ_NAME="TEST_SEC"
        // Body = 1+8 = 9
        private static readonly byte[] File =
        {
            0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,                       // FAR(LE)
            0x09, 0x00, 0x14, 0x0A,                                   // BPS header: REC_LEN=9, TYP=20, SUB=10
            0x08, 0x54, 0x45, 0x53, 0x54, 0x5F, 0x53, 0x45, 0x43    // SEQ_NAME = "TEST_SEC"
        };

        [Fact]
        public void ParsesBpsSeqName()
        {
            var records = new StdfParser().ParseAll(new MemoryStream(File));

            Assert.Equal(2, records.Count);
            var bps = Assert.IsType<BpsRecord>(records[1]);
            Assert.Equal(RecordType.BPS, bps.RecordType);
            Assert.Equal("TEST_SEC", bps.SeqName.Text);
            Assert.Equal("TEST_SEC", (string)bps.SeqName);
        }
    }
}
