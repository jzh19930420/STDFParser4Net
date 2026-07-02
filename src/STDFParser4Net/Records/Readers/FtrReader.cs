using STDFParser4Net.IO;
using STDFParser4Net.Records.Registry;

namespace STDFParser4Net.Records.Readers
{
    internal sealed class FtrReader : IRecordReader
    {
        public StdfRecord Read(StdfBinaryReader reader, in StdfRecordHeader header)
        {
            reader.BeginRecord(header);

            // Fixed fields: TEST_NUM, HEAD_NUM, SITE_NUM, TEST_FLG, OPT_FLAG
            uint testNum = reader.ReadU4();
            byte headNum = reader.ReadU1();
            byte siteNum = reader.ReadU1();
            byte testFlg = reader.ReadU1();
            byte optFlag = reader.ReadU1();

            // Fixed fields: CYCL_CNT, REL_VADR, REPT_CNT, NUM_FAIL, XFAIL_AD, YFAIL_AD, VECT_OFF
            uint cyclCnt = reader.ReadU4();
            uint relVadr = reader.ReadU4();
            uint rptCnt = reader.ReadU4();
            uint numFail = reader.ReadU4();
            int xfailAd = reader.ReadI4();
            int yfailAd = reader.ReadI4();
            short vectOff = reader.ReadI2();

            // Return pin group: RTN_ICNT, RTN_INDX[kxU2], RTN_STAT[kxB1] (conditional)
            ushort rtnIcnt = reader.ReadU2();
            ushort[] rtnIndx = reader.ReadArray(rtnIcnt, () => reader.ReadU2());
            byte[]? rtnStat = null;
            if ((testFlg & 0x08) == 0)
                rtnStat = reader.ReadArray(rtnIcnt, () => reader.ReadU1());

            // Programmed pin group: PGM_ICNT, PGM_INDX[kxU2], PGM_STAT[kxB1]
            ushort pgmIcnt = reader.ReadU2();
            ushort[] pgmIndx = reader.ReadArray(pgmIcnt, () => reader.ReadU2());
            byte[] pgmStat = reader.ReadArray(pgmIcnt, () => reader.ReadU1());

            // Fail pin group: FAIL_ICNT, FAIL_INDX[kxU2], FAIL_STAT[kxB1]
            ushort failIcnt = reader.ReadU2();
            ushort[] failIndx = reader.ReadArray(failIcnt, () => reader.ReadU2());
            byte[] failStat = reader.ReadArray(failIcnt, () => reader.ReadU1());

            // Trailing optional Cn fields
            StdfString? vectNam = null;
            if (reader.HasRemaining(1))
                vectNam = reader.ReadCn();

            StdfString? alarmId = null;
            if (reader.HasRemaining(1))
                alarmId = reader.ReadCn();

            StdfString? progTxt = null;
            if (reader.HasRemaining(1))
                progTxt = reader.ReadCn();

            StdfString? rsltTxt = null;
            if (reader.HasRemaining(1))
                rsltTxt = reader.ReadCn();

            // Optional scan-pin group: SPIN_CNT, SPIN_INDX[kxU4]
            uint? spinCnt = null;
            uint[]? spinIndx = null;
            if (reader.HasRemaining(4))
            {
                spinCnt = reader.ReadU4();
                spinIndx = reader.ReadArray((int)spinCnt.Value, () => reader.ReadU4());
            }

            return new FtrRecord(header, testNum, headNum, siteNum, testFlg, optFlag,
                cyclCnt, relVadr, rptCnt, numFail, xfailAd, yfailAd, vectOff,
                rtnIcnt, rtnIndx, rtnStat,
                pgmIcnt, pgmIndx, pgmStat,
                failIcnt, failIndx, failStat,
                vectNam, alarmId, progTxt, rsltTxt,
                spinCnt, spinIndx);
        }
    }
}
