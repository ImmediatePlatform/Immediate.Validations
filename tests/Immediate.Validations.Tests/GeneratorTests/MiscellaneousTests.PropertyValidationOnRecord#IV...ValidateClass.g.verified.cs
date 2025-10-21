//HintName: IV...ValidateClass.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591


partial record ValidateClass : global::Immediate.Validations.Shared.IValidationTarget
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
		

		__ValidateTesting1(errors, t, t.Testing1);
		__Validatedata(errors, t, t.data);


		return errors;
	}



	private static void __ValidateTesting1(
		global::Immediate.Validations.Shared.ValidationResult errors, ValidateClass instance, string target
	)
	{

		if (target is not { } t)
		{
			errors.Add(
				$"Testing1",
				global::Immediate.Validations.Shared.NotNullAttribute.DefaultMessage,
				new()
				{
					["PropertyName"] = $"Testing1",
					["PropertyValue"] = null,
				}
			);

			return;
		}



		{
			if (!global::Immediate.Validations.Shared.MaxLengthAttribute.ValidateProperty(
					t
					, maxLength: 3
				)
			)
			{
				errors.Add(
					$"Testing1",
					global::Immediate.Validations.Shared.MaxLengthAttribute.DefaultMessage,
					new()
					{
						["PropertyName"] = $"Testing1",
						["PropertyValue"] = t,
						["MaxLengthName"] = "",
						["MaxLengthValue"] = 3,
					}
				);
			}
		}
	}

	private static void __Validatedata0(
		global::Immediate.Validations.Shared.ValidationResult errors, ValidateClass instance, int target, int counter0
	)
	{

		var t = target;



		{
			if (!global::Immediate.Validations.Shared.GreaterThanAttribute.ValidateProperty(
					t
					, comparison: 0
				)
			)
			{
				errors.Add(
					$"data[{counter0}]",
					global::Immediate.Validations.Shared.GreaterThanAttribute.DefaultMessage,
					new()
					{
						["PropertyName"] = $"data[{counter0}]",
						["PropertyValue"] = t,
						["ComparisonName"] = "",
						["ComparisonValue"] = 0,
					}
				);
			}
		}
	}

	private static void __Validatedata(
		global::Immediate.Validations.Shared.ValidationResult errors, ValidateClass instance, global::System.Collections.Generic.List<int> target
	)
	{

		if (target is not { } t)
		{
			errors.Add(
				$"data",
				global::Immediate.Validations.Shared.NotNullAttribute.DefaultMessage,
				new()
				{
					["PropertyName"] = $"data",
					["PropertyValue"] = null,
				}
			);

			return;
		}


		var counter0 = 0;
		foreach (var item0 in t)
		{
			__Validatedata0(
				errors, instance, item0, counter0
			);
			counter0++;
		}

		{
			if (!global::Immediate.Validations.Shared.NotEmptyAttribute.ValidateProperty(
					t
				)
			)
			{
				errors.Add(
					$"data",
					global::Immediate.Validations.Shared.NotEmptyAttribute.DefaultMessage,
					new()
					{
						["PropertyName"] = $"data",
						["PropertyValue"] = t,
					}
				);
			}
		}
	}

}

