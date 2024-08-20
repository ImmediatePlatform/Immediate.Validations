//HintName: IV.Namespace..BaseInterface.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591

namespace Namespace;


partial interface BaseInterface
{
	static ValidationResult IValidationTarget<BaseInterface>.Validate(BaseInterface? target) =>
		Validate(target);

	public static new ValidationResult Validate(BaseInterface? target)
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

