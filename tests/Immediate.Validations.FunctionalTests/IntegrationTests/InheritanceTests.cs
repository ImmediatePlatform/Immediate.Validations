using Immediate.Validations.Shared;

namespace Immediate.Validations.FunctionalTests.IntegrationTests;

public sealed partial class InheritanceTests
{
	[Validate]
	public partial interface IBaseInterface : IValidationTarget<IBaseInterface>
	{
		[NotEmpty]
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

	[Test]
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

	[Test]
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
					ErrorMessage = "'Id' must be more than 4 characters.",
				}
			],
			errors
		);
	}

	[Test]
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
					ErrorMessage = "'Description' must not be empty.",
				}
			],
			errors
		);
	}
}
