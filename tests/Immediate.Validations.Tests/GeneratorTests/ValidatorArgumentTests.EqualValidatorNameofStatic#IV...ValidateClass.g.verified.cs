//HintName: IV...ValidateClass.g.cs
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


		__ValidateIntProperty(errors, t, t.IntProperty);


		return errors;
	}



	private static void __ValidateIntProperty(
		ValidationResult errors, ValidateClass instance, int target
	)
	{

		var t = target;



		{
			if (!global::Immediate.Validations.Shared.EqualAttribute.ValidateProperty(
					t
					, comparison: KeyValue
				)
			)
			{
				errors.Add(
					$"IntProperty",
					ValidationConfiguration.Localizer != null
					? ValidationConfiguration.Localizer["global::Immediate.Validations.Shared.EqualAttribute"]
					: global::Immediate.Validations.Shared.EqualAttribute.DefaultMessage,
					new()
					{
						["PropertyName"] = $"Int Property",
						["PropertyValue"] = t,
						["ComparisonName"] = "Key Value",
						["ComparisonValue"] = KeyValue,
					}
				);
			}
		}
	}

}

