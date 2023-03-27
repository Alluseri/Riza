using System;
using System.Runtime.CompilerServices;

namespace Alluseri.Riza;

internal static class Unbox {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ulong AsUlong(object U) {
		if (U is uint Q)
			return Q;
		if (U is int W)
			return (ulong) W;
		if (U is ushort E)
			return E;
		if (U is short R)
			return (ulong) R;
		if (U is byte T)
			return T;
		if (U is char Y)
			return Y;
		if (U is long I)
			return (ulong) I;
		if (U is ulong O)
			return O;
		if (U is sbyte P)
			return (ulong) P;
		return (ulong) U;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte AsByte(object U) {
		if (U is uint Q)
			return (byte) Q;
		if (U is int W)
			return (byte) W;
		if (U is ushort E)
			return (byte) E;
		if (U is short R)
			return (byte) R;
		if (U is byte T)
			return T;
		if (U is char Y)
			return (byte) Y;
		if (U is long I)
			return (byte) I;
		if (U is ulong O)
			return (byte) O;
		if (U is sbyte P)
			return (byte) P;
		return (byte) U;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ushort AsUshort(object U) {
		if (U is uint Q)
			return (ushort) Q;
		if (U is int W)
			return (ushort) W;
		if (U is ushort E)
			return E;
		if (U is short R)
			return (ushort) R;
		if (U is byte T)
			return T;
		if (U is char Y)
			return Y;
		if (U is long I)
			return (ushort) I;
		if (U is ulong O)
			return (ushort) O;
		if (U is sbyte P)
			return (ushort) P;
		return (ushort) U;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint AsUint(object U) {
		if (U is uint Q)
			return Q;
		if (U is int W)
			return (uint) W;
		if (U is ushort E)
			return E;
		if (U is short R)
			return (uint) R;
		if (U is byte T)
			return T;
		if (U is char Y)
			return Y;
		if (U is long I)
			return (uint) I;
		if (U is ulong O)
			return (uint) O;
		if (U is sbyte P)
			return (uint) P;
		return (uint) U;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float AsFloat(object U) {
		if (U is float A)
			return A;
		if (U is double S)
			return (float) S;
		if (U is uint Q)
			return Q;
		if (U is int W)
			return W;
		if (U is ushort E)
			return E;
		if (U is short R)
			return R;
		if (U is byte T)
			return T;
		if (U is char Y)
			return Y;
		if (U is long I)
			return I;
		if (U is ulong O)
			return O;
		if (U is sbyte P)
			return P;
		return (float) U;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double AsDouble(object U) {
		if (U is float A)
			return (double) A;
		if (U is double S)
			return (double) S;
		if (U is uint Q)
			return Q;
		if (U is int W)
			return W;
		if (U is ushort E)
			return E;
		if (U is short R)
			return R;
		if (U is byte T)
			return T;
		if (U is char Y)
			return Y;
		if (U is long I)
			return I;
		if (U is ulong O)
			return O;
		if (U is sbyte P)
			return P;
		return (double) U;
	}
}