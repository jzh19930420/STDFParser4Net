using System;

namespace STDFParser4Net.Enums
{
    /// <summary>
    /// PART_FLG flag bits for the PRR (5,20) record. Bit assignments follow the
    /// Loader convention: bit 3 (0x08) is the pass/fail control bit used by
    /// <c>PrrRecord.IsPass</c>.
    /// </summary>
    [Flags]
    public enum PartFlag : byte
    {
        /// <summary>Bit 0 (0x01): no reliable pass/fail information.</summary>
        NoPassFail = 0x01,

        /// <summary>Bit 1 (0x02): part was retested.</summary>
        Retested = 0x02,

        /// <summary>Bit 2 (0x04): part is abnormal.</summary>
        Abnormal = 0x04,

        /// <summary>Bit 3 (0x08): part failed. When this bit is 0 the part passed.</summary>
        Failed = 0x08,
    }
}
