using STDFParser4Net.IO;

namespace STDFParser4Net.Records.Registry
{
    /// <summary>
    /// Reads a single record's body from the binary stream. Implementations are
    /// registered in <see cref="RecordTypeRegistry"/> keyed by (TYP,SUB).
    /// </summary>
    public interface IRecordReader
    {
        /// <summary>
        /// Read the record body. The reader's record boundary has already been
        /// initialized (<see cref="StdfBinaryReader.BeginRecord"/>); implementations
        /// should consume fields and let the framework skip any trailing bytes.
        /// </summary>
        StdfRecord Read(StdfBinaryReader reader, in StdfRecordHeader header);
    }
}
