using System;
using System.Collections.Generic;
using System.Reflection;

namespace Alluseri.Riza;

// TODO: Review the possibility to [MethodImpl(MethodImplOptions.AggressiveInlining)]

internal class ReflectionHelper<T> where T : notnull {
	private readonly T Instance;
	Dictionary<uint, FieldInfo> Fields = new();

	public ReflectionHelper(T Instance) {
		this.Instance = Instance ?? throw new ArgumentNullException(nameof(Instance), "Something went horribly wrong! Instance shouldn't be null!");
		foreach (FieldInfo Field in Instance.GetType().GetFields()) {
			RizaFieldAttribute? RzAttr = Field.GetCustomAttribute<RizaFieldAttribute>(true);
			if (RzAttr != null)
				Fields[RzAttr.Ordinal] = Field;
		}
	}

	public object? Read(uint Index) {
		if (Fields.TryGetValue(Index, out FieldInfo? Field))
			return Field?.GetValue(Instance);
		else
			return default;
	}

	public V? Read<V>(uint Index) {
		if (Fields.TryGetValue(Index, out FieldInfo? Field))
			return (V?) Field?.GetValue(Instance);
		else
			return default;
	}

	/*public V? ReadConvertible<V>(uint Index) where V : IConvertible {
		if (Fields.TryGetValue(Index, out FieldInfo? Field))
			return (V?) Convert.ChangeType(Field.GetValue(Instance), typeof(V));
		else
			return default;
	}*/

	public void Write(uint Index, object Value) {
		if (Fields.TryGetValue(Index, out FieldInfo? Field))
			Field.SetValue(Instance, Value);
	}

	public void WriteBox(uint Index, IConvertible Value) {
		if (Fields.TryGetValue(Index, out FieldInfo? Field)) {
			Field.SetValue(Instance, Type.GetTypeCode(Field.FieldType) switch {
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
				_ => Convert.ChangeType(Value, Field.FieldType) // Guaranteed to fail because C# is cringe
			});
		}
	}

	/*public void WriteConvertible<V>(uint Index, V Value) where V : IConvertible {
		if (Fields.TryGetValue(Index, out FieldInfo? Field))
			Field.SetValue(Instance, Convert.ChangeType(Value, Field.FieldType));
	}*/
}