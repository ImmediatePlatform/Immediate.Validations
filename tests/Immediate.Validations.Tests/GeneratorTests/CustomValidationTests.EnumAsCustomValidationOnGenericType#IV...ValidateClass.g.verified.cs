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

		__ValidateEnumProperty(errors, t.EnumProperty);

		return errors;
	}



	private static void __ValidateEnumProperty(
		List<ValidationError> errors, global::TestEnum? target
	)
	{

		if (target is not { } t)
		{

			return;
		}



		if (
			global::Immediate.Validations.Shared.EnumValueAttribute.ValidateProperty(
				t
			) is (true, { } message)
		)
		{
			errors.Add(new()
			{
				PropertyName = $"EnumProperty", 
				ErrorMessage = message,
			});
		}
	}

}
