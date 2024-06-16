﻿//HintName: IV...ValidateClass.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591


partial class ValidateClass
{
	static ValidationResult IValidationTarget<ValidateClass>.Validate(ValidateClass? target) =>
		Validate(target);

	public static  ValidationResult Validate(ValidateClass? target)
	{
		if (target is not { } t)
		{
			return new()
			{
				{ ".self", "`target` must not be `null`." },
			};
		}
		
		var errors = new ValidationResult();


		__ValidateTesting(errors, t, t.Testing);


		return errors;
	}



	private static void __ValidateTesting(
		ValidationResult errors, ValidateClass instance, string target
	)
	{

		if (target is not { } t)
		{
			errors.Add(
				$"Testing",
				$"'Testing' must not be null."
			);

			return;
		}



	}

}

