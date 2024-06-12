#pragma warning disable IDE0057, CS8509

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Alluseri.Riza;

/// <summary>The core component of Riza.</summary>
public static partial class LuaPack {
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
			if (Operation == 2) {
				if (Count == 0)
					throw new FormatException($"The 'c' option requires a non-optional integral to be specified.");
				// Write nothing, it's 0 bytes long
				return;
			} else {
				if (Count > 0)
					throw new FormatException($"The '{Operation switch { 0 => "i/I", 1 => "s" }}' option requires a non-zero integral to be specified.");
				Integral = sizeof(uint);
			}
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
	private static void WriteRecord<T>(ReflectionHelper<T> Helper, Stream Data, int Integral, byte Count, byte Operation, bool BigEndian, uint Index) where T : notnull {
		if (Integral == 0) {
			if (Operation == 2) {
				if (Count == 0)
					throw new FormatException($"The 'c' option requires a non-optional integral to be specified.");
				// Write nothing, it's 0 bytes long
				return;
			} else {
				if (Count > 0)
					throw new FormatException($"The '{Operation switch { 0 => "i/I", 1 => "s" }}' option requires a non-zero integral to be specified.");
				Integral = sizeof(uint);
			}
		}

		switch (Operation) {
			case 0:
			WriteIntBytes(Data, BigEndian, Integral, Unbox.AsUlong(Helper.Read(Index)!));
			break;
			case 1:
			byte[] Str = Encoding.UTF8.GetBytes(Helper.Read<string>(Index)!);
			WriteIntBytes(Data, BigEndian, Integral, (ulong) Str.Length);
			Data.Write(Str);
			break;
			case 2:
			byte[] LargeStr = new byte[Integral];
			if (Encoding.UTF8.GetBytes(Helper.Read<string>(Index)!, LargeStr) > Integral)
				throw new ArgumentException($"Cannot write '{Helper.Read<string>(Index)}' into only {Integral} UTF-8 bytes."); // We don't really care about double-call here, since if that exception happens, it... happens.
			Data.Write(LargeStr);
			break;
		}
	}
	#endregion Writing

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
			if (Operation == 2) {
				if (Count == 0)
					throw new FormatException($"The 'c' option requires a non-optional integral to be specified.");
				Output.Add(""); // Just write a "", because you'll read nothing.
				return;
			} else {
				if (Count > 0)
					throw new FormatException($"The '{Operation switch { 0 => "i/I", 1 => "s" }}' option requires a non-zero integral to be specified.");
				Integral = sizeof(uint);
			}
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
	private static void ReadRecord<T>(ReflectionHelper<T> Instance, uint Index, Stream Data, int Integral, byte Count, byte Operation, bool BigEndian) where T : notnull {
		if (Integral == 0) {
			if (Operation == 2) {
				if (Count == 0)
					throw new FormatException($"The 'c' option requires a non-optional integral to be specified.");
				Instance.Write(Index, ""); // Just write a "", because you'll read nothing.
				return;
			} else {
				if (Count > 0)
					throw new FormatException($"The '{Operation switch { 0 => "i/I", 1 => "s" }}' option requires a non-zero integral to be specified.");
				Integral = sizeof(uint);
			}
		}

		switch (Operation) {
			case 0:
			Instance.WriteBox(Index, ReadIntBytes(Data, BigEndian, Integral));
			break;
			case 1:
			byte[] Str = new byte[ReadIntBytes(Data, BigEndian, Integral)];
			Data.Read(Str);
			Instance.Write(Index, Encoding.UTF8.GetString(Str));
			break;
			case 2:
			byte[] Fixed = new byte[Integral];
			Data.Read(Fixed);
			Instance.Write(Index, Encoding.UTF8.GetString(Fixed));
			break;
		}
	}
	#endregion Reading
}