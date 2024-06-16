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


		__ValidateValidationTargets(errors, t, t.ValidationTargets);


		return errors;
	}



	private static void __ValidateValidationTargets0(
		ValidationResult errors, ValidateClass instance, global::ValidationTarget target, int counter0
	)
	{

		if (target is not { } t)
		{
			errors.Add(
				$"ValidationTargets[{counter0}]",
				$"'Validation Targets[{counter0}]' must not be null."
			);

			return;
		}

		foreach (var error in global::ValidationTarget.Validate(t))
		{
			errors.Add(error with 
			{
				PropertyName = string.IsNullOrWhiteSpace(error.PropertyName)
					? $"ValidationTargets[{counter0}]"
					: $"ValidationTargets[{counter0}].{error.PropertyName}",
			});
		}


	}

	private static void __ValidateValidationTargets(
		ValidationResult errors, ValidateClass instance, global::ValidationTarget[] target
	)
	{

		if (target is not { } t)
		{
			errors.Add(
				$"ValidationTargets",
				$"'Validation Targets' must not be null."
			);

			return;
		}


		var counter0 = 0;
		foreach (var item0 in t)
		{
			__ValidateValidationTargets0(
				errors, instance, item0, counter0
			);
			counter0++;
		}

	}

}

