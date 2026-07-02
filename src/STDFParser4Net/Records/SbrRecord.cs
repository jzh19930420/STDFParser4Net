namespace STDFParser4Net.Records
{
    /// <summary>
    /// SBR — Software Bin Record (1,50). Records the count of parts that fell into a
    /// particular software bin for a head/site.
    /// </summary>
    public sealed class SbrRecord : StdfRecord
    {
        /// <summary>HEAD_NUM U1: test head number.</summary>
        public byte HeadNum { get; }

        /// <summary>SITE_NUM U1: test site number.</summary>
        public byte SiteNum { get; }

        /// <summary>SBIN_NUM U2: software bin number.</summary>
        public ushort SbinNum { get; }

        /// <summary>SBIN_CNT U4: number of parts in this software bin.</summary>
        public uint SbinCnt { get; }

        /// <summary>SBIN_PF C1: pass/fail indicator. 'P'=pass, 'F'=fail, ' '=unknown.</summary>
        public char SbinPf { get; }

        /// <summary>SBIN_NAM Cn: software bin name. Optional.</summary>
        public StdfString SbinNam { get; }

        public SbrRecord(in StdfRecordHeader header, byte headNum, byte siteNum,
            ushort sbinNum, uint sbinCnt, char sbinPf, StdfString sbinNam)
            : base(RecordType.SBR, header)
        {
            HeadNum = headNum;
            SiteNum = siteNum;
            SbinNum = sbinNum;
            SbinCnt = sbinCnt;
            SbinPf = sbinPf;
            SbinNam = sbinNam;
        }

        public override string ToString()
            => $"SBR Head={HeadNum} Site={SiteNum} Bin={SbinNum} Cnt={SbinCnt} PF='{SbinPf}' Nam=\"{SbinNam}\"";
    }
}
