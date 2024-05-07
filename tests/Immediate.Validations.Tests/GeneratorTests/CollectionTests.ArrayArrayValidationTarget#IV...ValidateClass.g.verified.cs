//HintName: IV...ValidateClass.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591


partial class ValidateClass
{
	public static List<ValidationError> Validate(ValidateClass? target)
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

		__ValidateValidationTargets(errors, t.ValidationTargets);

		return errors;
	}


		private static void __ValidateValidationTargets00(
		List<ValidationError> errors, global::ValidationTarget target, int counter0, int counter1
	)
	{

		if (target is not { } t)
		{
			errors.Add(new()
			{
				PropertyName = $"ValidationTargets[{counter0}][{counter1}]",
				ErrorMessage = "Property must not be `null`.",
			});

			return;
		}

		foreach (var error in global::ValidationTarget.Validate(t))
		{
			errors.Add(error with 
			{
				PropertyName = $"ValidationTargets[{counter0}][{counter1}].{error.PropertyName}",
			});
		}


	}

	private static void __ValidateValidationTargets0(
		List<ValidationError> errors, global::ValidationTarget[] target, int counter0
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


		var counter1 = 0;
		foreach (var item1 in t)
		{
			__ValidateValidationTargets00(
				errors, item1, counter0, counter1
			);
			counter1++;
		}

	}

	private static void __ValidateValidationTargets(
		List<ValidationError> errors, global::ValidationTarget[][] target
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
				errors, item0, counter0
			);
			counter0++;
		}

	}

}

