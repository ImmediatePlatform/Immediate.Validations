//HintName: IV...ValidateClass.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591


partial class ValidateClass
{
	static List<ValidationError> IValidationTarget<ValidateClass>.Validate(ValidateClass? target) =>
		Validate(target);

	public static  List<ValidationError> Validate(ValidateClass? target)
	{
		if (target is not { } t)
		{
			return 
			[
				new()
				{
					PropertyName = ".self",
					ErrorMessage = "`target` must not be `null`.",
				},
			];
		}
		
		var errors = new List<ValidationError>();


		__ValidateValidationTargets(errors, t, t.ValidationTargets);

		return errors;
	}



	private static void __ValidateValidationTargets0(
		List<ValidationError> errors, ValidateClass instance, global::ValidationTarget target, int counter0
	)
	{

		if (target is not { } t)
		{
			errors.Add(new()
			{
				PropertyName = $"ValidationTargets[{counter0}]",
				ErrorMessage = "Property must not be `null`.",
			});

			return;
		}

		foreach (var error in global::ValidationTarget.Validate(t))
		{
			errors.Add(error with 
			{
				PropertyName = $"ValidationTargets[{counter0}].{error.PropertyName}",
			});
		}


	}

	private static void __ValidateValidationTargets(
		List<ValidationError> errors, ValidateClass instance, global::ValidationTarget[] target
	)
	{

		if (target is not { } t)
		{
			errors.Add(new()
			{
				PropertyName = $"ValidationTargets",
				ErrorMessage = "Property must not be `null`.",
			});

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

