using System;
using System.ComponentModel;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Rum.Essentials;

/// <summary>
/// Wrapper for application settings.
/// </summary>
public sealed class Settings
{
	/// <summary>
	/// Application settings root.
	/// </summary>
	private readonly IConfigurationRoot _root;

	/// <summary>
	/// Constructor of the class.
	/// </summary>
	internal Settings()
	{
		this._root = FetchRoot();
	}

	/// <summary>
	/// Application configuration root.
	/// </summary>
	/// <returns>Application configuration root</returns>
	/// <exception cref="ApplicationException">Thrown if application configuration root has not been configured</exception>
	public IConfigurationRoot Root()
	{
		if(this._root is null)
		{
			throw new NullReferenceException(message: "Application configuration root has not been configured.");
		}

		return this._root;
	}

	/// <summary>
	/// Value of the application settings element by its <paramref name="key"/>.
	/// </summary>
	/// <param name="key">Key of the application settings element</param>
	/// <returns>Value of the application settings element</returns>
	public string? Value(Key key)
	{
		return this._root[key];
	}

	/// <summary>
	/// Value of the application settings element by its <paramref name="key"/>.
	/// </summary>
	/// <param name="key">Key of the application settings element</param>
	/// <typeparam name="TValue">Type of the value of the application settings element</typeparam>
	/// <returns>Value of the application settings element</returns>
	public TValue? Value<TValue>(Key key)
	{
		return (TValue?)TypeDescriptor.GetConverter(typeof(TValue))?.ConvertFrom(this._root[key] ?? string.Empty);
	}

	/// <summary>
	/// Fetches and saves application configuration root.
	/// </summary>
	/// <returns>Fetched application configuration root</returns>
	private static IConfigurationRoot FetchRoot()
	{
		var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? Environments.Production;

		return new ConfigurationBuilder()
			.SetBasePath(basePath: Directory.GetCurrentDirectory())
			.AddJsonFile(path: $"appsettings.json", optional: false, reloadOnChange: true)
			.AddJsonFile(path: $"appsettings.{environment}.json", optional: false, reloadOnChange: true)
			.AddEnvironmentVariables()
			.Build();
	}

	/// <summary>
	/// Key of the application settings element.
	/// </summary>
	public sealed class Key
	{
		/// <summary>
		/// <see cref="string"/> representation of the key.
		/// </summary>
		private readonly string _value;

		/// <summary>
		/// Constructor of the instance.
		/// </summary>
		/// <param name="value"><see cref="string"/> representation of the key</param>
		public Key(string value)
		{
			this._value = value;
		}

		/// <summary>
		/// Operator that implicitly converts <see cref="Key"/> to its <see cref="string"/> representation.
		/// </summary>
		/// <param name="source">The <see cref="Key"/></param>
		/// <returns><see cref="string"/> representation of the <see cref="Key"/></returns>
		public static implicit operator string(Key source)
		{
			return source._value;
		}
	}
}