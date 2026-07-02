using System.IO;
using STDFParser4Net;
using STDFParser4Net.Records;
using Xunit;

namespace STDFParser4Net.Tests
{
    public class RdrRecordTests
    {
        // FAR(LE) + RDR: HEAD=1, SITE=1, NUM_BINS=3, RTST_BIN=[1,2,3]
        // Body = 1+1+2 + 3*2 = 4 + 6 = 10
        private static readonly byte[] File =
        {
            0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,                       // FAR(LE)
            0x0A, 0x00, 0x01, 0x46,                                   // RDR header: REC_LEN=10, TYP=1, SUB=70
            0x01,                                                     // HEAD_NUM = 1
            0x01,                                                     // SITE_NUM = 1
            0x03, 0x00,                                               // NUM_BINS = 3
            0x01, 0x00,                                               // RTST_BIN[0] = 1
            0x02, 0x00,                                               // RTST_BIN[1] = 2
            0x03, 0x00                                                // RTST_BIN[2] = 3
        };

        [Fact]
        public void ParsesRdrArrayFields()
        {
            var records = new StdfParser().ParseAll(new MemoryStream(File));

            Assert.Equal(2, records.Count);
            var rdr = Assert.IsType<RdrRecord>(records[1]);
            Assert.Equal(RecordType.RDR, rdr.RecordType);
            Assert.Equal((byte)1, rdr.HeadNum);
            Assert.Equal((byte)1, rdr.SiteNum);
            Assert.Equal((ushort)3, rdr.NumBins);
            Assert.Equal(3, rdr.RtstBin.Length);
            Assert.Equal(new ushort[] { 1, 2, 3 }, rdr.RtstBin);
        }

        [Fact]
        public void ParsesRdrEmptyBins()
        {
            // RDR with NUM_BINS=0 → empty array
            byte[] file =
            {
                0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,
                0x04, 0x00, 0x01, 0x46,               // REC_LEN=4
                0x01,                                 // HEAD_NUM
                0x01,                                 // SITE_NUM
                0x00, 0x00                            // NUM_BINS = 0
            };
            var records = new StdfParser().ParseAll(new MemoryStream(file));

            var rdr = Assert.IsType<RdrRecord>(records[1]);
            Assert.Equal((ushort)0, rdr.NumBins);
            Assert.Empty(rdr.RtstBin);
        }
    }
}
