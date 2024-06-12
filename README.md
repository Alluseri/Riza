# Riza
Lua string.pack/unpack implementation in C#, made primarily to assist the creation of private servers for Unity games with XLua backends.

Only x64 systems. I have no respect towards nuint.

**Please use MessagePack instead of Riza, unless you are using it purposefully(e.g. for XLua private servers). This project's intention and purpose is clear.**

## Major differences between string.pack and LuaPack
* LuaPack.Pack does not care if the format option is signed or not, while Lua will raise an Integer Overflow error if the value indeed overflows:
```lua
print(string.pack(">b", 254)); -- input:1: bad argument #2 to 'pack' (integer overflow)
```
* LuaPack does not and will most likely never support the `!` and `X` options.
* LuaPack does not support `j`, `J`, `n` and `z` options **yet**.
* LuaPack allows you to use byte counts higher than 16 in `i/I/s/c[n]` options, unlike Lua.
* All overloads of LuaPack.Unpack will NOT fail if the data feed ends prematurely.

## Usage
### Sample Class
**WARNING:** You cannot use `struct` with LuaPack directly because __makeref and imaginary constraints don't make my life any easier!
```cs
public class SampleClass {
  [RizaField(0)]
  public int SomeInt;
  [RizaField(1)]
  public string SomeString;
  [RizaField(2)]
  public ulong SomeUlong;
  public ulong NotDeserializedField;
  [RizaField(3)]
  public byte SomeByte;
}
```
### Packing
Use LuaPack.Pack* just like you would use string.pack in Lua.

For this piece of code:
```lua
print(string.pack(">I4B<iLs", 1, 2, 3, 4, "abc"));
```
The direct LuaPack representation would be:
```cs
Console.WriteLine(LuaPack.PackToString(">I4B<iLs", 1, 2, 3, 4, "abc"));
```
**Additionally, Riza supports packing entire objects:**
```cs
Console.WriteLine(LuaPack.PackToString(">I4c8Lb", new SampleClass() {
  SomeInt = 58,
  SomeString = "abcdefgh",
  SomeUlong = 0xDEADC0DECAFEBABE,
  SomeByte = 210
}));
```

### Unpacking
The primitive way, which is faster, but really annoying to work with, is this:
```cs
int SomeInt;
string SomeString;
ulong SomeUlong;
byte SomeByte;

List<object> Unpacked = LuaPack.Unpack(">I4c8Lb", SomePackedData);
SomeInt = (int) (ulong) Unpacked[0]; // You have to cast to ulong first... So bad.
SomeString = (string) Unpacked[1];
SomeUlong = (ulong) Unpacked[2];
SomeByte = (byte) Unpacked[3];
```
**And the better way(but slower, since it uses reflection) is this:**
```cs
SampleClass Sample = LuaPack.Unpack<SampleClass>(">I4c8Lb", SomePackedData); // T has a constraint of new()
SampleClass SameSample = LuaPack.Unpack<SampleClass>(">I4c8Lb", SomePackedData, new SampleClass());
```

## Performance Benchmarks
- CPU: AMD Ryzen 7 5800X
- OS: tiny10 22H2
- class SmallDummy (S):`>fB<i4s>c12l<hd` - 8 fields.
- class LargeDummy (L): `>bfB<i4Hs>c12I3l<Lhd` - 12 fields.
- class ExtraLargeDummy (XL): `>bfB<i4Hs>c12I3l>Lhd<bfB>i4Hs<c12I3l>Lhd` - 24 fields.
#### Pack vs Pack<T>
|       Method |        Mean |     Error |    StdDev |
|------------- |------------:|----------:|----------:|
|  ManualPackS |    177.6 ns |   3.38 ns |   3.32 ns |
|  ManualPackL |    233.1 ns |   2.72 ns |   2.41 ns |
| ManualPackXL |    427.7 ns |   4.55 ns |   3.80 ns |
|  SerialPackS |  5,390.1 ns |  29.54 ns |  23.06 ns |
|  SerialPackL |  7,996.1 ns |  66.19 ns |  55.27 ns |
| SerialPackXL | 15,757.9 ns | 164.70 ns | 128.58 ns |

#### Unpack vs Unpack<T>
|         Method |        Mean |     Error |    StdDev |
|--------------- |------------:|----------:|----------:|
|  ManualUnpackS |    177.1 ns |   2.50 ns |   2.09 ns |
|  ManualUnpackL |    238.9 ns |   2.58 ns |   2.29 ns |
| ManualUnpackXL |    453.6 ns |   9.10 ns |  15.70 ns |
|  SerialUnpackS |  5,506.1 ns |  23.41 ns |  21.90 ns |
|  SerialUnpackL |  7,949.0 ns | 110.08 ns |  97.58 ns |
| SerialUnpackXL | 16,144.7 ns | 261.28 ns | 231.62 ns |

## Endianness
Big Endian is fully supported *when packing and unpacking*, thus using the `>` flag is fully supported and will work; **but** options `i/I/s/c[n]` **will not work** correctly under a Big Endian memory layout (e.g. under Solaris).

The `=` flag is always defaulted to Little Endian.

<!--
function pattern(str)
  local hex = ""
  for i = 1, #str do
    hex = hex .. string.format("%02X", string.byte(str, i))
    if i ~= #str then
      hex = hex .. " "
    end
  end
  return hex
end
print(pattern(string.pack(">B", 192)));
-->
