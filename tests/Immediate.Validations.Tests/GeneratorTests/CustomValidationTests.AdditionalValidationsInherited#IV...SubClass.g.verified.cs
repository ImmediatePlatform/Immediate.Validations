//HintName: IV...SubClass.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591


partial class SubClass
{
	static ValidationResult IValidationTarget<SubClass>.Validate(SubClass? target) =>
		Validate(target, []);

	static ValidationResult IValidationTarget<SubClass>.Validate(SubClass? target, ValidationResult errors) =>
		Validate(target, errors);

	public static  ValidationResult Validate(SubClass? target) =>
		Validate(target, []);

	public static  ValidationResult Validate(SubClass? target, ValidationResult errors)
	{
		if (target is not { } t)
		{
			return new()
			{
				{ ".self", "`target` must not be `null`." },
			};
		}

		if (!errors.VisitType(typeof(SubClass)))
			return errors;
		
		global::BaseClass.Validate(t, errors);



		return errors;
	}



}

