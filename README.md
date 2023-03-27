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
* The Stream overload of LuaPack.Unpack will NOT fail if the stream ends prematurely.

## Usage
Use LuaPack.Pack & LuaPack.PackBytes just like you would use string.pack in Lua. For this piece of code:
```lua
print(string.pack(">I4B<iLs", 1, 2, 3, 4, "abc"));
```
The direct LuaPack representation would be:
```cs
Console.WriteLine(LuaPack.PackToString(">I4B<iLs", 1, 2, 3, 4, "abc"));
```
And this is how to use LuaPack.Unpack & LuaPack.UnpackUnboxed. Those are really the only ways. You're not the only one disappointed.

**Warning:** The 'i' and 'I' options will ALWAYS be written into the type `ulong` when boxed, so don't forget to cast to that first.
```cs
uint Field1;
byte Field2;
int Field3;
ulong Field4;
string Field5;

List<object> Unpack1 = LuaPack.Unpack(">I4B<iLs", VarPacked); // Supports the entire set
Field1 = (uint) (ulong) Unpack1[0];
Field2 = (byte) Unpack1[1];
Field3 = (int) (ulong) Unpack1[2];
Field4 = (ulong) Unpack1[3];
Field5 = (string) Unpack1[4];

List<ulong> Unpack2 = LuaPack.UnpackUnboxed(">I4B<iL", VarPacked); // Faster, doesn't support s and c, f and d are rounded down(unrecoverable)
Field1 = (uint) Unpack2[0];
Field2 = (byte) Unpack2[1];
Field3 = (int) Unpack2[2];
Field4 = (ulong) Unpack2[3];
// Field5 is not set because s is not supported
```

## Performance Benchmarks
Tested on a Ryzen 7 5800X w/ 32 GB RAM on Arch Linux(`linux` kernel).
#### Unpack vs UnpackUnboxed
Keep in mind that UnpackUnboxed doesn't support `s`, `c`, `f` and `d`, and therefore those are not used here.
* class SmallDummy: `>L<lI4>i3>BI` - 6 fields: ulong, long, (uint) ulong, (int) ulong, byte, (uint) ulong
* class LargeDummy: `>L<lI4>i3>BI<L>lI4<i3<BI` - 12 fields: ulong, long, (uint) ulong, (int) ulong, byte, (uint) ulong, ulong, long, (uint) ulong, (int) ulong, byte, (uint) ulong
```cs
|             Method |     Mean |   Error |  StdDev |
|------------------- |---------:|--------:|--------:|
|        UnpackSmall | 150.8 ns | 2.83 ns | 2.50 ns |
| UnpackSmallUnboxed | 104.3 ns | 1.97 ns | 1.94 ns |
|        UnpackLarge | 276.9 ns | 3.88 ns | 3.63 ns |
| UnpackLargeUnboxed | 191.7 ns | 1.54 ns | 1.28 ns |
```

## Big Endian
Big Endian is fully supported *when packing and unpacking*, thus using the `>` flag is fully supported and will work; **but** options `i/I/s/c[n]` **will not work** correctly under a Big Endian memory layout (e.g. under Solaris).

## The name
<img src="https://cdn.nest.rip/uploads/30fa6b7e-ade7-45ac-8410-284f63516171.png">

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