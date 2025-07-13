using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Alluseri.Riza;

public static partial class LuaPack {
	/**
	<summary>Unpacks data from the given stream according to the given format.</summary>
	<remarks>No exception will be thrown if the stream ends prematurely, the unfinished entries will remain undefined.</remarks>
	<returns>The passed object with all necessary fields populated.</returns>
	*/
	public static T Unpack<T>(string Format, Stream Data, T Object) where T : class {
		uint CIndex = 0;
		ReflectionHelper<T> Helper = ReflectionHelper<T>.Fetch(Object);

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
				case 'j':
				Data.Read(Slong);
				Helper.Write(CIndex++, Slong.AsLong(BigEndian));
				break;
				case 'L':
				case 'T':
				case 'J':
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
				case 'n':
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
	<remarks>No exception will be thrown if the byte array ends prematurely, the unfinished entries will remain undefined.</remarks>
	<returns>The passed object with all necessary fields populated.</returns>
	*/
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Unpack<T>(string Format, byte[] Data, T Object) where T : class {
		using MemoryStream Mem = new(Data);
		return Unpack(Format, Mem, Object);
	}

	/**
	<summary>Unpacks data from the given stream according to the given format.</summary>
	<remarks>No exception will be thrown if the stream ends prematurely, the unfinished entries remain undefined.</remarks>
	<returns>A new object with all necessary fields populated.</returns>
	*/
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Unpack<T>(string Format, Stream Data) where T : class, new() => Unpack(Format, Data, new T());

	/**
	<summary>Unpacks data from the given byte array according to the given format.</summary>
	<remarks>No exception will be thrown if the byte array ends prematurely, the unfinished entries remain undefined.</remarks>
	<returns>A new object with all necessary fields populated.</returns>
	*/
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Unpack<T>(string Format, byte[] Data) where T : class, new() {
		using MemoryStream Mem = new(Data);
		return Unpack(Format, Mem, new T());
	}
}