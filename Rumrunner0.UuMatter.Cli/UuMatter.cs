using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Rumrunner0.UuMatter.Cli;

/// <summary>
/// Application universal usable matter.
/// </summary>
public static class UuMatter
{
	/// <summary>
	/// Name of the logger section in application configuration.
	/// </summary>
	private const string _loggerSectionName = "Serilog";

	/// <summary>
	/// Cached UU matter.
	/// </summary>
	private static readonly ConcurrentDictionary<Type, object> _cache;

	/// <summary>
	/// UU matter resolver.
	/// </summary>
	private static readonly ConcurrentDictionary<Type, Func<object>> _resolver;

	///
	/// <inheritdoc cref="UuMatter" />
	///
	static UuMatter()
	{
		UuMatter._cache = new ();
		UuMatter._resolver = new ()
		{
			[typeof(UuSettings)] = () => UuSettings.Instance.Value,
			[typeof(ILogger)] = () =>
			{
				var settings = UuSettings.Instance.Value;
				if(settings.Root().GetSection(_loggerSectionName).Exists() is false)
				{
					throw new UuException
					(
						$"UU matter can't be configured. " +
						$"Settings for logger don't exist in application settings. " +
						$"Please, ensure \"{_loggerSectionName}\" section " +
						$"exists in application settings with proper configuration parameters."
					);
				}

				return new LoggerConfiguration().ReadFrom.Configuration
				(
					configuration: settings.Root(),
					readerOptions: new () { SectionName = _loggerSectionName }
				)
				.CreateLogger();
			}
		};
	}

	/// <summary>
	/// Retrieves an UU matter of the specified type.
	/// </summary>
	/// <typeparam name="T">Type of the UU matter.</typeparam>
	public static T OfType<T>()
	{
		return (T)UuMatter.OfType(typeof(T));
	}

	/// <summary>
	/// Retrieves an UU matter of the specified type.
	/// </summary>
	/// <param name="type">Type of the UU matter.</param>
	public static object OfType(Type type)
	{
		if (UuMatter._cache.TryGetValue(type, out var instance))
		{
			return instance;
		}

		if (UuMatter._resolver.TryGetValue(type, out var resolver))
		{
			var value = resolver.Invoke();
			UuMatter._cache.TryAdd(type, value);
			return value;
		}

		throw new UuException
		(
			$"Instance of type {nameof(type)} can't be obtained. " +
			$"The type is registered in the UU matter container."
		);
	}
}