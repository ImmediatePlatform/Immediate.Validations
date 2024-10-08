﻿//HintName: IV.Namespace..IInterface.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591

namespace Namespace;


partial interface IInterface
{
	static ValidationResult IValidationTarget<IInterface>.Validate(IInterface? target) =>
		Validate(target, []);

	static ValidationResult IValidationTarget<IInterface>.Validate(IInterface? target, ValidationResult errors) =>
		Validate(target, errors);

	public static new ValidationResult Validate(IInterface? target) =>
		Validate(target, []);

	public static new ValidationResult Validate(IInterface? target, ValidationResult errors)
	{
		if (target is not { } t)
		{
			return new()
			{
				{ ".self", "`target` must not be `null`." },
			};
		}

		if (!errors.VisitType(typeof(IInterface)))
			return errors;
		
		global::Namespace.IBaseInterface.Validate(t, errors);

		__ValidateValueB(errors, t, t.ValueB);


		return errors;
	}



	private static void __ValidateValueB(
		ValidationResult errors, IInterface instance, int target
	)
	{

		var t = target;



		{
			if (!global::Immediate.Validations.Shared.EqualAttribute.ValidateProperty(
					t
					, comparison: instance.ValueA
				)
			)
			{
				errors.Add(
					$"ValueB",
					global::Immediate.Validations.Shared.EqualAttribute.DefaultMessage,
					new()
					{
						["PropertyName"] = $"Value B",
						["PropertyValue"] = t,
						["ComparisonName"] = "Value A",
						["ComparisonValue"] = instance.ValueA,
					}
				);
			}
		}
	}

}

