//HintName: IV..OuterRecord.ValidateRecord.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591

partial record OuterRecord
{

partial record ValidateRecord
{
	static ValidationResult IValidationTarget<ValidateRecord>.Validate(ValidateRecord? target) =>
		Validate(target);

	public static  ValidationResult Validate(ValidateRecord? target)
	{
		if (target is not { } t)
		{
			return new()
			{
				{ ".self", "`target` must not be `null`." },
			};
		}
		
		var errors = new ValidationResult();




		return errors;
	}



}

}
