namespace STDFParser4Net.Records
{
    /// <summary>
    /// PGR — Pin Group Record (1,62). Defines a named group of pins by referencing
    /// PMR indices.
    /// </summary>
    public sealed class PgrRecord : StdfRecord
    {
        /// <summary>U2: Group index.</summary>
        public ushort GRP_INDX { get; }

        /// <summary>Cn: Group name.</summary>
        public StdfString GRP_NAM { get; }

        /// <summary>U2: Number of pin indices in the group.</summary>
        public ushort INDX_CNT { get; }

        /// <summary>U2[k]: Array of PMR indices, length INDX_CNT.</summary>
        public ushort[] PMR_INDX { get; }

        public PgrRecord(in StdfRecordHeader header,
            ushort grpIndx, StdfString grpNam, ushort indxCnt, ushort[] pmrIndx)
            : base(RecordType.PGR, header)
        {
            GRP_INDX = grpIndx;
            GRP_NAM = grpNam;
            INDX_CNT = indxCnt;
            PMR_INDX = pmrIndx;
        }

        public override string ToString()
            => $"PGR GRP_INDX={GRP_INDX} GRP_NAM=\"{GRP_NAM}\" INDX_CNT={INDX_CNT}";
    }
}
