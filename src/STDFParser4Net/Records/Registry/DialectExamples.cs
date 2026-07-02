using STDFParser4Net.Records.Readers;

namespace STDFParser4Net.Records.Registry
{
    /// <summary>
    /// Equipment dialects encountered in the wild. These map non-standard (TYP,SUB)
    /// pairs to standard record readers so files emitted by certain test systems can be
    /// parsed. They are NOT enabled by default; call <see cref="RegisterLoaderKnownDialects"/>
    /// on a registry to opt in.
    ///
    /// The dialect set was extracted from a production Loader that handled Chinese
    /// equipment STDF variants. Note: STDF V4 standard codes are PCR=(1,30), HBR=(1,40),
    /// SBR=(1,50); some equipment instead emits PCR=(5,1), HBR=(1,1), SBR=(1,2).
    /// </summary>
    public static class DialectExamples
    {
        /// <summary>
        /// Register all Loader-known dialects onto <paramref name="registry"/>. WARNING:
        /// this includes (20,10)→PTR, which conflicts with the standard BPS record (20,10).
        /// Only use this if your input files actually use these dialects and never contain BPS.
        /// </summary>
        public static void RegisterLoaderKnownDialects(RecordTypeRegistry registry)
        {
            // PTR dialects
            registry.Register(20, 10, new PtrReader()); // conflicts with BPS — see warning
            registry.Register(10, 10, new PtrReader()); // conflicts with TSR (10,30)? no, sub differs
            // PCR dialect
            registry.Register(5, 1, new PcrReader());
            // HBR dialect
            registry.Register(1, 1, new HbrReader());
            // SBR dialect
            registry.Register(1, 2, new SbrReader());
        }

        /// <summary>
        /// Register only the dialects that do NOT collide with any standard V4 (TYP,SUB).
        /// Safer than <see cref="RegisterLoaderKnownDialects"/>: omits (20,10)→PTR.
        /// </summary>
        public static void RegisterSafeDialects(RecordTypeRegistry registry)
        {
            registry.Register(10, 10, new PtrReader());
            registry.Register(5, 1, new PcrReader());
            registry.Register(1, 1, new HbrReader());
            registry.Register(1, 2, new SbrReader());
        }
    }
}
