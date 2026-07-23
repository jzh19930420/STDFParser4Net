# STDFParserDemo

Minimal console sample that uses **STDFParser4Net** to stream-parse an STDF V4 file and print each record.

## Run

From the repository root:

```bash
dotnet run --project samples/STDFParserDemo
```

Parses the bundled `data/sample.std` (FAR + MIR + PIR + PTR + PRR + MRR).

Pass your own file:

```bash
dotnet run --project samples/STDFParserDemo -- path\to\lot123.std
```

## What it shows

- Streaming API: `foreach (var record in new StdfParser().Parse(path))`
- Pattern matching on concrete record types (`MirRecord`, `PrrRecord`, …)
- A per-type count summary at the end
