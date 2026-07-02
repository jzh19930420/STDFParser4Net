namespace STDFParser4Net
{
    /// <summary>
    /// Interpretation of the REC_LEN field that precedes every record.
    /// </summary>
    public enum RecLenMode
    {
        /// <summary>REC_LEN counts only the body bytes (excludes TYP/SUB and the header itself). STDF V4 standard.</summary>
        BodyOnly = 0,

        /// <summary>REC_LEN includes TYP+SUB (body = REC_LEN - 2). Some equipment dialects.</summary>
        BodyPlusTypSub = 1,

        /// <summary>REC_LEN includes the full 4-byte header (body = REC_LEN - 4).</summary>
        FullHeader = 2
    }
}
