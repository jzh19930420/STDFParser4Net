namespace STDFParser4Net.Records
{
    /// <summary>
    /// WIR — Wafer Information Record (2,10). Marks the start of a wafer's test
    /// data. FINISH_T and WAFER_ID are optional.
    /// </summary>
    public sealed class WirRecord : StdfRecord
    {
        /// <summary>U1: Test head number.</summary>
        public byte HEAD_NUM { get; }

        /// <summary>U1: Setup group number.</summary>
        public byte SETUP_GRP { get; }

        /// <summary>U4: Wafer test start time (seconds since epoch).</summary>
        public uint START_T { get; }

        /// <summary>U4: Wafer test finish time. Null when absent.</summary>
        public uint? FINISH_T { get; }

        /// <summary>Cn: Wafer identifier. Null when absent.</summary>
        public StdfString? WAFER_ID { get; }

        public WirRecord(in StdfRecordHeader header,
            byte headNum, byte setupGrp, uint startT, uint? finishT, StdfString? waferId)
            : base(RecordType.WIR, header)
        {
            HEAD_NUM = headNum;
            SETUP_GRP = setupGrp;
            START_T = startT;
            FINISH_T = finishT;
            WAFER_ID = waferId;
        }

        public override string ToString()
            => $"WIR HEAD_NUM={HEAD_NUM} SETUP_GRP={SETUP_GRP} START_T={START_T} WAFER_ID={WAFER_ID?.Text ?? "<null>"}";
    }
}
