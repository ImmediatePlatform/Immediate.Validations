//HintName: IV.Namespace.OuterClass.SubClass.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591

namespace Namespace;

partial class OuterClass
{

partial class SubClass : global::Immediate.Validations.Shared.IValidationTarget
{
	global::Immediate.Validations.Shared.ValidationResult global::Immediate.Validations.Shared.IValidationTarget.Validate() =>
		Validate(this, []);

	global::Immediate.Validations.Shared.ValidationResult global::Immediate.Validations.Shared.IValidationTarget.Validate(global::Immediate.Validations.Shared.ValidationResult errors) =>
		Validate(this, errors);

	static global::Immediate.Validations.Shared.ValidationResult global::Immediate.Validations.Shared.IValidationTarget<SubClass>.Validate(SubClass? target) =>
		Validate(target, []);

	static global::Immediate.Validations.Shared.ValidationResult global::Immediate.Validations.Shared.IValidationTarget<SubClass>.Validate(SubClass? target, global::Immediate.Validations.Shared.ValidationResult errors) =>
		Validate(target, errors);

	public static  global::Immediate.Validations.Shared.ValidationResult Validate(SubClass? target) =>
		Validate(target, []);

	public static  global::Immediate.Validations.Shared.ValidationResult Validate(SubClass? target, global::Immediate.Validations.Shared.ValidationResult errors)
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
		global::Immediate.Validations.Shared.ValidationResult errors, SubClass instance, int target
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
