//HintName: IV.Namespace.OuterStruct.ValidateStruct.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591

namespace Namespace;

partial struct OuterStruct
{

partial struct ValidateStruct : global::Immediate.Validations.Shared.IValidationTarget
{
	global::Immediate.Validations.Shared.ValidationResult global::Immediate.Validations.Shared.IValidationTarget.Validate() =>
		Validate(this, []);

	global::Immediate.Validations.Shared.ValidationResult global::Immediate.Validations.Shared.IValidationTarget.Validate(global::Immediate.Validations.Shared.ValidationResult errors) =>
		Validate(this, errors);

	static global::Immediate.Validations.Shared.ValidationResult global::Immediate.Validations.Shared.IValidationTarget<ValidateStruct>.Validate(ValidateStruct target) =>
		Validate(target, []);

	static global::Immediate.Validations.Shared.ValidationResult global::Immediate.Validations.Shared.IValidationTarget<ValidateStruct>.Validate(ValidateStruct target, global::Immediate.Validations.Shared.ValidationResult errors) =>
		Validate(target, errors);

	public static  global::Immediate.Validations.Shared.ValidationResult Validate(ValidateStruct target) =>
		Validate(target, []);

	public static  global::Immediate.Validations.Shared.ValidationResult Validate(ValidateStruct target, global::Immediate.Validations.Shared.ValidationResult errors)
	{
		var t = target;

		if (!errors.VisitType(typeof(ValidateStruct)))
			return errors;
		



		return errors;
	}



}

}
