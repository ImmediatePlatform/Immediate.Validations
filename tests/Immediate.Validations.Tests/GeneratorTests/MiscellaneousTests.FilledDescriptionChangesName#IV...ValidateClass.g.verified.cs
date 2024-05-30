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


		__ValidateTesting(errors, t, t.Testing);


		return errors;
	}



	private static void __ValidateTesting(
		List<ValidationError> errors, ValidateClass instance, string target
	)
	{

		if (target is not { } t)
		{
			errors.Add(new()
			{
				PropertyName = $"Hello World!",
				ErrorMessage = "Property must not be `null`.",
			});

			return;
		}



	}

}

