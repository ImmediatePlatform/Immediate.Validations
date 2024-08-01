using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Reflection;

namespace Immediate.Validations.Shared;

/// <summary>
///	    Applied to a property to indicate that the value of the property should be a defined value of the <see
///     langword="enum" />.
/// </summary>
public sealed class EnumValueAttribute : ValidatorAttribute
{
	/// <summary>
	///	    Validates that the given <see langword="enum" /> <paramref name="value"/> is properly defined.
	/// </summary>
	/// <typeparam name="T">
	///	    The type of the provided <see langword="enum" /> value.
	/// </typeparam>
	/// <param name="value">
	///	    The value to validate.
	/// </param>
	/// <returns>
	///	    <see langword="true" /> if the property is valid; <see langword="false" /> otherwise.
	/// </returns>
	public static bool ValidateProperty<T>(T value)
		where T : struct, Enum
	{
		if (Enum.IsDefined(value))
			return true;

		if (typeof(T).GetCustomAttribute<FlagsAttribute>() is not { })
			return false;

		return value.IsValidFlags();
	}

	/// <summary>
	///		The default message template when the property is invalid.
	/// </summary>
	public static string DefaultMessage => ValidationConfiguration.Localizer[nameof(EnumValueAttribute)].Value;
}

file static class Enums
{
	public static bool IsValidFlags<TEnum>(this TEnum value)
		where TEnum : struct, Enum =>
		Cache<TEnum>.Instance.IsValidFlags(value);

}

file abstract class Cache<TEnum>
	where TEnum : struct, Enum
{
	public static readonly Cache<TEnum> Instance = GetInstance();

	[SuppressMessage("Style", "IDE0072:Add missing cases", Justification = "Other types are not supported")]
	private static Cache<TEnum> GetInstance() =>
		Type.GetTypeCode(typeof(TEnum)) switch
		{
			TypeCode.Char => new Cache<TEnum, char>(),
			TypeCode.SByte => new Cache<TEnum, sbyte>(),
			TypeCode.Byte => new Cache<TEnum, byte>(),
			TypeCode.Int16 => new Cache<TEnum, short>(),
			TypeCode.UInt16 => new Cache<TEnum, ushort>(),
			TypeCode.Int32 => new Cache<TEnum, int>(),
			TypeCode.UInt32 => new Cache<TEnum, uint>(),
			TypeCode.Int64 => new Cache<TEnum, long>(),
			TypeCode.UInt64 => new Cache<TEnum, ulong>(),
			_ => throw new NotSupportedException($"Enum underlying type of {Enum.GetUnderlyingType(typeof(TEnum))} is not supported"),
		};

	public abstract bool IsValidFlags(TEnum value);
}

file sealed class Cache<TEnum, TUnderlying> : Cache<TEnum>
	where TEnum : struct, Enum
	where TUnderlying : struct,
		IComparable<TUnderlying>,
		IEquatable<TUnderlying>,
		IBinaryInteger<TUnderlying>,
		IConvertible
{
	private readonly TUnderlying _allFlags = GetAllFlags();

	private static TUnderlying GetAllFlags()
	{
		TUnderlying allFlags = default;

		var fields = typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static);
		foreach (var field in fields)
		{
			var value = (TUnderlying)field.GetValue(null)!;
			allFlags |= value;
		}

		return allFlags;
	}

	public override bool IsValidFlags(TEnum value)
	{
		var underlying = (TUnderlying)(object)value;
		return (underlying & _allFlags) == underlying;
	}
}
