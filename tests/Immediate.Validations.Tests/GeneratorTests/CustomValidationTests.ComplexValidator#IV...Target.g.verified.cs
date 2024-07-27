﻿//HintName: IV...Target.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591


partial record Target
{
	static ValidationResult IValidationTarget<Target>.Validate(Target? target) =>
		Validate(target);

	public static  ValidationResult Validate(Target? target)
	{
		if (target is not { } t)
		{
			return new()
			{
				{ ".self", "`target` must not be `null`." },
			};
		}
		
		var errors = new ValidationResult();


		__ValidateId(errors, t, t.Id);
		__ValidateFirstValue(errors, t, t.FirstValue);


		return errors;
	}



	private static void __ValidateId(
		ValidationResult errors, Target instance, string target
	)
	{

		if (target is not { } t)
		{
			errors.Add(
				$"Id",
				$"'Id' must not be null."
			);

			return;
		}



		{
			if (!global::DummyAttribute.ValidateProperty(
					t
					, first: instance.FirstValue
					, second: "Hello World"
					, third: "Value"
				)
			)
			{
				errors.Add(
					$"Id",
					ValidationConfiguration.Localizer != null
					? ValidationConfiguration.Localizer["global::DummyAttribute"]
					: "What's going on?",
					new()
					{
						["PropertyName"] = $"Id",
						["PropertyValue"] = t,
						["FirstName"] = "First Value",
						["FirstValue"] = instance.FirstValue,
						["SecondName"] = "",
						["SecondValue"] = "Hello World",
						["ThirdName"] = "",
						["ThirdValue"] = "Value",
					}
				);
			}
		}
	}

	private static void __ValidateFirstValue(
		ValidationResult errors, Target instance, string target
	)
	{

		if (target is not { } t)
		{
			errors.Add(
				$"FirstValue",
				$"'First Value' must not be null."
			);

			return;
		}



	}

}

