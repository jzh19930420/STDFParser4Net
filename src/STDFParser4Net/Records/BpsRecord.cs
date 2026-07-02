namespace STDFParser4Net.Records
{
    /// <summary>
    /// BPS — Begin Program Section (20,10). Marks the start of a named program
    /// section in the data stream.
    /// </summary>
    public sealed class BpsRecord : StdfRecord
    {
        /// <summary>SEQ_NAME Cn: name of the program section that begins here.</summary>
        public StdfString SeqName { get; }

        public BpsRecord(in StdfRecordHeader header, StdfString seqName)
            : base(RecordType.BPS, header)
        {
            SeqName = seqName;
        }

        public override string ToString()
            => $"BPS SeqName=\"{SeqName}\"";
    }
}
