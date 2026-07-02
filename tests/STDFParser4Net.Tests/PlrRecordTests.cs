using System.IO;
using STDFParser4Net;
using STDFParser4Net.Records;
using Xunit;

namespace STDFParser4Net.Tests
{
    public class PlrRecordTests
    {
        // FAR(LE) + PLR: GRP_CNT=2, all arrays with 2 entries.
        // Body = 2 + 2*2 + 2*2 + 2*1 + 2*(1+1)*4 = 2+4+4+2+16 = 28
        private static readonly byte[] File =
        {
            0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,                       // FAR(LE)
            0x1C, 0x00, 0x01, 0x3F,                                   // PLR: REC_LEN=28, TYP=1, SUB=63
            0x02, 0x00,                                               // GRP_CNT = 2
            0x01, 0x00,                                               // GRP_INDX[0] = 1
            0x02, 0x00,                                               // GRP_INDX[1] = 2
            0x00, 0x00,                                               // GRP_MODE[0] = 0
            0x01, 0x00,                                               // GRP_MODE[1] = 1
            0x00,                                                     // GRP_RADIX[0] = 0
            0x02,                                                     // GRP_RADIX[1] = 2
            0x01, 0x41,                                               // PGM_CHAR[0] = "A"
            0x01, 0x42,                                               // PGM_CHAR[1] = "B"
            0x01, 0x61,                                               // PGM_CHAL[0] = "a"
            0x01, 0x62,                                               // PGM_CHAL[1] = "b"
            0x01, 0x58,                                               // RSL_CHAR[0] = "X"
            0x01, 0x59,                                               // RSL_CHAR[1] = "Y"
            0x01, 0x78,                                               // RSL_CHAL[0] = "x"
            0x01, 0x79                                                // RSL_CHAL[1] = "y"
        };

        [Fact]
        public void ParsesPlrFields()
        {
            var records = new StdfParser().ParseAll(new MemoryStream(File));

            Assert.Equal(2, records.Count);
            var plr = Assert.IsType<PlrRecord>(records[1]);
            Assert.Equal(RecordType.PLR, plr.RecordType);
            Assert.Equal((ushort)2, plr.GRP_CNT);

            Assert.Equal((ushort)1, plr.GRP_INDX[0]);
            Assert.Equal((ushort)2, plr.GRP_INDX[1]);
            Assert.Equal((ushort)0, plr.GRP_MODE[0]);
            Assert.Equal((ushort)1, plr.GRP_MODE[1]);
            Assert.Equal((byte)0, plr.GRP_RADIX[0]);
            Assert.Equal((byte)2, plr.GRP_RADIX[1]);
            Assert.Equal("A", plr.PGM_CHAR[0].Text);
            Assert.Equal("B", plr.PGM_CHAR[1].Text);
            Assert.Equal("a", plr.PGM_CHAL[0].Text);
            Assert.Equal("b", plr.PGM_CHAL[1].Text);
            Assert.Equal("X", plr.RSL_CHAR[0].Text);
            Assert.Equal("Y", plr.RSL_CHAR[1].Text);
            Assert.Equal("x", plr.RSL_CHAL[0].Text);
            Assert.Equal("y", plr.RSL_CHAL[1].Text);
        }

        // PLR with GRP_CNT=0 → all arrays empty.
        // Body = 2
        private static readonly byte[] EmptyFile =
        {
            0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,                       // FAR(LE)
            0x02, 0x00, 0x01, 0x3F,                                   // PLR: REC_LEN=2
            0x00, 0x00                                                // GRP_CNT = 0
        };

        [Fact]
        public void ParsesPlrEmpty()
        {
            var records = new StdfParser().ParseAll(new MemoryStream(EmptyFile));

            var plr = Assert.IsType<PlrRecord>(records[1]);
            Assert.Equal((ushort)0, plr.GRP_CNT);
            Assert.Empty(plr.GRP_INDX);
            Assert.Empty(plr.GRP_MODE);
            Assert.Empty(plr.GRP_RADIX);
            Assert.Empty(plr.PGM_CHAR);
            Assert.Empty(plr.PGM_CHAL);
            Assert.Empty(plr.RSL_CHAR);
            Assert.Empty(plr.RSL_CHAL);
        }
    }
}
