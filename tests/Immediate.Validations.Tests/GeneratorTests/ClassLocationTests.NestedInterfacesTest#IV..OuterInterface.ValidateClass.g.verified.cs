//HintName: IV..OuterInterface.ValidateClass.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591

partial interface OuterInterface
{

partial interface ValidateClass
{
	static List<ValidationError> IValidationTarget<ValidateClass>.Validate(ValidateClass? target) =>
		Validate(target);

	public static new List<ValidationError> Validate(ValidateClass? target)
	{
		if (target is not { } t)
		{
			return 
			[
				new()
				{
					PropertyName = ".self",
					ErrorMessage = "`target` must not be `null`.",
				},
			];
		}
		
		var errors = new List<ValidationError>();




		return errors;
	}



}

}
