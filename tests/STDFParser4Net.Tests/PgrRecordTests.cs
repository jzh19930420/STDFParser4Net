using System.IO;
using STDFParser4Net;
using STDFParser4Net.Records;
using Xunit;

namespace STDFParser4Net.Tests
{
    public class PgrRecordTests
    {
        // FAR(LE) + PGR: GRP_INDX=1, GRP_NAM="G1", INDX_CNT=3, PMR_INDX={10,20,30}
        // Body = 2 + (1+2) + 2 + 3*2 = 13
        private static readonly byte[] File =
        {
            0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,                       // FAR(LE)
            0x0D, 0x00, 0x01, 0x3E,                                   // PGR: REC_LEN=13, TYP=1, SUB=62
            0x01, 0x00,                                               // GRP_INDX = 1
            0x02, 0x47, 0x31,                                         // GRP_NAM = "G1"
            0x03, 0x00,                                               // INDX_CNT = 3
            0x0A, 0x00,                                               // PMR_INDX[0] = 10
            0x14, 0x00,                                               // PMR_INDX[1] = 20
            0x1E, 0x00                                                // PMR_INDX[2] = 30
        };

        [Fact]
        public void ParsesPgrFields()
        {
            var records = new StdfParser().ParseAll(new MemoryStream(File));

            Assert.Equal(2, records.Count);
            var pgr = Assert.IsType<PgrRecord>(records[1]);
            Assert.Equal(RecordType.PGR, pgr.RecordType);
            Assert.Equal((ushort)1, pgr.GRP_INDX);
            Assert.Equal("G1", pgr.GRP_NAM.Text);
            Assert.Equal((ushort)3, pgr.INDX_CNT);
            Assert.Equal(3, pgr.PMR_INDX.Length);
            Assert.Equal((ushort)10, pgr.PMR_INDX[0]);
            Assert.Equal((ushort)20, pgr.PMR_INDX[1]);
            Assert.Equal((ushort)30, pgr.PMR_INDX[2]);
        }

        // PGR with empty group (INDX_CNT=0).
        // Body = 2 + (1+2) + 2 + 0 = 7
        private static readonly byte[] EmptyFile =
        {
            0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,                       // FAR(LE)
            0x07, 0x00, 0x01, 0x3E,                                   // PGR: REC_LEN=7
            0x05, 0x00,                                               // GRP_INDX = 5
            0x02, 0x47, 0x32,                                         // GRP_NAM = "G2"
            0x00, 0x00                                                // INDX_CNT = 0
        };

        [Fact]
        public void ParsesPgrEmptyGroup()
        {
            var records = new StdfParser().ParseAll(new MemoryStream(EmptyFile));

            var pgr = Assert.IsType<PgrRecord>(records[1]);
            Assert.Equal((ushort)5, pgr.GRP_INDX);
            Assert.Equal("G2", pgr.GRP_NAM.Text);
            Assert.Equal((ushort)0, pgr.INDX_CNT);
            Assert.Empty(pgr.PMR_INDX);
        }
    }
}
