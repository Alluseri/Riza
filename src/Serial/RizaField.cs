using System;

namespace Alluseri.Riza;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
sealed class RizaFieldAttribute : Attribute {
	public readonly uint Ordinal;

	public RizaFieldAttribute(uint Ordinal) {
		this.Ordinal = Ordinal;
	}
}