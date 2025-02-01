//HintName: IV.Namespace.OuterClass.BaseClass.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591

namespace Namespace;

partial class OuterClass
{

partial class BaseClass
{
	ValidationResult IValidationTarget.Validate() =>
		Validate(this, []);

	ValidationResult IValidationTarget.Validate(ValidationResult errors) =>
		Validate(this, errors);

	static ValidationResult IValidationTarget<BaseClass>.Validate(BaseClass? target) =>
		Validate(target, []);

	static ValidationResult IValidationTarget<BaseClass>.Validate(BaseClass? target, ValidationResult errors) =>
		Validate(target, errors);

	public static  ValidationResult Validate(BaseClass? target) =>
		Validate(target, []);

	public static  ValidationResult Validate(BaseClass? target, ValidationResult errors)
	{
		if (target is not { } t)
		{
			return new()
			{
				{ ".self", "`target` must not be `null`." },
			};
		}

		if (!errors.VisitType(typeof(BaseClass)))
			return errors;
		



		return errors;
	}



}

}
