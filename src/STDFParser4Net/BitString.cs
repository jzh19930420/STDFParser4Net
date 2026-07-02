namespace STDFParser4Net
{
    /// <summary>A Dn bit string: the declared bit count plus the packed bytes.</summary>
    public readonly struct BitString
    {
        /// <summary>Number of meaningful bits (U2 prefix value).</summary>
        public int BitCount { get; }

        /// <summary>Packed bytes, ceil(BitCount/8) long.</summary>
        public byte[] Bytes { get; }

        public BitString(int bitCount, byte[] bytes)
        {
            BitCount = bitCount;
            Bytes = bytes ?? System.Array.Empty<byte>();
        }

        public bool GetBit(int index)
        {
            if (index < 0 || index >= BitCount) return false;
            // STDF Dn: first bit is the MSB of the first byte.
            int byteIndex = index / 8;
            int bitIndex = 7 - (index % 8);
            return (Bytes[byteIndex] & (1 << bitIndex)) != 0;
        }

        public override string ToString() => $"Dn(bits={BitCount})";
    }
}
