using System.IO;
using STDFParser4Net;
using STDFParser4Net.Records;
using Xunit;

namespace STDFParser4Net.Tests
{
    public class HbrRecordTests
    {
        // FAR(LE) + HBR: HEAD=1, SITE=1, HBIN_NUM=1, HBIN_CNT=50, HBIN_PF='P', HBIN_NAM="PASS"
        // Body = 1+1+2+4+1 + (1+4) = 9 + 5 = 14
        private static readonly byte[] File =
        {
            0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,                       // FAR(LE)
            0x0E, 0x00, 0x01, 0x28,                                   // HBR header: REC_LEN=14, TYP=1, SUB=40
            0x01,                                                     // HEAD_NUM = 1
            0x01,                                                     // SITE_NUM = 1
            0x01, 0x00,                                               // HBIN_NUM = 1
            0x32, 0x00, 0x00, 0x00,                                   // HBIN_CNT = 50
            0x50,                                                     // HBIN_PF = 'P'
            0x04, 0x50, 0x41, 0x53, 0x53                             // HBIN_NAM = "PASS"
        };

        [Fact]
        public void ParsesHbrAllFields()
        {
            var records = new StdfParser().ParseAll(new MemoryStream(File));

            Assert.Equal(2, records.Count);
            var hbr = Assert.IsType<HbrRecord>(records[1]);
            Assert.Equal(RecordType.HBR, hbr.RecordType);
            Assert.Equal((byte)1, hbr.HeadNum);
            Assert.Equal((byte)1, hbr.SiteNum);
            Assert.Equal((ushort)1, hbr.HbinNum);
            Assert.Equal(50u, hbr.HbinCnt);
            Assert.Equal('P', hbr.HbinPf);
            Assert.Equal("PASS", hbr.HbinNam.Text);
        }

        [Fact]
        public void ParsesHbrWithoutOptionalName()
        {
            // HBR without HBIN_NAM
            byte[] file =
            {
                0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,
                0x09, 0x00, 0x01, 0x28,               // REC_LEN=9
                0x01,                                 // HEAD_NUM
                0x01,                                 // SITE_NUM
                0x0A, 0x00,                           // HBIN_NUM = 10
                0x05, 0x00, 0x00, 0x00,               // HBIN_CNT = 5
                0x46                                  // HBIN_PF = 'F'
            };
            var records = new StdfParser().ParseAll(new MemoryStream(file));

            var hbr = Assert.IsType<HbrRecord>(records[1]);
            Assert.Equal((ushort)10, hbr.HbinNum);
            Assert.Equal(5u, hbr.HbinCnt);
            Assert.Equal('F', hbr.HbinPf);
            Assert.Empty(hbr.HbinNam.Text);
        }
    }
}
