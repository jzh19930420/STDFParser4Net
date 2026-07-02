using System.IO;
using STDFParser4Net;
using STDFParser4Net.Records;
using Xunit;

namespace STDFParser4Net.Tests
{
    public class DtrRecordTests
    {
        // FAR(LE) + DTR: TEXT_DAT="Hello, STDF!"
        // Body = 1+12 = 13
        private static readonly byte[] File =
        {
            0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,                       // FAR(LE)
            0x0D, 0x00, 0x1E, 0x0A,                                   // DTR header: REC_LEN=13, TYP=30, SUB=10
            0x0C, 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x2C, 0x20,          // TEXT_DAT = "Hello, STDF!" (len=12)
            0x53, 0x54, 0x44, 0x46, 0x21
        };

        [Fact]
        public void ParsesDtrText()
        {
            var records = new StdfParser().ParseAll(new MemoryStream(File));

            Assert.Equal(2, records.Count);
            var dtr = Assert.IsType<DtrRecord>(records[1]);
            Assert.Equal(RecordType.DTR, dtr.RecordType);
            Assert.Equal("Hello, STDF!", dtr.TextDat.Text);
            Assert.Equal("Hello, STDF!", (string)dtr.TextDat);
        }

        [Fact]
        public void ParsesDtrEmptyString()
        {
            // DTR with zero-length text
            byte[] file =
            {
                0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,
                0x01, 0x00, 0x1E, 0x0A,               // REC_LEN=1
                0x00                                  // length prefix = 0 → empty string
            };
            var records = new StdfParser().ParseAll(new MemoryStream(file));

            var dtr = Assert.IsType<DtrRecord>(records[1]);
            Assert.Equal(string.Empty, dtr.TextDat.Text);
        }
    }
}
