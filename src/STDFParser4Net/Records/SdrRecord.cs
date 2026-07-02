namespace STDFParser4Net.Records
{
    /// <summary>
    /// SDR — Site Description Record (1,80). Describes the configuration of a site
    /// group. All Cn fields after SITE_NUM are optional.
    /// </summary>
    public sealed class SdrRecord : StdfRecord
    {
        /// <summary>U1: Test head number.</summary>
        public byte HEAD_NUM { get; }

        /// <summary>U1: Site group number.</summary>
        public byte SITE_GROUP { get; }

        /// <summary>U1: Number of sites in the group (k).</summary>
        public byte SITE_CNT { get; }

        /// <summary>U1[k]: Array of site numbers, length SITE_CNT.</summary>
        public byte[] SITE_NUM { get; }

        /// <summary>Cn: Handler type. Null when absent.</summary>
        public StdfString? HAND_TYP { get; }

        /// <summary>Cn: Handler ID. Null when absent.</summary>
        public StdfString? HAND_ID { get; }

        /// <summary>Cn: Probe card type. Null when absent.</summary>
        public StdfString? CARD_TYP { get; }

        /// <summary>Cn: Probe card ID. Null when absent.</summary>
        public StdfString? CARD_ID { get; }

        /// <summary>Cn: Load board type. Null when absent.</summary>
        public StdfString? LOAD_TYP { get; }

        /// <summary>Cn: Load board ID. Null when absent.</summary>
        public StdfString? LOAD_ID { get; }

        /// <summary>Cn: DIB type. Null when absent.</summary>
        public StdfString? DIB_TYP { get; }

        /// <summary>Cn: DIB ID. Null when absent.</summary>
        public StdfString? DIB_ID { get; }

        /// <summary>Cn: Cable type. Null when absent.</summary>
        public StdfString? CABL_TYP { get; }

        /// <summary>Cn: Cable ID. Null when absent.</summary>
        public StdfString? CABL_ID { get; }

        /// <summary>Cn: Contactor type. Null when absent.</summary>
        public StdfString? CONT_TYP { get; }

        /// <summary>Cn: Contactor ID. Null when absent.</summary>
        public StdfString? CONT_ID { get; }

        /// <summary>Cn: Laser type. Null when absent.</summary>
        public StdfString? LASR_TYP { get; }

        /// <summary>Cn: Laser ID. Null when absent.</summary>
        public StdfString? LASR_ID { get; }

        /// <summary>Cn: Extra equipment type. Null when absent.</summary>
        public StdfString? EXTRA_TYP { get; }

        /// <summary>Cn: Extra equipment ID. Null when absent.</summary>
        public StdfString? EXTRA_ID { get; }

        public SdrRecord(in StdfRecordHeader header,
            byte headNum, byte siteGroup, byte siteCnt, byte[] siteNum,
            StdfString? handTyp, StdfString? handId,
            StdfString? cardTyp, StdfString? cardId,
            StdfString? loadTyp, StdfString? loadId,
            StdfString? dibTyp, StdfString? dibId,
            StdfString? cablTyp, StdfString? cablId,
            StdfString? contTyp, StdfString? contId,
            StdfString? lasrTyp, StdfString? lasrId,
            StdfString? extraTyp, StdfString? extraId)
            : base(RecordType.SDR, header)
        {
            HEAD_NUM = headNum;
            SITE_GROUP = siteGroup;
            SITE_CNT = siteCnt;
            SITE_NUM = siteNum ?? System.Array.Empty<byte>();
            HAND_TYP = handTyp;
            HAND_ID = handId;
            CARD_TYP = cardTyp;
            CARD_ID = cardId;
            LOAD_TYP = loadTyp;
            LOAD_ID = loadId;
            DIB_TYP = dibTyp;
            DIB_ID = dibId;
            CABL_TYP = cablTyp;
            CABL_ID = cablId;
            CONT_TYP = contTyp;
            CONT_ID = contId;
            LASR_TYP = lasrTyp;
            LASR_ID = lasrId;
            EXTRA_TYP = extraTyp;
            EXTRA_ID = extraId;
        }

        public override string ToString()
            => $"SDR HEAD_NUM={HEAD_NUM} SITE_GROUP={SITE_GROUP} SITE_CNT={SITE_CNT} SITES=[{string.Join(",", SITE_NUM)}]";
    }
}
