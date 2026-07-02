namespace STDFParser4Net.Records
{
    /// <summary>
    /// MRR — Master Results Record (1,20). Marks the end of a logical test execution
    /// and carries summary disposition / description strings.
    /// </summary>
    public sealed class MrrRecord : StdfRecord
    {
        /// <summary>FINISH_T U4: UNIX-style time stamp of test completion.</summary>
        public uint FinishT { get; }

        /// <summary>DISP_COD C1: lot disposition code. Optional; default ' '.</summary>
        public char DispCod { get; }

        /// <summary>USR_DESC Cn: user-supplied lot description. Optional.</summary>
        public StdfString UsrDesc { get; }

        /// <summary>EXC_DESC Cn: executive lot description. Optional.</summary>
        public StdfString ExcDesc { get; }

        public MrrRecord(in StdfRecordHeader header, uint finishT, char dispCod, StdfString usrDesc, StdfString excDesc)
            : base(RecordType.MRR, header)
        {
            FinishT = finishT;
            DispCod = dispCod;
            UsrDesc = usrDesc;
            ExcDesc = excDesc;
        }

        public override string ToString()
            => $"MRR FinishT={FinishT} DispCod='{DispCod}' UsrDesc=\"{UsrDesc}\" ExcDesc=\"{ExcDesc}\"";
    }
}
