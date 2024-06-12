using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Alluseri.Riza;

public static partial class LuaPack {
	/**
	<summary>Unpacks data from the given Stream according to the given format.</summary>
	<remarks>No exception will be thrown if the Stream ends prematurely, and the unfinished entries will be undefined.</remarks>
	<returns>The List containing boxed entries.</returns>
	*/
	public static List<object> Unpack(string Format, Stream Data) {
		List<object> Output = new();

		bool BigEndian = false;
		int Integral = 0;
		byte Count = 0;
		byte Operation = 0;
		bool RecordInt = false;

		Span<byte> Sshort = stackalloc byte[sizeof(ushort)];
		Span<byte> Sint = stackalloc byte[sizeof(uint)];
		Span<byte> Slong = stackalloc byte[sizeof(ulong)];

		foreach (char Option in Format) {
			if (RecordInt) {
				if (Option >= '0' && Option <= '9') {
					Integral = (Integral * 10) + (byte) (Option - '0');
					Count++;
					continue;
				}

				ReadRecord(Output, Data, Integral, Count, Operation, BigEndian);

				RecordInt = false;
				Integral = Count = 0;
			}
			switch (Option) {
				#region Config
				case '>':
				BigEndian = true;
				break;
				case '<':
				case '=':
				BigEndian = false;
				break;
				#endregion Config
				#region Primitives
				case 'b':
				Output.Add((sbyte) Data.ReadByte());
				break;
				case 'B':
				Output.Add((byte) Data.ReadByte());
				break;
				case 'i':
				case 'I':
				RecordInt = true;
				Operation = 0;
				break;
				case 's':
				RecordInt = true;
				Operation = 1;
				break;
				case 'c':
				RecordInt = true;
				Operation = 2;
				break;
				case 'l':
				Data.Read(Slong);
				Output.Add(Slong.AsLong(BigEndian));
				break;
				case 'L':
				case 'T':
				Data.Read(Slong);
				Output.Add(Slong.AsUlong(BigEndian));
				break;
				case 'h':
				Data.Read(Sshort);
				Output.Add(Sshort.AsShort(BigEndian));
				break;
				case 'H':
				Data.Read(Sshort);
				Output.Add(Sshort.AsUshort(BigEndian));
				break;
				case 'f':
				Data.Read(Sint);
				Output.Add(Sint.AsFloat(BigEndian));
				break;
				case 'd':
				Data.Read(Slong);
				Output.Add(Slong.AsDouble(BigEndian));
				break;
				#endregion Primitives
				#region Control
				case ' ':
				case 'x':
				break;
				default:
				throw new FormatException($"Unknown option: '{Option}'.");
				#endregion Control
			}
		}

		if (RecordInt)
			ReadRecord(Output, Data, Integral, Count, Operation, BigEndian);

		return Output;
	}

	/**
	<summary>Unpacks data from the given Stream according to the given format.</summary>
	<remarks>No exception will be thrown if the Stream ends prematurely, and the unfinished entries will be undefined.</remarks>
	<returns>The passed object with all necessary fields modified.</returns>
	*/
	public static T Unpack<T>(T Object, string Format, Stream Data) where T : notnull {
		uint CIndex = 0;
		ReflectionHelper<T> Helper = new(Object);

		bool BigEndian = false;
		int Integral = 0;
		byte Count = 0;
		byte Operation = 0;
		bool RecordInt = false;

		Span<byte> Sshort = stackalloc byte[sizeof(ushort)];
		Span<byte> Sint = stackalloc byte[sizeof(uint)];
		Span<byte> Slong = stackalloc byte[sizeof(ulong)];

		foreach (char Option in Format) {
			if (RecordInt) {
				if (Option >= '0' && Option <= '9') {
					Integral = (Integral * 10) + (byte) (Option - '0');
					Count++;
					continue;
				}

				ReadRecord(Helper, CIndex++, Data, Integral, Count, Operation, BigEndian);

				RecordInt = false;
				Integral = Count = 0;
			}
			switch (Option) {
				#region Config
				case '>':
				BigEndian = true;
				break;
				case '<':
				case '=':
				BigEndian = false;
				break;
				#endregion Config
				#region Primitives
				case 'b':
				case 'B':
				Helper.WriteBox(CIndex++, Data.ReadByte());
				break;
				case 'i':
				case 'I':
				RecordInt = true;
				Operation = 0;
				break;
				case 's':
				RecordInt = true;
				Operation = 1;
				break;
				case 'c':
				RecordInt = true;
				Operation = 2;
				break;
				case 'l':
				Data.Read(Slong);
				Helper.Write(CIndex++, Slong.AsLong(BigEndian));
				break;
				case 'L':
				case 'T':
				Data.Read(Slong);
				Helper.Write(CIndex++, Slong.AsUlong(BigEndian));
				break;
				case 'h':
				Data.Read(Sshort);
				Helper.Write(CIndex++, Sshort.AsShort(BigEndian));
				break;
				case 'H':
				Data.Read(Sshort);
				Helper.Write(CIndex++, Sshort.AsUshort(BigEndian));
				break;
				case 'f':
				Data.Read(Sint);
				Helper.Write(CIndex++, Sint.AsFloat(BigEndian));
				break;
				case 'd':
				Data.Read(Slong);
				Helper.Write(CIndex++, Slong.AsDouble(BigEndian));
				break;
				#endregion Primitives
				#region Control
				case ' ':
				case 'x':
				break;
				default:
				throw new FormatException($"Unknown option: '{Option}'.");
				#endregion Control
			}
		}

		if (RecordInt)
			ReadRecord(Helper, CIndex++, Data, Integral, Count, Operation, BigEndian);

		return Object;
	}

	/**
	<summary>Unpacks data from the given byte array according to the given format.</summary>
	<remarks>No exception will be thrown if the byte array ends prematurely, and the unfinished entries will be undefined.</remarks>
	<returns>The List containing boxed entries.</returns>
	*/
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static List<object> Unpack(string Format, byte[] Data) {
		using MemoryStream Mem = new(Data);
		return Unpack(Format, Mem);
	}

	/**
	<summary>Unpacks data from the given byte array according to the given format.</summary>
	<remarks>No exception will be thrown if the byte array ends prematurely, and the unfinished entries will be undefined.</remarks>
	<returns>The passed object with all necessary fields modified.</returns>
	*/
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Unpack<T>(T Object, string Format, byte[] Data) where T : notnull {
		using MemoryStream Mem = new(Data);
		return Unpack(Object, Format, Mem);
	}

	/**
	<summary>Unpacks data from the given byte array according to the given format.</summary>
	<remarks>No exception will be thrown if the Stream ends prematurely, and the unfinished entries will be undefined.</remarks>
	<returns>A new object with all necessary fields modified.</returns>
	*/
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Unpack<T>(string Format, Stream Data) where T : notnull, new() => Unpack(new T(), Format, Data);

	/**
	<summary>Unpacks data from the given byte array according to the given format.</summary>
	<remarks>No exception will be thrown if the byte array ends prematurely, and the unfinished entries will be undefined.</remarks>
	<returns>A new object with all necessary fields modified.</returns>
	*/
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Unpack<T>(string Format, byte[] Data) where T : notnull, new() {
		using MemoryStream Mem = new(Data);
		return Unpack(new T(), Format, Mem);
	}
}