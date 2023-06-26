using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Serilog;

namespace Rumble.Essentials;

/// <summary>
/// Application essentials.
/// </summary>
public static class Essential
{
	/// <summary>
	/// Cached essentials.
	/// </summary>
	private static readonly IDictionary<Type, object> _cache;

	/// <summary>
	///
	/// </summary>
	private static readonly IDictionary<Type, Func<object>> _resolver;

	/// <summary>
	/// Constructor of the class.
	/// </summary>
	static Essential()
	{
		Essential._cache = new ConcurrentDictionary<Type, object>();
		Essential._resolver = new ConcurrentDictionary<Type, Func<object>>()
		{
			[typeof(Settings)] = Settings.Instance,
			[typeof(ILogger)] = () =>
			{
				var settings = Settings.Instance();
				const string LOGGER_SETTINGS_SECTION_NAME = "Serilog";
				if(settings.Root().GetSection(key: LOGGER_SETTINGS_SECTION_NAME).DoesNotExist())
				{
					throw new ApplicationException
					(
						$"Application essentials can't be configured. " +
						$"Settings for logger don't exist in application settings. " +
						$"Please, ensure \"{LOGGER_SETTINGS_SECTION_NAME}\" section " +
						$"exists in application settings with proper configuration parameters."
					);
				}

				return new LoggerConfiguration().ReadFrom.Configuration
				(
					configuration: settings.Root(),
					readerOptions: new () { SectionName = LOGGER_SETTINGS_SECTION_NAME }
				).CreateLogger();
			}
		};
	}

	/// <summary>
	/// Application essentials entry.
	/// </summary>
	/// <typeparam name="T">Type of the application essentials entry</typeparam>
	public static T OfType<T>()
	{
		return (T)Essential.OfType(typeof(T));
	}

	/// <summary>
	/// Application essentials entry.
	/// </summary>
	/// <param name="type">Type of the application essentials entry</param>
	public static object OfType(Type type)
	{
		if(Essential._cache.TryGetValue(type, out var instance))
		{
			return instance;
		}

		if(Essential._resolver.TryGetValue(type, out var resolver))
		{
			var value = resolver.Invoke();
			Essential._cache.Add(key: type, value);
			return value;
		}

		throw new ApplicationException
		(
			$"Instance of type {nameof(type)} can't be obtained." +
			$"No such type is registered in the application essentials bundle."
		);
	}
}