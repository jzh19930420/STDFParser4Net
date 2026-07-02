using System.IO;
using STDFParser4Net;
using STDFParser4Net.Records;
using Xunit;

namespace STDFParser4Net.Tests
{
    public class MrrRecordTests
    {
        // FAR(LE) + MRR: FINISH_T=3000000, DISP_COD='D', USR_DESC="DONE", EXC_DESC="OK"
        // Body = 4 + 1 + (1+4) + (1+2) = 4+1+5+3 = 13
        private static readonly byte[] File =
        {
            0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,                       // FAR(LE)
            0x0D, 0x00, 0x01, 0x14,                                   // MRR header: REC_LEN=13, TYP=1, SUB=20
            0xC0, 0xC6, 0x2D, 0x00,                                   // FINISH_T = 3000000
            0x44,                                                     // DISP_COD = 'D'
            0x04, 0x44, 0x4F, 0x4E, 0x45,                            // USR_DESC = "DONE"
            0x02, 0x4F, 0x4B                                         // EXC_DESC = "OK"
        };

        [Fact]
        public void ParsesMrrAllFields()
        {
            var records = new StdfParser().ParseAll(new MemoryStream(File));

            Assert.Equal(2, records.Count);
            var mrr = Assert.IsType<MrrRecord>(records[1]);
            Assert.Equal(RecordType.MRR, mrr.RecordType);
            Assert.Equal(3000000u, mrr.FinishT);
            Assert.Equal('D', mrr.DispCod);
            Assert.Equal("DONE", mrr.UsrDesc.Text);
            Assert.Equal("OK", mrr.ExcDesc.Text);
        }

        [Fact]
        public void ParsesMrrOnlyFinishT()
        {
            // MRR with only FINISH_T (all optional fields missing)
            byte[] file =
            {
                0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,
                0x04, 0x00, 0x01, 0x14,               // REC_LEN=4
                0xC0, 0xC6, 0x2D, 0x00               // FINISH_T only
            };
            var records = new StdfParser().ParseAll(new MemoryStream(file));

            var mrr = Assert.IsType<MrrRecord>(records[1]);
            Assert.Equal(3000000u, mrr.FinishT);
            Assert.Equal(' ', mrr.DispCod);
            Assert.Empty(mrr.UsrDesc.Text);
            Assert.Empty(mrr.ExcDesc.Text);
        }
    }
}
