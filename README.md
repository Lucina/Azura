# Azura
 source generation based serialization

| Package                | Release |
|------------------------|---------|
| `Azura`           | [![NuGet](https://img.shields.io/nuget/v/Azura.svg)](https://www.nuget.org/packages/Azura/)|
| `Azura.Generator` | [![NuGet](https://img.shields.io/nuget/v/Azura.Generator.svg)](https://www.nuget.org/packages/Azura.Generator/) |

## Why?

Generating source code instead of relying on reflection is nice. For example,
when using NativeAOT you should be able to strip reflection metadata and
still have serialization work, as everything is hooked up at design time.

Target use case is communication between game clients and third-party programs.

* Supports fields / properties (including init-only) on structs/classes/records
* Has default serializers for common BCL types
  - `byte` / `sbyte` / `ushort` / `short` / `uint` / `int` / `ulong` / `long`
  - `float` / `double` / `char` / `bool`
  - `Guid` / `TimeSpan` / `DateTime` / `decimal`
  - `string`
  - `T[]` / `List<T>` / `HashSet<T>` / `Dictionary<TKey, TValue>`
    - Includes faster path for arrays of primitives
* Supports nullable types (value types, reference types in nullable context)
* Supports enums as members (does not generate top-level serialization helpers)
* Native endianness conversion (stored as little-endian)
* `ref` passing of members when appropriate

## Limitations

* Type must have parameterless constructor to support serializer generation
* Cannot serialize generic types
* Cannot (currently) serialize nested classes

## Usage

0. Add generator library (includes base package, does not need to be explicitly included)

```xml
<PackageReference Include="Azura.Generator" Version="version" />
```

1. Define a data type and mark properties or fields

```csharp
using Azura;
[Azura]
public record Data
{
    [Azura] public int Property1 { get; init; }
    [Azura] public string? Property2 { get; init; }
    [Azura] public string?[]? Property3 { get; init; }
}
```

2. Serialize the data

```csharp
var data = new Data {Property2 = "Execute order", Property1 = 66};
data.Serialize(stream);
```

3. Retrieve the data
   - Helper types are generated as `<ClassName>Serialization`

```csharp
var data = DataSerialization.Deserialize(stream);
```

## Binary format

* Numeric types are little-endian.
* Primitives are serialized as expected.
* Nullable types are prefixed with a byte flag.
* Collections and strings are prefixed with an s32 count followed by
  elements serialized contiguously (if a supported type parameter is
  nullable, those values are prefixed with a byte flag).

## Custom serialization

Custom serialization for existing library types can be added but requires
manual implementation of the below signatures in a class under the target
type's namespace named `<ClassName>Serialization`.

Value types

```csharp
public static T Deserialize(Stream stream);
public static void Serialize(T self, Stream stream) => self.Serialize(stream);
public static void Serialize(this ref T self, Stream stream);
```

Reference types

```csharp
public static T Deserialize(Stream stream);
public static void Serialize(this T self, Stream stream) => Serialize(ref self, stream);
public static void Serialize(ref T self, Stream stream);
```
