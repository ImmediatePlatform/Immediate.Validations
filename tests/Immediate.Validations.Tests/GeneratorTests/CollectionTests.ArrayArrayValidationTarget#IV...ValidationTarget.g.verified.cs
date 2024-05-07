//HintName: IV...ValidationTarget.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591


partial class ValidationTarget
{
	public static List<ValidationError> Validate(ValidationTarget target)
	{
		if (target is null)
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

		__ValidateStringProperty(errors, target.StringProperty);

		return errors;
	}


		private static void __ValidateStringProperty(
		List<ValidationError> errors, string target
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

