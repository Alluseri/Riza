using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Alluseri.Riza;

internal static class PrimitiveHelper {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Span<byte> AsBytes(this ulong Value, Span<byte> Data, bool BigEndian) {
		if (BigEndian)
			BinaryPrimitives.WriteUInt64BigEndian(Data, Value);
		else
			BinaryPrimitives.WriteUInt64LittleEndian(Data, Value);
		return Data;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Span<byte> AsBytes(this uint Value, Span<byte> Data, bool BigEndian) {
		if (BigEndian)
			BinaryPrimitives.WriteUInt32BigEndian(Data, Value);
		else
			BinaryPrimitives.WriteUInt32LittleEndian(Data, Value);
		return Data;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Span<byte> AsBytes(this ushort Value, Span<byte> Data, bool BigEndian) {
		if (BigEndian)
			BinaryPrimitives.WriteUInt16BigEndian(Data, Value);
		else
			BinaryPrimitives.WriteUInt16LittleEndian(Data, Value);
		return Data;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Span<byte> AsBytes(this float Value, Span<byte> Data, bool BigEndian) {
		if (BigEndian)
			BinaryPrimitives.WriteSingleBigEndian(Data, Value);
		else
			BinaryPrimitives.WriteSingleLittleEndian(Data, Value);
		return Data;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Span<byte> AsBytes(this double Value, Span<byte> Data, bool BigEndian) {
		if (BigEndian)
			BinaryPrimitives.WriteDoubleBigEndian(Data, Value);
		else
			BinaryPrimitives.WriteDoubleLittleEndian(Data, Value);
		return Data;
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ulong AsUlong(this Span<byte> Data, bool BigEndian)
	=> BigEndian ? BinaryPrimitives.ReadUInt64BigEndian(Data) : BinaryPrimitives.ReadUInt64LittleEndian(Data);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static long AsLong(this Span<byte> Data, bool BigEndian)
	=> BigEndian ? BinaryPrimitives.ReadInt64BigEndian(Data) : BinaryPrimitives.ReadInt64LittleEndian(Data);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint AsUint(this Span<byte> Data, bool BigEndian)
	=> BigEndian ? BinaryPrimitives.ReadUInt32BigEndian(Data) : BinaryPrimitives.ReadUInt32LittleEndian(Data);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int AsInt(this Span<byte> Data, bool BigEndian)
	=> BigEndian ? BinaryPrimitives.ReadInt32BigEndian(Data) : BinaryPrimitives.ReadInt32LittleEndian(Data);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ushort AsUshort(this Span<byte> Data, bool BigEndian)
	=> BigEndian ? BinaryPrimitives.ReadUInt16BigEndian(Data) : BinaryPrimitives.ReadUInt16LittleEndian(Data);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static short AsShort(this Span<byte> Data, bool BigEndian)
	=> BigEndian ? BinaryPrimitives.ReadInt16BigEndian(Data) : BinaryPrimitives.ReadInt16LittleEndian(Data);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float AsFloat(this Span<byte> Data, bool BigEndian)
	=> BigEndian ? BinaryPrimitives.ReadSingleBigEndian(Data) : BinaryPrimitives.ReadSingleLittleEndian(Data);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double AsDouble(this Span<byte> Data, bool BigEndian)
	=> BigEndian ? BinaryPrimitives.ReadDoubleBigEndian(Data) : BinaryPrimitives.ReadDoubleLittleEndian(Data);
}