using System.IO;
using STDFParser4Net;
using STDFParser4Net.Records;
using Xunit;

namespace STDFParser4Net.Tests
{
    public class PcrRecordTests
    {
        // FAR(LE) + PCR: HEAD=1, SITE=1, PART_CNT=100, RTST_CNT=0, ABRT_CNT=0, GOOD_CNT=95, FUNC_CNT=0
        // Body = 1+1+4+4+4+4+4 = 22 bytes  (REC_LEN=22=0x16)
        // Note: the task brief's example showed REC_LEN=14 which is a typo; the 7 fields
        // total 22 bytes. We use the correct value here.
        private static readonly byte[] File =
        {
            0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,                       // FAR(LE)
            0x16, 0x00, 0x01, 0x1E,                                   // PCR header: REC_LEN=22, TYP=1, SUB=30
            0x01,                                                     // HEAD_NUM = 1
            0x01,                                                     // SITE_NUM = 1
            0x64, 0x00, 0x00, 0x00,                                   // PART_CNT = 100
            0x00, 0x00, 0x00, 0x00,                                   // RTST_CNT = 0
            0x00, 0x00, 0x00, 0x00,                                   // ABRT_CNT = 0
            0x5F, 0x00, 0x00, 0x00,                                   // GOOD_CNT = 95
            0x00, 0x00, 0x00, 0x00                                    // FUNC_CNT = 0
        };

        [Fact]
        public void ParsesPcrFields()
        {
            var records = new StdfParser().ParseAll(new MemoryStream(File));

            Assert.Equal(2, records.Count);
            var pcr = Assert.IsType<PcrRecord>(records[1]);
            Assert.Equal(RecordType.PCR, pcr.RecordType);
            Assert.Equal((byte)1, pcr.HeadNum);
            Assert.Equal((byte)1, pcr.SiteNum);
            Assert.Equal(100u, pcr.PartCnt);
            Assert.Equal(0u, pcr.RtstCnt);
            Assert.Equal(0u, pcr.AbrtCnt);
            Assert.Equal(95u, pcr.GoodCnt);
            Assert.Equal(0u, pcr.FuncCnt);
        }

        [Fact]
        public void ParsesPcrLargeCounts()
        {
            // HEAD=255, SITE=255, PART_CNT=uint.MaxValue, GOOD_CNT=1
            byte[] file =
            {
                0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,
                0x16, 0x00, 0x01, 0x1E,
                0xFF,                                            // HEAD_NUM = 255
                0xFF,                                            // SITE_NUM = 255
                0xFF, 0xFF, 0xFF, 0xFF,                          // PART_CNT = uint.MaxValue
                0x00, 0x00, 0x00, 0x00,                          // RTST_CNT = 0
                0x00, 0x00, 0x00, 0x00,                          // ABRT_CNT = 0
                0x01, 0x00, 0x00, 0x00,                          // GOOD_CNT = 1
                0x00, 0x00, 0x00, 0x00                           // FUNC_CNT = 0
            };
            var records = new StdfParser().ParseAll(new MemoryStream(file));

            var pcr = Assert.IsType<PcrRecord>(records[1]);
            Assert.Equal((byte)255, pcr.HeadNum);
            Assert.Equal(uint.MaxValue, pcr.PartCnt);
            Assert.Equal(1u, pcr.GoodCnt);
        }
    }
}
