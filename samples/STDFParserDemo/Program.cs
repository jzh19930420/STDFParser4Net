using System.Diagnostics;
using STDFParser4Net;
using STDFParser4Net.Records;

// ---------------------------------------------------------------------------
// STDFParser4Net demo
//
// Usage:
//   dotnet run --project samples/STDFParserDemo
//   dotnet run --project samples/STDFParserDemo -- path\to\file.std
//
// With no args, parses the bundled data/sample.std.
// ---------------------------------------------------------------------------

string path = ResolveInputPath(args);
if (!File.Exists(path))
{
    Console.Error.WriteLine($"File not found: {path}");
    return 1;
}

Console.WriteLine($"Parsing: {path}");
Console.WriteLine(new string('-', 72));

var parser = new StdfParser();
var sw = Stopwatch.StartNew();

int total = 0;
var counts = new Dictionary<RecordType, int>();

foreach (var record in parser.Parse(path))
{
    total++;
    counts.TryGetValue(record.RecordType, out int n);
    counts[record.RecordType] = n + 1;

    // Print a concise, record-specific line. Every StdfRecord has a useful ToString().
    Console.WriteLine(Format(record));
}

sw.Stop();

Console.WriteLine(new string('-', 72));
Console.WriteLine($"Done. {total} record(s) in {sw.Elapsed.TotalMilliseconds:F1} ms.");
if (counts.Count > 0)
{
    Console.WriteLine("By type:");
    foreach (var kv in counts.OrderBy(k => k.Key.ToString()))
        Console.WriteLine($"  {kv.Key,-6} {kv.Value,6}");
}

return 0;

// ---- helpers ---------------------------------------------------------------

static string ResolveInputPath(string[] args)
{
    if (args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
        return Path.GetFullPath(args[0]);

    // Prefer the sample next to the executable (copied by the csproj).
    string besideExe = Path.Combine(AppContext.BaseDirectory, "data", "sample.std");
    if (File.Exists(besideExe))
        return besideExe;

    // Fall back to source-tree relative path when running under `dotnet run`.
    return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "data", "sample.std"));
}

static string Format(StdfRecord record) => record switch
{
    FarRecord far =>
        $"[FAR] CPU_TYPE=0x{far.CpuType:X2} ({(far.IsLittleEndian ? "LE" : "BE")}) STDF_VER={far.StdfVer}",

    MirRecord mir =>
        $"[MIR] LOT_ID={mir.LotId.Text} PART_TYP={mir.PartTyp.Text} " +
        $"SETUP_T={mir.SetupT} START_T={mir.StartT} MODE_COD='{mir.ModeCod}'",

    PirRecord pir =>
        $"[PIR] HEAD={pir.HEAD_NUM} SITE={pir.SITE_NUM}",

    PtrRecord ptr =>
        $"[PTR] TEST_NUM={ptr.TEST_NUM} HEAD={ptr.HEAD_NUM} SITE={ptr.SITE_NUM} " +
        $"RESULT={ptr.RESULT} TXT={ptr.TEST_TXT?.Text ?? "-"} UNITS={ptr.UNITS?.Text ?? "-"} " +
        $"LO={ptr.LO_LIMIT?.ToString() ?? "-"} HI={ptr.HI_LIMIT?.ToString() ?? "-"}",

    PrrRecord prr =>
        $"[PRR] PART_NUM={prr.PART_NUM} HEAD={prr.HEAD_NUM} SITE={prr.SITE_NUM} " +
        $"HARD_BIN={prr.HARD_BIN} SOFT_BIN={prr.SOFT_BIN} " +
        $"{(prr.IsPass ? "PASS" : "FAIL")} PART_ID={prr.PART_ID?.Text ?? "-"}",

    MrrRecord mrr =>
        $"[MRR] FINISH_T={mrr.FinishT} DISP_COD='{mrr.DispCod}' " +
        $"USR_DESC={mrr.UsrDesc.Text} EXC_DESC={mrr.ExcDesc.Text}",

    // Fallback: every record type implements a useful ToString().
    _ => $"[{record.RecordType}] {record}"
};
