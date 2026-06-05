//HintName: IV.Namespace..BaseInterface.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591

namespace Namespace;


partial interface BaseInterface
{
	static global::Immediate.Validations.Shared.ValidationResult global::Immediate.Validations.Shared.IValidationTarget<BaseInterface>.Validate(BaseInterface? target) =>
		Validate(target, []);

	static global::Immediate.Validations.Shared.ValidationResult global::Immediate.Validations.Shared.IValidationTarget<BaseInterface>.Validate(BaseInterface? target, global::Immediate.Validations.Shared.ValidationResult errors) =>
		Validate(target, errors);

	public static new global::Immediate.Validations.Shared.ValidationResult Validate(BaseInterface? target) =>
		Validate(target, []);

	public static new global::Immediate.Validations.Shared.ValidationResult Validate(BaseInterface? target, global::Immediate.Validations.Shared.ValidationResult errors)
	{
		if (target is not { } t)
		{
			return new()
			{
				{ ".self", "`target` must not be `null`." },
			};
		}

		if (!errors.VisitType(typeof(BaseInterface)))
			return errors;
		



		return errors;
	}



}

