using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Alluseri.Riza;

public class BenchmarkPack {
	private readonly sbyte Field1 = (sbyte) (Random.Shared.Next() & 0xFF);
	private readonly float Field2 = Random.Shared.NextSingle();
	private readonly byte Field3 = (byte) (Random.Shared.Next() & 0xFF);
	private readonly uint Field4 = (uint) Random.Shared.Next();
	private readonly ushort Field5 = (ushort) Random.Shared.Next();
	private readonly string Field6 = "this dynamic string isnt random cuz im lazy";
	private readonly string Field7 = "abcDEF01234"; // not full 12 len
	private readonly short Field8 = (short) Random.Shared.Next();
	private readonly long Field9 = Random.Shared.NextInt64();
	private readonly ulong Field10 = (ulong) Random.Shared.NextInt64();
	private readonly short Field11 = (short) Random.Shared.Next();
	private readonly double Field12 = Random.Shared.NextDouble();
	private readonly sbyte Field13 = (sbyte) (Random.Shared.Next() & 0xFF);
	private readonly float Field14 = Random.Shared.NextSingle();
	private readonly byte Field15 = (byte) (Random.Shared.Next() & 0xFF);
	private readonly uint Field16 = (uint) Random.Shared.Next();
	private readonly ushort Field17 = (ushort) Random.Shared.Next();
	private readonly string Field18 = "same goes for this dynamic string, actually";
	private readonly string Field19 = "123456789abc"; // full 12 len
	private readonly short Field20 = (short) Random.Shared.Next();
	private readonly long Field21 = Random.Shared.NextInt64();
	private readonly ulong Field22 = (ulong) Random.Shared.NextInt64();
	private readonly short Field23 = (short) Random.Shared.Next();
	private readonly double Field24 = Random.Shared.NextDouble();

	private readonly SmallDummy S;
	private readonly LargeDummy L;
	private readonly ExtraLargeDummy XL;

	public BenchmarkPack() {
		S = new() {
			Field11 = Field11,
			Field12 = Field12,
			Field2 = Field2,
			Field3 = Field3,
			Field4 = Field4,
			Field6 = Field6,
			Field7 = Field7,
			Field9 = Field9
		};
		L = new() {
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
			Field12 = Field12
		};
		XL = new() {
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
	}

	[Benchmark]
	public byte[] ManualPackS() => LuaPack.Pack(SmallDummy.Pattern, Field2, Field3, Field4, Field6, Field7, Field9, Field11, Field12);

	[Benchmark]
	public byte[] ManualPackL() => LuaPack.Pack(LargeDummy.Pattern, Field1, Field2, Field3, Field4, Field5, Field6, Field7, Field8, Field9, Field10, Field11, Field12);

	[Benchmark]
	public byte[] ManualPackXL() => LuaPack.Pack(ExtraLargeDummy.Pattern, Field1, Field2, Field3, Field4, Field5, Field6, Field7, Field8, Field9, Field10, Field11, Field12, Field13, Field14, Field15, Field16, Field17, Field18, Field19, Field20, Field21, Field22, Field23, Field24);

	[Benchmark]
	public byte[] SerialPackS() => LuaPack.Pack(SmallDummy.Pattern, S);

	[Benchmark]
	public byte[] SerialPackL() => LuaPack.Pack(LargeDummy.Pattern, L);

	[Benchmark]
	public byte[] SerialPackXL() => LuaPack.Pack(ExtraLargeDummy.Pattern, XL);
}