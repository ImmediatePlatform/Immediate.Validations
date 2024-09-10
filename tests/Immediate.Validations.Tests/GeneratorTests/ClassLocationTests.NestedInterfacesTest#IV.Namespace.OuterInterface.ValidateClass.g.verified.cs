//HintName: IV.Namespace.OuterInterface.ValidateClass.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591

namespace Namespace;

partial interface OuterInterface
{

partial interface ValidateClass
{
	static ValidationResult IValidationTarget<ValidateClass>.Validate(ValidateClass? target) =>
		Validate(target, []);

	static ValidationResult IValidationTarget<ValidateClass>.Validate(ValidateClass? target, ValidationResult errors) =>
		Validate(target, errors);

	public static new ValidationResult Validate(ValidateClass? target) =>
		Validate(target, []);

	public static new ValidationResult Validate(ValidateClass? target, ValidationResult errors)
	{
		if (target is not { } t)
		{
			return new()
			{
				{ ".self", "`target` must not be `null`." },
			};
		}

		if (!errors.VisitType(typeof(ValidateClass)))
			return errors;
		



		return errors;
	}



}

}
