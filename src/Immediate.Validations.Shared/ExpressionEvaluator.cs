using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace Immediate.Validations.Shared;

internal static class ExpressionEvaluator
{
	public static object? GetValue(Expression? expr) =>
		expr switch
		{
			BinaryExpression be => GetValueFromBinary(be),
			ConditionalExpression ce => GetValueFromConditional(ce),
			ConstantExpression ce => ce.Value,
			MemberExpression me => GetValueFromMember(me),
			MethodCallExpression mce => GetValueFromMethodCall(mce),
			NewExpression ne => GetValueFromNew(ne),
			NewArrayExpression nae => GetValueFromNewArray(nae),
			UnaryExpression ue => GetValueFromUnary(ue),
			null => null,
			_ => throw new NotSupportedException(/* TODO: Error Message */),
		};

	[SuppressMessage(
		"Style",
		"IDE0072:Add missing cases",
		Justification = "Not possible with BinaryExpression"
	)]
	private static object? GetValueFromBinary(BinaryExpression be)
	{
		if (be.Method is { } method)
		{
			return method.Invoke(
				null,
				parameters: [
					GetValue(be.Left),
					GetValue(be.Right)
				]
			);
		}
		else if (be.NodeType is ExpressionType.AndAlso)
		{
			return (bool)GetValue(be.Left)! && (bool)GetValue(be.Right)!;
		}
		else if (be.NodeType is ExpressionType.OrElse)
		{
			return (bool)GetValue(be.Left)! || (bool)GetValue(be.Right)!;
		}
		else if (be.NodeType is ExpressionType.Coalesce)
		{
			return GetValue(be.Left) ?? GetValue(be.Right);
		}

		dynamic left = GetValue(be.Left)!;
		dynamic right = GetValue(be.Right)!;

		return be.NodeType switch
		{
			ExpressionType.Add => left + right,
			ExpressionType.AddChecked => checked(left + right),
			ExpressionType.Subtract => left - right,
			ExpressionType.SubtractChecked => checked(left - right),
			ExpressionType.Multiply => left * right,
			ExpressionType.MultiplyChecked => checked(left * right),
			ExpressionType.Divide => left / right,
			ExpressionType.Modulo => left % right,

			ExpressionType.ExclusiveOr => left ^ right,
			ExpressionType.And => left & right,
			ExpressionType.Or => left | right,
			ExpressionType.LeftShift => left << right,
			ExpressionType.RightShift => left >> right,
			ExpressionType.ArrayIndex => left.GetValue(right),

			ExpressionType.Equal => left == right,
			ExpressionType.NotEqual => left != right,
			ExpressionType.LessThan => left < right,
			ExpressionType.LessThanOrEqual => left <= right,
			ExpressionType.GreaterThan => left > right,
			ExpressionType.GreaterThanOrEqual => left >= right,

			_ => throw new UnreachableException(),
		};
	}

	private static object? GetValueFromConditional(ConditionalExpression ce) =>
		(bool)GetValue(ce.Test)!
			? GetValue(ce.IfTrue)
			: GetValue(ce.IfFalse);

	private static object? GetValueFromMember(MemberExpression me)
	{
		var obj = GetValue(me.Expression);

		return me.Member switch
		{
			PropertyInfo pi => pi.GetValue(obj),
			FieldInfo fi => fi.GetValue(obj),
			_ => throw new UnreachableException(),
		};
	}

	private static object? GetValueFromMethodCall(MethodCallExpression mce)
	{
		var obj = GetValue(mce.Object);

		var arguments = mce.Arguments
			.Select(GetValue)
			.ToArray();

		return mce.Method.Invoke(obj, arguments);
	}

	private static object? GetValueFromNew(NewExpression ne)
	{
		if (ne.Constructor is null)
			throw new NotSupportedException(/* TODO: Error Message */);

		var arguments = ne.Arguments
			.Select(GetValue)
			.ToArray();

		return ne.Constructor.Invoke(arguments);
	}

	private static Array? GetValueFromNewArray(NewArrayExpression nae)
	{
		if (nae.Type.GetElementType() is not { } elementType)
			throw new NotSupportedException(/* TODO: Error Message */);

		var array = Array.CreateInstance(
			elementType,
			nae.Expressions.Count
		);

		for (var i = 0; i < array.Length; ++i)
		{
			array.SetValue(value: GetValue(nae.Expressions[i]), index: i);
		}

		return array;
	}

	private static object? GetValueFromUnary(UnaryExpression ue) =>
		ue switch
		{
			{ Method: { } method } => method.Invoke(null, parameters: [GetValue(ue.Operand)]),
			{ NodeType: ExpressionType.TypeAs } =>
				ue.Type.IsAssignableFrom(ue.Operand.Type)
					? ue.Operand
					: null,

			{ NodeType: ExpressionType.Not } => !(dynamic)GetValue(ue.Operand)!,
			{ NodeType: ExpressionType.Negate } => -(dynamic)GetValue(ue.Operand)!,
			{ NodeType: ExpressionType.UnaryPlus } => +(dynamic)GetValue(ue.Operand)!,
			{ NodeType: ExpressionType.OnesComplement } => ~(dynamic)GetValue(ue.Operand)!,
			{ NodeType: ExpressionType.Quote } => GetValue(ue.Operand),

			{ NodeType: ExpressionType.Convert } => throw new NotImplementedException(/* TODO: Error Message */),
			{ NodeType: ExpressionType.ConvertChecked } => throw new NotImplementedException(/* TODO: Error Message */),

			_ => throw new UnreachableException(),
		};
}
