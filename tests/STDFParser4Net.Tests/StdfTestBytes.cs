using System.Collections.Generic;

namespace STDFParser4Net.Tests
{
    /// <summary>
    /// Helpers to build little-endian STDF byte streams for tests. Each method
    /// appends the raw bytes of one STDF field to a <see cref="List{Byte}"/>.
    /// </summary>
    internal static class StdfTestBytes
    {
        /// <summary>FAR(LE): REC_LEN=2, TYP=0, SUB=10, CPU_TYPE=1(LE), STDF_VER=4.</summary>
        public static byte[] FarLe() => new byte[] { 0x02, 0x00, 0x00, 0x0A, 0x01, 0x04 };

        public static void U1(List<byte> b, byte v) => b.Add(v);

        public static void U2(List<byte> b, ushort v)
        {
            b.Add((byte)(v & 0xFF));
            b.Add((byte)((v >> 8) & 0xFF));
        }

        public static void U4(List<byte> b, uint v)
        {
            b.Add((byte)(v & 0xFF));
            b.Add((byte)((v >> 8) & 0xFF));
            b.Add((byte)((v >> 16) & 0xFF));
            b.Add((byte)((v >> 24) & 0xFF));
        }

        public static void I2(List<byte> b, short v) => U2(b, (ushort)v);

        public static void I4(List<byte> b, int v) => U4(b, (uint)v);

        public static void R4(List<byte> b, float v) => U4(b, (uint)System.BitConverter.SingleToInt32Bits(v));

        public static void C1(List<byte> b, char c) => b.Add((byte)c);

        public static void Cn(List<byte> b, string s)
        {
            b.Add((byte)s.Length);
            foreach (char c in s) b.Add((byte)c);
        }

        public static void Bn(List<byte> b, byte[] data)
        {
            b.Add((byte)data.Length);
            foreach (byte d in data) b.Add(d);
        }

        /// <summary>Append a record header: REC_LEN (U2 LE) + TYP (U1) + SUB (U1).</summary>
        public static void Header(List<byte> b, ushort recLen, byte typ, byte sub)
        {
            U2(b, recLen);
            b.Add(typ);
            b.Add(sub);
        }

        /// <summary>Append FAR(LE) + a record with the given TYP/SUB and body, computing REC_LEN automatically.</summary>
        public static byte[] FarPlusRecord(byte typ, byte sub, byte[] body)
        {
            var b = new List<byte>();
            b.AddRange(FarLe());
            Header(b, (ushort)body.Length, typ, sub);
            b.AddRange(body);
            return b.ToArray();
        }
    }
}
