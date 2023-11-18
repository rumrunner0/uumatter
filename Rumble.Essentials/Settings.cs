using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Rumble.Essentials;

/// <summary>
/// Wrapper of the application settings.
/// </summary>
public sealed class Settings
{
	///
	/// <inheritdoc cref="IConfigurationRoot" />
	///
	private readonly IConfigurationRoot _root;

	/// <summary>
	/// <see cref="Lazy{T}" /> singleton instance.
	/// </summary>
	public static Lazy<Settings> Instance { get; private set; }

	///
	/// <inheritdoc cref="Settings" />
	///
	static Settings() => Settings.Instance = new (() => new (), LazyThreadSafetyMode.ExecutionAndPublication);

	///
	/// <inheritdoc cref="Settings" />
	///
	private Settings() => this._root = Settings.FetchRoot();

	/// <summary>
	/// Application configuration root.
	/// </summary>
	/// <returns>Application configuration root</returns>
	/// <exception cref="ApplicationException">Thrown if application configuration root has not been configured</exception>
	public IConfigurationRoot Root() => this._root;

	/// <summary>
	/// Value of the application settings item by its <paramref name="key"/>.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <returns>Value.</returns>
	public string? Value(Key key) => this._root[key];

	/// <summary>
	/// Value of the application settings item by its <paramref name="key"/>.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <typeparam name="TValue">Type of the value.</typeparam>
	/// <returns>Value.</returns>
	public TValue? Value<TValue>(Key key) => (TValue?)TypeDescriptor.GetConverter(typeof(TValue))?.ConvertFrom(this._root[key] ?? string.Empty);

	/// <summary>
	/// Fetches and saves an application configuration root.
	/// </summary>
	/// <returns>Application configuration root.</returns>
	private static IConfigurationRoot FetchRoot()
	{
		var environment = Environment
			.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
			?? Environments.Production;

		return new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile(path: $"appsettings.json", optional: false, reloadOnChange: true)
			.AddJsonFile(path: $"appsettings.{environment}.json", optional: false, reloadOnChange: true)
			.AddEnvironmentVariables()
			.Build();
	}

	/// <summary>
	/// Key of the application settings item.
	/// </summary>
	public sealed class Key
	{
		/// <summary>
		/// <see cref="string"/> representation of the key.
		/// </summary>
		private readonly string _value;

		///
		/// <inheritdoc cref="Key" />
		///
		/// <param name="value"><see cref="string"/> representation of the key.</param>
		public Key(string value) => this._value = value;

		/// <summary>
		/// Operator that implicitly converts <see cref="Key"/> to its <see cref="string"/> representation.
		/// </summary>
		/// <param name="source">The <see cref="Key"/>.</param>
		/// <returns><see cref="string"/> representation of the <see cref="Key"/>.</returns>
		public static implicit operator string(Key source) => source._value;
	}
}