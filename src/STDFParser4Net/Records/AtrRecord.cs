namespace STDFParser4Net.Records
{
    /// <summary>
    /// ATR — Audit Trail Record (0,20). Records a command-line invocation that
    /// modified the file after initial creation.
    /// </summary>
    public sealed class AtrRecord : StdfRecord
    {
        /// <summary>MOD_TIM U4: UNIX-style time stamp of the modification.</summary>
        public uint ModTim { get; }

        /// <summary>CMD_LINE Cn: command line that produced the file modification.</summary>
        public StdfString CmdLine { get; }

        public AtrRecord(in StdfRecordHeader header, uint modTim, StdfString cmdLine)
            : base(RecordType.ATR, header)
        {
            ModTim = modTim;
            CmdLine = cmdLine;
        }

        public override string ToString()
            => $"ATR ModTim={ModTim} CmdLine=\"{CmdLine}\"";
    }
}
