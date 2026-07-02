using STDFParser4Net.Records.Readers;

namespace STDFParser4Net.Records.Registry
{
    /// <summary>
    /// Registers the standard STDF V4 record readers into a <see cref="RecordTypeRegistry"/>.
    /// </summary>
    internal static class StandardRecordReaders
    {
        public static void RegisterAll(RecordTypeRegistry registry)
        {
            // File attributes
            registry.Register(0, 10, new FarReader());
            // Execution / file-information group
            registry.Register(0, 20, new AtrReader());
            registry.Register(1, 10, new MirReader());
            registry.Register(1, 20, new MrrReader());
            registry.Register(1, 30, new PcrReader());
            registry.Register(1, 40, new HbrReader());
            registry.Register(1, 50, new SbrReader());
            registry.Register(1, 70, new RdrReader());
            registry.Register(20, 10, new BpsReader());
            registry.Register(20, 20, new EpsReader());
            registry.Register(30, 10, new DtrReader());
            // Wafer / part / site group records
            registry.Register(1, 80, new SdrReader());
            registry.Register(2, 10, new WirReader());
            registry.Register(2, 20, new WrrReader());
            registry.Register(2, 30, new WcrReader());
            registry.Register(5, 10, new PirReader());
            registry.Register(5, 20, new PrrReader());
            registry.Register(10, 30, new TsrReader());
            // Pin / parametric / functional / generic group
            registry.Register(1, 60, new PmrReader());
            registry.Register(1, 62, new PgrReader());
            registry.Register(1, 63, new PlrReader());
            registry.Register(15, 10, new PtrReader());
            registry.Register(15, 15, new MprReader());
            registry.Register(15, 30, new FtrReader());
            registry.Register(50, 10, new GdrReader());
        }
    }
}
