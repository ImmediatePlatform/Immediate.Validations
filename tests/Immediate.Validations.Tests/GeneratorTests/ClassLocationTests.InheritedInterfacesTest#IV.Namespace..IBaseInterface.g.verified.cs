//HintName: IV.Namespace..IBaseInterface.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591

namespace Namespace;


partial interface IBaseInterface
{
	static ValidationResult IValidationTarget<IBaseInterface>.Validate(IBaseInterface? target) =>
		Validate(target, []);

	static ValidationResult IValidationTarget<IBaseInterface>.Validate(IBaseInterface? target, ValidationResult errors) =>
		Validate(target, errors);

	public static new ValidationResult Validate(IBaseInterface? target) =>
		Validate(target, []);

	public static new ValidationResult Validate(IBaseInterface? target, ValidationResult errors)
	{
		if (target is not { } t)
		{
			return new()
			{
				{ ".self", "`target` must not be `null`." },
			};
		}

		if (!errors.VisitType(typeof(IBaseInterface)))
			return errors;
		



		return errors;
	}



}

