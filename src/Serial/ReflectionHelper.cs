using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;
using static System.Reflection.Emit.OpCodes;

namespace Alluseri.Riza;

internal class ReflectionHelper<T> where T : notnull {
	private readonly T Instance;
	private static readonly object SyncLock = new();
	private static Dictionary<Type, Dictionary<uint, ReflectionRecord>> Cache = new();
	private Dictionary<uint, ReflectionRecord> Fields = new();

	private ReflectionHelper(T Instance) {
		this.Instance = Instance ?? throw new ArgumentNullException(nameof(Instance), "Something went horribly wrong! Instance shouldn't be null!");
		foreach (FieldInfo Field in Instance.GetType().GetFields()) {
			RizaFieldAttribute? RzAttr = Field.GetCustomAttribute<RizaFieldAttribute>(true);
			if (RzAttr != null)
				Fields[RzAttr.Ordinal] = new(Field);
		}
	}

	private ReflectionHelper(T Instance, Dictionary<uint, ReflectionRecord> Cache) {
		this.Instance = Instance ?? throw new ArgumentNullException(nameof(Instance), "Something went horribly wrong! Instance shouldn't be null!");
		Fields = Cache;
	}

	public static ReflectionHelper<T> Fetch(T Instance) {
		Monitor.Enter(SyncLock);
		try {
			if (Cache.TryGetValue(typeof(T), out var Cached)) {
				return new(Instance, Cached);
			} else {
				ReflectionHelper<T> New = new(Instance);
				Cache[typeof(T)] = New.Fields;
				return New;
			}
		} finally { Monitor.Exit(SyncLock); }
	}

	public object? Read(uint Index) {
		if (Fields.TryGetValue(Index, out ReflectionRecord? Field))
			return Field.Read(Instance);
		else
			return default;
	}

	public V? Read<V>(uint Index) => (V?) Read(Index);

	public void Write(uint Index, object Value) {
		if (Value is IConvertible Conv) {
			WriteBox(Index, Conv);
		} else if (Fields.TryGetValue(Index, out ReflectionRecord? Field)) {
			throw new NotImplementedException($"Writing arbitrary objects (such as {Value.GetType()} {Value}) is not supported yet.");
		}
	}

	public void WriteBox(uint Index, IConvertible Value) {
		if (Fields.TryGetValue(Index, out ReflectionRecord? Field))
			Field.WriteBox(Instance, Value);
	}

	class ReflectionRecord {
		private delegate object ReadValue(T Instance);
		private delegate void WriteValue(T Instance, object Value);

		private readonly FieldInfo Info;
		private readonly TypeCode Code;

		private readonly bool HasDynamic = false;
		private readonly ReadValue DynamicGetter = null!;
		private readonly WriteValue DynamicSetter = null!;

		public ReflectionRecord(FieldInfo Field) {
			Info = Field;
			Code = Type.GetTypeCode(Field.FieldType);

			if (RuntimeFeature.IsDynamicCodeSupported) {
				#region Getter
				DynamicMethod DynGetter = new(
					"ZGetterFor" + Field.Name,
					typeof(object),
					[
						typeof(T)
					],
					typeof(T)
				);

				ILGenerator DynGetterCode = DynGetter.GetILGenerator();
				DynGetterCode.Emit(Ldarg_0);
				DynGetterCode.Emit(Ldfld, Field);
				DynGetterCode.Emit(Box, Field.FieldType);
				DynGetterCode.Emit(Ret);
				#endregion Getter

				#region Setter
				DynamicMethod DynSetter = new(
					"ZSetterFor" + Field.Name,
					typeof(void),
					[
						typeof(T),
						typeof(object)
					],
					typeof(T)
				);

				ILGenerator DynSetterCode = DynSetter.GetILGenerator();
				DynSetterCode.Emit(Ldarg_0);
				DynSetterCode.Emit(Ldarg_1);
				DynSetterCode.Emit(Unbox_Any, Field.FieldType);
				DynSetterCode.Emit(Stfld, Field);
				DynSetterCode.Emit(Ret);
				#endregion Setter

				DynamicGetter = DynGetter.CreateDelegate<ReadValue>();
				DynamicSetter = DynSetter.CreateDelegate<WriteValue>();

				HasDynamic = true;
			} else {
				HasDynamic = false;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public object? Read(T Instance) => HasDynamic ? DynamicGetter(Instance) : Info.GetValue(Instance);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteBox(T Instance, IConvertible Value) {
			object UnboxedValue = Code switch {
				TypeCode.String => (string) Value,
				TypeCode.SByte => (sbyte) Unbox.AsByte(Value),
				TypeCode.Byte => Unbox.AsByte(Value),
				TypeCode.Int16 => (short) Unbox.AsUshort(Value),
				TypeCode.UInt16 => Unbox.AsUshort(Value),
				TypeCode.Int32 => (int) Unbox.AsUint(Value),
				TypeCode.UInt32 => Unbox.AsUint(Value),
				TypeCode.Int64 => (long) Unbox.AsUlong(Value),
				TypeCode.UInt64 => Unbox.AsUlong(Value),
				TypeCode.Single => Unbox.AsFloat(Value),
				TypeCode.Double => Unbox.AsDouble(Value),
				_ => throw new NotSupportedException($"Bad type for field {Info.Name} of class {Instance.GetType().Name}.")
			};

			if (HasDynamic) {
				DynamicSetter(Instance, UnboxedValue);
			} else {
				Info.SetValue(Instance, UnboxedValue);
			}
		}
	}
}