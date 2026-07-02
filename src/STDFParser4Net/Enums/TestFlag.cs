using System;

namespace STDFParser4Net.Enums
{
    /// <summary>
    /// TEST_FLG (B1) — test flag byte shared by PTR, MPR and FTR.
    /// Bit assignments per STDF V4:
    /// <list type="bullet">
    /// <item>bit 7 (0x80): alarm detected during the test.</item>
    /// <item>bit 6 (0x40): result is unreliable.</item>
    /// <item>bit 5 (0x20): test was not executed.</item>
    /// <item>bit 4 (0x10): test failed.</item>
    /// <item>bit 3 (0x08): test aborted. For MPR/FTR this bit also means the
    /// RTN_STAT array is absent from the record.</item>
    /// <item>bit 2 (0x04): warning flag.</item>
    /// <item>bits 0-1: reserved.</item>
    /// </list>
    /// </summary>
    [Flags]
    public enum TestFlag : byte
    {
        None             = 0x00,
        Warning          = 0x04,
        TestAborted      = 0x08,
        TestFailed       = 0x10,
        TestNotExecuted  = 0x20,
        ResultUnreliable = 0x40,
        AlarmDetected    = 0x80,
    }
}
