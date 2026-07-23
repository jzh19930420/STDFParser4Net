using System;
using System.Collections.Generic;
using System.IO;
using STDFParser4Net.Exceptions;
using STDFParser4Net.IO;
using STDFParser4Net.Records;
using STDFParser4Net.Records.Registry;

namespace STDFParser4Net
{
    /// <summary>
    /// Streaming STDF V4 parser. Iterate <see cref="Parse"/> to consume records one at a
    /// time (memory independent of file size), or call <see cref="ParseAll"/> for the full
    /// object graph. The first record must be FAR, whose CPU_TYPE determines byte order.
    /// </summary>
    public sealed class StdfParser
    {
        private readonly StdfParserOptions _options;

        public StdfParser() : this(new StdfParserOptions()) { }

        public StdfParser(StdfParserOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>Parse a file path, streaming records.</summary>
        public IEnumerable<StdfRecord> Parse(string path)
        {
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);
            foreach (var r in Parse(fs)) yield return r;
        }

        /// <summary>Parse a stream, streaming records. The stream must be readable.</summary>
        public IEnumerable<StdfRecord> Parse(Stream stream) => ParseCore(stream);

        /// <summary>Eagerly read all records into a list.</summary>
        public IReadOnlyList<StdfRecord> ParseAll(string path)
        {
            var list = new List<StdfRecord>();
            foreach (var r in Parse(path)) list.Add(r);
            return list;
        }

        public IReadOnlyList<StdfRecord> ParseAll(Stream stream)
        {
            var list = new List<StdfRecord>();
            foreach (var r in Parse(stream)) list.Add(r);
            return list;
        }

        private IEnumerable<StdfRecord> ParseCore(Stream stream)
        {
            var reader = new StdfBinaryReader(stream, Endianness.LittleEndian, _options.Encoding, _options.RecLenMode);

            // ---- FAR first: determines byte order ----
            FarRecord far;
            try
            {
                far = ReadFar(reader);
            }
            catch (UnexpectedEndOfStreamException)
            {
                yield break;
            }
            yield return far;

            // ---- remaining records ----
            while (reader.Position < reader.Length)
            {
                StdfRecordHeader header;
                try
                {
                    header = reader.ReadRecordHeader(_options.RecLenMode);
                }
                catch (UnexpectedEndOfStreamException)
                {
                    yield break;
                }

                // Cap body length to remaining file bytes so Unknown reads never invent
                // a huge length past EOF (corrupt REC_LEN near end of file).
                long maxBody = Math.Max(0, reader.Length - header.BodyStart);
                if (header.BodyLength > maxBody)
                {
                    header = new StdfRecordHeader(
                        header.RecordLength, header.RecordType, header.RecordSub,
                        header.HeaderStart, header.BodyStart, maxBody);
                }

                reader.BeginRecord(header);

                StdfRecord? record = null;
                if (_options.Registry.TryGetReader(header.RecordType, header.RecordSub, out var recordReader))
                {
                    try
                    {
                        record = recordReader.Read(reader, header);
                    }
                    catch (Exception) when (_options.ErrorMode == ErrorMode.SkipRecord)
                    {
                        reader.SkipRestOfRecord();
                        continue;
                    }
                }
                else
                {
                    try
                    {
                        long bodyLen = Math.Max(0, header.BodyLength);
                        int toRead = bodyLen <= int.MaxValue ? (int)bodyLen : int.MaxValue;
                        byte[] raw = reader.ReadBytes(toRead);
                        record = new UnknownRecord(header, raw);
                    }
                    catch (Exception) when (_options.ErrorMode == ErrorMode.SkipRecord)
                    {
                        reader.SkipRestOfRecord();
                        continue;
                    }
                }

                reader.SkipRestOfRecord();
                if (record != null) yield return record;
            }
        }

        private FarRecord ReadFar(StdfBinaryReader reader)
        {
            // FAR header is U2 REC_LEN + U1 TYP + U1 SUB, all before byte order is known.
            // The two REC_LEN bytes disambiguate endianness: FAR body is exactly 2 bytes
            // (CPU_TYPE + STDF_VER), so REC_LEN == 2 in the standard BodyOnly mode.
            long farStart = reader.Position;
            byte[] rawHdr = reader.ReadBytes(4);
            if (rawHdr.Length < 4)
                throw new MissingFarRecordException();

            byte b0 = rawHdr[0], b1 = rawHdr[1];
            ushort le = (ushort)(b0 | (b1 << 8));
            ushort be = (ushort)((b0 << 8) | b1);
            byte typ = rawHdr[2];
            byte sub = rawHdr[3];

            if (typ != 0 || sub != 10)
                throw new MissingFarRecordException();

            Endianness end = DetermineFarEndianness(le, be, reader.Length);
            reader.SetEndianness(end);

            ushort recLen = end == Endianness.LittleEndian ? le : be;
            long bodyStart = farStart + 4;
            long bodyLen = ComputeBodyLength(recLen, _options.RecLenMode);

            var header = new StdfRecordHeader(recLen, typ, sub, farStart, bodyStart, bodyLen);
            reader.BeginRecord(header);

            byte cpuType = reader.ReadU1();
            byte stdfVer = reader.ReadU1();
            if (stdfVer != 4)
                throw new UnsupportedStdfVersionException(stdfVer);

            reader.SkipRestOfRecord();
            return new FarRecord(header, cpuType, stdfVer);
        }

        private static Endianness DetermineFarEndianness(ushort le, ushort be, long fileLen)
        {
            bool leIsTwo = le == 2;
            bool beIsTwo = be == 2;

            if (leIsTwo && !beIsTwo) return Endianness.LittleEndian;
            if (beIsTwo && !leIsTwo) return Endianness.BigEndian;
            if (leIsTwo && beIsTwo) return Endianness.LittleEndian;

            // Neither is exactly 2 (dialect / non-standard). Pick the plausible one.
            bool lePlausible = le > 0 && le < fileLen;
            bool bePlausible = be > 0 && be < fileLen;
            if (lePlausible && !bePlausible) return Endianness.LittleEndian;
            if (bePlausible && !lePlausible) return Endianness.BigEndian;
            return Endianness.LittleEndian;
        }

        private static long ComputeBodyLength(ushort recLen, RecLenMode mode)
        {
            long body;
            switch (mode)
            {
                case RecLenMode.BodyOnly: body = recLen; break;
                case RecLenMode.BodyPlusTypSub: body = recLen - 2; break;
                case RecLenMode.FullHeader: body = recLen - 4; break;
                default: body = recLen; break;
            }
            return body < 0 ? 0 : body;
        }
    }
}
