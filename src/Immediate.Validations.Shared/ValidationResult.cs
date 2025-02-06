using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Immediate.Validations.Shared;

/// <summary>
///		Represents the result of an in-progress or completed validation.
/// </summary>
[SuppressMessage("Security", "MA0009:Add regex evaluation timeout", Justification = "Limited danger scope (property names and message templates)")]
public sealed partial class ValidationResult : IEnumerable<ValidationError>
{
	[GeneratedRegex("{(?<name>[^{}:]+)(?::(?<format>[^{}]+))?}", RegexOptions.ExplicitCapture)]
	private static partial Regex FormatRegex();

	private List<ValidationError>? _errors;
	private HashSet<string>? _types;

	/// <summary>
	///		Internal function used to support tracking which types have been visited as part of validation.
	/// </summary>
	/// <remarks>
	///		Should not be used by consumers.
	/// </remarks>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
	public bool VisitType(Type type) =>
		(_types ??= []).Add(type.ToString());

	/// <summary>
	///		Indicates whether the validation was successful.
	/// </summary>
	public bool IsValid => _errors is null or [];

	/// <summary>
	///		The list of errors held by the current validation attempt.
	/// </summary>
	public IReadOnlyList<ValidationError> Errors => _errors ?? [];

	IEnumerator<ValidationError> IEnumerable<ValidationError>.GetEnumerator() => (_errors ?? []).AsEnumerable().GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => (this as IEnumerable<ValidationError>).GetEnumerator();

	/// <summary>
	///	    Unconditionally add a <see cref="ValidationError"/>.
	/// </summary>
	/// <param name="validationError">
	///		The <see cref="ValidationError"/> to add to the current list.
	/// </param>
	public void Add(
		ValidationError validationError
	)
	{
		(_errors ??= []).Add(validationError);
	}

	/// <summary>
	///	    Unconditionally add a range of <see cref="ValidationError" />s.
	/// </summary>
	/// <param name="errors">
	///		A sequence <see cref="ValidationError"/> to add to the current list.
	/// </param>
	public void AddRange(
		IEnumerable<ValidationError> errors
	)
	{
		(_errors ??= []).AddRange(errors);
	}

	/// <summary>
	///	    Unconditionally add a <see cref="ValidationError"/> for <paramref name="propertyName"/>.
	/// </summary>
	/// <param name="propertyName">
	///	    The name of the property that failed the validation.
	/// </param>
	/// <param name="messageTemplate">
	///	    A template message, which will be filled in with the values from <paramref name="arguments"/>.
	/// </param>
	/// <param name="arguments">
	///	    The values which can be used with <paramref name="messageTemplate"/> to build the final validation message.
	/// </param>
	[SuppressMessage("Design", "MA0016:Prefer using collection abstraction instead of implementation")]
	public void Add(
		string propertyName,
		string messageTemplate,
		Dictionary<string, object?>? arguments = default
	)
	{
		(_errors ??= []).Add(new()
		{
			PropertyName = propertyName,
			ErrorMessage =
				arguments is null
				? messageTemplate
				: FormatRegex().Replace(
					messageTemplate,
					m =>
					{
						var key = m.Groups["name"].Value;

						if (!arguments.TryGetValue(key, out var value))
							return m.Value;

						if (m.Groups["format"] is { Success: true, Value: var formatSpecifier })
							return string.Format(provider: null, $"{{0:{formatSpecifier}}}", value);

						return value?.ToString()!;
					}
				),
		});
	}

	/// <summary>
	///	    Evaluates an <paramref name="expression"/> and conditionally adds a <see cref="ValidationError"/> if the
	///     property is invalid.
	/// </summary>
	/// <param name="expression">
	///	    An expression representing a property as well as how it should be validated.
	/// </param>
	/// <param name="overrideMessage">
	///	    An optional message template to be used instead of the default message for the <see
	///     cref="ValidatorAttribute"/> that was used.
	/// </param>
	/// <exception cref="NotSupportedException">
	///	    If an invalid <see cref="Expression"/> is provided, for example, 
	/// </exception>
	/// <remarks>
	///	    The <see cref="Expression" /> must follow the form
	///	    <code>() => ValidatorAttribute.ValidateProperty(t.Property/*, other parameters as necessary */)</code>
	///	    where <c>ValidatorAttribute</c> is the validator and <c>t.Property</c> is the value that should be 
	///	    validated. 
	/// </remarks>
	public void Add(
		Expression<Func<bool>> expression,
		string? overrideMessage = null
	)
	{
		if (expression is not
			{
				Body: MethodCallExpression
				{
					Method: { IsStatic: true, Name: "ValidateProperty", } method,
					Arguments: [{ } targetPropertyExpression, ..] argumentExpressions,
				},
				Parameters: [],
			}
			|| method.ReturnType != typeof(bool))
		{
			throw new NotSupportedException("Invalid Validation Expression");
		}

		if (method.DeclaringType?.IsAssignableTo(typeof(ValidatorAttribute)) is not true)
			throw new NotSupportedException("Invalid Validation Expression");

		var argumentValues = argumentExpressions.Select(ExpressionEvaluator.GetValue).ToArray();

		if ((bool)method.Invoke(null, argumentValues)!)
			return;

		var targetObject = GetTargetObject(targetPropertyExpression);

		var message = overrideMessage ?? GetValidationMessage(method.DeclaringType);

		var arguments = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
		{
			["PropertyValue"] = argumentValues[0],
			["PropertyName"] = GetMemberName(targetPropertyExpression, targetObject),
		};

		foreach (var (expr, param, value) in argumentExpressions.Zip(method.GetParameters(), argumentValues).Skip(1))
		{
			var prefix = GetParameterPrefix(param);
			arguments[$"{prefix}Value"] = value;
			arguments[$"{prefix}Name"] = GetMemberName(expr, targetObject) ?? "";
		}

		Add(
			GetTargetPropertyName(targetPropertyExpression),
			message,
			arguments
		);
	}

