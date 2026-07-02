using System.Text;
using STDFParser4Net.IO;
using STDFParser4Net.Records.Registry;

namespace STDFParser4Net
{
    /// <summary>How the parser reacts to a record body that fails to parse.</summary>
    public enum ErrorMode
    {
        /// <summary>Skip the offending record (boundary-aligned) and continue.</summary>
        SkipRecord = 0,

        /// <summary>Throw the parsing exception, aborting iteration.</summary>
        Throw = 1
    }

    /// <summary>
    /// Configuration for <see cref="StdfParser"/>. Defaults: ASCII lossless encoding,
    /// standard STDF V4 RecLenMode, the default record registry, SkipRecord error mode.
    /// </summary>
    public sealed class StdfParserOptions
    {
        public StdfStringEncoding Encoding { get; set; } = new StdfStringEncoding();

        public RecLenMode RecLenMode { get; set; } = RecLenMode.BodyOnly;

        public RecordTypeRegistry Registry { get; set; } = RecordTypeRegistry.CreateDefault();

        public ErrorMode ErrorMode { get; set; } = ErrorMode.SkipRecord;

        /// <summary>Convenience: use a specific text encoding for Cn/C1 strings.</summary>
        public StdfParserOptions WithEncoding(Encoding encoding)
        {
            Encoding = new StdfStringEncoding(encoding);
            return this;
        }

        /// <summary>Convenience: use a non-default RecLenMode.</summary>
        public StdfParserOptions WithRecLenMode(RecLenMode mode)
        {
            RecLenMode = mode;
            return this;
        }
    }
}
