using Immediate.Handlers.Shared;

namespace Immediate.Validations.Shared;

/// <summary>
///	    A <see cref="Behavior{TRequest, TResponse}"/> which uses the <see cref="IValidationTarget{T}.Validate(T)"/>
///	    method to validate the provided <typeparamref name="TRequest"/>.
/// </summary>
/// <inheritdoc />
public sealed class ValidationBehavior<TRequest, TResponse>
	: Behavior<TRequest, TResponse>
	where TRequest : IValidationTarget<TRequest>
{
	/// <summary>
	///	    Validate the <paramref name="request"/> and throw an exception if it fails.
	/// </summary>
	/// <inheritdoc />
	/// <exception cref="ValidationException">
	///	    Thrown if the <paramref name="request"/> does not validate successfully.
	/// </exception>
	public override async ValueTask<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken)
	{
		var errors = TRequest.Validate(request);
		if (errors is { Count: > 0 })
			throw new ValidationException(errors);

		return await Next(request, cancellationToken).ConfigureAwait(false);
	}
}
