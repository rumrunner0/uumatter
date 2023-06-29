using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Rumble.Essentials;

/// <summary>
/// Extensions for <see cref="IEnumerable{T}"/>.
/// </summary>
public static class EnumerableExtensions
{
	/// <summary>
	/// Gets random unique indexes in a sequence.
	/// </summary>
	/// <param name="source">Sequence of elements</param>
	/// <param name="count">Number of random unique indexes</param>
	/// <typeparam name="T">Type of the sequence elements</typeparam>
	/// <returns>Random unique indexes</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown if "<paramref name="count"/>" is out of "<paramref name="source"/>" range</exception>
	public static IEnumerable<int> RandomUniqueIndexes<T>(this IEnumerable<T> source, int count)
	{
		const int MIN_LENGTH = 1;
		var enumeratedSource = source.ToArray();
		if(count < MIN_LENGTH)
		{
			throw new ArgumentOutOfRangeException
			(
				$"Random unique indexes can't be created. " +
				$"Requested number of indexes ({count}) is less than {MIN_LENGTH}."
			);
		}

		if(count > enumeratedSource.Length)
		{
			throw new ArgumentOutOfRangeException
			(
				$"Random unique indexes can't be created. " +
				$"Requested number of indexes ({count}) is greater " +
				$"than sequence length ({enumeratedSource.Length})."
			);
		}

		var randomUniqueIndexes = new List<int>();
		for(var i = 0; i < count; i++)
		{
			var randomUniqueIndex = default(int);
			do randomUniqueIndex = RandomNumberGenerator.GetInt32(enumeratedSource.Length);
			while(randomUniqueIndexes.Contains(randomUniqueIndex));
			randomUniqueIndexes.Add(randomUniqueIndex);
		}

		return randomUniqueIndexes;
	}

	/// <summary>
	/// Deconstructs a sequence into a tuple of 2 separate elements and the rest.
	/// </summary>
	/// <param name="source">Sequence of elements</param>
	/// <param name="first">First element of the sequence</param>
	/// <param name="second">Second element of the sequence</param>
	/// <param name="rest">Rest elements of the sequence</param>
	/// <typeparam name="T">Type of the sequence elements</typeparam>
	/// <exception cref="ApplicationException">Thrown if number of elements is less than 2</exception>
	public static void Deconstruct<T>(this IEnumerable<T> source, out T first, out T second, out IEnumerable<T> rest)
	{
		const int MIN_LENGTH = 2;
		var enumeratedSource = source.ToArray();
		if(enumeratedSource.Length < MIN_LENGTH)
		{
			throw new ArgumentOutOfRangeException
			(
				$"Random unique indexes can't be created. " +
				$"Requested number of indexes ({enumeratedSource.Length}) is less than {MIN_LENGTH}."
			);
		}

		(first, second) = (enumeratedSource[0], enumeratedSource[1]);
		rest = enumeratedSource.Skip(2).ToArray();
	}

	/// <summary>
	/// Determines if the sequence is sorted in ascending order.
	/// </summary>
	/// <param name="source">Sequence of elements</param>
	/// <typeparam name="TElement">Type of the sequence elements</typeparam>
	/// <returns>true, if sequence is sorted in ascending order, false, if it's not</returns>
	public static bool IsOrdered<TElement>(this IEnumerable<TElement> source)
	where TElement : IComparable<TElement>
	{
		var array = source.ToArray();
		bool Compare(TElement current, TElement next)
		{
			return current.CompareTo(next) > 0;
		}

		for(var i = 0; i < array.Length - 1; i++)
		{
			if(Compare(array[i], array[i + 1]))
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// Created <see cref="string"/> representation of a sequence joined with "<paramref name="separator"/>".
	/// </summary>
	/// <param name="source">Sequence of elements</param>
	/// <param name="separator">Separator used to join elements of the sequence</param>
	/// <typeparam name="TElement">Type of the sequence elements</typeparam>
	/// <returns>String representation of the sequence joined with "<paramref name="separator"/>"</returns>
	public static string Joined<TElement>(this IEnumerable<TElement> source, string separator = " ")
	{
		return string.Join(separator, source);
	}
}