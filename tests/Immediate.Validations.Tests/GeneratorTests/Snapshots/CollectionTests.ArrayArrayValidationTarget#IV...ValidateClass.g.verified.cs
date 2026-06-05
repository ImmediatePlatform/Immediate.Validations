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
		

		__ValidateValidationTargets(errors, t, t.ValidationTargets);


		return errors;
	}



	private static void __ValidateValidationTargets00(
		global::Immediate.Validations.Shared.ValidationResult errors, ValidateClass instance, global::ValidationTarget target, int counter0, int counter1
	)
	{

		if (target is not { } t)
		{
			errors.Add(
				$"ValidationTargets[{counter0}][{counter1}]",
				global::Immediate.Validations.Shared.NotNullAttribute.DefaultMessage,
				new()
				{
					["PropertyName"] = $"Validation Targets[{counter0}][{counter1}]",
					["PropertyValue"] = null,
				}
			);

			return;
		}

		foreach (var error in global::ValidationTarget.Validate(t))
		{
			errors.Add(error with 
			{
				PropertyName = string.IsNullOrWhiteSpace(error.PropertyName)
					? $"ValidationTargets[{counter0}][{counter1}]"
					: $"ValidationTargets[{counter0}][{counter1}].{error.PropertyName}",
			});
		}


	}

	private static void __ValidateValidationTargets0(
		global::Immediate.Validations.Shared.ValidationResult errors, ValidateClass instance, global::ValidationTarget[] target, int counter0
	)
	{

		if (target is not { } t)
		{
			errors.Add(
				$"ValidationTargets[{counter0}]",
				global::Immediate.Validations.Shared.NotNullAttribute.DefaultMessage,
				new()
				{
					["PropertyName"] = $"Validation Targets[{counter0}]",
					["PropertyValue"] = null,
				}
			);

			return;
		}


		var counter1 = 0;
		foreach (var item1 in t)
		{
			__ValidateValidationTargets00(
				errors, instance, item1, counter0, counter1
			);
			counter1++;
		}

	}

	private static void __ValidateValidationTargets(
		global::Immediate.Validations.Shared.ValidationResult errors, ValidateClass instance, global::ValidationTarget[][] target
	)
	{

		if (target is not { } t)
		{
			errors.Add(
				$"ValidationTargets",
				global::Immediate.Validations.Shared.NotNullAttribute.DefaultMessage,
				new()
				{
					["PropertyName"] = $"Validation Targets",
					["PropertyValue"] = null,
				}
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

