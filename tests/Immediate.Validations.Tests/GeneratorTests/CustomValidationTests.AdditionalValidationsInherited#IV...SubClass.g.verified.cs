//HintName: IV...SubClass.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591


partial class SubClass
{
	static ValidationResult IValidationTarget<SubClass>.Validate(SubClass? target) =>
		Validate(target);

	public static  ValidationResult Validate(SubClass? target)
	{
		if (target is not { } t)
		{
			return new()
			{
				{ ".self", "`target` must not be `null`." },
			};
		}
		
		var errors = new ValidationResult();

		errors.AddRange(global::BaseClass.Validate(t));



		return errors;
	}



}

