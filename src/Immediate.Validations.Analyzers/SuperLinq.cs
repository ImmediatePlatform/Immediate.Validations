namespace Immediate.Validations.Analyzers;

internal static class SuperLinq
{
	public static IEnumerable<(TLeft? Left, TRight? Right)> JoinMerge<TLeft, TRight>(
		this IEnumerable<TLeft> left,
		IEnumerable<TRight> right,
		Func<TLeft, string> leftKeySelector,
		Func<TRight, string> rightKeySelector,
		IComparer<string>? comparer = null
	)
		where TLeft : class
		where TRight : class
	{
		comparer ??= Comparer<string>.Default;

		using var e1 = left.GetEnumerator();
		using var e2 = right.GetEnumerator();

		var gotLeft = e1.MoveNext();
		var gotRight = e2.MoveNext();

		while (gotLeft && gotRight)
		{
			var l = e1.Current;
			var r = e2.Current;
			var comparison = comparer.Compare(leftKeySelector(l), rightKeySelector(r));

			switch (comparison)
			{
				case < 0:
				{
					yield return (l, null);
					gotLeft = e1.MoveNext();
					break;
				}

				case > 0:
				{
					yield return (null, r);
					gotRight = e2.MoveNext();
					break;
				}

				default:
				{
					yield return (l, r);
					gotLeft = e1.MoveNext();
					gotRight = e2.MoveNext();
					break;
				}
			}
		}

		if (gotLeft)
		{
			do
			{
				yield return (e1.Current, null);
			}
			while (e1.MoveNext());
			yield break;
		}

		if (gotRight)
		{
			do
			{
				yield return (null, e2.Current);
			}
			while (e2.MoveNext());
		}
	}
}
