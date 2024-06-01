//HintName: IV...IInterface.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591


partial interface IInterface
{
	static List<ValidationError> IValidationTarget<IInterface>.Validate(IInterface? target) =>
		Validate(target);

	public static new List<ValidationError> Validate(IInterface? target)
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

		errors.AddRange(global::IBaseInterface.Validate(t));

		__ValidateValueB(errors, t, t.ValueB);


		return errors;
	}



	private static void __ValidateValueB(
		List<ValidationError> errors, IInterface instance, int target
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

