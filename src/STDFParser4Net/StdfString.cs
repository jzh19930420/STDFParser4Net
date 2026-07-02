using System;
using System.Text;

namespace STDFParser4Net
{
    /// <summary>
    /// An STDF Cn/C1 string value. Preserves the raw bytes read from the file
    /// alongside a best-effort decoded text. This keeps parsing lossless: the
    /// library never silently replaces bytes it cannot decode.
    /// </summary>
    public readonly struct StdfString : IEquatable<StdfString>
    {
        private readonly byte[]? _raw;
        private readonly string? _text;

        /// <summary>Raw bytes as stored in the file (length prefix excluded). Never null.</summary>
        public byte[] Raw => _raw ?? Array.Empty<byte>();

        /// <summary>
        /// Decoded text. Decoded with the configured <see cref="Encoding"/>; bytes that
        /// cannot be decoded are mapped 1:1 to chars (ISO-8859-1 style) so no information
        /// is lost. Never null (defaults to <see cref="string.Empty"/>).
        /// </summary>
        public string Text => _text ?? string.Empty;

        public StdfString(byte[] raw, string text)
        {
            _raw = raw ?? Array.Empty<byte>();
            _text = text ?? string.Empty;
        }

        public override string ToString() => Text;

        public static implicit operator string(StdfString s) => s.Text;

        public bool Equals(StdfString other)
            => ReferenceEquals(Raw, other.Raw) || (Raw.AsSpan().SequenceEqual(other.Raw) && Text == other.Text);

        public override bool Equals(object? obj) => obj is StdfString other && Equals(other);

        public override int GetHashCode() => Text.GetHashCode();
    }
}
