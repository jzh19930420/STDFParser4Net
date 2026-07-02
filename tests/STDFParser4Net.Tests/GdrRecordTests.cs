using System.IO;
using STDFParser4Net;
using STDFParser4Net.Records;
using Xunit;

namespace STDFParser4Net.Tests
{
    public class GdrRecordTests
    {
        // FAR(LE) + GDR with 5 fields: U1, R4, Cn, Bn, Dn.
        // Body = 1+2 + (1+1) + (1+4) + (1+1+2) + (1+1+2) + (1+2+1) = 3+2+5+4+4+4 = 22
        private static readonly byte[] MixedTypesFile =
        {
            0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,                       // FAR(LE)
            0x16, 0x00, 0x32, 0x0A,                                   // GDR: REC_LEN=22, TYP=50, SUB=10
            0x01,                                                     // GEN_FLG = 0x01
            0x05, 0x00,                                               // FLD_CNT = 5
            0x00, 0x2A,                                               // Field 0: U1 = 42
            0x06, 0x00, 0x00, 0x40, 0x40,                             // Field 1: R4 = 3.0f
            0x0A, 0x02, 0x48, 0x69,                                   // Field 2: Cn = "Hi"
            0x0B, 0x02, 0xAA, 0xBB,                                   // Field 3: Bn = {0xAA, 0xBB}
            0x0C, 0x08, 0x00, 0xFF                                    // Field 4: Dn = 8 bits, 0xFF
        };

        [Fact]
        public void ParsesGdrMixedTypes()
        {
            var records = new StdfParser().ParseAll(new MemoryStream(MixedTypesFile));

            Assert.Equal(2, records.Count);
            var gdr = Assert.IsType<GdrRecord>(records[1]);
            Assert.Equal(RecordType.GDR, gdr.RecordType);
            Assert.Equal(0x01, gdr.GEN_FLG);
            Assert.Equal((ushort)5, gdr.FLD_CNT);
            Assert.Equal(5, gdr.FIELDS.Length);

            // Field 0: U1 = 42
            Assert.Equal(0, gdr.FIELDS[0].TypeCode);
            Assert.Equal((byte)42, gdr.FIELDS[0].Value);

            // Field 1: R4 = 3.0f
            Assert.Equal(6, gdr.FIELDS[1].TypeCode);
            Assert.Equal(3.0f, gdr.FIELDS[1].Value);

            // Field 2: Cn = "Hi"
            Assert.Equal(10, gdr.FIELDS[2].TypeCode);
            var cn = Assert.IsType<StdfString>(gdr.FIELDS[2].Value);
            Assert.Equal("Hi", cn.Text);

            // Field 3: Bn = {0xAA, 0xBB}
            Assert.Equal(11, gdr.FIELDS[3].TypeCode);
            var bn = Assert.IsType<byte[]>(gdr.FIELDS[3].Value);
            Assert.Equal(new byte[] { 0xAA, 0xBB }, bn);

            // Field 4: Dn = 8 bits, 0xFF
            Assert.Equal(12, gdr.FIELDS[4].TypeCode);
            var dn = Assert.IsType<BitString>(gdr.FIELDS[4].Value);
            Assert.Equal(8, dn.BitCount);
            Assert.Equal(0xFF, dn.Bytes[0]);
            Assert.True(dn.GetBit(0));
            Assert.True(dn.GetBit(7));
        }

        // GDR with N1, B1 and I1 fields.
        // Body = 1+2 + (1+1) + (1+1) + (1+1) = 3+2+2+2 = 9
        private static readonly byte[] NibbleAndByteFile =
        {
            0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,                       // FAR(LE)
            0x09, 0x00, 0x32, 0x0A,                                   // GDR: REC_LEN=9
            0x00,                                                     // GEN_FLG = 0x00
            0x03, 0x00,                                               // FLD_CNT = 3
            0x0D, 0x5F,                                               // Field 0: N1 = low nibble of 0x5F = 0x0F
            0x08, 0xAB,                                               // Field 1: B1 = 0xAB
            0x03, 0xFF                                                // Field 2: I1 = -1
        };

        [Fact]
        public void ParsesGdrNibbleAndByteTypes()
        {
            var records = new StdfParser().ParseAll(new MemoryStream(NibbleAndByteFile));

            var gdr = Assert.IsType<GdrRecord>(records[1]);
            Assert.Equal(3, gdr.FIELDS.Length);

            // Field 0: N1 = low nibble of 0x5F = 0x0F
            Assert.Equal(13, gdr.FIELDS[0].TypeCode);
            Assert.Equal((byte)0x0F, gdr.FIELDS[0].Value);

            // Field 1: B1 = 0xAB
            Assert.Equal(8, gdr.FIELDS[1].TypeCode);
            Assert.Equal((byte)0xAB, gdr.FIELDS[1].Value);

            // Field 2: I1 = -1
            Assert.Equal(3, gdr.FIELDS[2].TypeCode);
            Assert.Equal((sbyte)-1, gdr.FIELDS[2].Value);
        }

        // GDR with zero fields.
        // Body = 1+2 = 3
        private static readonly byte[] EmptyFile =
        {
            0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,                       // FAR(LE)
            0x03, 0x00, 0x32, 0x0A,                                   // GDR: REC_LEN=3
            0x00,                                                     // GEN_FLG = 0x00
            0x00, 0x00                                                // FLD_CNT = 0
        };

        [Fact]
        public void ParsesGdrEmpty()
        {
            var records = new StdfParser().ParseAll(new MemoryStream(EmptyFile));

            var gdr = Assert.IsType<GdrRecord>(records[1]);
            Assert.Equal((ushort)0, gdr.FLD_CNT);
            Assert.Empty(gdr.FIELDS);
        }
    }
}
