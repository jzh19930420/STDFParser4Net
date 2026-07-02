using System;
using System.Collections.Generic;
using STDFParser4Net.IO;

namespace STDFParser4Net.Records.Registry
{
    /// <summary>
    /// Maps (REC_TYP, REC_SUB) keys to <see cref="IRecordReader"/> implementations.
    /// The default registry covers all STDF V4 standard records; equipment dialects
    /// and custom records can be registered via <see cref="Register"/>.
    /// </summary>
    public sealed class RecordTypeRegistry
    {
        private readonly Dictionary<(byte Typ, byte Sub), IRecordReader> _readers
            = new Dictionary<(byte, byte), IRecordReader>();

        public bool TryGetReader(byte typ, byte sub, out IRecordReader reader)
            => _readers.TryGetValue((typ, sub), out reader!);

        /// <summary>Register or replace a reader for a (TYP,SUB) key.</summary>
        public void Register(byte typ, byte sub, IRecordReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            _readers[(typ, sub)] = reader;
        }

        public int Count => _readers.Count;

        /// <summary>Create a registry populated with all STDF V4 standard record readers.</summary>
        public static RecordTypeRegistry CreateDefault()
        {
            var r = new RecordTypeRegistry();
            StandardRecordReaders.RegisterAll(r);
            return r;
        }
    }
}
