//HintName: Immediate.Validations...ValidateClass.g.cs

using System.Collections.Generic;
using Immediate.Validations.Shared;

#pragma warning disable CS1591


partial class ValidateClass
{
	public static List<ValidationError> Validate(ValidateClass target)
	{
		var errors = new List<ValidationError>();

{

if (
	global::IntGreaterThanZeroAttribute.ValidateProperty(
		target.IntProperty
	) is (true, var message)
)
{
	errors.Add(new()
	{
		PropertyName = "IntProperty", 
		ErrorMessage = null ?? message,
	});
}
}

		return errors;
	}
}

