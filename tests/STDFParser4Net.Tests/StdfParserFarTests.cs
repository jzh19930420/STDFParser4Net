using System.IO;
using System.Linq;
using System.Text;
using STDFParser4Net;
using STDFParser4Net.Exceptions;
using STDFParser4Net.Records;
using Xunit;

namespace STDFParser4Net.Tests
{
    public class StdfParserFarTests
    {
        // FAR(LE): REC_LEN=2, TYP=0, SUB=10, CPU_TYPE=1(LE), STDF_VER=4
        // Unknown: REC_LEN=1, TYP=99, SUB=99, body=0x42
        private static readonly byte[] LeFile =
        {
            0x02, 0x00, 0x00, 0x0A, 0x01, 0x04,
            0x01, 0x00, 0x63, 0x63, 0x42
        };

        // FAR(BE): REC_LEN=2 (00 02), TYP=0, SUB=10, CPU_TYPE=0(BE), STDF_VER=4
        private static readonly byte[] BeFile =
        {
            0x00, 0x02, 0x00, 0x0A, 0x00, 0x04
        };

        [Fact]
        public void ParsesLittleEndianFarAndUnknownRecord()
        {
            var parser = new StdfParser();
            var records = parser.ParseAll(new MemoryStream(LeFile));

            Assert.Equal(2, records.Count);

            var far = Assert.IsType<FarRecord>(records[0]);
            Assert.True(far.IsLittleEndian);
            Assert.Equal(4, far.StdfVer);

            var unk = Assert.IsType<UnknownRecord>(records[1]);
            Assert.Equal(99, unk.Header.RecordType);
            Assert.Equal(99, unk.Header.RecordSub);
            Assert.Single(unk.RawBody);
            Assert.Equal(0x42, unk.RawBody[0]);
        }

        [Fact]
        public void DetectsBigEndianFromFarRecLen()
        {
            var parser = new StdfParser();
            var records = parser.ParseAll(new MemoryStream(BeFile));

            var far = Assert.IsType<FarRecord>(records[0]);
            Assert.False(far.IsLittleEndian);
            Assert.Equal(4, far.StdfVer);
        }

        [Fact]
        public void ThrowsOnNonV4Version()
        {
            // FAR with STDF_VER=3
            byte[] file = { 0x02, 0x00, 0x00, 0x0A, 0x01, 0x03 };
            var parser = new StdfParser();
            Assert.Throws<UnsupportedStdfVersionException>(() =>
                parser.ParseAll(new MemoryStream(file)));
        }

        [Fact]
        public void ThrowsWhenFirstRecordIsNotFar()
        {
            // TYP=1 SUB=10 (MIR) instead of FAR
            byte[] file = { 0x02, 0x00, 0x01, 0x0A, 0x01, 0x04 };
            var parser = new StdfParser();
            Assert.Throws<MissingFarRecordException>(() =>
                parser.ParseAll(new MemoryStream(file)));
        }

        [Fact]
        public void StreamingYieldsRecordsLazily()
        {
            var parser = new StdfParser();
            var stream = new MemoryStream(LeFile);
            int count = 0;
            foreach (var r in parser.Parse(stream))
            {
                count++;
                Assert.NotNull(r);
            }
            Assert.Equal(2, count);
        }
    }
}
