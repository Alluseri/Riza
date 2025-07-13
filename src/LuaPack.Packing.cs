using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Alluseri.Riza;

// TODO: You. From the future. In v3.0.0, you MUST generalize all methods into one single route. I WILL NOT TOLERATE THIS SHIT

public static unsafe partial class LuaPack {
	/**
	<summary>Packs the given boxed arguments according to the given format into the given stream.</summary>
	<returns>The same stream.</returns>
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
				case 'j':
				case 'J':
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
				case 'n':
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
	<summary>Packs the given object according to the given format into the given stream.</summary>
	<returns>The same stream.</returns>
	*/
	public static Stream PackToStream<T>(string Format, Stream Data, T Object) where T : class {
		ReflectionHelper<T> Helper = ReflectionHelper<T>.Fetch(Object);

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

				WriteRecord(Helper, Data, Integral, Count, Operation, BigEndian, Stack);

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
				// Fuick this poiece of shit
				Data.WriteByte(Unbox.AsByte(Helper.Read(Stack++)!));
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
				case 'j':
				case 'J':
				Data.Write(Unbox.AsUlong(Helper.Read(Stack++)!).AsBytes(Ul, BigEndian));
				break;
				case 'h':
				case 'H':
				Data.Write(Unbox.AsUshort(Helper.Read(Stack++)!).AsBytes(Us, BigEndian));
				break;
				case 'f':
				Data.Write(Helper.Read<float>(Stack++).AsBytes(Ui, BigEndian));
				break;
				case 'd':
				case 'n':
				Data.Write(Helper.Read<double>(Stack++).AsBytes(Ul, BigEndian));
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
			WriteRecord(Helper, Data, Integral, Count, Operation, BigEndian, Stack);

		return Data;
	}

	/**
	<inheritdoc cref="PackToStream(string, Stream, object[])"/>
	*/
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Stream PackToStream(Stream Data, string Format, params object[] Arguments)
	=> PackToStream(Format, Data, Arguments);

	/**
	<inheritdoc cref="PackToStream{T}(string, Stream, T)"/>
	*/
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Stream PackToStream<T>(Stream Data, string Format, T Object) where T : class
	=> PackToStream(Format, Data, Object);

	/**
	<summary>Packs the given boxed arguments according to the given format.</summary>
	<returns>The MemoryStream containing the packed data. The Position of that MemoryStream is not reset.</returns>
	*/
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static MemoryStream PackToMemoryStream(string Format, params object[] Arguments)
	=> (MemoryStream) PackToStream(Format, new MemoryStream(), Arguments);

	/**
	<summary>Packs the given object according to the given format.</summary>
	<returns>The MemoryStream containing the packed data. The Position of that MemoryStream is not reset.</returns>
	*/
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static MemoryStream PackToMemoryStream<T>(string Format, T Object) where T : class
	=> (MemoryStream) PackToStream(Format, new MemoryStream(), Object);

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
	<summary>Packs the given object according to the given format.</summary>
	<returns>The string containing the packed data.</returns>
	*/
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string PackToString<T>(string Format, T Object) where T : class {
		Stream Packed = PackToMemoryStream(Format, Object);
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
	<summary>Packs the given object according to the given format.</summary>
	<returns>The byte array containing the packed data.</returns>
	*/
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte[] Pack<T>(string Format, T Object) where T : class {
		using MemoryStream Packed = PackToMemoryStream(Format, Object);
		return Packed.ToArray();
	}
}