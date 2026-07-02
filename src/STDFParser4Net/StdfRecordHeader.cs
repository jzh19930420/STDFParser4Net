namespace STDFParser4Net
{
    /// <summary>
    /// Header of a single STDF record: the raw REC_LEN / REC_TYP / REC_SUB fields
    /// plus positions needed for record-boundary bookkeeping.
    /// </summary>
    public readonly struct StdfRecordHeader
    {
        /// <summary>The raw REC_LEN field value as read from the file.</summary>
        public ushort RecordLength { get; }

        /// <summary>REC_TYP.</summary>
        public byte RecordType { get; }

        /// <summary>REC_SUB.</summary>
        public byte RecordSub { get; }

        /// <summary>Stream position of the REC_LEN field (start of the 4-byte header).</summary>
        public long HeaderStart { get; }

        /// <summary>Stream position immediately after TYP/SUB (start of the record body).</summary>
        public long BodyStart { get; }

        /// <summary>Number of body bytes (REC_LEN adjusted for the configured RecLenMode).</summary>
        public long BodyLength { get; }

        public StdfRecordHeader(ushort recordLength, byte typ, byte sub, long headerStart, long bodyStart, long bodyLength)
        {
            RecordLength = recordLength;
            RecordType = typ;
            RecordSub = sub;
            HeaderStart = headerStart;
            BodyStart = bodyStart;
            BodyLength = bodyLength;
        }

        public (byte Typ, byte Sub) Key => (RecordType, RecordSub);

        public override string ToString() => $"REC (TYP={RecordType},SUB={RecordSub}) len={RecordLength}";
    }
}
