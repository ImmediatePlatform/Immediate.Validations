//HintName: IV.Namespace.OuterStruct.ValidateStruct.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591

namespace Namespace;

partial struct OuterStruct
{

partial struct ValidateStruct
{
	static ValidationResult IValidationTarget<ValidateStruct>.Validate(ValidateStruct target) =>
		Validate(target);

	public static  ValidationResult Validate(ValidateStruct target)
	{
		var t = target;
		
		var errors = new ValidationResult();




		return errors;
	}



}

}
