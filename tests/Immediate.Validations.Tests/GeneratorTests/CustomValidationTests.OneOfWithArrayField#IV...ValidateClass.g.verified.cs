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
			if (!global::Immediate.Validations.Shared.OneOfAttribute.ValidateProperty(
					t
					, values: s_validStrings
				)
			)
			{
				errors.Add(
					$"StringProperty",
					global::Immediate.Validations.Shared.OneOfAttribute.DefaultMessage,
					new()
					{
						["PropertyName"] = $"String Property",
						["PropertyValue"] = t,
						["ValuesName"] = "s_valid Strings",
						["ValuesValue"] = string.Join<string>(", ", s_validStrings),
					}
				);
			}
		}
	}

}
