namespace STDFParser4Net.Records
{
    /// <summary>
    /// MIR — Master Information Record (1,10). Carries lot-level identification and
    /// environment metadata. The first 8 fields are always present; the remaining 30
    /// Cn fields are optional and may be absent in truncated files. Optional fields
    /// that were not present in the file are left at <c>default(StdfString)</c>.
    /// </summary>
    public sealed class MirRecord : StdfRecord
    {
        // ---- 8 fixed fields (always present) ----

        /// <summary>SETUP_T U4: time of program setup.</summary>
        public uint SetupT { get; }

        /// <summary>START_T U4: time of test start.</summary>
        public uint StartT { get; }

        /// <summary>STAT_NUM U1: test station number.</summary>
        public byte StatNum { get; }

        /// <summary>MODE_COD C1: test mode code.</summary>
        public char ModeCod { get; }

        /// <summary>RTST_COD C1: retest code.</summary>
        public char RtstCod { get; }

        /// <summary>PROT_COD C1: protection code.</summary>
        public char ProtCod { get; }

        /// <summary>BURN_TIM U2: burn-in time in minutes.</summary>
        public ushort BurnTim { get; }

        /// <summary>CMOD_COD C1: command mode code.</summary>
        public char CmodCod { get; }

        // ---- 30 optional Cn fields (present in order; missing ones are default) ----

        /// <summary>LOT_ID Cn. Optional.</summary>
        public StdfString LotId { get; }
        /// <summary>PART_TYP Cn. Optional.</summary>
        public StdfString PartTyp { get; }
        /// <summary>NODE_NAM Cn. Optional.</summary>
        public StdfString NodeNam { get; }
        /// <summary>TSTR_TYP Cn. Optional.</summary>
        public StdfString TstrTyp { get; }
        /// <summary>JOB_NAM Cn. Optional.</summary>
        public StdfString JobNam { get; }
        /// <summary>JOB_REV Cn. Optional.</summary>
        public StdfString JobRev { get; }
        /// <summary>SBLOT_ID Cn. Optional.</summary>
        public StdfString SblotId { get; }
        /// <summary>OPER_NAM Cn. Optional.</summary>
        public StdfString OperNam { get; }
        /// <summary>EXEC_TYP Cn. Optional.</summary>
        public StdfString ExecTyp { get; }
        /// <summary>EXEC_VER Cn. Optional.</summary>
        public StdfString ExecVer { get; }
        /// <summary>TEST_COD Cn. Optional.</summary>
        public StdfString TestCod { get; }
        /// <summary>TST_TEMP Cn. Optional.</summary>
        public StdfString TstTemp { get; }
        /// <summary>USER_TXT Cn. Optional.</summary>
        public StdfString UserTxt { get; }
        /// <summary>AUX_FILE Cn. Optional.</summary>
        public StdfString AuxFile { get; }
        /// <summary>PKG_TYP Cn. Optional.</summary>
        public StdfString PkgTyp { get; }
        /// <summary>FAMLY_ID Cn. Optional.</summary>
        public StdfString FamlyId { get; }
        /// <summary>DATE_COD Cn. Optional.</summary>
        public StdfString DateCod { get; }
        /// <summary>FACIL_ID Cn. Optional.</summary>
        public StdfString FacilId { get; }
        /// <summary>FLOOR_ID Cn. Optional.</summary>
        public StdfString FloorId { get; }
        /// <summary>PROC_ID Cn. Optional.</summary>
        public StdfString ProcId { get; }
        /// <summary>OPER_FRQ Cn. Optional.</summary>
        public StdfString OperFrq { get; }
        /// <summary>SPEC_NAM Cn. Optional.</summary>
        public StdfString SpecNam { get; }
        /// <summary>SPEC_VER Cn. Optional.</summary>
        public StdfString SpecVer { get; }
        /// <summary>FLOW_ID Cn. Optional.</summary>
        public StdfString FlowId { get; }
        /// <summary>SETUP_ID Cn. Optional.</summary>
        public StdfString SetupId { get; }
        /// <summary>DSGN_REV Cn. Optional.</summary>
        public StdfString DsgnRev { get; }
        /// <summary>ENG_ID Cn. Optional.</summary>
        public StdfString EngId { get; }
        /// <summary>ROM_COD Cn. Optional.</summary>
        public StdfString RomCod { get; }
        /// <summary>SERL_NUM Cn. Optional.</summary>
        public StdfString SerlNum { get; }
        /// <summary>SUPR_NAM Cn. Optional.</summary>
        public StdfString SuprNam { get; }

        public MirRecord(in StdfRecordHeader header,
            uint setupT, uint startT, byte statNum,
            char modeCod, char rtstCod, char protCod, ushort burnTim, char cmodCod,
            StdfString lotId, StdfString partTyp, StdfString nodeNam, StdfString tstrTyp,
            StdfString jobNam, StdfString jobRev, StdfString sblotId, StdfString operNam,
            StdfString execTyp, StdfString execVer, StdfString testCod, StdfString tstTemp,
            StdfString userTxt, StdfString auxFile, StdfString pkgTyp, StdfString famlyId,
            StdfString dateCod, StdfString facilId, StdfString floorId, StdfString procId,
            StdfString operFrq, StdfString specNam, StdfString specVer, StdfString flowId,
            StdfString setupId, StdfString dsgnRev, StdfString engId, StdfString romCod,
            StdfString serlNum, StdfString suprNam)
            : base(RecordType.MIR, header)
        {
            SetupT = setupT;
            StartT = startT;
            StatNum = statNum;
            ModeCod = modeCod;
            RtstCod = rtstCod;
            ProtCod = protCod;
            BurnTim = burnTim;
            CmodCod = cmodCod;
            LotId = lotId;
            PartTyp = partTyp;
            NodeNam = nodeNam;
            TstrTyp = tstrTyp;
            JobNam = jobNam;
            JobRev = jobRev;
            SblotId = sblotId;
            OperNam = operNam;
            ExecTyp = execTyp;
            ExecVer = execVer;
            TestCod = testCod;
            TstTemp = tstTemp;
            UserTxt = userTxt;
            AuxFile = auxFile;
            PkgTyp = pkgTyp;
            FamlyId = famlyId;
            DateCod = dateCod;
            FacilId = facilId;
            FloorId = floorId;
            ProcId = procId;
            OperFrq = operFrq;
            SpecNam = specNam;
            SpecVer = specVer;
            FlowId = flowId;
            SetupId = setupId;
            DsgnRev = dsgnRev;
            EngId = engId;
            RomCod = romCod;
            SerlNum = serlNum;
            SuprNam = suprNam;
        }

        public override string ToString()
            => $"MIR SetupT={SetupT} StartT={StartT} StatNum={StatNum} LotId=\"{LotId}\" PartTyp=\"{PartTyp}\"";
    }
}
