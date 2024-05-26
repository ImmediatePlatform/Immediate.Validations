//HintName: IV...Target.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591


partial record Target
{
	static List<ValidationError> IValidationTarget<Target>.Validate(Target? target) =>
		Validate(target);

	public static  List<ValidationError> Validate(Target? target)
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


		__ValidateId(errors, t, t.Id);
		__ValidateFirstValue(errors, t, t.FirstValue);


		return errors;
	}



	private static void __ValidateId(
		List<ValidationError> errors, Target instance, string target
	)
	{

		if (target is not { } t)
		{
			errors.Add(new()
			{
				PropertyName = $"Id",
				ErrorMessage = "Property must not be `null`.",
			});

			return;
		}



		{
			if (
				global::DummyAttribute.ValidateProperty(
					t
					, first: instance.FirstValue
					, second: "Hello World"
					, fourth: "Abcd"
					, fifth: "The end?"
					, "Test1"
					, "FirstValue"
					, "Test3"
				) is (true, { } message)
			)
			{
				errors.Add(new()
				{
					PropertyName = $"Id", 
					ErrorMessage = "What's going on?",
				});
			}
		}
	}

	private static void __ValidateFirstValue(
		List<ValidationError> errors, Target instance, string target
	)
	{

		if (target is not { } t)
		{
			errors.Add(new()
			{
				PropertyName = $"FirstValue",
				ErrorMessage = "Property must not be `null`.",
			});

			return;
		}



	}

}

