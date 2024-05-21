//HintName: IV...BaseInterface.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591


partial interface BaseInterface
{
	static List<ValidationError> IValidationTarget<BaseInterface>.Validate(BaseInterface? target) =>
		Validate(target);

	public static new List<ValidationError> Validate(BaseInterface? target)
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




		return errors;
	}



}

