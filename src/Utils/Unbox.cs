using System;
using System.Runtime.CompilerServices;

namespace Alluseri.Riza;

// TODO: Rewrite for pattern matching

internal static class Unbox {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ulong AsUlong(object M) => M switch {
		ulong Q => Q,
		uint W => W,
		ushort E => E,
		byte R => R,
		char T => T,
		long Y => (ulong) Y,
		int U => (uint) U,
		short I => (ushort) I,
		sbyte O => (byte) O,
		_ => Convert.ToUInt64(M)
	};

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte AsByte(object M) => M switch {
		ulong Q => (byte) Q,
		uint W => (byte) W,
		ushort E => (byte) E,
		byte R => R,
		char T => (byte) T,
		long Y => (byte) Y,
		int U => (byte) U,
		short I => (byte) I,
		sbyte O => (byte) O,
		_ => Convert.ToByte(M)
	};

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ushort AsUshort(object M) => M switch {
		ulong Q => (ushort) Q,
		uint W => (ushort) W,
		ushort E => E,
		byte R => R,
		char T => T,
		long Y => (ushort) (ulong) Y,
		int U => (ushort) (uint) U,
		short I => (ushort) I,
		sbyte O => (byte) O,
		_ => Convert.ToUInt16(M)
	};

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint AsUint(object M) => M switch {
		ulong Q => (uint) Q,
		uint W => W,
		ushort E => E,
		byte R => R,
		char T => T,
		long Y => (uint) (ulong) Y,
		int U => (uint) U,
		short I => (ushort) I,
		sbyte O => (byte) O,
		_ => Convert.ToUInt32(M)
	};

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float AsFloat(object M) => M switch {
		ulong Q => Q,
		uint W => W,
		ushort E => E,
		byte R => R,
		char T => T,
		long Y => Y,
		int U => U,
		short I => I,
		sbyte O => O,
		_ => Convert.ToSingle(M)
	};

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double AsDouble(object M) => M switch {
		ulong Q => Q,
		uint W => W,
		ushort E => E,
		byte R => R,
		char T => T,
		long Y => Y,
		int U => U,
		short I => I,
		sbyte O => O,
		_ => Convert.ToDouble(M)
	};
}