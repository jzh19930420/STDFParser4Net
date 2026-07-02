using System;
using System.Text;

namespace STDFParser4Net.IO
{
    /// <summary>
    /// Decodes STDF Cn/C1 byte sequences into <see cref="StdfString"/>. The default
    /// behaviour is ASCII; bytes &gt; 0x7F are preserved 1:1 (ISO-8859-1 style) instead
    /// of being replaced by the Unicode replacement character, so parsing is lossless.
    /// A caller may inject any <see cref="Encoding"/> (e.g. GB18030) via
    /// <see cref="StdfParserOptions"/>.
    /// </summary>
    public sealed class StdfStringEncoding
    {
        private readonly Encoding _encoding;

        public StdfStringEncoding() : this(Encoding.ASCII) { }

        public StdfStringEncoding(Encoding encoding)
        {
            _encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
        }

        /// <summary>
        /// Decode a byte range. Lossless: undecodable high bytes map 1:1 to chars.
        /// </summary>
        public StdfString Decode(byte[] raw)
        {
            if (raw == null || raw.Length == 0)
                return new StdfString(Array.Empty<byte>(), string.Empty);

            string text;
            try
            {
                // Strict decode; fall back to lossless on failure.
                text = _encoding.GetString(raw);
            }
            catch (DecoderFallbackException)
            {
                text = LosslessDecode(raw);
            }

            // If the strict decoder emitted replacement chars for high bytes, redo losslessly.
            if (ContainsReplacement(text))
                text = LosslessDecode(raw);

            return new StdfString(raw, text);
        }

        private static string LosslessDecode(byte[] raw)
        {
            var chars = new char[raw.Length];
            for (int i = 0; i < raw.Length; i++)
                chars[i] = (char)raw[i]; // 0..255 → U+0000..U+00FF
            return new string(chars);
        }

        private static bool ContainsReplacement(string s)
        {
            foreach (var c in s)
                if (c == '�') return true;
            return false;
        }
    }
}
