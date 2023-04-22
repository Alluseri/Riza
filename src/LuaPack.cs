#pragma warning disable IDE0057, CS8509

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Alluseri.Riza;

// TODO: Why not just use `partial`, bruh?

/// <summary>The core component of Riza.</summary>
public static class LuaPack {
	#region Writing
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void WriteIntBytes(Stream Data, bool BigEndian, int BytesToWrite, ulong Argument) {
		#pragma warning disable format
		switch (BytesToWrite) {
			case 0: throw new OverflowException($"{Argument} cannot be represented in only {BytesToWrite} bytes.");
			case 1: if (Argument > 0xFF) goto case 0; break;
			case 2: if (Argument > 0xFFFF) goto case 0; break;
			case 3: if (Argument > 0xFFFFFF) goto case 0; break;
			case 4: if (Argument > 0xFFFFFFFF) goto case 0; break;
			case 5: if (Argument > 0xFFFFFFFFFF) goto case 0; break;
			case 6: if (Argument > 0xFFFFFFFFFFFF) goto case 0; break;
			case 7: if (Argument > 0xFFFFFFFFFFFFFF) goto case 0; break;
			case 8: if (Argument > 0xFFFFFFFFFFFFFFFF) goto case 0; break;
		}
		#pragma warning restore format

		Span<byte> Entire = Argument.AsBytes(stackalloc byte[8], BigEndian);

		if (BytesToWrite > 8) {
			if (BigEndian) {
				for (int i = 8; i < BytesToWrite; i++)
					Data.WriteByte(0);
			}
			Data.Write(Entire);
			if (!BigEndian) {
				for (int i = 8; i < BytesToWrite; i++)
					Data.WriteByte(0);
			}
		} else
			Data.Write(BigEndian ? Entire.Slice(8 - BytesToWrite) : Entire.Slice(0, BytesToWrite));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void WriteRecord(Stream Data, int Integral, byte Count, byte Operation, bool BigEndian, object Argument) {
		if (Integral == 0) {
			if (Count > 0)
				throw new FormatException($"The '{Operation switch { 0 => "i/I", 1 => "s", 2 => "c" }}' option requires a non-zero integral to be specified.");
			if (Operation == 2)
				return;
			Integral = sizeof(uint);
		}

		switch (Operation) {
			case 0:
			WriteIntBytes(Data, BigEndian, Integral, Unbox.AsUlong(Argument));
			break;
			case 1:
			byte[] Str = Encoding.UTF8.GetBytes((string) Argument);
			WriteIntBytes(Data, BigEndian, Integral, (ulong) Str.Length);
			Data.Write(Str);
			break;
			case 2:
			byte[] LargeStr = new byte[Integral];
			if (Encoding.UTF8.GetBytes((string) Argument, LargeStr) > Integral)
				throw new ArgumentException($"Cannot write '{Argument}' into only {Integral} UTF-8 bytes.");
			Data.Write(LargeStr);
			break;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void WriteRecord(Stream Data, int Integral, byte Count, bool BigEndian, ulong Argument) {
		if (Integral == 0) {
			if (Count > 0)
				throw new FormatException($"The 'i/I' option requires a non-zero integral to be specified.");
			Integral = sizeof(uint);
		}

		WriteIntBytes(Data, BigEndian, Integral, Unbox.AsUlong(Argument));
	}
	#endregion Writing

	#region Packing
	/**
	<summary>Packs the given boxed arguments according to the given format into the given Stream.</summary>
	<returns>The same Stream.</returns>
	*/
	public static Stream PackToStream(string Format, Stream Data, params object[] Arguments) {
		bool BigEndian = false;
		uint Stack = 0;
		int Integral = 0;
		byte Count = 0;
		byte Operation = 0;
		bool RecordInt = false;

		Span<byte> Us = stackalloc byte[sizeof(ushort)];
		Span<byte> Ui = stackalloc byte[sizeof(uint)];
		Span<byte> Ul = stackalloc byte[sizeof(ulong)];

		foreach (char Option in Format) {
			if (RecordInt) {
				if (Option >= '0' && Option <= '9') {
					Integral = (Integral * 10) + (byte) (Option - '0');
					Count++;
					continue;
				}

				WriteRecord(Data, Integral, Count, Operation, BigEndian, Arguments[Stack]);

				RecordInt = false;
				Stack++;
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
				Data.WriteByte(Unbox.AsByte(Arguments[Stack++]));
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
				case 'L':
				case 'T':
				Data.Write(Unbox.AsUlong(Arguments[Stack++]).AsBytes(Ul, BigEndian));
				break;
				case 'h':
				case 'H':
				Data.Write(Unbox.AsUshort(Arguments[Stack++]).AsBytes(Us, BigEndian));
				break;
				case 'f':
				Data.Write(Unbox.AsFloat(Arguments[Stack++]).AsBytes(Ui, BigEndian));
				break;
				case 'd':
				Data.Write(Unbox.AsDouble(Arguments[Stack++]).AsBytes(Ul, BigEndian));
				break;
				#endregion Primitives
				#region Control
				case ' ':
				break;
				case 'x':
				Data.WriteByte(0);
				break;
				default:
				throw new FormatException($"Unknown option: '{Option}'.");
				#endregion Control
			}
		}

		if (RecordInt)
			WriteRecord(Data, Integral, Count, Operation, BigEndian, Arguments[Stack]);

		return Data;
	}

	/**
	<summary>Packs the given unboxed arguments according to the given format into the given Stream.</summary>
	<remarks>This method has a limited set of options: s, c are not supported; f, d are rounded down.</remarks>
	<returns>The same Stream.</returns>
	*/
	public static Stream PackToStream(string Format, Stream Data, params ulong[] Arguments) {
		bool BigEndian = false;
		uint Stack = 0;
		int Integral = 0;
		byte Count = 0;
		bool RecordInt = false;

		Span<byte> Us = stackalloc byte[sizeof(ushort)];
		Span<byte> Ui = stackalloc byte[sizeof(uint)];
		Span<byte> Ul = stackalloc byte[sizeof(ulong)];

		foreach (char Option in Format) {
			if (RecordInt) {
				if (Option >= '0' && Option <= '9') {
					Integral = (Integral * 10) + (byte) (Option - '0');
					Count++;
					continue;
				}

				WriteRecord(Data, Integral, Count, BigEndian, Arguments[Stack]);

				RecordInt = false;
				Stack++;
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
				Data.WriteByte((byte) Arguments[Stack++]);
				break;
				case 'i':
				case 'I':
				RecordInt = true;
				break;
				case 'l':
				case 'L':
				case 'T':
				Data.Write(Arguments[Stack++].AsBytes(Ul, BigEndian));
				break;
				case 'h':
				case 'H':
				Data.Write(((ushort) Arguments[Stack++]).AsBytes(Us, BigEndian));
				break;
				case 'f':
				Data.Write(((float) Arguments[Stack++]).AsBytes(Ui, BigEndian));
				break;
				case 'd':
				Data.Write(((double) Arguments[Stack++]).AsBytes(Ul, BigEndian));
				break;
				#endregion Primitives
				#region Control
				case ' ':
				break;
				case 'x':
				Data.WriteByte(0);
				break;
				default:
				throw new FormatException($"Unknown option: '{Option}' (you're using unboxed overload).");
				#endregion Control
			}
		}

		if (RecordInt)
			WriteRecord(Data, Integral, Count, BigEndian, Arguments[Stack]);

		return Data;
	}

	/**
	<summary>Packs the given boxed arguments according to the given format into the given Stream.</summary>
	<returns>The same Stream.</returns>
	*/
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Stream PackToStream(Stream Data, string Format, params object[] Arguments)
	=> PackToStream(Format, Data, Arguments);

	/**
	<summary>Packs the given unboxed arguments according to the given format into the given Stream.</summary>
	<remarks>This method has a limited set of options: s, c are not supported; f, d are rounded down.</remarks>
	<returns>The same Stream.</returns>
	*/
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Stream PackToStream(Stream Data, string Format, params ulong[] Arguments)
	=> PackToStream(Format, Data, Arguments);

	/**
	<summary>Packs the given boxed arguments according to the given format.</summary>
	<returns>The MemoryStream with unreset(non-0) position containing the packed data.</returns>
	*/
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static MemoryStream PackToMemoryStream(string Format, params object[] Arguments)
	=> (MemoryStream) PackToStream(Format, new MemoryStream(), Arguments);

	/**
	<summary>Packs the given unboxed arguments according to the given format.</summary>
	<remarks>This method has a limited set of options: s, c are not supported; f, d are rounded down.</remarks>
	<returns>The MemoryStream with unreset(non-0) position containing the packed data.</returns>
	*/
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static MemoryStream PackToMemoryStream(string Format, params ulong[] Arguments)
	=> (MemoryStream) PackToStream(Format, new MemoryStream(), Arguments);

	/**
	<summary>Packs the given boxed arguments according to the given format.</summary>
	<returns>The string containing the packed data.</returns>
	*/
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string PackToString(string Format, params object[] Arguments) {
		Stream Packed = PackToMemoryStream(Format, Arguments);
		Packed.Position = 0;
		using StreamReader Reader = new(Packed); // This should auto dispose the Packed stream...
		return Reader.ReadToEnd();
	}

	/**
	<summary>Packs the given unboxed arguments according to the given format.</summary>
	<remarks>This method has a limited set of options: s, c are not supported; f, d are rounded down.</remarks>
	<returns>The string containing the packed data.</returns>
	*/
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string PackToString(string Format, params ulong[] Arguments) {
		Stream Packed = PackToMemoryStream(Format, Arguments);
		Packed.Position = 0;
		using StreamReader Reader = new(Packed); // This should auto dispose the Packed stream...
		return Reader.ReadToEnd();
	}

	/**
	<summary>Packs the given boxed arguments according to the given format.</summary>
	<returns>The byte array containing the packed data.</returns>
	*/
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte[] Pack(string Format, params object[] Arguments) {
		using MemoryStream Packed = PackToMemoryStream(Format, Arguments);
		return Packed.ToArray();
	}

	/**
	<summary>Packs the given unboxed arguments according to the given format.</summary>
	<remarks>This method has a limited set of options: s, c are not supported; f, d are rounded down.</remarks>
	<returns>The byte array containing the packed data.</returns>
	*/
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte[] Pack(string Format, params ulong[] Arguments) {
		using MemoryStream Packed = PackToMemoryStream(Format, Arguments);
		return Packed.ToArray();
	}
	#endregion Packing

	#region Reading
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ulong ReadIntBytes(Stream Data, bool BigEndian, int BytesToRead) {
		ulong Out = 0;
		if (BigEndian)
			for (int i = 0; i < BytesToRead; i++)
				Out = (Out << 8) | ((byte) Data.ReadByte());
		else
			for (int i = 0; i < BytesToRead; i++)
				Out |= ((ulong) Data.ReadByte()) << (i * 8);
		return Out;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ReadRecord(List<object> Output, Stream Data, int Integral, byte Count, byte Operation, bool BigEndian) {
		if (Integral == 0) {
			if (Count > 0)
				throw new FormatException($"The '{Operation switch { 0 => 'I', 1 => 's', 2 => 'c', 3 => 'i' }}' option requires a non-zero integral to be specified.");
			if (Operation == 2) {
				Output.Add("");
				return;
			}
			Integral = sizeof(uint);
		}

		switch (Operation) {
			case 0:
			Output.Add(ReadIntBytes(Data, BigEndian, Integral));
			break;
			case 1:
			byte[] Str = new byte[ReadIntBytes(Data, BigEndian, Integral)];
			Data.Read(Str);
			Output.Add(Encoding.UTF8.GetString(Str));
			break;
			case 2:
			byte[] Fixed = new byte[Integral];
			Data.Read(Fixed);
			Output.Add(Encoding.UTF8.GetString(Fixed));
			break;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ReadRecord(List<ulong> Output, Stream Data, int Integral, byte Count, bool BigEndian) {
		if (Integral == 0) {
			if (Count > 0)
				throw new FormatException($"The 'i/I' option requires a non-zero integral to be specified.");
			Integral = sizeof(uint);
		}

		Output.Add(ReadIntBytes(Data, BigEndian, Integral));
	}
	#endregion Reading

	#region Unpacking
	/**
	<summary>Unpacks data in the given Stream according to the given format.</summary>
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
	<summary>Unpacks data in the given Stream according to the given format.</summary>
	<remarks>This method has a limited set of options: s, c are not supported; f, d are rounded down.<br/>No exception will be thrown if the Stream ends prematurely, and the unfinished entries will be undefined.</remarks>
	<returns>The List containing unboxed entries.</returns>
	*/
	public static List<ulong> UnpackUnboxed(string Format, Stream Data) {
		List<ulong> Output = new();

		bool BigEndian = false;
		int Integral = 0;
		byte Count = 0;
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

				ReadRecord(Output, Data, Integral, Count, BigEndian);

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
				Output.Add((ulong) Data.ReadByte());
				break;
				case 'B':
				Output.Add((ulong) Data.ReadByte());
				break;
				case 'i':
				case 'I':
				RecordInt = true;
				break;
				case 'l':
				Data.Read(Slong);
				Output.Add(Slong.AsUlong(BigEndian));
				break;
				case 'L':
				case 'T':
				Data.Read(Slong);
				Output.Add(Slong.AsUlong(BigEndian));
				break;
				case 'h':
				Data.Read(Sshort);
				Output.Add(Sshort.AsUshort(BigEndian));
				break;
				case 'H':
				Data.Read(Sshort);
				Output.Add(Sshort.AsUshort(BigEndian));
				break;
				case 'f':
				Data.Read(Sint);
				Output.Add((ulong) Sint.AsFloat(BigEndian));
				break;
				case 'd':
				Data.Read(Slong);
				Output.Add((ulong) Slong.AsDouble(BigEndian));
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
			ReadRecord(Output, Data, Integral, Count, BigEndian);

		return Output;
	}

	/**
	<summary>Unpacks data in the given byte array according to the given format.</summary>
	<remarks>No exception will be thrown if the byte array ends prematurely, and the unfinished entries will be undefined.</remarks>
	<returns>The List containing boxed entries.</returns>
	*/
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static List<object> Unpack(string Format, byte[] Data) {
		using MemoryStream Mem = new(Data);
		return Unpack(Format, Mem);
	}

	/**
	<summary>Unpacks data in the given byte array according to the given format.</summary>
	<remarks>This method has a limited set of options: s, c are not supported; f, d are rounded down.<br/>No exception will be thrown if the byte array ends prematurely, and the unfinished entries will be undefined.</remarks>
	<returns>The List containing unboxed entries.</returns>
	*/
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static List<ulong> UnpackUnboxed(string Format, byte[] Data) {
		using MemoryStream Mem = new(Data);
		return UnpackUnboxed(Format, Mem);
	}
	#endregion Unpacking
}