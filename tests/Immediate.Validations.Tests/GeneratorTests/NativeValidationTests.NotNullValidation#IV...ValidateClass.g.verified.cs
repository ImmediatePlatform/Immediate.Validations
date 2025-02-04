//HintName: IV...ValidateClass.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591


partial class ValidateClass : IValidationTarget
{
	ValidationResult IValidationTarget.Validate() =>
		Validate(this, []);

	ValidationResult IValidationTarget.Validate(ValidationResult errors) =>
		Validate(this, errors);

	static ValidationResult IValidationTarget<ValidateClass>.Validate(ValidateClass? target) =>
		Validate(target, []);

	static ValidationResult IValidationTarget<ValidateClass>.Validate(ValidateClass? target, ValidationResult errors) =>
		Validate(target, errors);

	public static  ValidationResult Validate(ValidateClass? target) =>
		Validate(target, []);

	public static  ValidationResult Validate(ValidateClass? target, ValidationResult errors)
	{
		if (target is not { } t)
		{
			return new()
			{
				{ ".self", "`target` must not be `null`." },
			};
		}

		if (!errors.VisitType(typeof(ValidateClass)))
			return errors;
		

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
				global::Immediate.Validations.Shared.NotNullAttribute.DefaultMessage,
				new()
				{
					["PropertyName"] = $"String Property",
					["PropertyValue"] = null,
				}
			);

			return;
		}



	}

}

