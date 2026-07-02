using System.IO;
using STDFParser4Net;
using STDFParser4Net.Records;
using Xunit;

namespace STDFParser4Net.Tests
{
    public class PmrRecordTests
    {
        // FAR(LE) + PMR with all 3 optional Cn fields.
        // Body = 2+2 + (1+3) + (1+2) + (1+2) = 14
        private static readonly byte[] FullFile =
        {
            0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,                       // FAR(LE)
            0x0E, 0x00, 0x01, 0x3C,                                   // PMR: REC_LEN=14, TYP=1, SUB=60
            0x01, 0x00,                                               // PMR_INDX = 1
            0x00, 0x00,                                               // CHAN_TYP = 0
            0x03, 0x43, 0x68, 0x31,                                   // CHAN_NAM = "Ch1"
            0x02, 0x50, 0x31,                                         // PHY_NAM = "P1"
            0x02, 0x4C, 0x31                                          // LOG_NAM = "L1"
        };

        [Fact]
        public void ParsesPmrAllFields()
        {
            var records = new StdfParser().ParseAll(new MemoryStream(FullFile));

            Assert.Equal(2, records.Count);
            var pmr = Assert.IsType<PmrRecord>(records[1]);
            Assert.Equal(RecordType.PMR, pmr.RecordType);
            Assert.Equal((ushort)1, pmr.PMR_INDX);
            Assert.Equal((ushort)0, pmr.CHAN_TYP);
            Assert.Equal("Ch1", pmr.CHAN_NAM!.Value.Text);
            Assert.Equal("P1", pmr.PHY_NAM!.Value.Text);
            Assert.Equal("L1", pmr.LOG_NAM!.Value.Text);
        }

        // Minimal PMR: only the 2 fixed U2 fields, no optional Cn.
        // Body = 2+2 = 4
        private static readonly byte[] MinimalFile =
        {
            0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,                       // FAR(LE)
            0x04, 0x00, 0x01, 0x3C,                                   // PMR: REC_LEN=4
            0x02, 0x00,                                               // PMR_INDX = 2
            0x01, 0x00                                                // CHAN_TYP = 1
        };

        [Fact]
        public void ParsesPmrMinimalNoOptional()
        {
            var records = new StdfParser().ParseAll(new MemoryStream(MinimalFile));

            var pmr = Assert.IsType<PmrRecord>(records[1]);
            Assert.Equal((ushort)2, pmr.PMR_INDX);
            Assert.Equal((ushort)1, pmr.CHAN_TYP);
            Assert.Null(pmr.CHAN_NAM);
            Assert.Null(pmr.PHY_NAM);
            Assert.Null(pmr.LOG_NAM);
        }
    }
}
