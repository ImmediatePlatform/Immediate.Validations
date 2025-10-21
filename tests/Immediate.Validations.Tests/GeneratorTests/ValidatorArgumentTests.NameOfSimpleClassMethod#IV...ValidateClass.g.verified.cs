//HintName: IV...ValidateClass.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591


partial class ValidateClass : global::Immediate.Validations.Shared.IValidationTarget
{
	global::Immediate.Validations.Shared.ValidationResult global::Immediate.Validations.Shared.IValidationTarget.Validate() =>
		Validate(this, []);

	global::Immediate.Validations.Shared.ValidationResult global::Immediate.Validations.Shared.IValidationTarget.Validate(global::Immediate.Validations.Shared.ValidationResult errors) =>
		Validate(this, errors);

	static global::Immediate.Validations.Shared.ValidationResult global::Immediate.Validations.Shared.IValidationTarget<ValidateClass>.Validate(ValidateClass? target) =>
		Validate(target, []);

	static global::Immediate.Validations.Shared.ValidationResult global::Immediate.Validations.Shared.IValidationTarget<ValidateClass>.Validate(ValidateClass? target, global::Immediate.Validations.Shared.ValidationResult errors) =>
		Validate(target, errors);

	public static  global::Immediate.Validations.Shared.ValidationResult Validate(ValidateClass? target) =>
		Validate(target, []);

	public static  global::Immediate.Validations.Shared.ValidationResult Validate(ValidateClass? target, global::Immediate.Validations.Shared.ValidationResult errors)
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
		

		__ValidateDummyValue(errors, t, t.DummyValue);


		return errors;
	}



	private static void __ValidateDummyValue(
		global::Immediate.Validations.Shared.ValidationResult errors, ValidateClass instance, string target
	)
	{

		if (target is not { } t)
		{
			errors.Add(
				$"DummyValue",
				global::Immediate.Validations.Shared.NotNullAttribute.DefaultMessage,
				new()
				{
					["PropertyName"] = $"Dummy Value",
					["PropertyValue"] = null,
				}
			);

			return;
		}



		{
			if (!global::Immediate.Validations.Shared.EqualAttribute.ValidateProperty(
					t
					, comparison: global::TestClass.Test()
				)
			)
			{
				errors.Add(
					$"DummyValue",
					global::Immediate.Validations.Shared.EqualAttribute.DefaultMessage,
					new()
					{
						["PropertyName"] = $"Dummy Value",
						["PropertyValue"] = t,
						["ComparisonName"] = "Test Class Test",
						["ComparisonValue"] = global::TestClass.Test(),
					}
				);
			}
		}
	}

}

