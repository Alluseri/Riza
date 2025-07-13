# Riza
An implementation of Lua's string.pack/unpack in C#, made primarily to assist in the creation of private servers for Unity games with XLua front/backends.

**Only x64 hosts are currently supported.** This is likely to never change.

**Please do not use Riza for generic serialization. This project's intention and purpose is clear.**

## Major differences between string.pack and LuaPack
* LuaPack.Pack does not care if the format option is signed or not, while Lua will raise an Integer Overflow error if the value indeed overflows:
```lua
print(string.pack(">b", 254)); -- input:1: bad argument #2 to 'pack' (integer overflow)
```
* LuaPack does not and will most likely never support the options `!` and `X`. Make an issue if you need them, though.
* LuaPack does not support the `z` option **yet**. Make an issue if you want it implemented.
* LuaPack allows you to use byte counts higher than 16 in options `i[n]`, `I[n]` and `s[n]`, unlike Lua.
* All overloads of LuaPack.Unpack will NOT fail if the data feed ends prematurely.

## Usage
### Important
- You **CANNOT** use `struct` with LuaPack.
- Riza has not been tested with generic classes. The dynamic caching system introduced in v3.0.0 might break them.
- Inheritance has not been tested within Riza, **but** if you want to try, you must ensure that field ordinals don't overlap.
- In environments where dynamic code generation is disabled, dynamic packing and unpacking will be ~17x slower.
- While byte counts for `i/I/s` higher than 8 are technically supported, Riza's integral values are currently limited to `ulong`s (8 bytes).

### Sample Class
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
You may skip field ordinals entirely and that functionality is intended.

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
```cs
SampleClass Sample = LuaPack.Unpack<SampleClass>(">I4c8Lb", SomePackedData); // T has a constraint of new()
SampleClass SameSample = LuaPack.Unpack<SampleClass>(">I4c8Lb", SomePackedData, new SampleClass());
```

## Performance Benchmarks
- CPU: AMD Ryzen 7 5800X (4.50 GHz)
- OS: Windows 10 Enterprise LTSC (OS Build 19044.2965)
- class SmallDummy (S):`>fB<i4s>c12l<hd` - 8 fields.
- class LargeDummy (L): `>bfB<i4Hs>c12I3l<Lhd` - 12 fields.
- class ExtraLargeDummy (XL): `>bfB<i4Hs>c12I3l>Lhd<bfB>i4Hs<c12I3l>Lhd` - 24 fields.

| Method        | Mean     | Error    | StdDev  |
|-------------- |---------:|---------:|--------:|
| VarargsPackS  | 172.5 ns |  1.09 ns | 0.85 ns |
| VarargsPackL  | 229.4 ns |  2.36 ns | 2.09 ns |
| VarargsPackXL | 436.4 ns |  3.90 ns | 3.65 ns |
| DynamicPackS  | 245.1 ns |  2.50 ns | 2.22 ns |
| DynamicPackL  | 312.8 ns |  1.07 ns | 0.90 ns |
| DynamicPackXL | 610.5 ns |  3.69 ns | 3.08 ns |
| UnpackS       | 295.4 ns |  1.87 ns | 1.56 ns |
| UnpackL       | 465.5 ns |  5.90 ns | 5.52 ns |
| UnpackXL      | 924.2 ns | 11.34 ns | 9.47 ns |

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

-- ^ why the fuck is this in the readme???
-->