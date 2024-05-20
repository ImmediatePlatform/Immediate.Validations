//HintName: IV..OuterStruct.ValidateStruct.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591

partial struct OuterStruct
{

partial struct ValidateStruct
{
	static List<ValidationError> IValidationTarget<ValidateStruct>.Validate(ValidateStruct target) =>
		Validate(target);

	public static  List<ValidationError> Validate(ValidateStruct target)
	{
		var t = target;
		
		var errors = new List<ValidationError>();



		return errors;
	}



}

}
