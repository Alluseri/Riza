using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Alluseri.Riza.Extensions;

/// <summary>Adds some useful extension methods.</summary>
public static class LuaPackExtensions {
	#region Packing
	/**
	<summary>Packs the given boxed arguments according to the given format into the given Stream.</summary>
	<returns>The same Stream.</returns>
	*/
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Stream Pack(this Stream Stream, string Format, params object[] Arguments)
	=> LuaPack.PackToStream(Format, Stream, Arguments);

	/**
	<summary>Packs the given unboxed arguments according to the given format into the given Stream.</summary>
	<remarks>This method has a limited set of options: s, c are not supported; f, d are rounded down.</remarks>
	<returns>The same Stream.</returns>
	*/
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Stream Pack(this Stream Stream, string Format, params ulong[] Arguments)
	=> LuaPack.PackToStream(Format, Stream, Arguments);
	#endregion Packing

	#region Unpacking
	/**
	<summary>Unpacks data in the given Stream according to the given format.</summary>
	<remarks>No exception will be thrown if the Stream ends prematurely, and the unfinished entries will be undefined.</remarks>
	<returns>The List containing boxed entries.</returns>
	*/
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static List<object> Unpack(this Stream Data, string Format)
	=> LuaPack.Unpack(Format, Data);

	/**
	<summary>Unpacks data in the given byte array according to the given format.</summary>
	<remarks>No exception will be thrown if the byte array ends prematurely, and the unfinished entries will be undefined.</remarks>
	<returns>The List containing boxed entries.</returns>
	*/
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static List<object> Unpack(this byte[] Data, string Format)
	=> LuaPack.Unpack(Format, Data);

	/**
	<summary>Unpacks data in the given Stream according to the given format.</summary>
	<remarks>This method has a limited set of options: s, c are not supported; f, d are rounded down.<br/>No exception will be thrown if the Stream ends prematurely, and the unfinished entries will be undefined.</remarks>
	<returns>The List containing unboxed entries.</returns>
	*/
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static List<ulong> UnpackUnboxed(this Stream Data, string Format)
	=> LuaPack.UnpackUnboxed(Format, Data);

	/**
	<summary>Unpacks data in the given byte array according to the given format.</summary>
	<remarks>This method has a limited set of options: s, c are not supported; f, d are rounded down.<br/>No exception will be thrown if the byte array ends prematurely, and the unfinished entries will be undefined.</remarks>
	<returns>The List containing unboxed entries.</returns>
	*/
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static List<ulong> UnpackUnboxed(this byte[] Data, string Format)
	=> LuaPack.UnpackUnboxed(Format, Data);
	#endregion Unpacking
}