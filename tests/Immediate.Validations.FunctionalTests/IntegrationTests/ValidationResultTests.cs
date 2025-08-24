using System.Diagnostics.CodeAnalysis;
using Immediate.Validations.Shared;

namespace Immediate.Validations.FunctionalTests.IntegrationTests;

public sealed partial class ValidationResultTests
{
	public sealed partial record Command
	{
		public required string HelloWorld { get; init; }

		public required int Value { get; init; }

		public const int MaxLength = 250;
		public static int MaxValue => 300;

		public static string DoSomething() => "Hello world!";
		public static int DoSomething(Value v) => v?.X ?? 123;

		public int DoSomethingInstance() => Value;
	}

	public sealed record Value(int X);

	private static Command BuildCommand() =>
		new()
		{
			HelloWorld = "Hello world!",
			Value = 123,
		};

	[Fact]
	public void BinaryExpressionTest1()
	{
		var results = new ValidationResult();
		var command = BuildCommand();

		results.Add(
			() => EqualAttribute.ValidateProperty(
				command.Value,
				100 + 23
			)
		);

		Assert.Equal([], results);
	}

	[Fact]
	public void BinaryExpressionTest2()
	{
		var results = new ValidationResult();
		var command = BuildCommand();
		var v1 = default(int?);
		var v2 = 123;

		results.Add(
			() => EqualAttribute.ValidateProperty(
				command.Value,
				v1 ?? v2
			)
		);

		Assert.Equal([], results);
	}

	[Fact]
	public void BinaryExpressionTest3()
	{
		var results = new ValidationResult();
		var command = BuildCommand();

		var array = new int[] { 123 };

		results.Add(
			() => EqualAttribute.ValidateProperty(
				command.Value,
				array[0]
			)
		);

		Assert.Equal([], results);
	}

	[Fact]
	public void ConditionalExpressionTest1()
	{
		var results = new ValidationResult();
		var command = BuildCommand();

		results.Add(
			() => EqualAttribute.ValidateProperty(
				command.Value,
				true ? 123 : 0
			)
		);

		Assert.Equal([], results);
	}

	[Fact]
	public void ConditionalExpressionTest2()
	{
		var results = new ValidationResult();
		var command = BuildCommand();

		results.Add(
			() => EqualAttribute.ValidateProperty(
				command.Value,
				false ? 0 : 123
			)
		);

		Assert.Equal([], results);
	}

	[Fact]
	public void ConstantExpressionTest()
	{
		var results = new ValidationResult();
		var command = BuildCommand();

		results.Add(
			() => EqualAttribute.ValidateProperty(
				command.Value,
				123
			)
		);

		Assert.Equal([], results);
	}

	[Fact]
	public void MemberExpressionTest()
	{
		var results = new ValidationResult();
		var command = BuildCommand();

		results.Add(
			() => NotEmptyAttribute.ValidateProperty(
				command.Value
			)
		);

		Assert.Equal([], results);
	}

	[Fact]
	public void MethodCallExpressionTest1()
	{
		var results = new ValidationResult();
		var command = BuildCommand();

		results.Add(
			() => EqualAttribute.ValidateProperty(
				command.HelloWorld,
				Command.DoSomething()
			)
		);

		Assert.Equal([], results);
	}

	[Fact]
	public void MethodCallExpressionTest2()
	{
		var results = new ValidationResult();
		var command = BuildCommand();

		var array = new List<int> { 123 };

		results.Add(
			() => EqualAttribute.ValidateProperty(
				command.Value,
				array[0]
			)
		);

		Assert.Equal([], results);
	}

	[Fact]
	public void NewExpressionTest1()
	{
		var results = new ValidationResult();
		var command = BuildCommand();

		results.Add(
			() => EqualAttribute.ValidateProperty(
				command.Value,
				Command.DoSomething(new Value(123))
			)
		);

		Assert.Equal([], results);
	}

	[Fact]
	public void NewArrayExpressionTest1()
	{
		var results = new ValidationResult();
		var command = BuildCommand();
		var v = 123;

		results.Add(
			() => EqualAttribute.ValidateProperty(
				command.Value,
				new int[] { v }[0]
			)
		);

		Assert.Equal([], results);
	}

	[Fact]
	public void UnaryExpressionTest1()
	{
		var results = new ValidationResult();
		var command = BuildCommand();

		results.Add(
			() => EqualAttribute.ValidateProperty(
				command.Value,
				+123
			)
		);

		Assert.Equal([], results);
	}

	[Fact]
	public void UnaryExpressionTest2()
	{
		var results = new ValidationResult();
		var command = BuildCommand();
		var v = -123;

		results.Add(
			() => EqualAttribute.ValidateProperty(
				command.Value,
				-v
			)
		);

		Assert.Equal([], results);
	}

	[Fact]
	public void InvalidExpressionTest1()
	{
		var results = new ValidationResult();

		var ex = Assert.Throws<NotSupportedException>(() =>
			results.Add(
				() => true
			)
		);

		Assert.Equal("Invalid Validation Expression", ex.Message);
	}

