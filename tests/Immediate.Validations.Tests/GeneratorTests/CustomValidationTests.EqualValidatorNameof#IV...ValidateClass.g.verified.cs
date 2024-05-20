﻿//HintName: IV...ValidateClass.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591


partial class ValidateClass
{
	public static List<ValidationError> Validate(ValidateClass? target)
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


		__ValidateIntProperty(errors, t, t.IntProperty);
		__ValidateKeyValue(errors, t, t.KeyValue);

		return errors;
	}



	private static void __ValidateIntProperty(
		List<ValidationError> errors, ValidateClass instance, int target
	)
	{

		var t = target;



		if (
			global::Immediate.Validations.Shared.EqualAttribute.ValidateProperty(
				t
				, operand: instance.KeyValue
			) is (true, { } message)
		)
		{
			errors.Add(new()
			{
				PropertyName = $"IntProperty", 
				ErrorMessage = message,
			});
		}
	}

	private static void __ValidateKeyValue(
		List<ValidationError> errors, ValidateClass instance, int target
	)
	{

		var t = target;



	}

}

