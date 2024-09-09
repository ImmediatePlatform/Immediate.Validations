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


		__ValidateDummyValue(errors, t, t.DummyValue);


		return errors;
	}



	private static void __ValidateDummyValue(
		ValidationResult errors, ValidateClass instance, global::Dummy target
	)
	{

		var t = target;



		{
			if (!global::Immediate.Validations.Shared.EnumValueAttribute.ValidateProperty(
					t
				)
			)
			{
				errors.Add(
					$"DummyValue",
					global::Immediate.Validations.Shared.EnumValueAttribute.DefaultMessage,
					new()
					{
						["PropertyName"] = $"Dummy Value",
						["PropertyValue"] = t,
					}
				);
			}
		}
		{
			if (!global::Immediate.Validations.Shared.OneOfAttribute.ValidateProperty(
					t
					, values: [Dummy.Dummy1]
				)
			)
			{
				errors.Add(
					$"DummyValue",
					global::Immediate.Validations.Shared.OneOfAttribute.DefaultMessage,
					new()
					{
						["PropertyName"] = $"Dummy Value",
						["PropertyValue"] = t,
						["ValuesName"] = "",
						["ValuesValue"] = string.Join<global::Dummy>(", ", [Dummy.Dummy1]),
					}
				);
			}
		}
	}

}
