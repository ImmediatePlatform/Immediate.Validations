//HintName: IV.Namespace..BaseClass.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591

namespace Namespace;


partial class BaseClass
{
	static ValidationResult IValidationTarget<BaseClass>.Validate(BaseClass? target) =>
		Validate(target);

	public static  ValidationResult Validate(BaseClass? target)
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

