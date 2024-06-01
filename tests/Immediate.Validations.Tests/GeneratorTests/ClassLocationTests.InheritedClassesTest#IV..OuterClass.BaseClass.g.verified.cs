//HintName: IV..OuterClass.BaseClass.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591

partial class OuterClass
{

partial class BaseClass
{
	static List<ValidationError> IValidationTarget<BaseClass>.Validate(BaseClass? target) =>
		Validate(target);

	public static  List<ValidationError> Validate(BaseClass? target)
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
