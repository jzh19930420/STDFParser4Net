using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using STDFParser4Net;
using STDFParser4Net.Records;
using Xunit;

namespace STDFParser4Net.Tests
{
    /// <summary>
    /// Optional integration tests against real production STDF files on the local machine.
    /// Skipped automatically when the file is not present (CI / other machines).
    /// Override paths with env vars STDF_REAL_SAMPLE_1 / _2 / _3.
    /// </summary>
    public class RealStdfFileTests
    {
        private static readonly string[] DefaultPaths =
        {
            @"D:\WorkSpace\TestSpace\WJS\数据\AAA-260721\AAA-260721\S1XKA217ZA20-AAA-260721_26.stdf",
            @"D:\share\WJS\backup\2026\IP\S1XSD03J\S1XSD03J-63300A\S1XSD03J-63300A_18.STD",
            @"D:\share\WJS\backup\2026\IP\W1XT561N\W1XT561N-65489B\W1XT561N-65489B_17.stdf",
        };

        public static IEnumerable<object[]> RealFilePaths()
        {
            for (int i = 0; i < DefaultPaths.Length; i++)
            {
                string env = Environment.GetEnvironmentVariable($"STDF_REAL_SAMPLE_{i + 1}") ?? DefaultPaths[i];
                yield return new object[] { env };
            }
        }

        [Theory]
        [MemberData(nameof(RealFilePaths))]
        public void ParsesRealFileCompletely(string path)
        {
            if (!File.Exists(path))
            {
                // Soft-skip when the real sample is not on this machine.
#pragma warning disable xUnit1004
                // Prefer explicit skip when Xunit.SkippableFact is unavailable.
#pragma warning restore xUnit1004
                Assert.True(true, $"Skipped (file missing): {path}");
                return;
            }

            var parser = new StdfParser();
            int total = 0;
            int ptrCount = 0;
            int unknownCount = 0;
            FarRecord? far = null;
            MirRecord? mir = null;
            PtrRecord? samplePtr = null;

            foreach (var r in parser.Parse(path))
            {
                total++;
                switch (r)
                {
                    case FarRecord f: far ??= f; break;
                    case MirRecord m: mir ??= m; break;
                    case PtrRecord p:
                        ptrCount++;
                        samplePtr ??= p;
                        break;
                    case UnknownRecord: unknownCount++; break;
                }
            }

            Assert.True(total > 0, $"No records parsed from {path}");
            Assert.NotNull(far);
            Assert.Equal(4, far!.StdfVer);
            Assert.NotNull(mir);
            Assert.True(ptrCount > 0, $"Expected at least one PTR in {path}");
            // Real files under test should not flood Unknown records after the PTR fix.
            double unknownRatio = (double)unknownCount / total;
            Assert.True(unknownRatio < 0.01,
                $"Unknown ratio {unknownRatio:P1} too high ({unknownCount}/{total}) in {path}");

            Assert.NotNull(samplePtr);
            // At least one of the common optional fields should be populated on first PTR.
            Assert.True(
                samplePtr!.TEST_TXT != null || samplePtr.UNITS != null || samplePtr.OPT_FLAG != null,
                "Sample PTR has no optional fields populated");
        }
    }
}
