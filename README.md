# STDFParser4Net

English | [简体中文](README.zh-CN.md)

A pure, streaming **STDF V4** binary parser for .NET.

[![NuGet](https://img.shields.io/nuget/v/STDFParser4Net.svg)](https://www.nuget.org/packages/STDFParser4Net/)

- Target framework: **.NET Standard 2.1** (consumable by .NET Core 3.0+ / .NET 5+ / .NET 6/7/8).
- **Streaming**: `IEnumerable<StdfRecord>` pull model — memory independent of file size.
- **Pure**: no unit conversion, no hardcoded text encoding, no DataTable/yield/Cpk/database logic. Original field values are returned as stored.
- **Endian-safe**: reads `FAR.CPU_TYPE` and switches byte order automatically (big- or little-endian).
- **Extensible**: standard record dispatch via a registry; equipment dialects and custom records can be registered.
- **Lossless strings**: `Cn`/`C1` fields keep both raw bytes and decoded text.

> This library parses only. It does not write STDF files and does not perform any business-level transformation.

## Installation

Available on [NuGet](https://www.nuget.org/packages/STDFParser4Net/):

```bash
dotnet add package STDFParser4Net
```

## Quick start

```csharp
using STDFParser4Net;
using STDFParser4Net.Records;

var parser = new StdfParser();

// Streaming: iterate records one at a time
foreach (var record in parser.Parse(@"C:\data\lot123.std"))
{
    switch (record)
    {
        case MirRecord mir:
            Console.WriteLine($"Lot: {mir.LOT_ID}");
            break;
        case PrrRecord prr:
            Console.WriteLine($"Part {prr.PART_NUM} {(prr.IsPass ? "PASS" : "FAIL")}");
            break;
        case PtrRecord ptr:
            Console.WriteLine($"Test {ptr.TEST_NUM} = {ptr.RESULT}");
            break;
    }
}

// Or read everything at once
var all = parser.ParseAll(@"C:\data\lot123.std");
```

## Sample

A runnable console demo lives under [`samples/STDFParserDemo`](samples/STDFParserDemo):

```bash
dotnet run --project samples/STDFParserDemo
# or with your own file:
dotnet run --project samples/STDFParserDemo -- path\to\lot123.std
```

## Endianness

The first record of an STDF V4 file must be `FAR`. The parser reads `FAR.CPU_TYPE` and sets
the byte order for the rest of the file. Both little-endian and big-endian files are supported
and yield identical field values.

Non-V4 files (FAR.STDF_VER != 4) raise `UnsupportedStdfVersionException`.

## Text encoding

By default `Cn`/`C1` strings decode as ASCII, and any byte > 0x7F is preserved 1:1 (lossless —
no replacement characters). Inject any `System.Text.Encoding` for vendor-specific encodings:

```csharp
// Use UTF-8
var opts = new StdfParserOptions().WithEncoding(System.Text.Encoding.UTF8);
var parser = new StdfParser(opts);
```

### GB18030 (optional)

GB18030 is not built into the core library (it would require `System.Text.Encoding.CodePages`).
To enable it, add that package to your consuming project, register the provider, and inject the
encoding:

```csharp
// In your app (not in STDFParser4Net):
//  dotnet add package System.Text.Encoding.CodePages
using System.Text;
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var opts = new StdfParserOptions().WithEncoding(Encoding.GetEncoding("GB18030"));
var parser = new StdfParser(opts);
```

## Equipment dialects

STDF V4 standard (TYP,SUB) codes are used by default. Some equipment emits non-standard codes;
register them onto the registry to parse those files:

```csharp
var opts = new StdfParserOptions();
DialectExamples.RegisterSafeDialects(opts.Registry);
var parser = new StdfParser(opts);
```

`DialectExamples.RegisterLoaderKnownDialects` adds the full dialect set observed in a production
Loader (PTR=(20,10)/(10,10), PCR=(5,1), HBR=(1,1), SBR=(1,2)). Note `(20,10)` collides with the
standard BPS record — use `RegisterSafeDialects` (which omits it) unless you are certain your
files use that dialect and contain no BPS.

You can also register your own reader for any (TYP,SUB):

```csharp
opts.Registry.Register(typ, sub, new MyCustomReader());
```

## REC_LEN mode

The default assumes `REC_LEN` counts only the record body (STDF V4 standard). If your files use
a different convention, set it explicitly:

```csharp
var opts = new StdfParserOptions().WithRecLenMode(RecLenMode.BodyPlusTypSub);
```

## Record coverage (STDF V4)

| Type | (TYP,SUB) | Record                     |
| ---- | --------- | -------------------------- |
| FAR  | (0,10)    | File Attributes            |
| ATR  | (0,20)    | Audit Trail                |
| MIR  | (1,10)    | Master Information         |
| MRR  | (1,20)    | Master Results             |
| PCR  | (1,30)    | Part Count                 |
| HBR  | (1,40)    | Hardware Bin               |
| SBR  | (1,50)    | Software Bin               |
| PMR  | (1,60)    | Pin Map                    |
| PGR  | (1,62)    | Pin Group                  |
| PLR  | (1,63)    | Pin List                   |
| RDR  | (1,70)    | Retest Data                |
| SDR  | (1,80)    | Site Description           |
| WIR  | (2,10)    | Wafer Information          |
| WRR  | (2,20)    | Wafer Results              |
| WCR  | (2,30)    | Wafer Configuration        |
| PIR  | (5,10)    | Part Information           |
| PRR  | (5,20)    | Part Results               |
| BPS  | (20,10)   | Begin Program Section      |
| EPS  | (20,20)   | End Program Section        |
| GDR  | (50,10)   | Generic Data               |
| DTR  | (30,10)   | Datalog Text               |
| TSR  | (10,30)   | Test Synopsis              |
| PTR  | (15,10)   | Parametric Test            |
| MPR  | (15,15)   | Multiple Result Parametric |
| FTR  | (15,30)   | Functional Test            |

Unrecognized (TYP,SUB) pairs are returned as `UnknownRecord` with the raw body bytes preserved;
iteration is not interrupted.

Flag fields (e.g. `TEST_FLG`, `PARM_FLG`, `OPT_FLAG`, `PART_FLG`) are exposed both as raw `byte`
and as `[Flags]` enums in the `STDFParser4Net.Enums` namespace.

## API overview

- `StdfParser` — entry point. `Parse(path/stream)` → `IEnumerable<StdfRecord>`; `ParseAll(...)` → `IReadOnlyList<StdfRecord>`.
- `StdfRecord` — abstract base; concrete records in `STDFParser4Net.Records`.
- `StdfParserOptions` — `Encoding`, `RecLenMode`, `Registry`, `ErrorMode`.
- `RecordTypeRegistry` — (TYP,SUB) → `IRecordReader` dispatch; `CreateDefault()` for all V4 records.
- `StdfBinaryReader` — endian-aware primitive reader (`ReadU1/U2/U4`, `ReadCn`, `ReadBn`, `ReadDn`, `ReadN1Array`, `ReadArray<T>`, ...).
- `StdfString` — raw bytes + decoded text (lossless).
- Exceptions: `StdfException`, `UnsupportedStdfVersionException`, `MissingFarRecordException`, `UnexpectedEndOfStreamException`.

## Limitations

- STDF **V4 only** (V3/GTF not supported).
- Parse-only — no writer.
- `REC_LEN` mode is not auto-detected; default is body-only. Set it explicitly for non-standard files.
- Optional/conditional fields (e.g. `OPT_FLAG`-gated fields in TSR/PTR/MPR) are read when present
  according to the flag bits; missing fields are `null`.

## License

MIT — see [LICENSE](LICENSE).
