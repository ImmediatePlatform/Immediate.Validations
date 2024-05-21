//HintName: IV..OuterRecordStruct.ValidateRecordStruct.g.cs
using System.Collections.Generic;
using Immediate.Validations.Shared;

#nullable enable
#pragma warning disable CS1591

partial record struct OuterRecordStruct
{

partial record struct ValidateRecordStruct
{
	static List<ValidationError> IValidationTarget<ValidateRecordStruct>.Validate(ValidateRecordStruct target) =>
		Validate(target);

	public static  List<ValidationError> Validate(ValidateRecordStruct target)
	{
		var t = target;
		
		var errors = new List<ValidationError>();




		return errors;
	}



}

}
