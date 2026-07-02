namespace STDFParser4Net.Records
{
    /// <summary>
    /// WCR — Wafer Configuration Record (2,30). Describes the physical layout of a
    /// wafer. All fields are fixed; there are no optional fields.
    /// </summary>
    public sealed class WcrRecord : StdfRecord
    {
        /// <summary>R4: Wafer diameter.</summary>
        public float WAFR_SIZ { get; }

        /// <summary>R4: Die height.</summary>
        public float DIE_HT { get; }

        /// <summary>R4: Die width.</summary>
        public float DIE_WID { get; }

        /// <summary>U1: Units for WAFR_SIZ/DIE_HT/DIE_WID (e.g. inches or mm).</summary>
        public byte WF_UNITS { get; }

        /// <summary>C1: Wafer flat orientation ('U','D','L','R' or space).</summary>
        public char WF_FLAT { get; }

        /// <summary>I2: Center die X coordinate.</summary>
        public short CENTER_X { get; }

        /// <summary>I2: Center die Y coordinate.</summary>
        public short CENTER_Y { get; }

        /// <summary>C1: Positive X direction ('L','R' or space).</summary>
        public char POS_X { get; }

        /// <summary>C1: Positive Y direction ('U','D' or space).</summary>
        public char POS_Y { get; }

        public WcrRecord(in StdfRecordHeader header,
            float wafrSiz, float dieHt, float dieWid, byte wfUnits, char wfFlat,
            short centerX, short centerY, char posX, char posY)
            : base(RecordType.WCR, header)
        {
            WAFR_SIZ = wafrSiz;
            DIE_HT = dieHt;
            DIE_WID = dieWid;
            WF_UNITS = wfUnits;
            WF_FLAT = wfFlat;
            CENTER_X = centerX;
            CENTER_Y = centerY;
            POS_X = posX;
            POS_Y = posY;
        }

        public override string ToString()
            => $"WCR WAFR_SIZ={WAFR_SIZ} DIE_HT={DIE_HT} DIE_WID={DIE_WID} WF_UNITS={WF_UNITS}";
    }
}
