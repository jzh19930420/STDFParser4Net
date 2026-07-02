namespace STDFParser4Net.Records
{
    /// <summary>
    /// HBR — Hardware Bin Record (1,40). Records the count of parts that fell into a
    /// particular hardware bin for a head/site.
    /// </summary>
    public sealed class HbrRecord : StdfRecord
    {
        /// <summary>HEAD_NUM U1: test head number.</summary>
        public byte HeadNum { get; }

        /// <summary>SITE_NUM U1: test site number.</summary>
        public byte SiteNum { get; }

        /// <summary>HBIN_NUM U2: hardware bin number.</summary>
        public ushort HbinNum { get; }

        /// <summary>HBIN_CNT U4: number of parts in this hardware bin.</summary>
        public uint HbinCnt { get; }

        /// <summary>HBIN_PF C1: pass/fail indicator. 'P'=pass, 'F'=fail, ' '=unknown.</summary>
        public char HbinPf { get; }

        /// <summary>HBIN_NAM Cn: hardware bin name. Optional.</summary>
        public StdfString HbinNam { get; }

        public HbrRecord(in StdfRecordHeader header, byte headNum, byte siteNum,
            ushort hbinNum, uint hbinCnt, char hbinPf, StdfString hbinNam)
            : base(RecordType.HBR, header)
        {
            HeadNum = headNum;
            SiteNum = siteNum;
            HbinNum = hbinNum;
            HbinCnt = hbinCnt;
            HbinPf = hbinPf;
            HbinNam = hbinNam;
        }

        public override string ToString()
            => $"HBR Head={HeadNum} Site={SiteNum} Bin={HbinNum} Cnt={HbinCnt} PF='{HbinPf}' Nam=\"{HbinNam}\"";
    }
}
