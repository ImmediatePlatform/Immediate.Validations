//HintName: IV..OuterClass.SubClass.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591

partial class OuterClass
{

partial class SubClass
{
	static List<ValidationError> IValidationTarget<SubClass>.Validate(SubClass? target) =>
		Validate(target);

	public static  List<ValidationError> Validate(SubClass? target)
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

		errors.AddRange(global::OuterClass.BaseClass.Validate(t));

		__ValidateValueB(errors, t, t.ValueB);


		return errors;
	}



	private static void __ValidateValueB(
		List<ValidationError> errors, SubClass instance, int target
	)
	{

		var t = target;



		errors.Add(
			global::Immediate.Validations.Shared.EqualAttribute.ValidateProperty(
				t
				, operand: instance.ValueA
			),
			$"ValueB",
			null
		);
	}

}

}
