namespace STDFParser4Net
{
    /// <summary>
    /// Base class for every parsed STDF record. Concrete subclasses live in
    /// <see cref="Records"/>. Unrecognized records become <see cref="Records.UnknownRecord"/>.
    /// </summary>
    public abstract class StdfRecord
    {
        /// <summary>The 4-byte header read from the file.</summary>
        public StdfRecordHeader Header { get; }

        /// <summary>Logical record type.</summary>
        public RecordType RecordType { get; }

        protected StdfRecord(RecordType recordType, in StdfRecordHeader header)
        {
            RecordType = recordType;
            Header = header;
        }

        public override string ToString() => $"{RecordType} {Header}";
    }
}
