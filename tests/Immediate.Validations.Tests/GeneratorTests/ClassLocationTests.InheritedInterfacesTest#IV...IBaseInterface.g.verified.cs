//HintName: IV...IBaseInterface.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591


partial interface IBaseInterface
{
	static ValidationResult IValidationTarget<IBaseInterface>.Validate(IBaseInterface? target) =>
		Validate(target);

	public static new ValidationResult Validate(IBaseInterface? target)
	{
		if (target is not { } t)
		{
			return new()
			{
				{ ".self", "`target` must not be `null`." },
			};
		}
		
		var errors = new ValidationResult();




		return errors;
	}



}

