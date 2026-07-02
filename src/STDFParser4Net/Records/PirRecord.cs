namespace STDFParser4Net.Records
{
    /// <summary>
    /// PIR — Part Information Record (5,10). Marks the beginning of a part's test
    /// data in the stream. Always paired with a later PRR for the same part.
    /// </summary>
    public sealed class PirRecord : StdfRecord
    {
        /// <summary>U1: Test head number.</summary>
        public byte HEAD_NUM { get; }

        /// <summary>U1: Test site number.</summary>
        public byte SITE_NUM { get; }

        public PirRecord(in StdfRecordHeader header, byte headNum, byte siteNum)
            : base(RecordType.PIR, header)
        {
            HEAD_NUM = headNum;
            SITE_NUM = siteNum;
        }

        public override string ToString()
            => $"PIR HEAD_NUM={HEAD_NUM} SITE_NUM={SITE_NUM}";
    }
}
