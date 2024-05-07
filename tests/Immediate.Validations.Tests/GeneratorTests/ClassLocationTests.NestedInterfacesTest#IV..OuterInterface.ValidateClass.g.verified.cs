//HintName: IV..OuterInterface.ValidateClass.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591

partial interface OuterInterface
{

partial class ValidateClass
{
	public static List<ValidationError> Validate(ValidateClass target)
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


		return errors;
	}


	}

}
