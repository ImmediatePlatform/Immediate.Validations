//HintName: IV...Implementation.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591


partial class Implementation : IValidationTarget
{
	ValidationResult IValidationTarget.Validate() =>
		Validate(this, []);

	ValidationResult IValidationTarget.Validate(ValidationResult errors) =>
		Validate(this, errors);

	static ValidationResult IValidationTarget<Implementation>.Validate(Implementation? target) =>
		Validate(target, []);

	static ValidationResult IValidationTarget<Implementation>.Validate(Implementation? target, ValidationResult errors) =>
		Validate(target, errors);

	public static  ValidationResult Validate(Implementation? target) =>
		Validate(target, []);

	public static  ValidationResult Validate(Implementation? target, ValidationResult errors)
	{
		if (target is not { } t)
		{
			return new()
			{
				{ ".self", "`target` must not be `null`." },
			};
		}

		if (!errors.VisitType(typeof(Implementation)))
			return errors;
		
		global::IInterface1.Validate(t, errors);


		return errors;
	}



}

