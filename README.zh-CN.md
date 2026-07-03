# STDFParser4Net

[English](README.md) | 简体中文

一个纯净、流式的 **STDF V4** 二进制解析器，面向 .NET。

[![NuGet](https://img.shields.io/nuget/v/STDFParser4Net.svg)](https://www.nuget.org/packages/STDFParser4Net/)

- 目标框架：**.NET Standard 2.1**（可被 .NET Core 3.0+ / .NET 5+ / .NET 6/7/8 消费）。
- **流式**：`IEnumerable<StdfRecord>` 拉模型 —— 内存占用与文件大小无关。
- **纯净**：不含单位换算、不含硬编码文本编码、不含 DataTable/良率/Cpk/数据库逻辑。字段值按文件原始存储原样返回。
- **字节序自适应**：读取 `FAR.CPU_TYPE` 自动切换大小端（支持大端与小端文件）。
- **可扩展**：标准记录通过注册表分发；机台方言与自定义记录可注册。
- **无损字符串**：`Cn`/`C1` 字段同时保留原始字节与解码文本。

> 本库只做解析，不写 STDF 文件，不做任何业务层转换。

## 安装

包已发布到 [NuGet](https://www.nuget.org/packages/STDFParser4Net/)：

```bash
dotnet add package STDFParser4Net
```

## 快速开始

```csharp
using STDFParser4Net;
using STDFParser4Net.Records;

var parser = new StdfParser();

// 流式：逐条迭代记录
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

// 或一次性读取全部
var all = parser.ParseAll(@"C:\data\lot123.std");
```

## 字节序

STDF V4 文件的第一条记录必须是 `FAR`。解析器读取 `FAR.CPU_TYPE` 并据此设置整文件的字节序。
大端与小端文件均支持，且字段值完全一致。

非 V4 文件（FAR.STDF_VER != 4）会抛出 `UnsupportedStdfVersionException`。

## 文本编码

默认 `Cn`/`C1` 字符串按 ASCII 解码，且任何大于 0x7F 的字节按 1:1 保留（无损 —— 不产生替换字符）。
可注入任意 `System.Text.Encoding` 以适配厂商特定编码：

```csharp
// 使用 UTF-8
var opts = new StdfParserOptions().WithEncoding(System.Text.Encoding.UTF8);
var parser = new StdfParser(opts);
```

### GB18030（可选）

GB18030 不内置在核心库中（否则需依赖 `System.Text.Encoding.CodePages`）。
如需启用，在你的消费项目里添加该包、注册 provider，再注入编码：

```csharp
// 在你的应用中（不在 STDFParser4Net 内）：
//  dotnet add package System.Text.Encoding.CodePages
using System.Text;
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var opts = new StdfParserOptions().WithEncoding(Encoding.GetEncoding("GB18030"));
var parser = new StdfParser(opts);
```

## 机台方言

默认使用 STDF V4 标准 (TYP,SUB) 编码。某些设备会发出非标编码；将其注册到注册表即可解析这类文件：

```csharp
var opts = new StdfParserOptions();
DialectExamples.RegisterSafeDialects(opts.Registry);
var parser = new StdfParser(opts);
```

`DialectExamples.RegisterLoaderKnownDialects` 注册某生产 Loader 中观察到的全部方言集
（PTR=(20,10)/(10,10)、PCR=(5,1)、HBR=(1,1)、SBR=(1,2)）。注意 `(20,10)` 与标准 BPS 记录冲突 ——
除非你确定输入文件使用该方言且不含 BPS，否则请使用 `RegisterSafeDialects`（已剔除该冲突项）。

你也可以为任意 (TYP,SUB) 注册自定义 Reader：

```csharp
opts.Registry.Register(typ, sub, new MyCustomReader());
```

## REC_LEN 模式

默认假定 `REC_LEN` 只计记录体字节数（STDF V4 标准）。若你的文件采用不同约定，请显式设置：

```csharp
var opts = new StdfParserOptions().WithRecLenMode(RecLenMode.BodyPlusTypSub);
```

## 记录覆盖范围（STDF V4）

| 类型  | (TYP,SUB) | 记录      |
| --- | --------- | ------- |
| FAR | (0,10)    | 文件属性    |
| ATR | (0,20)    | 审计追踪    |
| MIR | (1,10)    | 主信息     |
| MRR | (1,20)    | 主结果     |
| PCR | (1,30)    | 部件计数    |
| HBR | (1,40)    | 硬件 Bin  |
| SBR | (1,50)    | 软件 Bin  |
| PMR | (1,60)    | 引脚映射    |
| PGR | (1,62)    | 引脚组     |
| PLR | (1,63)    | 引脚列表    |
| RDR | (1,70)    | 复测数据    |
| SDR | (1,80)    | 站点描述    |
| WIR | (2,10)    | 晶圆信息    |
| WRR | (2,20)    | 晶圆结果    |
| WCR | (2,30)    | 晶圆配置    |
| PIR | (5,10)    | 部件信息    |
| PRR | (5,20)    | 部件结果    |
| BPS | (20,10)   | 程序段开始   |
| EPS | (20,20)   | 程序段结束   |
| GDR | (50,10)   | 通用数据    |
| DTR | (30,10)   | 数据日志文本  |
| TSR | (10,30)   | 测试概要    |
| PTR | (15,10)   | 参数测试    |
| MPR | (15,15)   | 多结果参数测试 |
| FTR | (15,30)   | 功能测试    |

未识别的 (TYP,SUB) 对会以 `UnknownRecord` 返回，保留原始体字节；迭代不会被中断。

标志位字段（如 `TEST_FLG`、`PARM_FLG`、`OPT_FLAG`、`PART_FLG`）同时以原始 `byte` 和
`[Flags]` 枚举形式暴露，枚举位于 `STDFParser4Net.Enums` 命名空间。

## API 概览

- `StdfParser` —— 入口。`Parse(path/stream)` → `IEnumerable<StdfRecord>`；`ParseAll(...)` → `IReadOnlyList<StdfRecord>`。
- `StdfRecord` —— 抽象基类；具体记录在 `STDFParser4Net.Records`。
- `StdfParserOptions` —— `Encoding`、`RecLenMode`、`Registry`、`ErrorMode`。
- `RecordTypeRegistry` —— (TYP,SUB) → `IRecordReader` 分发；`CreateDefault()` 返回含全部 V4 记录的注册表。
- `StdfBinaryReader` —— 字节序感知原语读取器（`ReadU1/U2/U4`、`ReadCn`、`ReadBn`、`ReadDn`、`ReadN1Array`、`ReadArray<T>` 等）。
- `StdfString` —— 原始字节 + 解码文本（无损）。
- 异常：`StdfException`、`UnsupportedStdfVersionException`、`MissingFarRecordException`、`UnexpectedEndOfStreamException`。

## 限制

- 仅支持 STDF **V4**（不支持 V3/GTF）。
- 只解析，不提供写入器。
- `REC_LEN` 模式不自动探测；默认为仅计记录体。非标文件请显式设置。
- 可选/条件字段（如 TSR/PTR/MPR 中由 `OPT_FLAG` 控制的字段）在存在时按标志位读取；缺失时为 `null`。

## 许可

MIT —— 详见 [LICENSE](LICENSE)。
