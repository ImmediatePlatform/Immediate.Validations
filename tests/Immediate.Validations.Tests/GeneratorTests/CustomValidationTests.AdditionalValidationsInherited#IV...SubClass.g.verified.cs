//HintName: IV...SubClass.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591


partial class SubClass
{
	static List<ValidationError> IValidationTarget<SubClass>.Validate(SubClass? target) =>
		Validate(target);

	public static  List<ValidationError> Validate(SubClass? target)
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

		errors.AddRange(global::BaseClass.Validate(t));



		return errors;
	}



}

