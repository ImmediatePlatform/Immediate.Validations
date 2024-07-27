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


		__ValidateStringProperty(errors, t, t.StringProperty);


		return errors;
	}



	private static void __ValidateStringProperty00(
		ValidationResult errors, ValidateClass instance, string target, int counter0, int counter1
	)
	{

		if (target is not { } t)
		{
			errors.Add(
				$"StringProperty[{counter0}][{counter1}]",
				$"'String Property[{counter0}][{counter1}]' must not be null."
			);

			return;
		}



		{
			if (!global::Immediate.Validations.Shared.NotEmptyAttribute.ValidateProperty(
					t
				)
			)
			{
				errors.Add(
					$"StringProperty[{counter0}][{counter1}]",
					global::Immediate.Validations.Shared.NotEmptyAttribute.DefaultMessage,
					new()
					{
						["PropertyName"] = $"String Property[{counter0}][{counter1}]",
						["PropertyValue"] = t,
					}
				);
			}
		}
	}

	private static void __ValidateStringProperty0(
		ValidationResult errors, ValidateClass instance, global::System.Collections.Generic.List<string> target, int counter0
	)
	{

		if (target is not { } t)
		{
			errors.Add(
				$"StringProperty[{counter0}]",
				$"'String Property[{counter0}]' must not be null."
			);

			return;
		}


		var counter1 = 0;
		foreach (var item1 in t)
		{
			__ValidateStringProperty00(
				errors, instance, item1, counter0, counter1
			);
			counter1++;
		}

		{
			if (!global::Immediate.Validations.Shared.NotEmptyAttribute.ValidateProperty(
					t
				)
			)
			{
				errors.Add(
					$"StringProperty[{counter0}]",
					global::Immediate.Validations.Shared.NotEmptyAttribute.DefaultMessage,
					new()
					{
						["PropertyName"] = $"String Property[{counter0}]",
						["PropertyValue"] = t,
					}
				);
			}
		}
	}

	private static void __ValidateStringProperty(
		ValidationResult errors, ValidateClass instance, global::System.Collections.Generic.List<global::System.Collections.Generic.List<string>> target
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


		var counter0 = 0;
		foreach (var item0 in t)
		{
			__ValidateStringProperty0(
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
					$"StringProperty",
					global::Immediate.Validations.Shared.NotEmptyAttribute.DefaultMessage,
					new()
					{
						["PropertyName"] = $"String Property",
						["PropertyValue"] = t,
					}
				);
			}
		}
	}

}

