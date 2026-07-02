namespace STDFParser4Net.Records
{
    /// <summary>
    /// PLR — Pin List Record (1,63). Describes the state and character of pin
    /// groups. All array fields have GRP_CNT entries.
    /// </summary>
    public sealed class PlrRecord : StdfRecord
    {
        /// <summary>U2: Number of groups.</summary>
        public ushort GRP_CNT { get; }

        /// <summary>U2[k]: Group indices, length GRP_CNT.</summary>
        public ushort[] GRP_INDX { get; }

        /// <summary>U2[k]: Group modes, length GRP_CNT.</summary>
        public ushort[] GRP_MODE { get; }

        /// <summary>U1[k]: Group radix, length GRP_CNT.</summary>
        public byte[] GRP_RADIX { get; }

        /// <summary>Cn[k]: Programmed character (upper), length GRP_CNT.</summary>
        public StdfString[] PGM_CHAR { get; }

        /// <summary>Cn[k]: Programmed character (lower), length GRP_CNT.</summary>
        public StdfString[] PGM_CHAL { get; }

        /// <summary>Cn[k]: Result character (upper), length GRP_CNT.</summary>
        public StdfString[] RSL_CHAR { get; }

        /// <summary>Cn[k]: Result character (lower), length GRP_CNT.</summary>
        public StdfString[] RSL_CHAL { get; }

        public PlrRecord(in StdfRecordHeader header,
            ushort grpCnt, ushort[] grpIndx, ushort[] grpMode, byte[] grpRadix,
            StdfString[] pgmChar, StdfString[] pgmChal,
            StdfString[] rslChar, StdfString[] rslChal)
            : base(RecordType.PLR, header)
        {
            GRP_CNT = grpCnt;
            GRP_INDX = grpIndx;
            GRP_MODE = grpMode;
            GRP_RADIX = grpRadix;
            PGM_CHAR = pgmChar;
            PGM_CHAL = pgmChal;
            RSL_CHAR = rslChar;
            RSL_CHAL = rslChal;
        }

        public override string ToString()
            => $"PLR GRP_CNT={GRP_CNT}";
    }
}
