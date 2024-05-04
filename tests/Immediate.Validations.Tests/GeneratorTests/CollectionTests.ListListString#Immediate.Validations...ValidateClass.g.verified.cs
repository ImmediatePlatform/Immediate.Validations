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
	var counter0 = 0;
	foreach (var item0 in target.StringProperty ?? [])
	{
{

if (
	global::Immediate.Validations.Shared.NotNullAttribute.ValidateProperty(
		item0
	) is (true, var message)
)
{
	errors.Add(new()
	{
		PropertyName = $"StringProperty[{counter0}]", 
		ErrorMessage = null ?? message,
	});
}
}
{
	var counter1 = 0;
	foreach (var item1 in item0 ?? [])
	{
{

if (
	global::Immediate.Validations.Shared.NotNullAttribute.ValidateProperty(
		item1
	) is (true, var message)
)
{
	errors.Add(new()
	{
		PropertyName = $"StringProperty[{counter0}][{counter1}]", 
		ErrorMessage = null ?? message,
	});
}
}
		counter1++;
	}
}
		counter0++;
	}
}

		return errors;
	}
}

