namespace STDFParser4Net.Records
{
    /// <summary>
    /// EPS — End Program Section (20,20). Marks the end of the program section that
    /// was begun by the most recent BPS. Carries no body fields.
    /// </summary>
    public sealed class EpsRecord : StdfRecord
    {
        public EpsRecord(in StdfRecordHeader header)
            : base(RecordType.EPS, header) { }

        public override string ToString() => "EPS";
    }
}
