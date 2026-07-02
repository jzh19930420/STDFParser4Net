using System.IO;
using STDFParser4Net;
using STDFParser4Net.Records;
using Xunit;

namespace STDFParser4Net.Tests
{
    public class MirRecordTests
    {
        // FAR(LE) + MIR with 8 fixed fields + LOT_ID("LOT123") + PART_TYP("DEVICE")
        // Fixed: SETUP_T=1000000, START_T=2000000, STAT_NUM=1, MODE_COD='P',
        //        RTST_COD=' ', PROT_COD=' ', BURN_TIM=0, CMOD_COD=' '
        // Body = 4+4+1+1+1+1+2+1 + (1+6)+(1+6) = 15 + 7 + 7 = 29 bytes
        private static readonly byte[] File =
        {
            0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,                       // FAR(LE)
            0x1D, 0x00, 0x01, 0x0A,                                   // MIR header: REC_LEN=29, TYP=1, SUB=10
            0x40, 0x42, 0x0F, 0x00,                                   // SETUP_T = 1000000
            0x80, 0x84, 0x1E, 0x00,                                   // START_T = 2000000
            0x01,                                                     // STAT_NUM = 1
            0x50,                                                     // MODE_COD = 'P'
            0x20,                                                     // RTST_COD = ' '
            0x20,                                                     // PROT_COD = ' '
            0x00, 0x00,                                               // BURN_TIM = 0
            0x20,                                                     // CMOD_COD = ' '
            0x06, 0x4C, 0x4F, 0x54, 0x31, 0x32, 0x33,               // LOT_ID = "LOT123"
            0x06, 0x44, 0x45, 0x56, 0x49, 0x43, 0x45                // PART_TYP = "DEVICE"
        };

        [Fact]
        public void ParsesMirFixedFieldsAndFirstOptionals()
        {
            var records = new StdfParser().ParseAll(new MemoryStream(File));

            Assert.Equal(2, records.Count);
            var mir = Assert.IsType<MirRecord>(records[1]);
            Assert.Equal(RecordType.MIR, mir.RecordType);

            Assert.Equal(1000000u, mir.SetupT);
            Assert.Equal(2000000u, mir.StartT);
            Assert.Equal((byte)1, mir.StatNum);
            Assert.Equal('P', mir.ModeCod);
            Assert.Equal(' ', mir.RtstCod);
            Assert.Equal(' ', mir.ProtCod);
            Assert.Equal((ushort)0, mir.BurnTim);
            Assert.Equal(' ', mir.CmodCod);

            Assert.Equal("LOT123", mir.LotId.Text);
            Assert.Equal("DEVICE", mir.PartTyp.Text);
        }

        [Fact]
        public void MissingOptionalMirFieldsAreDefault()
        {
            var records = new StdfParser().ParseAll(new MemoryStream(File));
            var mir = (MirRecord)records[1];

            // Fields after PART_TYP (index 1) were not in the stream
            Assert.Empty(mir.NodeNam.Text);
            Assert.Empty(mir.TstrTyp.Text);
            Assert.Empty(mir.JobNam.Text);
            Assert.Empty(mir.OperNam.Text);
            Assert.Empty(mir.SuprNam.Text);
            // Spot-check a few more
            Assert.Empty(mir.SpecNam.Text);
            Assert.Empty(mir.FlowId.Text);
        }

        [Fact]
        public void ParsesMirWithOnlyFixedFields()
        {
            // MIR with exactly the 8 fixed fields, no optional Cn fields
            byte[] file =
            {
                0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,
                0x0F, 0x00, 0x01, 0x0A,               // REC_LEN=15
                0x40, 0x42, 0x0F, 0x00,               // SETUP_T
                0x80, 0x84, 0x1E, 0x00,               // START_T
                0x01,                                 // STAT_NUM
                0x50,                                 // MODE_COD
                0x20,                                 // RTST_COD
                0x20,                                 // PROT_COD
                0x00, 0x00,                           // BURN_TIM
                0x20                                  // CMOD_COD
            };
            var records = new StdfParser().ParseAll(new MemoryStream(file));
            var mir = Assert.IsType<MirRecord>(records[1]);

            Assert.Equal(1000000u, mir.SetupT);
            Assert.Equal('P', mir.ModeCod);
            Assert.Empty(mir.LotId.Text);
            Assert.Empty(mir.SuprNam.Text);
        }
    }
}
