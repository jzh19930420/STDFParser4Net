namespace STDFParser4Net.Records
{
    /// <summary>
    /// GDR — Generic Data Record (50,10). A variable-length container for
    /// arbitrary typed fields. Each field is a <see cref="GdrField"/> consisting of
    /// a 1-byte STDF V4 type code and the corresponding decoded value.
    /// </summary>
    public sealed class GdrRecord : StdfRecord
    {
        /// <summary>B1: Generic flags.</summary>
        public byte GEN_FLG { get; }

        /// <summary>U2: Field count (length of <see cref="FIELDS"/>).</summary>
        public ushort FLD_CNT { get; }

        /// <summary>Vn[k]: Variable-type fields, length FLD_CNT.</summary>
        public GdrField[] FIELDS { get; }

        public GdrRecord(in StdfRecordHeader header, byte genFlg, ushort fldCnt, GdrField[] fields)
            : base(RecordType.GDR, header)
        {
            GEN_FLG = genFlg;
            FLD_CNT = fldCnt;
            FIELDS = fields;
        }

        public override string ToString()
            => $"GDR GEN_FLG=0x{GEN_FLG:X2} FLD_CNT={FLD_CNT}";
    }
}
