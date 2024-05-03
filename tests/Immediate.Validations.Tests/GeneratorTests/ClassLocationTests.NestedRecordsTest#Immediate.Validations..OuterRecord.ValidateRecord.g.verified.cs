//HintName: Immediate.Validations..OuterRecord.ValidateRecord.g.cs

using System.Collections.Generic;
using Immediate.Validations.Shared;

#pragma warning disable CS1591

partial record OuterRecord
{

partial record ValidateRecord : IValidationTarget<ValidateRecord>
{
	public static List<ValidationError> Validate(ValidateRecord target)
	{
		var errors = new List<ValidationError>();


		return errors;
	}
}

}
