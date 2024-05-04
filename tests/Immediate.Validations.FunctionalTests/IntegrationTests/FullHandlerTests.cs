using Immediate.Validations.Shared;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Immediate.Validations.FunctionalTests.IntegrationTests;

public sealed class FullHandlerTests
{
	[Fact]
	public async Task FullTestWithValidData()
	{
		var services = new ServiceCollection();

		_ = services
			.AddHandlers()
			.AddBehaviors();

		var provider = services.BuildServiceProvider();
		using var scope = provider.CreateScope();
		var handler = scope.ServiceProvider.GetRequiredService<SaveRecord.Handler>();

		_ = await handler.HandleAsync(
			new()
			{
				Name = "Hello World!",
				Status = SaveRecord.Status.Open,
				Value = 3,
			}
		);
	}

	[Fact]
	public async Task FullTestWithInvalidName()
	{
		var services = new ServiceCollection();

		_ = services
			.AddHandlers()
			.AddBehaviors();

		var provider = services.BuildServiceProvider();
		using var scope = provider.CreateScope();
		var handler = scope.ServiceProvider.GetRequiredService<SaveRecord.Handler>();

		var ex = await Assert.ThrowsAsync<ValidationException>(async () =>
			await handler.HandleAsync(
				new()
				{
					Name = null!,
					Status = SaveRecord.Status.Open,
					Value = 3,
				}
			)
		);

		Assert.Equal(
			[
				new()
				{
					PropertyName = "Name",
					ErrorMessage = "Property must not be `null`.",
				},
				new()
				{
					PropertyName = "Name",
					ErrorMessage = "Name must be provided.",
				},
			],
			ex.Errors
		);
	}

	[Fact]
	public async Task FullTestWithInvalidStatus()
	{
		var services = new ServiceCollection();

		_ = services
			.AddHandlers()
			.AddBehaviors();

		var provider = services.BuildServiceProvider();
		using var scope = provider.CreateScope();
		var handler = scope.ServiceProvider.GetRequiredService<SaveRecord.Handler>();

		var ex = await Assert.ThrowsAsync<ValidationException>(async () =>
			await handler.HandleAsync(
				new()
				{
					Name = "Hello World!",
					Status = (SaveRecord.Status)10,
					Value = 3,
				}
			)
		);

		Assert.Equal(
			[
				new()
				{
					PropertyName = "Status",
					ErrorMessage = "The value `10` is not defined in the enum type `Status`.",
				},
			],
			ex.Errors
		);
	}

	[Fact]
	public async Task FullTestWithInvalidValue()
	{
		var services = new ServiceCollection();

		_ = services
			.AddHandlers()
			.AddBehaviors();

		var provider = services.BuildServiceProvider();
		using var scope = provider.CreateScope();
		var handler = scope.ServiceProvider.GetRequiredService<SaveRecord.Handler>();

		var ex = await Assert.ThrowsAsync<ValidationException>(async () =>
			await handler.HandleAsync(
				new()
				{
					Name = "Hello World!",
					Status = SaveRecord.Status.Open,
					Value = -1,
				}
			)
		);

		Assert.Equal(
			[
				new()
				{
					PropertyName = "Value",
					ErrorMessage = "Value `-1` is not greater than `0`.",
				},
			],
			ex.Errors
		);
	}
}
