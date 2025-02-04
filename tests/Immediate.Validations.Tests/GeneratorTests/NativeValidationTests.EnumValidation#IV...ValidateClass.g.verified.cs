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
		

		__ValidateTestEnum(errors, t, t.TestEnum);


		return errors;
	}



	private static void __ValidateTestEnum(
		ValidationResult errors, ValidateClass instance, global::TestEnum target
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
					$"TestEnum",
					global::Immediate.Validations.Shared.EnumValueAttribute.DefaultMessage,
					new()
					{
						["PropertyName"] = $"Test Enum",
						["PropertyValue"] = t,
					}
				);
			}
		}
	}

}

