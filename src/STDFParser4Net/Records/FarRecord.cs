namespace STDFParser4Net.Records
{
    /// <summary>
    /// FAR — File Attributes Record (0,10). Always the first record of an STDF V4 file.
    /// Carries CPU_TYPE (byte order) and STDF_VER (must be 4).
    /// </summary>
    public sealed class FarRecord : StdfRecord
    {
        /// <summary>
        /// CPU_TYPE. Bit 0: 0 = big-endian, 1 = little-endian. Other bits reserved.
        /// </summary>
        public byte CpuType { get; }

        /// <summary>STDF format version. Must be 4 for this library.</summary>
        public byte StdfVer { get; }

        public bool IsLittleEndian => (CpuType & 0x01) != 0;

        public FarRecord(in StdfRecordHeader header, byte cpuType, byte stdfVer)
            : base(RecordType.FAR, header)
        {
            CpuType = cpuType;
            StdfVer = stdfVer;
        }

        public override string ToString()
            => $"FAR CpuType=0x{CpuType:X2} ({(IsLittleEndian ? "LE" : "BE")}) StdfVer={StdfVer}";
    }
}
