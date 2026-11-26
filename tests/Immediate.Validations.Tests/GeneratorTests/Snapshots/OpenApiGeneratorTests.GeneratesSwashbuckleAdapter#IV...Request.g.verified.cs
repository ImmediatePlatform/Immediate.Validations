//HintName: IV...Request.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591


[global::Immediate.Validations.Shared.OpenApiConstraintAttribute("Name", global::Immediate.Validations.Shared.OpenApiConstraintKind.NotNull, null, 0)]
[global::Immediate.Validations.Shared.OpenApiConstraintAttribute("Name", global::Immediate.Validations.Shared.OpenApiConstraintKind.MinLength, "3", 0)]
[global::Immediate.Validations.Shared.OpenApiConstraintAttribute("Name", global::Immediate.Validations.Shared.OpenApiConstraintKind.MaxLength, "12", 0)]
[global::Immediate.Validations.Shared.OpenApiConstraintAttribute("Id", global::Immediate.Validations.Shared.OpenApiConstraintKind.ExclusiveMinimum, "2", 0)]
[global::Immediate.Validations.Shared.OpenApiConstraintAttribute("Tags", global::Immediate.Validations.Shared.OpenApiConstraintKind.NotNull, null, 0)]
[global::Immediate.Validations.Shared.OpenApiConstraintAttribute("Tags", global::Immediate.Validations.Shared.OpenApiConstraintKind.NotNull, null, 1)]
[global::Immediate.Validations.Shared.OpenApiConstraintAttribute("Tags", global::Immediate.Validations.Shared.OpenApiConstraintKind.MaxLength, "8", 2)]
partial record Request : global::Immediate.Validations.Shared.IValidationTarget
{
	global::Immediate.Validations.Shared.ValidationResult global::Immediate.Validations.Shared.IValidationTarget.Validate() =>
		Validate(this, []);

	global::Immediate.Validations.Shared.ValidationResult global::Immediate.Validations.Shared.IValidationTarget.Validate(global::Immediate.Validations.Shared.ValidationResult errors) =>
		Validate(this, errors);

	static global::Immediate.Validations.Shared.ValidationResult global::Immediate.Validations.Shared.IValidationTarget<Request>.Validate(Request? target) =>
		Validate(target, []);

	static global::Immediate.Validations.Shared.ValidationResult global::Immediate.Validations.Shared.IValidationTarget<Request>.Validate(Request? target, global::Immediate.Validations.Shared.ValidationResult errors) =>
		Validate(target, errors);

	public static  global::Immediate.Validations.Shared.ValidationResult Validate(Request? target) =>
		Validate(target, []);

	public static  global::Immediate.Validations.Shared.ValidationResult Validate(Request? target, global::Immediate.Validations.Shared.ValidationResult errors)
	{
		if (target is not { } t)
		{
			return new()
			{
				{ ".self", "`target` must not be `null`." },
			};
		}

		if (!errors.VisitType(typeof(Request)))
			return errors;
		

		__ValidateName(errors, t, t.Name);
		__ValidateId(errors, t, t.Id);
		__ValidateLimit(errors, t, t.Limit);
		__ValidateTags(errors, t, t.Tags);


		return errors;
	}



	private static void __ValidateName(
		global::Immediate.Validations.Shared.ValidationResult errors, Request instance, string? target
	)
	{

		if (target is not { } t)
		{
			errors.Add(
				$"Name",
				global::Immediate.Validations.Shared.NotNullAttribute.DefaultMessage,
				new()
				{
					["PropertyName"] = $"Name",
					["PropertyValue"] = null,
				}
			);

			return;
		}



		{
			if (!global::Immediate.Validations.Shared.LengthAttribute.ValidateProperty(
					t
					, minLength: 5
					, maxLength: 12
				)
			)
			{
				errors.Add(
					$"Name",
					global::Immediate.Validations.Shared.LengthAttribute.DefaultMessage,
					new()
					{
						["PropertyName"] = $"Name",
						["PropertyValue"] = t,
						["MinLengthName"] = "",
						["MinLengthValue"] = 5,
						["MaxLengthName"] = "",
						["MaxLengthValue"] = 12,
					}
				);
			}
		}
	}

	private static void __ValidateId(
		global::Immediate.Validations.Shared.ValidationResult errors, Request instance, int target
	)
	{

		var t = target;



		{
			if (!global::Immediate.Validations.Shared.GreaterThanAttribute.ValidateProperty(
					t
					, comparison: 2
				)
			)
			{
				errors.Add(
					$"Id",
					global::Immediate.Validations.Shared.GreaterThanAttribute.DefaultMessage,
					new()
					{
						["PropertyName"] = $"Id",
						["PropertyValue"] = t,
						["ComparisonName"] = "",
						["ComparisonValue"] = 2,
					}
				);
			}
		}
	}

	private static void __ValidateLimit(
		global::Immediate.Validations.Shared.ValidationResult errors, Request instance, int target
	)
	{

		var t = target;



		{
			if (!global::Immediate.Validations.Shared.LessThanAttribute.ValidateProperty(
					t
					, comparison: instance.Id
				)
			)
			{
				errors.Add(
					$"Limit",
					global::Immediate.Validations.Shared.LessThanAttribute.DefaultMessage,
					new()
					{
						["PropertyName"] = $"Limit",
						["PropertyValue"] = t,
						["ComparisonName"] = "Id",
						["ComparisonValue"] = instance.Id,
					}
				);
			}
		}
	}

	private static void __ValidateTags00(
		global::Immediate.Validations.Shared.ValidationResult errors, Request instance, string? target, int counter0, int counter1
	)
	{

		if (target is not { } t)
		{

			return;
		}



		{
			if (!global::Immediate.Validations.Shared.MaxLengthAttribute.ValidateProperty(
					t
					, maxLength: 8
				)
			)
			{
				errors.Add(
					$"Tags[{counter0}][{counter1}]",
					global::Immediate.Validations.Shared.MaxLengthAttribute.DefaultMessage,
					new()
					{
						["PropertyName"] = $"Tags[{counter0}][{counter1}]",
						["PropertyValue"] = t,
						["MaxLengthName"] = "",
						["MaxLengthValue"] = 8,
					}
				);
			}
		}
	}

	private static void __ValidateTags0(
		global::Immediate.Validations.Shared.ValidationResult errors, Request instance, global::System.Collections.Generic.List<string?> target, int counter0
	)
	{

		if (target is not { } t)
		{
			errors.Add(
				$"Tags[{counter0}]",
				global::Immediate.Validations.Shared.NotNullAttribute.DefaultMessage,
				new()
				{
					["PropertyName"] = $"Tags[{counter0}]",
					["PropertyValue"] = null,
				}
			);

			return;
		}


		var counter1 = 0;
		foreach (var item1 in t)
		{
			__ValidateTags00(
				errors, instance, item1, counter0, counter1
			);
			counter1++;
		}

	}

	private static void __ValidateTags(
		global::Immediate.Validations.Shared.ValidationResult errors, Request instance, global::System.Collections.Generic.List<global::System.Collections.Generic.List<string?>> target
	)
	{

		if (target is not { } t)
		{
			errors.Add(
				$"Tags",
				global::Immediate.Validations.Shared.NotNullAttribute.DefaultMessage,
				new()
				{
					["PropertyName"] = $"Tags",
					["PropertyValue"] = null,
				}
			);

			return;
		}


		var counter0 = 0;
		foreach (var item0 in t)
		{
			__ValidateTags0(
				errors, instance, item0, counter0
			);
			counter0++;
		}

	}

}

