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


		__ValidateStringProperty(errors, t, t.StringProperty);


		return errors;
	}



	private static void __ValidateStringProperty(
		ValidationResult errors, ValidateClass instance, string target
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



		{
			if (!global::Immediate.Validations.Shared.NotEqualAttribute.ValidateProperty(
					t
					, comparison: _argumentValue
				)
			)
			{
				errors.Add(
					$"StringProperty",
					ValidationConfiguration.Localizer != null
					? ValidationConfiguration.Localizer["global::Immediate.Validations.Shared.NotEqualAttribute"]
					: global::Immediate.Validations.Shared.NotEqualAttribute.DefaultMessage,
					new()
					{
						["PropertyName"] = $"String Property",
						["PropertyValue"] = t,
						["ComparisonName"] = "_argument Value",
						["ComparisonValue"] = _argumentValue,
					}
				);
			}
		}
	}

}

