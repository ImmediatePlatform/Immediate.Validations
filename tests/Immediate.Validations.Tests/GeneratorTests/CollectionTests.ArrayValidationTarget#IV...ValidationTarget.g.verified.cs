﻿//HintName: IV...ValidationTarget.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591


partial class ValidationTarget
{
	static ValidationResult IValidationTarget<ValidationTarget>.Validate(ValidationTarget? target) =>
		Validate(target);

	public static  ValidationResult Validate(ValidationTarget? target)
	{
		if (target is not { } t)
		{
			return new()
			{
				{ ".self", "`target` must not be `null`." },
			};
		}
		
		var errors = new ValidationResult();


		__ValidateStringProperty(errors, t, t.StringProperty);


		return errors;
	}



	private static void __ValidateStringProperty(
		ValidationResult errors, ValidationTarget instance, string target
	)
	{

		if (target is not { } t)
		{
			errors.Add(
				$"StringProperty",
				$"'String Property' must not be null."
			);

			return;
		}



	}

}

