namespace STDFParser4Net.Records
{
    /// <summary>
    /// WRR — Wafer Results Record (2,20). Summarizes the results for a wafer.
    /// WAFER_ID, FABWF_ID and WAFR_NOTE are optional.
    /// </summary>
    public sealed class WrrRecord : StdfRecord
    {
        /// <summary>U1: Test head number.</summary>
        public byte HEAD_NUM { get; }

        /// <summary>U4: Number of parts tested.</summary>
        public uint PART_CNT { get; }

        /// <summary>U4: Number of parts retested.</summary>
        public uint RTST_CNT { get; }

        /// <summary>U4: Number of aborted parts.</summary>
        public uint ABRT_CNT { get; }

        /// <summary>U4: Number of good parts.</summary>
        public uint GOOD_CNT { get; }

        /// <summary>U4: Number of functional parts.</summary>
        public uint FUNC_CNT { get; }

        /// <summary>Cn: Wafer identifier. Null when absent.</summary>
        public StdfString? WAFER_ID { get; }

        /// <summary>Cn: Fabrication wafer identifier. Null when absent.</summary>
        public StdfString? FABWF_ID { get; }

        /// <summary>Cn: Wafer note text. Null when absent.</summary>
        public StdfString? WAFR_NOTE { get; }

        public WrrRecord(in StdfRecordHeader header,
            byte headNum, uint partCnt, uint rtstCnt, uint abrtCnt, uint goodCnt, uint funcCnt,
            StdfString? waferId, StdfString? fabwfId, StdfString? wafrNote)
            : base(RecordType.WRR, header)
        {
            HEAD_NUM = headNum;
            PART_CNT = partCnt;
            RTST_CNT = rtstCnt;
            ABRT_CNT = abrtCnt;
            GOOD_CNT = goodCnt;
            FUNC_CNT = funcCnt;
            WAFER_ID = waferId;
            FABWF_ID = fabwfId;
            WAFR_NOTE = wafrNote;
        }

        public override string ToString()
            => $"WRR HEAD_NUM={HEAD_NUM} PART_CNT={PART_CNT} GOOD_CNT={GOOD_CNT} WAFER_ID={WAFER_ID?.Text ?? "<null>"}";
    }
}
