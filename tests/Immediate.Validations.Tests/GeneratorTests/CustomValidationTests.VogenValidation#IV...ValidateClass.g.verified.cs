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


		__ValidateUserId(errors, t, t.UserId);


		return errors;
	}



	private static void __ValidateUserId(
		List<ValidationError> errors, ValidateClass instance, global::UserId target
	)
	{

		var t = target;

		{
			var validation = global::UserId.Validate(t.Value);
			if (!string.IsNullOrWhiteSpace(validation.ErrorMessage))
			{
				errors.Add(new()
				{
					PropertyName = $"UserId",
					ErrorMessage = validation.ErrorMessage,
				});
			}
		}


	}

}

