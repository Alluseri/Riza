using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace Alluseri.Riza.Extensions;

/// <summary>Adds some useful extension methods.</summary>
public static class LuaPackExtensions {
	#region Packing
	/**
	<summary>Packs the given boxed arguments according to the given format into this stream.</summary>
	<returns>The same stream.</returns>
	*/
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Stream LuaPack(this Stream Stream, string Format, params object[] Arguments)
	=> Riza.LuaPack.PackToStream(Format, Stream, Arguments);

	/**
	<summary>Packs the given object according to the given format into this stream.</summary>
	<returns>The same stream.</returns>
	*/
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Stream LuaPack<T>(this Stream Stream, string Format, T Object) where T : notnull
	=> Riza.LuaPack.PackToStream(Format, Stream, Object);
	#endregion Packing

	#region Unpacking
	/**
	<summary>Unpacks data from this stream according to the given format.</summary>
	<remarks>No exception will be thrown if the stream ends prematurely, the unfinished entries will remain undefined.</remarks>
	<returns>A new object with all necessary fields populated.</returns>
	*/
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[StackTraceHidden]
	public static T LuaUnpack<T>(this Stream Data, string Format) where T : class, new()
	=> Riza.LuaPack.Unpack(Format, Data, new T());

	/**
	<summary>Unpacks data from this byte array according to the given format.</summary>
	<remarks>No exception will be thrown if the byte array ends prematurely, the unfinished entries will remain undefined.</remarks>
	<returns>A new object with all necessary fields populated.</returns>
	*/
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[StackTraceHidden]
	public static T LuaUnpack<T>(this byte[] Data, string Format) where T : class, new()
	=> Riza.LuaPack.Unpack(Format, Data, new T());

	/**
	<summary>Unpacks data from this stream according to the given format.</summary>
	<remarks>No exception will be thrown if the stream ends prematurely, the unfinished entries will remain undefined.</remarks>
	<returns>The passed object with all necessary fields populated.</returns>
	*/
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[StackTraceHidden]
	public static T LuaUnpack<T>(this Stream Data, string Format, T Instance) where T : class
	=> Riza.LuaPack.Unpack(Format, Data, Instance);

	/**
	<summary>Unpacks data from this byte array according to the given format.</summary>
	<remarks>No exception will be thrown if the byte array ends prematurely, the unfinished entries will remain undefined.</remarks>
	<returns>The passed object with all necessary fields populated.</returns>
	*/
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[StackTraceHidden]
	public static T LuaUnpack<T>(this byte[] Data, string Format, T Instance) where T : class
	=> Riza.LuaPack.Unpack(Format, Data, Instance);
	#endregion Unpacking
}