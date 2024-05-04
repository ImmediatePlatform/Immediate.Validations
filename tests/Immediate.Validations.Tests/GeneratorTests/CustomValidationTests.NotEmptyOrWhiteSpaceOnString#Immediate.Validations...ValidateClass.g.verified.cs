//HintName: Immediate.Validations...ValidateClass.g.cs

using System.Collections.Generic;
using Immediate.Validations.Shared;

#pragma warning disable CS1591


partial class ValidateClass : IValidationTarget<ValidateClass>
{
	public static List<ValidationError> Validate(ValidateClass target)
	{
		var errors = new List<ValidationError>();

{

if (
	global::Immediate.Validations.Shared.NotNullAttribute.ValidateProperty(
		target.StringProperty
	) is (true, var message)
)
{
	errors.Add(new()
	{
		PropertyName = "StringProperty", 
		ErrorMessage = null ?? message,
	});
}
}
{

if (
	global::Immediate.Validations.Shared.NotEmptyOrWhiteSpaceAttribute.ValidateProperty(
		target.StringProperty
	) is (true, var message)
)
{
	errors.Add(new()
	{
		PropertyName = "StringProperty", 
		ErrorMessage = null ?? message,
	});
}
}

		return errors;
	}
}

