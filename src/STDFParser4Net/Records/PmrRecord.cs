namespace STDFParser4Net.Records
{
    /// <summary>
    /// PMR — Pin Map Record (1,60). Maps a physical channel to a logical pin.
    /// CHAN_NAM, PHY_NAM and LOG_NAM are optional Cn fields.
    /// </summary>
    public sealed class PmrRecord : StdfRecord
    {
        /// <summary>U2: Pin index.</summary>
        public ushort PMR_INDX { get; }

        /// <summary>U2: Channel type.</summary>
        public ushort CHAN_TYP { get; }

        /// <summary>Cn: Channel name. Null when absent.</summary>
        public StdfString? CHAN_NAM { get; }

        /// <summary>Cn: Physical pin name. Null when absent.</summary>
        public StdfString? PHY_NAM { get; }

        /// <summary>Cn: Logical pin name. Null when absent.</summary>
        public StdfString? LOG_NAM { get; }

        public PmrRecord(in StdfRecordHeader header,
            ushort pmrIndx, ushort chanTyp,
            StdfString? chanNam, StdfString? phyNam, StdfString? logNam)
            : base(RecordType.PMR, header)
        {
            PMR_INDX = pmrIndx;
            CHAN_TYP = chanTyp;
            CHAN_NAM = chanNam;
            PHY_NAM = phyNam;
            LOG_NAM = logNam;
        }

        public override string ToString()
            => $"PMR PMR_INDX={PMR_INDX} CHAN_TYP={CHAN_TYP} CHAN_NAM=\"{CHAN_NAM}\"";
    }
}