	[Fact]
	public void PropertyNameTest1()
	{
		var results = new ValidationResult();
		var command = BuildCommand();

		results.Add(
			() => EmptyAttribute.ValidateProperty(
				command.Value
			)
		);

		Assert.Equal(
			[
				new()
				{
					PropertyName = "Value",
					ErrorMessage = "'Value' must be empty.",
				},
			],
			results
		);
	}

	[Fact]
	public void PropertyNameTest2()
	{
		var results = new ValidationResult();
		var command = BuildCommand();

		results.Add(
			() => EmptyAttribute.ValidateProperty(
				command.HelloWorld
			)
		);

		Assert.Equal(
			[
				new()
				{
					PropertyName = "HelloWorld",
					ErrorMessage = "'Hello World' must be empty.",
				},
			],
			results
		);
	}

	[SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Testing")]
	[SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "Testing")]
	public sealed record ArrayListDictionaryProperties
	{
		public required int[] SomeArray { get; init; }
		public required List<int> SomeList { get; init; }
		public required Dictionary<int, int> SomeDictionary { get; init; }
	}

	private static ArrayListDictionaryProperties BuildArrayListDictionary() =>
		new()
		{
			SomeArray = [123],
			SomeList = [123],
			SomeDictionary = new() { [123] = 123 },
		};

	[Fact]
	public void PropertyNameTest3()
	{
		var results = new ValidationResult();
		var command = BuildArrayListDictionary();

		results.Add(
			() => EmptyAttribute.ValidateProperty(
				command.SomeArray[0]
			)
		);

		Assert.Equal(
			[
				new()
				{
					PropertyName = "SomeArray[0]",
					ErrorMessage = "'Some Array[0]' must be empty.",
				},
			],
			results
		);
	}

	[Fact]
	public void PropertyNameTest4()
	{
		var results = new ValidationResult();
		var command = BuildArrayListDictionary();

		results.Add(
			() => EmptyAttribute.ValidateProperty(
				command.SomeList[0]
			)
		);

		Assert.Equal(
			[
				new()
				{
					PropertyName = "SomeList[0]",
					ErrorMessage = "'Some List[0]' must be empty.",
				},
			],
			results
		);
	}

	[Fact]
	public void PropertyNameTest5()
	{
		var results = new ValidationResult();
		var command = BuildArrayListDictionary();

		results.Add(
			() => EmptyAttribute.ValidateProperty(
				command.SomeDictionary[123]
			)
		);

		Assert.Equal(
			[
				new()
				{
					PropertyName = "SomeDictionary[123]",
					ErrorMessage = "'Some Dictionary[123]' must be empty.",
				},
			],
			results
		);
	}

	[Fact]
	public void PropertyNameTest6()
	{
		var results = new ValidationResult();
		var command = BuildCommand();

		results.Add(
			() => NotEqualAttribute.ValidateProperty(
				command.Value,
				command.DoSomethingInstance()
			),
			"'{ComparisonName}'"
		);

		Assert.Equal(
			[
				new()
				{
					PropertyName = "Value",
					ErrorMessage = "'DoSomethingInstance()'",
				},
			],
			results
		);
	}

	[Fact]
	public void PropertyNameTest7()
	{
		var results = new ValidationResult();
		var command = BuildCommand();

		results.Add(
			() => NotEqualAttribute.ValidateProperty(
				command.HelloWorld,
				Command.DoSomething()
			),
			"'{ComparisonName}'"
		);

		Assert.Equal(
			[
				new()
				{
					PropertyName = "HelloWorld",
					ErrorMessage = "'DoSomething()'",
				},
			],
			results
		);
	}

	[Fact]
	public void PropertyNameTest8()
	{
		var results = new ValidationResult();
		var command = BuildCommand();

		results.Add(
			() => EqualAttribute.ValidateProperty(
				command.Value,
				Command.MaxValue
			),
			"'{ComparisonName}'"
		);

		Assert.Equal(
			[
				new()
				{
					PropertyName = "Value",
					ErrorMessage = "'Max Value'",
				},
			],
			results
		);
	}

	[Validate]
	public sealed partial class MessageFormatCommand : IValidationTarget<MessageFormatCommand>
	{
		[LessThan(0.0d, Message = "{PropertyValue:N2}")]
		public required double Value1 { get; init; }
		[LessThan(0.0d, Message = "{Invalid}")]
		public required double Value2 { get; init; }
	}

	[Fact]
	public void MessageFormatTest1()
	{
		var command = new MessageFormatCommand()
		{
			Value1 = 1,
			Value2 = 1,
		};

		var results = MessageFormatCommand.Validate(command);

		Assert.Equal(
			[
				new()
				{
					PropertyName = "Value1",
					ErrorMessage = "1.00",
				},
				new()
				{
					PropertyName = "Value2",
					ErrorMessage = "{Invalid}",
				},
			],
			results
		);
	}

	[Fact]
	public void MessageFormatTest2()
	{
		var command = new MessageFormatCommand()
		{
			Value1 = 1.2345,
			Value2 = 1.2345,
		};

		var results = MessageFormatCommand.Validate(command);

		Assert.Equal(
			[
				new()
				{
					PropertyName = "Value1",
					ErrorMessage = "1.23",
				},
				new()
				{
					PropertyName = "Value2",
					ErrorMessage = "{Invalid}",
				},
			],
			results
		);
	}
}
