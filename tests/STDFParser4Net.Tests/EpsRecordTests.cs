using System.IO;
using STDFParser4Net;
using STDFParser4Net.Records;
using Xunit;

namespace STDFParser4Net.Tests
{
    public class EpsRecordTests
    {
        // FAR(LE) + EPS (empty body)
        private static readonly byte[] File =
        {
            0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,                       // FAR(LE)
            0x00, 0x00, 0x14, 0x14                                    // EPS header: REC_LEN=0, TYP=20, SUB=20
        };

        [Fact]
        public void ParsesEpsNoFields()
        {
            var records = new StdfParser().ParseAll(new MemoryStream(File));

            Assert.Equal(2, records.Count);
            var eps = Assert.IsType<EpsRecord>(records[1]);
            Assert.Equal(RecordType.EPS, eps.RecordType);
        }

        [Fact]
        public void ParsesBpsThenEpsInSequence()
        {
            // BPS followed immediately by EPS — confirms record-boundary handling
            byte[] file =
            {
                0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,
                0x03, 0x00, 0x14, 0x0A,               // BPS REC_LEN=3
                0x02, 0x41, 0x42,                     // SEQ_NAME = "AB"
                0x00, 0x00, 0x14, 0x14                // EPS REC_LEN=0
            };
            var records = new StdfParser().ParseAll(new MemoryStream(file));

            Assert.Equal(3, records.Count);
            var bps = Assert.IsType<BpsRecord>(records[1]);
            Assert.Equal("AB", bps.SeqName.Text);
            Assert.IsType<EpsRecord>(records[2]);
        }
    }
}
