﻿//HintName: IV..OuterRecordStruct.ValidateRecordStruct.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591

partial record struct OuterRecordStruct
{

partial record struct ValidateRecordStruct
{
	static ValidationResult IValidationTarget<ValidateRecordStruct>.Validate(ValidateRecordStruct target) =>
		Validate(target);

	public static  ValidationResult Validate(ValidateRecordStruct target)
	{
		var t = target;
		
		var errors = new ValidationResult();




		return errors;
	}



}

}