	private static string GetValidationMessage(Type declaringType)
	{
		if (declaringType
				.GetMember(
					"DefaultMessage",
					BindingFlags.Static | BindingFlags.Public
				) is not [{ } messageProperty])
		{
			throw new InvalidOperationException("Unable to access the `DefaultMessage` of the validator.");
		}

		return messageProperty switch
		{
			FieldInfo fi => (string)fi.GetValue(null)!,
			PropertyInfo pi => (string)pi.GetValue(null)!,
			_ => throw new UnreachableException(),
		};
	}

	private static object GetTargetObject(Expression? expression) =>
		expression switch
		{
			MemberExpression { Expression: ConstantExpression ce } => ce.Value!,
			MemberExpression me => GetTargetObject(me.Expression),
			BinaryExpression { NodeType: ExpressionType.ArrayIndex } ie => GetTargetObject(ie.Left),
			MethodCallExpression { Method.Name: "get_Item", Arguments.Count: 1 } mce => GetTargetObject(mce.Object),
			_ => throw new NotSupportedException("Unable to determine target validation object."),
		};

	private static string? GetMemberName(Expression? expression, object targetObject) =>
		expression switch
		{
			MemberExpression { Expression: ConstantExpression ce } when ReferenceEquals(ce.Value, targetObject) =>
				"",
			MemberExpression me =>
				PrependMemberParent(GetMemberName(me.Expression, targetObject) ?? "", GetMemberName(me.Member), MemberIndex.Member),
			BinaryExpression { NodeType: ExpressionType.ArrayIndex } ie =>
				PrependMemberParent(GetMemberName(ie.Left, targetObject), $"{ExpressionEvaluator.GetValue(ie.Right)}", MemberIndex.Index),
			MethodCallExpression { Method.Name: "get_Item", Arguments: [{ } arg] } mce =>
				PrependMemberParent(GetMemberName(mce.Object, targetObject), $"{ExpressionEvaluator.GetValue(arg)}", MemberIndex.Index),
			MethodCallExpression { Arguments.Count: 0 } mce =>
				PrependMemberParent(GetMemberName(mce.Object, targetObject) ?? "", mce.Method.Name, MemberIndex.Method),
			UnaryExpression { NodeType: ExpressionType.Convert or ExpressionType.ConvertChecked } ue =>
				GetMemberName(ue.Operand, targetObject),
			_ => null,
		};

	private enum MemberIndex { None = 0, Member, Index, Method }

	private static string? PrependMemberParent(string? parent, string name, MemberIndex memberIndex) =>
		(parent, memberIndex) switch
		{
			(null, _) => null,
			("", MemberIndex.Method) => $"{name}()",
			("", _) => name,
			(_, MemberIndex.Member) => $"{parent}.{name}",
			(_, MemberIndex.Index) => $"{parent}[{name}]",
			(_, MemberIndex.Method) => $"{parent}.{name}()",
			_ => null,
		};

	[GeneratedRegex(@"(?<=[^A-Z])([A-Z])", RegexOptions.None)]
	private static partial Regex FirstCharInWord();

	private static string GetMemberName(MemberInfo member)
	{
		if (member.GetCustomAttribute<DescriptionAttribute>() is { } desc)
			return desc.Description;

		return FirstCharInWord().Replace(member.Name, " $0");
	}

	private static string GetParameterPrefix(ParameterInfo param) =>
		string.Create(provider: null, $"{char.ToUpperInvariant(param.Name![0])}{param.Name[1..]}");

	private static string GetTargetPropertyName(Expression? expression) =>
		expression switch
		{
			MemberExpression { Expression: ConstantExpression } =>
				"",
			MemberExpression me =>
				PrependTargetParent(GetTargetPropertyName(me.Expression), me.Member.Name, MemberIndex.Member),
			BinaryExpression { NodeType: ExpressionType.ArrayIndex } ie =>
				PrependTargetParent(GetTargetPropertyName(ie.Left), $"{ExpressionEvaluator.GetValue(ie.Right)}", MemberIndex.Index),
			MethodCallExpression { Method.Name: "get_Item", Arguments: [{ } arg] } mce =>
				PrependTargetParent(GetTargetPropertyName(mce.Object), $"{ExpressionEvaluator.GetValue(arg)}", MemberIndex.Index),
			_ => throw new NotSupportedException("Unable to determine target validation object."),
		};

	private static string PrependTargetParent(string parent, string name, MemberIndex memberIndex) =>
		(parent, memberIndex) switch
		{
			(null or "", _) => name,
			(_, MemberIndex.Member) => $"{parent}.{name}",
			(_, MemberIndex.Index) => $"{parent}[{name}]",
			_ => name,
		};
}
