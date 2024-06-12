using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Alluseri.Riza;

public class BenchmarkUnpack {
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

	private readonly MemoryStream Small;
	private readonly MemoryStream Large;
	private readonly MemoryStream ExtraLarge;

	public BenchmarkUnpack() {
		Small = new(LuaPack.Pack(SmallDummy.Pattern, new SmallDummy() {
			Field11 = Field11,
			Field12 = Field12,
			Field2 = Field2,
			Field3 = Field3,
			Field4 = Field4,
			Field6 = Field6,
			Field7 = Field7,
			Field9 = Field9
		}));
		Large = new(LuaPack.Pack(LargeDummy.Pattern, new LargeDummy() {
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
		}));
		ExtraLarge = new(LuaPack.Pack(ExtraLargeDummy.Pattern, new ExtraLargeDummy() {
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
		}));
	}

	[Benchmark]
	public SmallDummy ManualUnpackS() {
		Small.Position = 0;
		SmallDummy D = new();
		List<object> Upx = LuaPack.Unpack(SmallDummy.Pattern, Small);
		D.Field2 = (float) Upx[0];
		D.Field3 = (byte) Upx[1];
		D.Field4 = (uint) (ulong) Upx[2];
		D.Field6 = (string) Upx[3];
		D.Field7 = (string) Upx[4];
		D.Field9 = (long) Upx[5];
		D.Field11 = (short) Upx[6];
		D.Field12 = (double) Upx[7];
		return D;
	}

	[Benchmark]
	public LargeDummy ManualUnpackL() {
		Large.Position = 0;
		LargeDummy D = new();
		List<object> Upx = LuaPack.Unpack(LargeDummy.Pattern, Large);
		D.Field1 = (sbyte) Upx[0];
		D.Field2 = (float) Upx[1];
		D.Field3 = (byte) Upx[2];
		D.Field4 = (uint) (ulong) Upx[3];
		D.Field5 = (ushort) Upx[4];
		D.Field6 = (string) Upx[5];
		D.Field7 = (string) Upx[6];
		D.Field8 = (short) (ulong) Upx[7];
		D.Field9 = (long) Upx[8];
		D.Field10 = (ulong) Upx[9];
		D.Field11 = (short) Upx[10];
		D.Field12 = (double) Upx[11];
		return D;
	}

	[Benchmark]
	public ExtraLargeDummy ManualUnpackXL() {
		ExtraLarge.Position = 0;
		ExtraLargeDummy D = new();
		List<object> Upx = LuaPack.Unpack(ExtraLargeDummy.Pattern, ExtraLarge);
		D.Field1 = (sbyte) Upx[0];
		D.Field2 = (float) Upx[1];
		D.Field3 = (byte) Upx[2];
		D.Field4 = (uint) (ulong) Upx[3];
		D.Field5 = (ushort) Upx[4];
		D.Field6 = (string) Upx[5];
		D.Field7 = (string) Upx[6];
		D.Field8 = (short) (ulong) Upx[7];
		D.Field9 = (long) Upx[8];
		D.Field10 = (ulong) Upx[9];
		D.Field11 = (short) Upx[10];
		D.Field12 = (double) Upx[11];
		D.Field13 = (sbyte) Upx[12];
		D.Field14 = (float) Upx[13];
		D.Field15 = (byte) Upx[14];
		D.Field16 = (uint) (ulong) Upx[15];
		D.Field17 = (ushort) Upx[16];
		D.Field18 = (string) Upx[17];
		D.Field19 = (string) Upx[18];
		D.Field20 = (short) (ulong) Upx[19];
		D.Field21 = (long) Upx[20];
		D.Field22 = (ulong) Upx[21];
		D.Field23 = (short) Upx[22];
		D.Field24 = (double) Upx[23];
		return D;
	}

	[Benchmark]
	public SmallDummy SerialUnpackS() {
		Small.Position = 0;
		return LuaPack.Unpack<SmallDummy>(SmallDummy.Pattern, Small);
	}

	[Benchmark]
	public LargeDummy SerialUnpackL() {
		Large.Position = 0;
		return LuaPack.Unpack<LargeDummy>(LargeDummy.Pattern, Large);
	}

	[Benchmark]
	public ExtraLargeDummy SerialUnpackXL() {
		ExtraLarge.Position = 0;
		return LuaPack.Unpack<ExtraLargeDummy>(ExtraLargeDummy.Pattern, ExtraLarge);
	}
}