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


		__ValidateTestEnum(errors, t, t.TestEnum);


		return errors;
	}



	private static void __ValidateTestEnum(
		List<ValidationError> errors, ValidateClass instance, global::TestEnum target
	)
	{

		var t = target;



		{
			if (
				global::Immediate.Validations.Shared.EnumValueAttribute.ValidateProperty(
					t
				) is (true, { } message)
			)
			{
				errors.Add(new()
				{
					PropertyName = $"TestEnum", 
					ErrorMessage = message,
				});
			}
		}
	}

}

