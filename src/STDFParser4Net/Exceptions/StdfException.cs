using System;

namespace STDFParser4Net.Exceptions
{
    public class StdfException : Exception
    {
        public StdfException(string message) : base(message) { }
        public StdfException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class UnsupportedStdfVersionException : StdfException
    {
        public byte DetectedVersion { get; }

        public UnsupportedStdfVersionException(byte detectedVersion)
            : base($"Unsupported STDF version: {detectedVersion}. Only STDF V4 is supported.")
        {
            DetectedVersion = detectedVersion;
        }
    }

    public class UnexpectedEndOfStreamException : StdfException
    {
        public UnexpectedEndOfStreamException(string message) : base(message) { }
    }

    /// <summary>
    /// Thrown when a field read would consume bytes past the current record's body
    /// (REC_LEN boundary). The parser can skip to the next record without desyncing
    /// the stream when the underlying stream is seekable.
    /// </summary>
    public class RecordBoundaryExceededException : StdfException
    {
        public RecordBoundaryExceededException(string message) : base(message) { }
    }

    public class MissingFarRecordException : StdfException
    {
        public MissingFarRecordException() : base("The first record of an STDF V4 file must be FAR (0,10).") { }
    }
}
