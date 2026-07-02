using System.IO;
using STDFParser4Net;
using STDFParser4Net.Records;
using Xunit;

namespace STDFParser4Net.Tests
{
    public class SbrRecordTests
    {
        // FAR(LE) + SBR: HEAD=1, SITE=1, SBIN_NUM=3, SBIN_CNT=7, SBIN_PF='F', SBIN_NAM="FAIL"
        // Body = 1+1+2+4+1 + (1+4) = 9 + 5 = 14
        private static readonly byte[] File =
        {
            0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,                       // FAR(LE)
            0x0E, 0x00, 0x01, 0x32,                                   // SBR header: REC_LEN=14, TYP=1, SUB=50
            0x01,                                                     // HEAD_NUM = 1
            0x01,                                                     // SITE_NUM = 1
            0x03, 0x00,                                               // SBIN_NUM = 3
            0x07, 0x00, 0x00, 0x00,                                   // SBIN_CNT = 7
            0x46,                                                     // SBIN_PF = 'F'
            0x04, 0x46, 0x41, 0x49, 0x4C                             // SBIN_NAM = "FAIL"
        };

        [Fact]
        public void ParsesSbrAllFields()
        {
            var records = new StdfParser().ParseAll(new MemoryStream(File));

            Assert.Equal(2, records.Count);
            var sbr = Assert.IsType<SbrRecord>(records[1]);
            Assert.Equal(RecordType.SBR, sbr.RecordType);
            Assert.Equal((byte)1, sbr.HeadNum);
            Assert.Equal((byte)1, sbr.SiteNum);
            Assert.Equal((ushort)3, sbr.SbinNum);
            Assert.Equal(7u, sbr.SbinCnt);
            Assert.Equal('F', sbr.SbinPf);
            Assert.Equal("FAIL", sbr.SbinNam.Text);
        }

        [Fact]
        public void ParsesSbrWithoutOptionalName()
        {
            byte[] file =
            {
                0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,
                0x09, 0x00, 0x01, 0x32,               // REC_LEN=9
                0x02,                                 // HEAD_NUM
                0x04,                                 // SITE_NUM
                0x01, 0x00,                           // SBIN_NUM = 1
                0x64, 0x00, 0x00, 0x00,               // SBIN_CNT = 100
                0x50                                  // SBIN_PF = 'P'
            };
            var records = new StdfParser().ParseAll(new MemoryStream(file));

            var sbr = Assert.IsType<SbrRecord>(records[1]);
            Assert.Equal((byte)2, sbr.HeadNum);
            Assert.Equal((byte)4, sbr.SiteNum);
            Assert.Equal((ushort)1, sbr.SbinNum);
            Assert.Equal(100u, sbr.SbinCnt);
            Assert.Equal('P', sbr.SbinPf);
            Assert.Empty(sbr.SbinNam.Text);
        }
    }
}
