using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace Immediate.Validations;

internal static class SyntaxExtensions
{
	[ExcludeFromCodeCoverage]
	public static bool IsNameOfExpression(this ExpressionSyntax syntax, [NotNullWhen(returnValue: true)] out ExpressionSyntax? argumentExpression)
	{
		if (syntax is InvocationExpressionSyntax
			{
				Expression: SimpleNameSyntax { Identifier.ValueText: "nameof" },
				ArgumentList.Arguments: [{ Expression: { } expr }],
			}
		)
		{
			argumentExpression = expr;
			return true;
		}
		else
		{
			argumentExpression = null;
			return false;
		}
	}

	public static IReadOnlyList<(string? Target, IObjectCreationOperation Attribute)> GetTargetedAttributes(
		this SyntaxNode declaration,
		SemanticModel semanticModel,
		CancellationToken token
	)
	{
		var attributeLists = declaration switch
		{
			PropertyDeclarationSyntax property => property.AttributeLists,
			ParameterSyntax parameter => parameter.AttributeLists,
			_ => throw new InvalidOperationException($"Property declared using a `{declaration.GetType().FullName}`."),
		};

		var attributes = new List<(string? Target, IObjectCreationOperation Attribute)>();
		foreach (var attributeList in attributeLists)
		{
			var target = attributeList.Target?.Identifier.ValueText;
			if (declaration is ParameterSyntax && target is "property")
				target = null;

			foreach (var attribute in attributeList.Attributes)
			{
				if (semanticModel.GetOperation(attribute, token) is IAttributeOperation { Operation: IObjectCreationOperation operation })
					attributes.Add((target, operation));
			}
		}

		return attributes;
	}
}
