//HintName: IV.Namespace.OuterRecord.ValidateRecord.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591

namespace Namespace;

partial record OuterRecord
{

partial record ValidateRecord
{
	static ValidationResult IValidationTarget<ValidateRecord>.Validate(ValidateRecord? target) =>
		Validate(target, []);

	static ValidationResult IValidationTarget<ValidateRecord>.Validate(ValidateRecord? target, ValidationResult errors) =>
		Validate(target, errors);

	public static  ValidationResult Validate(ValidateRecord? target) =>
		Validate(target, []);

	public static  ValidationResult Validate(ValidateRecord? target, ValidationResult errors)
	{
		if (target is not { } t)
		{
			return new()
			{
				{ ".self", "`target` must not be `null`." },
			};
		}

		if (!errors.VisitType(typeof(ValidateRecord)))
			return errors;
		



		return errors;
	}



}

}
