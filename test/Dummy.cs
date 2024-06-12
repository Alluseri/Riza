namespace Alluseri.Riza;

#pragma warning disable

public class SmallDummy {
	public static readonly string Pattern = ">fB<i4s>c12l>hd";
	[RizaField(0)]
	public float Field2;
	[RizaField(1)]
	public byte Field3;
	[RizaField(2)]
	public uint Field4;
	[RizaField(3)]
	public string Field6;
	[RizaField(4)]
	public string Field7;
	[RizaField(5)]
	public long Field9;
	[RizaField(6)]
	public short Field11;
	[RizaField(7)]
	public double Field12;
}
public class LargeDummy {
	public static readonly string Pattern = ">bfB<i4Hs>c12I3l<Lhd";
	[RizaField(0)]
	public sbyte Field1;
	[RizaField(1)]
	public float Field2;
	[RizaField(2)]
	public byte Field3;
	[RizaField(3)]
	public uint Field4;
	[RizaField(4)]
	public ushort Field5;
	[RizaField(5)]
	public string Field6;
	[RizaField(6)]
	public string Field7;
	[RizaField(7)]
	public short Field8;
	[RizaField(8)]
	public long Field9;
	[RizaField(9)]
	public ulong Field10;
	[RizaField(10)]
	public short Field11;
	[RizaField(11)]
	public double Field12;
}
public class ExtraLargeDummy {
	public static readonly string Pattern = ">bfB<i4Hs>c12I3l>Lhd<bfB>i4Hs<c12I3l>Lhd";
	[RizaField(0)]
	public sbyte Field1;
	[RizaField(1)]
	public float Field2;
	[RizaField(2)]
	public byte Field3;
	[RizaField(3)]
	public uint Field4;
	[RizaField(4)]
	public ushort Field5;
	[RizaField(5)]
	public string Field6;
	[RizaField(6)]
	public string Field7;
	[RizaField(7)]
	public short Field8;
	[RizaField(8)]
	public long Field9;
	[RizaField(9)]
	public ulong Field10;
	[RizaField(10)]
	public short Field11;
	[RizaField(11)]
	public double Field12;
	[RizaField(12)]
	public sbyte Field13;
	[RizaField(13)]
	public float Field14;
	[RizaField(14)]
	public byte Field15;
	[RizaField(15)]
	public uint Field16;
	[RizaField(16)]
	public ushort Field17;
	[RizaField(17)]
	public string Field18;
	[RizaField(18)]
	public string Field19;
	[RizaField(19)]
	public short Field20;
	[RizaField(20)]
	public long Field21;
	[RizaField(21)]
	public ulong Field22;
	[RizaField(22)]
	public short Field23;
	[RizaField(23)]
	public double Field24;
}