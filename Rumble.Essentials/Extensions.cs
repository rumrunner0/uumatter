using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Rumble.Essentials;

/// <summary>
/// Extensions.
/// </summary>
internal static class Extensions
{
	/// <summary>
	/// Determines whether a dictionary is empty or not.
	/// </summary>
	/// <param name="source">The dictionary.</param>
	/// <returns><c>true</c> if the dictionary is empty, otherwise, <c>false</c>.</returns>
	internal static bool Any(this IDictionary source)
	{
		return source.Count is < 1;
	}

	///
	/// <inheritdoc cref="Any" />
	///
	/// <typeparam name="TKey">Type of the dictionary key.</typeparam>
	/// <typeparam name="TValue">Type of the dictionary value.</typeparam>
	/// <returns><c>true</c> if the dictionary is empty, otherwise, <c>false</c>.</returns>
	internal static bool Any<TKey, TValue>(this IDictionary<TKey, TValue> source)
	{
		return ((IDictionary)source).Any();
	}

	/// <summary>
	/// Generates random unique indexes in a collection.
	/// </summary>
	/// <param name="source">The collection.</param>
	/// <param name="indexesNumber">Number of random unique indexes</param>
	/// <typeparam name="T">Type of the sequence elements</typeparam>
	/// <returns>Random unique indexes</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown if "<paramref name="indexesNumber"/>" is out of "<paramref name="source"/>" range</exception>
	public static IEnumerable<int> RandomUniqueIndexes<T>(this ICollection<T> source, int indexesNumber)
	{
		const string header = "Random unique indexes can't be created";

		if(indexesNumber < 0)
		{
			throw new ArgumentOutOfRangeException($"{header}. Requested number of indexes ({indexesNumber}) is less than 0.");
		}

		if(indexesNumber > source.Count)
		{
			throw new ArgumentOutOfRangeException($"{header}. Requested number of indexes ({indexesNumber}) is greater than collection length ({source.Count}).");
		}

		var resultCollection = new List<int>();
		for(var i = 0; i < indexesNumber; i++)
		{
			var index = 0;
			do index = RandomNumberGenerator.GetInt32(source.Count);
			while (resultCollection.Contains(index));

			yield return index;

			resultCollection.Add(index);
		}
	}
}