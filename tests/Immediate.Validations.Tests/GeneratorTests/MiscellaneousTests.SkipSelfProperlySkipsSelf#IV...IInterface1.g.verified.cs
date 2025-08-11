//HintName: IV...IInterface1.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591


partial interface IInterface1
{
	static ValidationResult IValidationTarget<IInterface1>.Validate(IInterface1? target) =>
		Validate(target, []);

	static ValidationResult IValidationTarget<IInterface1>.Validate(IInterface1? target, ValidationResult errors) =>
		Validate(target, errors);

	public static new ValidationResult Validate(IInterface1? target) =>
		Validate(target, []);

	public static new ValidationResult Validate(IInterface1? target, ValidationResult errors)
	{
		if (target is not { } t)
		{
			return new()
			{
				{ ".self", "`target` must not be `null`." },
			};
		}

		if (!errors.VisitType(typeof(IInterface1)))
			return errors;
		

		__ValidateProperty(errors, t, t.Property);


		return errors;
	}



	private static void __ValidateProperty(
		ValidationResult errors, IInterface1 instance, string target
	)
	{

		if (target is not { } t)
		{
			errors.Add(
				$"Property",
				global::Immediate.Validations.Shared.NotNullAttribute.DefaultMessage,
				new()
				{
					["PropertyName"] = $"Property",
					["PropertyValue"] = null,
				}
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
					$"Property",
					global::Immediate.Validations.Shared.NotEmptyAttribute.DefaultMessage,
					new()
					{
						["PropertyName"] = $"Property",
						["PropertyValue"] = t,
					}
				);
			}
		}
	}

}

