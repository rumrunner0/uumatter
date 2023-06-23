using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Serilog;

namespace Rum.Essentials;

/// <summary>
/// Application essentials.
/// </summary>
public static class Essential
{
	/// <summary>
	/// Lock for new build.
	/// </summary>
	private static readonly object _newBuildLock;

	/// <summary>
	/// Cached essentials.
	/// </summary>
	private static readonly IDictionary<Type, object> _cache;

	/// <summary>
	/// Constructor of the class.
	/// </summary>
	static Essential()
	{
		Essential._newBuildLock = new ();
		Essential._cache = new ConcurrentDictionary<Type, object>();

		foreach(var (type, instance) in Essential.NewBuild())
		{
			Essential._cache.Add(type, instance);
		}
	}

	/// <summary>
	/// Application essentials bundle.
	/// </summary>
	/// <returns>Application essentials bundle</returns>
	public static (Settings Settings, ILogger Logger) Bundle(bool isRebuildRequired = false)
	{
		if(isRebuildRequired || Essential._cache.IsEmpty())
		{
			lock(Essential._newBuildLock)
			{
				Essential._cache.Clear();
				foreach(var (type, instance) in Essential.NewBuild())
				{
					Essential._cache.Add(type, instance);
				}
			}
		}

		return
		(
			(Settings)Essential._cache[typeof(Settings)],
			(ILogger)Essential._cache[typeof(ILogger)]
		);
	}

	/// <summary>
	/// Bundle entry of requested type.
	/// </summary>
	/// <typeparam name="TInstance">Type of the requested bundle entry</typeparam>
	/// <returns>Bundle entry of the requested type</returns>
	public static TInstance OfType<TInstance>()
	where TInstance : class
	{
		if(Essential._cache.TryGetValue(typeof(TInstance), out var instance) is false)
		{
			throw new ApplicationException
			(
				$"Instance of type {typeof(TInstance)} can't be obtained." +
				$"No such type is registered in the application bundle."
			);
		}

		return (TInstance)instance;
	}

	/// <summary>
	/// New build of application essentials.
	/// </summary>
	/// <returns>New build of application essentials</returns>
	/// <exception cref="ApplicationException">Thrown if the build can't be created for any reason</exception>
	private static IDictionary<Type, object> NewBuild()
	{
		var settings = new Settings();

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

		var logger = new LoggerConfiguration().ReadFrom.Configuration
		(
			configuration: settings.Root(),
			readerOptions: new () { SectionName = LOGGER_SETTINGS_SECTION_NAME }
		).CreateLogger();

		return new Dictionary<Type, object>()
		{
			[typeof(Settings)] = settings,
			[typeof(ILogger)] = logger
		};
	}
}