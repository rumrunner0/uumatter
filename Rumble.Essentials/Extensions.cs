using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Rumble.Essentials;

/// <summary>
/// Extensions for types used in application essentials.
/// </summary>
internal static class Extensions
{
	/// <summary>
	/// Determines whether a dictionary is empty or not.
	/// </summary>
	/// <param name="source">The dictionary</param>
	/// <returns>True if the dictionary is empty. Otherwise, false</returns>
	internal static bool IsEmpty(this IDictionary source)
	{
		return source.Count is < 1;
	}

	///
	/// <inheritdoc cref="IsEmpty"/>
	///
	/// <typeparam name="TKey">Type of the dictionary key</typeparam>
	/// <typeparam name="TValue">Type of the dictionary value</typeparam>
	/// <returns>True if the dictionary is empty. Otherwise, false</returns>
	internal static bool IsEmpty<TKey, TValue>(this IDictionary<TKey, TValue> source)
	{
		return ((IDictionary)source).IsEmpty();
	}

	/// <summary>
	/// Determines whether a configuration section exists or not.
	/// </summary>
	/// <param name="section">The configuration section</param>
	/// <returns>True if the configuration section exists. Otherwise, false</returns>
	internal static bool DoesNotExist(this IConfigurationSection section)
	{
		return section.Exists() is false;
	}
}