//HintName: IV..OuterClass.SubClass.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591

partial class OuterClass
{

partial class SubClass
{
	static ValidationResult IValidationTarget<SubClass>.Validate(SubClass? target) =>
		Validate(target);

	public static  ValidationResult Validate(SubClass? target)
	{
		if (target is not { } t)
		{
			return new()
			{
				{ ".self", "`target` must not be `null`." },
			};
		}
		
		var errors = new ValidationResult();

		foreach (var error in global::OuterClass.BaseClass.Validate(t))
			errors.Add(error);

		__ValidateValueB(errors, t, t.ValueB);


		return errors;
	}



	private static void __ValidateValueB(
		ValidationResult errors, SubClass instance, int target
	)
	{

		var t = target;



		{
			if (!global::Immediate.Validations.Shared.EqualAttribute.ValidateProperty(
					t
					, comparison: instance.ValueA
				)
			)
			{
				errors.Add(
					$"ValueB",
					global::Immediate.Validations.Shared.EqualAttribute.DefaultMessage,
					new()
					{
						["PropertyName"] = $"Value B",
						["PropertyValue"] = t,
						["ComparisonName"] = "Value A",
						["ComparisonValue"] = instance.ValueA,
					}
				);
			}
		}
	}

}

}
