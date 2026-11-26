using System.Globalization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace Immediate.Validations.Generators;

internal static class OpenApiConstraints
{
	public static void Add(List<OpenApiConstraint> constraints, ITypeSymbol type, IObjectCreationOperation attribute)
	{
		if (attribute.Type is not INamedTypeSymbol
			{
				ContainingNamespace.IsImmediateValidationsShared: true,
				ContainingAssembly.Name: "Immediate.Validations.Shared",
			} validator)
		{
			return;
		}

		if (type.SpecialType is SpecialType.System_String)
		{
			AddStringConstraints(constraints, validator.Name, attribute);
			return;
		}

		var bound = validator.Name switch
		{
			"GreaterThanAttribute" => "ExclusiveMinimum",
			"GreaterThanOrEqualAttribute" => "Minimum",
			"LessThanAttribute" => "ExclusiveMaximum",
			"LessThanOrEqualAttribute" => "Maximum",
			_ => null,
		};
		if (bound is not null
			&& type.SpecialType is SpecialType.System_SByte or SpecialType.System_Byte
				or SpecialType.System_Int16 or SpecialType.System_UInt16
				or SpecialType.System_Int32 or SpecialType.System_UInt32
				or SpecialType.System_Int64 or SpecialType.System_UInt64
			&& GetArgument(attribute, 0) is (sbyte or byte or short or ushort or int or uint or long or ulong) and IFormattable value)
		{
			constraints.Add(new(bound, value.ToString(format: null, CultureInfo.InvariantCulture)));
		}

		if (validator.Name is "EmptyAttribute" or "NotEmptyAttribute" && IsCollection(type))
			constraints.Add(validator.Name is "EmptyAttribute" ? new("MaxItems", "0") : new("MinItems", "1"));
	}

	private static void AddStringConstraints(List<OpenApiConstraint> constraints, string validator, IObjectCreationOperation attribute)
	{
		if (validator is "NotEmptyAttribute")
			constraints.Add(new("MinLength", "1"));

		if (validator is "LengthAttribute" or "MinLengthAttribute" && GetArgument(attribute, 0) is int and >= 0 and var minimum)
		{
			// JSON Schema counts code points. A code point can occupy two UTF-16 code units.
			var minimumCodePoints = minimum / 2 + minimum % 2;
			constraints.Add(new("MinLength", minimumCodePoints.ToString(CultureInfo.InvariantCulture)));
		}

		if (validator is "LengthAttribute" or "MaxLengthAttribute"
			&& GetArgument(attribute, validator is "LengthAttribute" ? 1 : 0) is int and >= 0 and var maximum)
		{
			constraints.Add(new("MaxLength", maximum.ToString(CultureInfo.InvariantCulture)));
		}
	}

	private static object? GetArgument(IObjectCreationOperation attribute, int index)
	{
		var value = attribute.Arguments.FirstOrDefault(a => a.Parameter?.Ordinal == index)?.Value;
		while (value is IConversionOperation { IsImplicit: true, Type.SpecialType: SpecialType.System_Object, Operand: var operand })
			value = operand;

		return value?.ConstantValue is { HasValue: true, Value: var constant } ? constant : null;
	}

	private static bool IsCollection(ITypeSymbol type) =>
		type is IArrayTypeSymbol
		|| type.ToDisplayString() is "System.Collections.ICollection"
		|| type.AllInterfaces.Any(i => i.ToDisplayString() is "System.Collections.ICollection");
}
