using System;

namespace STDFParser4Net.Enums
{
    /// <summary>
    /// PARM_FLG (B1) — parametric flag byte for PTR and MPR.
    /// Bit assignments per STDF V4:
    /// <list type="bullet">
    /// <item>bit 7 (0x80): optional high limit (HI_LIMIT) failed.</item>
    /// <item>bit 6 (0x40): optional low limit (LO_LIMIT) failed.</item>
    /// <item>bit 5 (0x20): spec high limit (HI_SPEC) failed.</item>
    /// <item>bit 4 (0x10): spec low limit (LO_SPEC) failed.</item>
    /// <item>bits 0-3: reserved.</item>
    /// </list>
    /// </summary>
    [Flags]
    public enum ParamFlag : byte
    {
        None                    = 0x00,
        SpecLowLimitFailed      = 0x10,
        SpecHighLimitFailed     = 0x20,
        OptionalLowLimitFailed  = 0x40,
        OptionalHighLimitFailed = 0x80,
    }
}
