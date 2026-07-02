namespace STDFParser4Net.Records
{
    /// <summary>
    /// RDR — Retest Data Record (1,70). Lists the bins that contained parts selected
    /// for retest. Array length is NUM_BINS.
    /// </summary>
    public sealed class RdrRecord : StdfRecord
    {
        /// <summary>HEAD_NUM U1: test head number.</summary>
        public byte HeadNum { get; }

        /// <summary>SITE_NUM U1: test site number.</summary>
        public byte SiteNum { get; }

        /// <summary>NUM_BINS U2: number of bins in the RTST_BIN array.</summary>
        public ushort NumBins { get; }

        /// <summary>RTST_BIN U2[] (kxU2): bin numbers containing retested parts.</summary>
        public ushort[] RtstBin { get; }

        public RdrRecord(in StdfRecordHeader header, byte headNum, byte siteNum,
            ushort numBins, ushort[] rtstBin)
            : base(RecordType.RDR, header)
        {
            HeadNum = headNum;
            SiteNum = siteNum;
            NumBins = numBins;
            RtstBin = rtstBin ?? System.Array.Empty<ushort>();
        }

        public override string ToString()
            => $"RDR Head={HeadNum} Site={SiteNum} NumBins={NumBins} Bins=[{string.Join(",", RtstBin)}]";
    }
}
