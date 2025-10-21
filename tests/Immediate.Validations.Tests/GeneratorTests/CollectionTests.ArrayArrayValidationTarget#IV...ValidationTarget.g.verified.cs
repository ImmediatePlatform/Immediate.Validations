//HintName: IV...ValidationTarget.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591


partial class ValidationTarget : global::Immediate.Validations.Shared.IValidationTarget
{
	global::Immediate.Validations.Shared.ValidationResult global::Immediate.Validations.Shared.IValidationTarget.Validate() =>
		Validate(this, []);

	global::Immediate.Validations.Shared.ValidationResult global::Immediate.Validations.Shared.IValidationTarget.Validate(global::Immediate.Validations.Shared.ValidationResult errors) =>
		Validate(this, errors);

	static global::Immediate.Validations.Shared.ValidationResult global::Immediate.Validations.Shared.IValidationTarget<ValidationTarget>.Validate(ValidationTarget? target) =>
		Validate(target, []);

	static global::Immediate.Validations.Shared.ValidationResult global::Immediate.Validations.Shared.IValidationTarget<ValidationTarget>.Validate(ValidationTarget? target, global::Immediate.Validations.Shared.ValidationResult errors) =>
		Validate(target, errors);

	public static  global::Immediate.Validations.Shared.ValidationResult Validate(ValidationTarget? target) =>
		Validate(target, []);

	public static  global::Immediate.Validations.Shared.ValidationResult Validate(ValidationTarget? target, global::Immediate.Validations.Shared.ValidationResult errors)
	{
		if (target is not { } t)
		{
			return new()
			{
				{ ".self", "`target` must not be `null`." },
			};
		}

		if (!errors.VisitType(typeof(ValidationTarget)))
			return errors;
		

		__ValidateStringProperty(errors, t, t.StringProperty);


		return errors;
	}



	private static void __ValidateStringProperty(
		global::Immediate.Validations.Shared.ValidationResult errors, ValidationTarget instance, string target
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

