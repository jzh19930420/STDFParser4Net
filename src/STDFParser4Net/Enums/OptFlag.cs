using System;

namespace STDFParser4Net.Enums
{
    /// <summary>
    /// OPT_FLAG (B1) — optional-data flag for PTR and MPR as defined by STDF V4.
    /// A set bit means the corresponding optional field is ABSENT according to the
    /// specification; a clear bit means the field is present.
    /// <para>
    /// <b>Parsing note:</b> this library stores the raw flag but does <b>not</b> use
    /// the bits to skip fields when reading. Production equipment commonly writes
    /// RES_SCAL…HI_LIMIT in fixed order regardless of OPT_FLAG. Field presence is
    /// determined by remaining body bytes after OPT_FLAG.
    /// </para>
    /// <list type="bullet">
    /// <item>bit 0 (0x01): RES_SCAL absent (spec meaning).</item>
    /// <item>bit 1 (0x02): LLM_SCAL absent.</item>
    /// <item>bit 2 (0x04): HLM_SCAL absent.</item>
    /// <item>bit 3 (0x08): LO_LIMIT absent.</item>
    /// <item>bit 4 (0x10): HI_LIMIT absent.</item>
    /// <item>bit 5 (0x20): UNITS absent.</item>
    /// <item>bit 6 (0x40): C_RESFMT absent.</item>
    /// <item>bit 7 (0x80): C_LLMFMT absent.</item>
    /// </list>
    /// </summary>
    [Flags]
    public enum OptFlag : byte
    {
        None          = 0x00,
        ResScalAbsent = 0x01,
        LlmScalAbsent = 0x02,
        HlmScalAbsent = 0x04,
        LoLimitAbsent = 0x08,
        HiLimitAbsent = 0x10,
        UnitsAbsent   = 0x20,
        CResfmtAbsent = 0x40,
        CLlmfmtAbsent = 0x80,
    }
}
