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
	ValidationResult IValidationTarget.Validate() =>
		Validate(this, []);

	ValidationResult IValidationTarget.Validate(ValidationResult errors) =>
		Validate(this, errors);

	static ValidationResult IValidationTarget<ValidateStruct>.Validate(ValidateStruct target) =>
		Validate(target, []);

	static ValidationResult IValidationTarget<ValidateStruct>.Validate(ValidateStruct target, ValidationResult errors) =>
		Validate(target, errors);

	public static  ValidationResult Validate(ValidateStruct target) =>
		Validate(target, []);

	public static  ValidationResult Validate(ValidateStruct target, ValidationResult errors)
	{
		var t = target;

		if (!errors.VisitType(typeof(ValidateStruct)))
			return errors;
		



		return errors;
	}



}

}
