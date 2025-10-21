//HintName: IV...Target.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591


partial record Target : global::Immediate.Validations.Shared.IValidationTarget
{
	global::Immediate.Validations.Shared.ValidationResult global::Immediate.Validations.Shared.IValidationTarget.Validate() =>
		Validate(this, []);

	global::Immediate.Validations.Shared.ValidationResult global::Immediate.Validations.Shared.IValidationTarget.Validate(global::Immediate.Validations.Shared.ValidationResult errors) =>
		Validate(this, errors);

	static global::Immediate.Validations.Shared.ValidationResult global::Immediate.Validations.Shared.IValidationTarget<Target>.Validate(Target? target) =>
		Validate(target, []);

	static global::Immediate.Validations.Shared.ValidationResult global::Immediate.Validations.Shared.IValidationTarget<Target>.Validate(Target? target, global::Immediate.Validations.Shared.ValidationResult errors) =>
		Validate(target, errors);

	public static  global::Immediate.Validations.Shared.ValidationResult Validate(Target? target) =>
		Validate(target, []);

	public static  global::Immediate.Validations.Shared.ValidationResult Validate(Target? target, global::Immediate.Validations.Shared.ValidationResult errors)
	{
		if (target is not { } t)
		{
			return new()
			{
				{ ".self", "`target` must not be `null`." },
			};
		}

		if (!errors.VisitType(typeof(Target)))
			return errors;
		

		__ValidateId(errors, t, t.Id);
		__ValidateFirstValue(errors, t, t.FirstValue);


		return errors;
	}



	private static void __ValidateId(
		global::Immediate.Validations.Shared.ValidationResult errors, Target instance, string target
	)
	{

		if (target is not { } t)
		{
			errors.Add(
				$"Id",
				global::Immediate.Validations.Shared.NotNullAttribute.DefaultMessage,
				new()
				{
					["PropertyName"] = $"Id",
					["PropertyValue"] = null,
				}
			);

			return;
		}



		{
			if (!global::DummyAttribute.ValidateProperty(
					t
					, first: instance.FirstValue
					, second: "Hello World"
					, third: ["Test1", instance.FirstValue, "Test3"]
					, fourth: "Abcd"
					, fifth: "The end?"
				)
			)
			{
				errors.Add(
					$"Id",
					"What's going on?",
					new()
					{
						["PropertyName"] = $"Id",
						["PropertyValue"] = t,
						["FirstName"] = "First Value",
						["FirstValue"] = instance.FirstValue,
						["SecondName"] = "",
						["SecondValue"] = "Hello World",
						["ThirdName"] = "",
						["ThirdValue"] = string.Join<string>(", ", ["Test1", instance.FirstValue, "Test3"]),
						["FourthName"] = "",
						["FourthValue"] = "Abcd",
						["FifthName"] = "",
						["FifthValue"] = "The end?",
					}
				);
			}
		}
	}

	private static void __ValidateFirstValue(
		global::Immediate.Validations.Shared.ValidationResult errors, Target instance, string target
	)
	{

		if (target is not { } t)
		{
			errors.Add(
				$"FirstValue",
				global::Immediate.Validations.Shared.NotNullAttribute.DefaultMessage,
				new()
				{
					["PropertyName"] = $"First Value",
					["PropertyValue"] = null,
				}
			);

			return;
		}



	}

}

