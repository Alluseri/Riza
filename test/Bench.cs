using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.IO;

namespace Alluseri.Riza;

public class Bench {
	private const string SmallFormat = ">L<lI4>i3>BI";
	private const string LargeFormat = ">L<lI4>i3>BI<L>lI4<i3<BI";

	private readonly ulong Field1 = (ulong) Random.Shared.NextInt64();
	private readonly long Field2 = Random.Shared.NextInt64();
	private readonly uint Field3 = (uint) Random.Shared.Next();
	private readonly int Field4 = Random.Shared.Next() % 0xFFFFFF;
	private readonly byte Field5 = (byte) (Random.Shared.Next() & 0xFF);
	private readonly uint Field6 = (uint) Random.Shared.Next();
	private readonly ulong Field7 = (ulong) Random.Shared.NextInt64();
	private readonly long Field8 = Random.Shared.NextInt64();
	private readonly uint Field9 = (uint) Random.Shared.Next();
	private readonly int Field10 = Random.Shared.Next() % 0xFFFFFF;
	private readonly byte Field11 = (byte) (Random.Shared.Next() & 0xFF);
	private readonly uint Field12 = (uint) Random.Shared.Next();

	private readonly MemoryStream Small;
	private readonly MemoryStream Large;

	public Bench() {
		Small = new(LuaPack.Pack(SmallFormat, Field1, Field2, Field3, Field4, Field5, Field6));
		Large = new(LuaPack.Pack(LargeFormat, Field1, Field2, Field3, Field4, Field5, Field6, Field7, Field8, Field9, Field10, Field11, Field12));
	}

	[Benchmark]
	public SmallDummy UnpackSmall() {
		Small.Position = 0;
		SmallDummy D = new();
		List<object> Upx = LuaPack.Unpack(SmallFormat, Small);
		D.Field1 = (ulong) Upx[0];
		D.Field2 = (long) Upx[1];
		D.Field3 = (uint) (ulong) Upx[2];
		D.Field4 = (int) (ulong) Upx[3];
		D.Field5 = (byte) Upx[4];
		D.Field6 = (uint) (ulong) Upx[5];
		return D;
	}

	[Benchmark]
	public SmallDummy UnpackSmallUnboxed() {
		Small.Position = 0;
		SmallDummy D = new();
		List<ulong> Upx = LuaPack.UnpackUnboxed(SmallFormat, Small);
		D.Field1 = Upx[0];
		D.Field2 = (long) Upx[1];
		D.Field3 = (uint) Upx[2];
		D.Field4 = (int) Upx[3];
		D.Field5 = (byte) Upx[4];
		D.Field6 = (uint) Upx[5];
		return D;
	}

	[Benchmark]
	public LargeDummy UnpackLarge() {
		Large.Position = 0;
		LargeDummy D = new();
		List<object> Upx = LuaPack.Unpack(LargeFormat, Large);
		D.Field1 = (ulong) Upx[0];
		D.Field2 = (long) Upx[1];
		D.Field3 = (uint) (ulong) Upx[2];
		D.Field4 = (int) (ulong) Upx[3];
		D.Field5 = (byte) Upx[4];
		D.Field6 = (uint) (ulong) Upx[5];
		D.Field7 = (ulong) Upx[6];
		D.Field8 = (long) Upx[7];
		D.Field9 = (uint) (ulong) Upx[8];
		D.Field10 = (int) (ulong) Upx[9];
		D.Field11 = (byte) Upx[10];
		D.Field12 = (uint) (ulong) Upx[11];
		return D;
	}

	[Benchmark]
	public LargeDummy UnpackLargeUnboxed() {
		Large.Position = 0;
		LargeDummy D = new();
		List<ulong> Upx = LuaPack.UnpackUnboxed(LargeFormat, Large);
		D.Field1 = Upx[0];
		D.Field2 = (long) Upx[1];
		D.Field3 = (uint) Upx[2];
		D.Field4 = (int) Upx[3];
		D.Field5 = (byte) Upx[4];
		D.Field6 = (uint) Upx[5];
		D.Field7 = Upx[6];
		D.Field8 = (long) Upx[7];
		D.Field9 = (uint) Upx[8];
		D.Field10 = (int) Upx[9];
		D.Field11 = (byte) Upx[10];
		D.Field12 = (uint) Upx[11];
		return D;
	}
}