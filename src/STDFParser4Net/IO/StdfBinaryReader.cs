using System;
using System.Buffers.Binary;
using System.IO;
using STDFParser4Net.Exceptions;

namespace STDFParser4Net.IO
{
    /// <summary>
    /// Endianness-aware binary reader for STDF V4. Reads primitive STDF data types,
    /// tracks the current record boundary so optional fields can be skipped safely,
    /// and decodes Cn/C1 strings through an injectable <see cref="StdfStringEncoding"/>.
    /// </summary>
    public sealed class StdfBinaryReader
    {
        private readonly Stream _stream;
        private readonly byte[] _buf2 = new byte[2];
        private readonly byte[] _buf4 = new byte[4];
        private readonly byte[] _buf8 = new byte[8];

        private Endianness _endianness;
        private readonly StdfStringEncoding _encoding;
        private readonly RecLenMode _recLenMode;

        private long _recordStart;
        private long _recordLen;

        public StdfBinaryReader(Stream stream, Endianness endianness, StdfStringEncoding encoding, RecLenMode recLenMode)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _endianness = endianness;
            _encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
            _recLenMode = recLenMode;
            _recordStart = 0;
            _recordLen = 0;
        }

        public long Position => _stream.Position;
        public long Length => _stream.Length;
        public Endianness Endianness => _endianness;
        public StdfStringEncoding Encoding => _encoding;
        public RecLenMode RecLenMode => _recLenMode;

        public void SetEndianness(Endianness endianness) => _endianness = endianness;

        /// <summary>Bytes remaining in the current record body.</summary>
        public long RemainingInRecord => _recordStart + _recordLen - _stream.Position;

        /// <summary>True if at least <paramref name="n"/> body bytes remain in the current record.</summary>
        public bool HasRemaining(long n) => RemainingInRecord >= n;

        /// <summary>Mark the start/length of a record body so boundary tracking works.</summary>
        public void BeginRecord(in StdfRecordHeader header)
        {
            _recordStart = header.BodyStart;
            _recordLen = header.BodyLength;
        }

        /// <summary>Advance past any unread body bytes of the current record.</summary>
        public void SkipRestOfRecord()
        {
            long rem = RemainingInRecord;
            if (rem > 0 && _stream.CanSeek)
                _stream.Seek(rem, SeekOrigin.Current);
            else if (rem > 0)
                ReadExact(new byte[rem], 0, (int)rem);
        }

        // ---- raw byte reading ----

        private void ReadExact(byte[] buffer, int offset, int count)
        {
            int total = 0;
            while (total < count)
            {
                int n = _stream.Read(buffer, offset + total, count - total);
                if (n == 0)
                    throw new UnexpectedEndOfStreamException(
                        $"Expected {count} bytes, only {total} available at position {Position}.");
                total += n;
            }
        }

        public byte[] ReadBytes(int count)
        {
            var data = new byte[count];
            if (count > 0) ReadExact(data, 0, count);
            return data;
        }

        // ---- fixed-size integer primitives ----

        public byte ReadU1()
        {
            int b = _stream.ReadByte();
            if (b < 0) throw new UnexpectedEndOfStreamException("Unexpected end of stream reading U1.");
            return (byte)b;
        }

        public ushort ReadU2()
        {
            ReadExact(_buf2, 0, 2);
            return _endianness == Endianness.LittleEndian
                ? BinaryPrimitives.ReadUInt16LittleEndian(_buf2)
                : BinaryPrimitives.ReadUInt16BigEndian(_buf2);
        }

        public uint ReadU4()
        {
            ReadExact(_buf4, 0, 4);
            return _endianness == Endianness.LittleEndian
                ? BinaryPrimitives.ReadUInt32LittleEndian(_buf4)
                : BinaryPrimitives.ReadUInt32BigEndian(_buf4);
        }

        public sbyte ReadI1()
        {
            return (sbyte)ReadU1();
        }

        public short ReadI2()
        {
            return (short)ReadU2();
        }

        public int ReadI4()
        {
            return (int)ReadU4();
        }

        // ---- floating point ----

        public float ReadR4()
        {
            int v = ReadI4();
            return BitConverter.Int32BitsToSingle(v);
        }

