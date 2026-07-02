namespace STDFParser4Net.Records
{
    /// <summary>
    /// PCR — Part Count Record (1,30). Summarises part counts (total, retested,
    /// aborted, good, functional) for a head/site.
    /// </summary>
    public sealed class PcrRecord : StdfRecord
    {
        /// <summary>HEAD_NUM U1: test head number.</summary>
        public byte HeadNum { get; }

        /// <summary>SITE_NUM U1: test site number.</summary>
        public byte SiteNum { get; }

        /// <summary>PART_CNT U4: total number of parts tested.</summary>
        public uint PartCnt { get; }

        /// <summary>RTST_CNT U4: number of retested parts.</summary>
        public uint RtstCnt { get; }

        /// <summary>ABRT_CNT U4: number of aborted parts.</summary>
        public uint AbrtCnt { get; }

        /// <summary>GOOD_CNT U4: number of good parts.</summary>
        public uint GoodCnt { get; }

        /// <summary>FUNC_CNT U4: number of parts passing functional tests.</summary>
        public uint FuncCnt { get; }

        public PcrRecord(in StdfRecordHeader header, byte headNum, byte siteNum,
            uint partCnt, uint rtstCnt, uint abrtCnt, uint goodCnt, uint funcCnt)
            : base(RecordType.PCR, header)
        {
            HeadNum = headNum;
            SiteNum = siteNum;
            PartCnt = partCnt;
            RtstCnt = rtstCnt;
            AbrtCnt = abrtCnt;
            GoodCnt = goodCnt;
            FuncCnt = funcCnt;
        }

        public override string ToString()
            => $"PCR Head={HeadNum} Site={SiteNum} Part={PartCnt} Rtst={RtstCnt} Abrt={AbrtCnt} Good={GoodCnt} Func={FuncCnt}";
    }
}
