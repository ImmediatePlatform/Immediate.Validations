//HintName: IV...ValidationTarget.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591


partial class ValidationTarget
{
	static List<ValidationError> IValidationTarget<ValidationTarget>.Validate(ValidationTarget? target) =>
		Validate(target);

	public static  List<ValidationError> Validate(ValidationTarget? target)
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


		__ValidateStringProperty(errors, t, t.StringProperty);

		return errors;
	}



	private static void __ValidateStringProperty(
		List<ValidationError> errors, ValidationTarget instance, string target
	)
	{

		if (target is not { } t)
		{
			errors.Add(new()
			{
				PropertyName = $"StringProperty",
				ErrorMessage = "Property must not be `null`.",
			});

			return;
		}



	}

}