        public double ReadR8()
        {
            ReadExact(_buf8, 0, 8);
            long v = _endianness == Endianness.LittleEndian
                ? BinaryPrimitives.ReadInt64LittleEndian(_buf8)
                : BinaryPrimitives.ReadInt64BigEndian(_buf8);
            return BitConverter.Int64BitsToDouble(v);
        }

        // ---- single-byte char ----

        public char ReadC1()
        {
            return (char)ReadU1();
        }

        // ---- variable-length strings ----

        /// <summary>Read a Cn string: 1-byte length prefix (U1) + n bytes.</summary>
        public StdfString ReadCn()
        {
            int len = ReadU1();
            byte[] raw = len > 0 ? ReadBytes(len) : Array.Empty<byte>();
            return _encoding.Decode(raw);
        }

        /// <summary>
        /// Read a Cnx string (2-byte length prefix, an equipment dialect). Provided so
        /// dialect readers can opt-in; the standard type is <see cref="ReadCn"/>.
        /// </summary>
        public StdfString ReadCnx()
        {
            int len = ReadU2();
            byte[] raw = len > 0 ? ReadBytes(len) : Array.Empty<byte>();
            return _encoding.Decode(raw);
        }

        // ---- byte arrays ----

        /// <summary>Read a Bn byte string: 1-byte length prefix (U1, per V4 spec) + n bytes.</summary>
        public byte[] ReadBn()
        {
            int len = ReadU1();
            return ReadBytes(len);
        }

        // ---- bit string (Dn) ----

        /// <summary>
        /// Read a Dn bit string: 2-byte bit-count prefix (U2) + ceil(bits/8) bytes.
        /// Returns the bit count alongside the packed bytes.
        /// </summary>
        public BitString ReadDn()
        {
            int numBits = ReadU2();
            int numBytes = (numBits + 7) / 8;
            byte[] data = numBytes > 0 ? ReadBytes(numBytes) : Array.Empty<byte>();
            return new BitString(numBits, data);
        }

        // ---- nibble arrays (N1) ----

        /// <summary>
        /// Read <paramref name="count"/> N1 nibbles. N1 packs two values per byte
        /// (low nibble first). Returns one byte (0..15) per nibble.
        /// </summary>
        public byte[] ReadN1Array(int count)
        {
            if (count <= 0) return Array.Empty<byte>();
            int numBytes = (count + 1) / 2;
            byte[] packed = ReadBytes(numBytes);
            var result = new byte[count];
            for (int i = 0; i < count; i++)
            {
                byte b = packed[i / 2];
                result[i] = (i % 2 == 0) ? (byte)(b & 0x0F) : (byte)((b >> 4) & 0x0F);
            }
            return result;
        }

        /// <summary>Read a single N1 nibble at the given nibble offset within a freshly read byte pair.</summary>
        public byte ReadN1()
        {
            // N1 is normally array-typed; this convenience reads one byte and returns the low nibble.
            byte b = ReadU1();
            return (byte)(b & 0x0F);
        }

        // ---- kxTYPE arrays ----

        /// <summary>Read a fixed-length array of k elements using <paramref name="readElement"/>.</summary>
        public T[] ReadArray<T>(int count, Func<T> readElement)
        {
            if (readElement == null) throw new ArgumentNullException(nameof(readElement));
            var arr = new T[count];
            for (int i = 0; i < count; i++)
                arr[i] = readElement();
            return arr;
        }

        // ---- record header ----

        /// <summary>
        /// Read the 4-byte record header (REC_LEN U2, REC_TYP U1, REC_SUB U1) and compute
        /// the body length according to <see cref="RecLenMode"/>.
        /// </summary>
        public StdfRecordHeader ReadRecordHeader(RecLenMode mode)
        {
            long headerStart = Position;
            ushort recLen = ReadU2();
            byte typ = ReadU1();
            byte sub = ReadU1();
            long bodyStart = Position;

            long bodyLen;
            switch (mode)
            {
                case RecLenMode.BodyOnly: bodyLen = recLen; break;
                case RecLenMode.BodyPlusTypSub: bodyLen = recLen - 2; break;
                case RecLenMode.FullHeader: bodyLen = recLen - 4; break;
                default: bodyLen = recLen; break;
            }
            if (bodyLen < 0) bodyLen = 0;
            return new StdfRecordHeader(recLen, typ, sub, headerStart, bodyStart, bodyLen);
        }
    }
}
