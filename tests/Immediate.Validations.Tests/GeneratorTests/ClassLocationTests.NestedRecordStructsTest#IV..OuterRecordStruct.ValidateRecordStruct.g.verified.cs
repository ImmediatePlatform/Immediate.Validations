//HintName: IV..OuterRecordStruct.ValidateRecordStruct.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591

partial record struct OuterRecordStruct
{

partial record struct ValidateRecordStruct
{
	public static List<ValidationError> Validate(ValidateRecordStruct? target)
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

}
