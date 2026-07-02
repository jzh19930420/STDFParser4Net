namespace STDFParser4Net.Records
{
    /// <summary>
    /// DTR — Datalog Text Record (30,10). A free-text line in the datalog output.
    /// </summary>
    public sealed class DtrRecord : StdfRecord
    {
        /// <summary>TEXT_DAT Cn: the text line.</summary>
        public StdfString TextDat { get; }

        public DtrRecord(in StdfRecordHeader header, StdfString textDat)
            : base(RecordType.DTR, header)
        {
            TextDat = textDat;
        }

        public override string ToString()
            => $"DTR TextDat=\"{TextDat}\"";
    }
}
