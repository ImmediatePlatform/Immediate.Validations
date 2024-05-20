using Immediate.Validations.Shared;
using Xunit;

namespace Immediate.Validations.FunctionalTests.IntegrationTests;

public sealed partial class InheritanceTests
{
	[Validate]
	public partial interface IBaseInterface : IValidationTarget<IBaseInterface>
	{
		[NotEmptyOrWhiteSpace]
		string Description { get; }
	}

	[Validate]
	public partial record BaseClass : IValidationTarget<BaseClass>
	{
		[MinLength(4)]
		public required string Id { get; init; }
	}

	[Validate]
	public partial record Class : BaseClass, IBaseInterface, IValidationTarget<Class>
	{
		public required string Description { get; init; }
	}

	[Fact]
	public void ValidClassHasNoErrors()
	{
		var @class = new Class
		{
			Id = "1234",
			Description = "Hello World!",
		};

		var errors = Class.Validate(@class);

		Assert.Empty(errors);
	}

	[Fact]
	public void ShortIdHasErrors()
	{
		var @class = new Class
		{
			Id = "123",
			Description = "Hello World!",
		};

		var errors = Class.Validate(@class);

		Assert.Equal(
			[
				new()
				{
					PropertyName = "Id",
					ErrorMessage = "String is of length '3', which is shorter than the minimum allowed length of '4'.",
				}
			],
			errors
		);
	}

	[Fact]
	public void EmptyDescriptionHasErrors()
	{
		var @class = new Class
		{
			Id = "1234",
			Description = "",
		};

		var errors = Class.Validate(@class);

		Assert.Equal(
			[
				new()
				{
					PropertyName = "Description",
					ErrorMessage = "Property must not be `null` or whitespace.",
				}
			],
			errors
		);
	}
}
