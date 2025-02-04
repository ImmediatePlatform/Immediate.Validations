//HintName: IV...IInterface1.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591


partial interface IInterface1
{
	static ValidationResult IValidationTarget<IInterface1>.Validate(IInterface1? target) =>
		Validate(target, []);

	static ValidationResult IValidationTarget<IInterface1>.Validate(IInterface1? target, ValidationResult errors) =>
		Validate(target, errors);

	public static new ValidationResult Validate(IInterface1? target) =>
		Validate(target, []);

	public static new ValidationResult Validate(IInterface1? target, ValidationResult errors)
	{
		if (target is not { } t)
		{
			return new()
			{
				{ ".self", "`target` must not be `null`." },
			};
		}

		if (!errors.VisitType(typeof(IInterface1)))
			return errors;
		



		return errors;
	}



}

