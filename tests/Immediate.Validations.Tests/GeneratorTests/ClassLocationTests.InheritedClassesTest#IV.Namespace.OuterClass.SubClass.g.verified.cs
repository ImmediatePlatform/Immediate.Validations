﻿//HintName: IV.Namespace.OuterClass.SubClass.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591

namespace Namespace;

partial class OuterClass
{

partial class SubClass : IValidationTarget
{
	ValidationResult IValidationTarget.Validate() =>
		Validate(this, []);

	ValidationResult IValidationTarget.Validate(ValidationResult errors) =>
		Validate(this, errors);

	static ValidationResult IValidationTarget<SubClass>.Validate(SubClass? target) =>
		Validate(target, []);

	static ValidationResult IValidationTarget<SubClass>.Validate(SubClass? target, ValidationResult errors) =>
		Validate(target, errors);

	public static  ValidationResult Validate(SubClass? target) =>
		Validate(target, []);

	public static  ValidationResult Validate(SubClass? target, ValidationResult errors)
	{
		if (target is not { } t)
		{
			return new()
			{
				{ ".self", "`target` must not be `null`." },
			};
		}

		if (!errors.VisitType(typeof(SubClass)))
			return errors;
		
		global::Namespace.OuterClass.BaseClass.Validate(t, errors);

		__ValidateValueB(errors, t, t.ValueB);


		return errors;
	}



	private static void __ValidateValueB(
		ValidationResult errors, SubClass instance, int target
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

}
