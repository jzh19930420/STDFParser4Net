namespace STDFParser4Net.Records
{
    /// <summary>
    /// A record whose (TYP,SUB) was not registered. The raw body bytes are preserved so
    /// callers can inspect non-standard / equipment-specific records without losing data.
    /// </summary>
    public sealed class UnknownRecord : StdfRecord
    {
        /// <summary>Raw body bytes (excluding the 4-byte header).</summary>
        public byte[] RawBody { get; }

        public UnknownRecord(in StdfRecordHeader header, byte[] rawBody)
            : base(RecordType.Unknown, header)
        {
            RawBody = rawBody ?? System.Array.Empty<byte>();
        }
    }
}
