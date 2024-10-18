using Immediate.Validations.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Immediate.Validations.FunctionalTests.IntegrationTests;

public sealed class FullHandlerTests
{
	[Test]
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

	[Test]
	public async Task FullTestWithNullData()
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
				null!
			)
		);

		Assert.Equal(
			[
				new()
				{
					PropertyName = ".self",
					ErrorMessage = "`target` must not be `null`.",
				},
			],
			ex.Errors
		);
	}

	[Test]
	public async Task FullTestWithNullName()
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
					ErrorMessage = "'Name' must not be null.",
				},
			],
			ex.Errors
		);
	}

	[Test]
	public async Task FullTestWithEmptyName()
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
					Name = "",
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
					ErrorMessage = "Name must be provided.",
				},
			],
			ex.Errors
		);
	}

	[Test]
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
					ErrorMessage = "'test' has a range of values which does not include '10'.",
				},
			],
			ex.Errors
		);
	}

	[Test]
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
