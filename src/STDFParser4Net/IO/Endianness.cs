namespace STDFParser4Net.IO
{
    /// <summary>Byte order used to read multi-byte STDF fields.</summary>
    public enum Endianness
    {
        /// <summary>Little-endian (CPU_TYPE bit0 = 1). The most common case on x86.</summary>
        LittleEndian = 0,

        /// <summary>Big-endian (CPU_TYPE bit0 = 0).</summary>
        BigEndian = 1
    }
}
