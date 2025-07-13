using BenchmarkDotNet.Running;
using System;
using System.IO;
using System.Text;

namespace Alluseri.Riza;

public class Program {
	private static readonly sbyte Field1 = (sbyte) (Random.Shared.Next() & 0xFF);
	private static readonly float Field2 = Random.Shared.NextSingle();
	private static readonly byte Field3 = (byte) (Random.Shared.Next() & 0xFF);
	private static readonly uint Field4 = (uint) Random.Shared.Next();
	private static readonly ushort Field5 = (ushort) Random.Shared.Next();
	private static readonly string Field6 = "this dynamic string isnt random cuz im lazy";
	private static readonly string Field7 = "abcDEF01234"; // not full 12 len
	private static readonly short Field8 = (short) Random.Shared.Next();
	private static readonly long Field9 = Random.Shared.NextInt64();
	private static readonly ulong Field10 = (ulong) Random.Shared.NextInt64();
	private static readonly short Field11 = (short) Random.Shared.Next();
	private static readonly double Field12 = Random.Shared.NextDouble();
	private static readonly sbyte Field13 = (sbyte) (Random.Shared.Next() & 0xFF);
	private static readonly float Field14 = Random.Shared.NextSingle();
	private static readonly byte Field15 = (byte) (Random.Shared.Next() & 0xFF);
	private static readonly uint Field16 = (uint) Random.Shared.Next();
	private static readonly ushort Field17 = (ushort) Random.Shared.Next();
	private static readonly string Field18 = "same goes for this dynamic string, actually";
	private static readonly string Field19 = "123456789abc"; // full 12 len
	private static readonly short Field20 = (short) Random.Shared.Next();
	private static readonly long Field21 = Random.Shared.NextInt64();
	private static readonly ulong Field22 = (ulong) Random.Shared.NextInt64();
	private static readonly short Field23 = (short) Random.Shared.Next();
	private static readonly double Field24 = Random.Shared.NextDouble();

	public static void Main(string[] Args) {
		/*ExtraLargeDummy XL = new() {
			Field1 = Field1,
			Field2 = Field2,
			Field3 = Field3,
			Field4 = Field4,
			Field5 = Field5,
			Field6 = Field6,
			Field7 = Field7,
			Field8 = Field8,
			Field9 = Field9,
			Field10 = Field10,
			Field11 = Field11,
			Field12 = Field12,
			Field13 = Field13,
			Field14 = Field14,
			Field15 = Field15,
			Field16 = Field16,
			Field17 = Field17,
			Field18 = Field18,
			Field19 = Field19,
			Field20 = Field20,
			Field21 = Field21,
			Field22 = Field22,
			Field23 = Field23,
			Field24 = Field24
		};

		MemoryStream Ms = LuaPack.PackToMemoryStream(ExtraLargeDummy.Pattern, XL);
		Ms.Position = 0;
		ExtraLargeDummy DS = LuaPack.Unpack<ExtraLargeDummy>(ExtraLargeDummy.Pattern, Ms);

		if (XL.Field1 != DS.Field1)
			Console.WriteLine($"Fail Field1: want {XL.Field1}, got {DS.Field1}");

		if (XL.Field2 != DS.Field2)
			Console.WriteLine($"Fail Field2: want {XL.Field2}, got {DS.Field2}");

		if (XL.Field3 != DS.Field3)
			Console.WriteLine($"Fail Field3: want {XL.Field3}, got {DS.Field3}");

		if (XL.Field4 != DS.Field4)
			Console.WriteLine($"Fail Field4: want {XL.Field4}, got {DS.Field4}");

		if (XL.Field5 != DS.Field5)
			Console.WriteLine($"Fail Field5: want {XL.Field5}, got {DS.Field5}");

		if (XL.Field6 != DS.Field6)
			Console.WriteLine($"Fail Field6: want {XL.Field6}, got {DS.Field6}");

		if (XL.Field7 != DS.Field7)
			Console.WriteLine($"Fail Field7: want {XL.Field7}, got {DS.Field7} (this particular failure is to be expected)");

		if (XL.Field8 != DS.Field8)
			Console.WriteLine($"Fail Field8: want {XL.Field8}, got {DS.Field8}");

		if (XL.Field9 != DS.Field9)
			Console.WriteLine($"Fail Field9: want {XL.Field9}, got {DS.Field9}");

		if (XL.Field10 != DS.Field10)
			Console.WriteLine($"Fail Field10: want {XL.Field10}, got {DS.Field10}");

		if (XL.Field11 != DS.Field11)
			Console.WriteLine($"Fail Field11: want {XL.Field11}, got {DS.Field11}");

		if (XL.Field12 != DS.Field12)
			Console.WriteLine($"Fail Field12: want {XL.Field12}, got {DS.Field12}");

		if (XL.Field13 != DS.Field13)
			Console.WriteLine($"Fail Field13: want {XL.Field13}, got {DS.Field13}");

		if (XL.Field14 != DS.Field14)
			Console.WriteLine($"Fail Field14: want {XL.Field14}, got {DS.Field14}");

		if (XL.Field15 != DS.Field15)
			Console.WriteLine($"Fail Field15: want {XL.Field15}, got {DS.Field15}");

		if (XL.Field16 != DS.Field16)
			Console.WriteLine($"Fail Field16: want {XL.Field16}, got {DS.Field16}");

		if (XL.Field17 != DS.Field17)
			Console.WriteLine($"Fail Field17: want {XL.Field17}, got {DS.Field17}");

		if (XL.Field18 != DS.Field18)
			Console.WriteLine($"Fail Field18: want {XL.Field18}, got {DS.Field18}");

		if (XL.Field19 != DS.Field19)
			Console.WriteLine($"Fail Field19: want {XL.Field19}, got {DS.Field19}");

		if (XL.Field20 != DS.Field20)
			Console.WriteLine($"Fail Field20: want {XL.Field20}, got {DS.Field20}");

		if (XL.Field21 != DS.Field21)
			Console.WriteLine($"Fail Field21: want {XL.Field21}, got {DS.Field21}");

		if (XL.Field22 != DS.Field22)
			Console.WriteLine($"Fail Field22: want {XL.Field22}, got {DS.Field22}");

		if (XL.Field23 != DS.Field23)
			Console.WriteLine($"Fail Field23: want {XL.Field23}, got {DS.Field23}");

		if (XL.Field24 != DS.Field24)
			Console.WriteLine($"Fail Field24: want {XL.Field24}, got {DS.Field24}");

		Console.WriteLine($"End check: {DS}");
		*/

		BenchmarkRunner.Run<RizaBasicBenchmarks>();
	}
}