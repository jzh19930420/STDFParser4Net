namespace STDFParser4Net
{
    /// <summary>
    /// Logical record type, derived from the (REC_TYP, REC_SUB) pair per STDF V4.
    /// <see cref="Unknown"/> is used for any (TYP,SUB) the registry does not recognize.
    /// </summary>
    public enum RecordType
    {
        Unknown = 0,
        FAR,    // 0,10  File Attributes Record
        ATR,    // 0,20  Audit Trail Record
        MIR,    // 1,10  Master Information Record
        MRR,    // 1,20  Master Results Record
        PCR,    // 1,30  Part Count Record
        HBR,    // 1,40  Hardware Bin Record
        SBR,    // 1,50  Software Bin Record
        PMR,    // 1,60  Pin Map Record
        PGR,    // 1,62  Pin Group Record
        PLR,    // 1,63  Pin List Record
        RDR,    // 1,70  Retest Data Record
        SDR,    // 1,80  Site Description Record
        WIR,    // 2,10  Wafer Information Record
        WRR,    // 2,20  Wafer Results Record
        WCR,    // 2,30  Wafer Configuration Record
        PIR,    // 5,10  Part Information Record
        PRR,    // 5,20  Part Results Record
        BPS,    // 20,10 Begin Program Section Record
        EPS,    // 20,20 End Program Section Record
        GDR,    // 50,10 Generic Data Record
        DTR,    // 30,10 Datalog Text Record
        TSR,    // 10,30 Test Synopsis Record
        PTR,    // 15,10 Parametric Test Record
        MPR,    // 15,15 Multiple Result Parametric Record
        FTR     // 15,30 Functional Test Record
    }
}
