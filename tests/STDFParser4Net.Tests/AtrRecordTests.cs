using System.IO;
using STDFParser4Net;
using STDFParser4Net.Records;
using Xunit;

namespace STDFParser4Net.Tests
{
    public class AtrRecordTests
    {
        // FAR(LE) + ATR: MOD_TIM=1000000, CMD_LINE="START"
        // ATR body: 0x40,0x42,0x0F,0x00 (U4 LE) + 0x05 + "START"
        private static readonly byte[] File =
        {
            0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,                       // FAR(LE)
            0x0A, 0x00, 0x00, 0x14,                                   // ATR header: REC_LEN=10, TYP=0, SUB=20
            0x40, 0x42, 0x0F, 0x00,                                   // MOD_TIM = 1000000
            0x05, 0x53, 0x54, 0x41, 0x52, 0x54                        // CMD_LINE = "START"
        };

        [Fact]
        public void ParsesAtrFields()
        {
            var records = new StdfParser().ParseAll(new MemoryStream(File));

            Assert.Equal(2, records.Count);
            var atr = Assert.IsType<AtrRecord>(records[1]);
            Assert.Equal(RecordType.ATR, atr.RecordType);
            Assert.Equal(1000000u, atr.ModTim);
            Assert.Equal("START", atr.CmdLine.Text);
            Assert.Equal("START", (string)atr.CmdLine);
        }

        [Fact]
        public void ParsesAtrWithoutOptionalCmdLine()
        {
            // ATR with only MOD_TIM (no CMD_LINE)
            byte[] file =
            {
                0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,
                0x04, 0x00, 0x00, 0x14,            // REC_LEN=4
                0x40, 0x42, 0x0F, 0x00             // MOD_TIM only
            };
            var records = new StdfParser().ParseAll(new MemoryStream(file));

            var atr = Assert.IsType<AtrRecord>(records[1]);
            Assert.Equal(1000000u, atr.ModTim);
            Assert.Empty(atr.CmdLine.Text);
        }
    }
}
