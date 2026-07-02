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

    public class MissingFarRecordException : StdfException
    {
        public MissingFarRecordException() : base("The first record of an STDF V4 file must be FAR (0,10).") { }
    }
}
