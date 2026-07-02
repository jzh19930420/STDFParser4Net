using System;

namespace STDFParser4Net.Records
{
    /// <summary>
    /// A single Vn (variable-type) field within a GDR record. The <see cref="TypeCode"/>
    /// follows the STDF V4 data type code enumeration:
    /// 0=U1, 1=U2, 2=U4, 3=I1, 4=I2, 5=I4, 6=R4, 7=R8, 8=B1, 10=Cn, 11=Bn, 12=Dn, 13=N1.
    /// (Code 9 is not used in STDF V4.)
    /// <para>
    /// The CLR type of <see cref="Value"/> per code:
    /// U1→<see cref="byte"/>, U2→<see cref="ushort"/>, U4→<see cref="uint"/>,
    /// I1→<see cref="sbyte"/>, I2→<see cref="short"/>, I4→<see cref="int"/>,
    /// R4→<see cref="float"/>, R8→<see cref="double"/>,
    /// B1→<see cref="byte"/>, Cn→<see cref="StdfString"/>,
    /// Bn→<see cref="byte"/>[], Dn→<see cref="BitString"/>, N1→<see cref="byte"/>.
    /// </para>
    /// </summary>
    public readonly struct GdrField
    {
        /// <summary>STDF V4 data type code (0..13, excluding 9).</summary>
        public byte TypeCode { get; }

        /// <summary>Decoded value. See type summary for the CLR type per code.</summary>
        public object? Value { get; }

        public GdrField(byte typeCode, object? value)
        {
            TypeCode = typeCode;
            Value = value;
        }

        public override string ToString() => $"GdrField(code={TypeCode}, value={Value})";
    }
}
