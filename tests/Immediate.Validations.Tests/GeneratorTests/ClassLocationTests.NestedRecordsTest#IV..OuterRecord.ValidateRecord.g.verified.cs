//HintName: IV..OuterRecord.ValidateRecord.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591

partial record OuterRecord
{

partial record ValidateRecord
{
	static List<ValidationError> IValidationTarget<ValidateRecord>.Validate(ValidateRecord? target) =>
		Validate(target);

	public static  List<ValidationError> Validate(ValidateRecord? target)
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
