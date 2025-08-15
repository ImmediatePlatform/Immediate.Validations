using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Immediate.Validations;

[ExcludeFromCodeCoverage]
internal static class SyntaxExtensions
{
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
}
