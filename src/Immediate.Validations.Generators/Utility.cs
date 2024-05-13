using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Scriban;

namespace Immediate.Validations.Generators;

[ExcludeFromCodeCoverage]
internal static class Utility
{
	public static Template GetTemplate(string name)
	{
		using var stream = Assembly
			.GetExecutingAssembly()
			.GetManifestResourceStream(
				$"Immediate.Validations.Generators.Templates.{name}.sbntxt"
			)!;

		using var reader = new StreamReader(stream);
		return Template.Parse(reader.ReadToEnd());
	}

	public static string? NullIf(this string value, string check) =>
		value.Equals(check, StringComparison.Ordinal) ? null : value;

	public static T? SingleValue<T>(this IEnumerable<T> source)
	{
		using var enumerator = source.GetEnumerator();
		if (!enumerator.MoveNext())
			return default;

		var c = enumerator.Current;
		if (enumerator.MoveNext())
			return default;

		return c;
	}

	public static bool SatisfiesConstraints(ITypeParameterSymbol typeParameter, ITypeSymbol typeArgument, Compilation compilation)
	{
		if (typeArgument.IsPointerOrFunctionPointer() || typeArgument.IsRefLikeType)
			return false;

		if ((typeParameter.HasReferenceTypeConstraint && !typeArgument.IsReferenceType)
			|| (typeParameter.HasValueTypeConstraint && !typeArgument.IsNonNullableValueType())
			|| (typeParameter.HasUnmanagedTypeConstraint && !(typeArgument.IsUnmanagedType && typeArgument.IsNonNullableValueType()))
			|| (typeParameter.HasConstructorConstraint && !SatisfiesConstructorConstraint(typeArgument)))
		{
			return false;
		}

		foreach (var typeConstraint in typeParameter.ConstraintTypes)
		{
			var substitutedConstraintType = SubstituteType(compilation, typeConstraint, typeParameter, typeArgument);
			var conversion = compilation.ClassifyConversion(typeArgument, substitutedConstraintType);

			if (typeArgument.IsNullableType()
				|| conversion is not ({ IsIdentity: true } or { IsImplicit: true, IsReference: true } or { IsBoxing: true }))
			{
				return false;
			}
		}

		return true;
	}

	public static bool IsNonNullableValueType(this ITypeSymbol typeArgument)
	{
		if (!typeArgument.IsValueType)
			return false;

		return !IsNullableTypeOrTypeParameter(typeArgument);
	}

	public static bool IsNullableTypeOrTypeParameter(this ITypeSymbol? type)
	{
		if (type is null)
			return false;

		if (type.TypeKind == TypeKind.TypeParameter)
		{
			var constraintTypes = ((ITypeParameterSymbol)type).ConstraintTypes;
			foreach (var constraintType in constraintTypes)
			{
				if (constraintType.IsNullableTypeOrTypeParameter())
					return true;
			}

			return false;
		}

		return type.IsNullableType();
	}

	/// <summary>
	/// Is this System.Nullable`1 type, or its substitution.
	///
	/// To check whether a type is System.Nullable`1 or is a type parameter constrained to System.Nullable`1
	/// use <see cref="IsNullableTypeOrTypeParameter" /> instead.
	/// </summary>
	public static bool IsNullableType(this ITypeSymbol type) =>
		type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;

	public static bool IsPointerOrFunctionPointer(this ITypeSymbol type) =>
		type.TypeKind is TypeKind.Pointer or TypeKind.FunctionPointer;

	[SuppressMessage("Style", "IDE0072:Add missing cases")]
	private static bool SatisfiesConstructorConstraint(ITypeSymbol typeArgument) =>
		typeArgument.TypeKind switch
		{
			TypeKind.Struct => true,
			TypeKind.Enum => true,
			TypeKind.Dynamic => true,

			TypeKind.Class =>
				HasPublicParameterlessConstructor((INamedTypeSymbol)typeArgument) && !typeArgument.IsAbstract,

			TypeKind.TypeParameter =>
				typeArgument is ITypeParameterSymbol tps
				&& (tps.HasConstructorConstraint || tps.IsValueType),

			_ => false,
		};

	private static bool HasPublicParameterlessConstructor(INamedTypeSymbol type)
	{
		foreach (var constructor in type.InstanceConstructors)
		{
			if (constructor.Parameters.Length == 0)
				return constructor.DeclaredAccessibility == Accessibility.Public;
		}

		return false;
	}

	private static ITypeSymbol SubstituteType(Compilation compilation, ITypeSymbol type, ITypeParameterSymbol typeParameter, ITypeSymbol typeArgument)
	{
		return Visit(type);

		ITypeSymbol Visit(ITypeSymbol type)
		{
			switch (type)
			{
				case ITypeParameterSymbol typeParameterSymbol:
					return SymbolEqualityComparer.Default.Equals(typeParameterSymbol, typeParameter)
						? typeArgument
						: type;

				case IArrayTypeSymbol { ElementType: var elementType, Rank: var rank } arrayTypeSymbol:
					var visitedElementType = Visit(elementType);
					return ReferenceEquals(elementType, visitedElementType)
						? arrayTypeSymbol
						: compilation.CreateArrayTypeSymbol(visitedElementType, rank);

				case INamedTypeSymbol { OriginalDefinition: var originalDefinition, TypeArguments: var typeArguments } namedTypeSymbol:
					var visitedTypeArguments = new ITypeSymbol[typeArguments.Length];
					var anyChanged = false;
					for (var i = 0; i < typeArguments.Length; i++)
					{
						var typeArgument = typeArguments[i];
						var visited = Visit(typeArgument);
						if (!ReferenceEquals(visited, typeArgument))
							anyChanged = true;
						visitedTypeArguments[i] = visited;
					}

					return anyChanged ? originalDefinition.Construct(visitedTypeArguments) : namedTypeSymbol;

				default:
					return type;
			}
		}
	}
}
