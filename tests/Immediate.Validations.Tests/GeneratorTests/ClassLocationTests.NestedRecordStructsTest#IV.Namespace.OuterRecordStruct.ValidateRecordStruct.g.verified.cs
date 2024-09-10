//HintName: IV.Namespace.OuterRecordStruct.ValidateRecordStruct.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591

namespace Namespace;

partial record struct OuterRecordStruct
{

partial record struct ValidateRecordStruct
{
	static ValidationResult IValidationTarget<ValidateRecordStruct>.Validate(ValidateRecordStruct target) =>
		Validate(target, []);

	static ValidationResult IValidationTarget<ValidateRecordStruct>.Validate(ValidateRecordStruct target, ValidationResult errors) =>
		Validate(target, errors);

	public static  ValidationResult Validate(ValidateRecordStruct target) =>
		Validate(target, []);

	public static  ValidationResult Validate(ValidateRecordStruct target, ValidationResult errors)
	{
		var t = target;

		if (!errors.VisitType(typeof(ValidateRecordStruct)))
			return errors;
		



		return errors;
	}



}

}
