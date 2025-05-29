using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Rumrunner0.UuMatter.Cli;

/// <summary>
/// Wrapper of the Universal Usable Settings.
/// </summary>
public sealed class UuSettings
{
	/// <summary>
	/// <see cref="Lazy{T}" /> singleton instance.
	/// </summary>
	public static Lazy<UuSettings> Instance { get; private set; }

	///
	/// <inheritdoc cref="UuSettings" />
	///
	static UuSettings() => UuSettings.Instance = new (() => new (), LazyThreadSafetyMode.ExecutionAndPublication);

	///
	/// <inheritdoc cref="IConfigurationRoot" />
	///
	private readonly IConfigurationRoot _root;

	///
	/// <inheritdoc cref="UuSettings" />
	///
	private UuSettings() => this._root = UuSettings.BuildRoot();

	/// <summary>
	/// Application configuration root.
	/// </summary>
	/// <returns>Application configuration root.</returns>
	public IConfigurationRoot Root() => this._root;

	/// <summary>
	/// Value of the UU settings item by its <paramref name="key"/>.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <returns>Value.</returns>
	public string? Value(Key key) => this._root[key];

	/// <summary>
	/// Value of the UU settings item by its <paramref name="key"/>.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <typeparam name="TValue">Type of the value.</typeparam>
	/// <returns>Value.</returns>
	public TValue? Value<TValue>(Key key) => (TValue?)TypeDescriptor.GetConverter(typeof(TValue))?.ConvertFrom(this._root[key] ?? string.Empty);

	/// <summary>
	/// Builds an application configuration root.
	/// </summary>
	/// <returns>Application configuration root.</returns>
	private static IConfigurationRoot BuildRoot()
	{
		var environment = Environment
			.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
			?? Environments.Production;

		return new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile(path: $"appsettings.json", optional: false, reloadOnChange: true)
			.AddJsonFile(path: $"appsettings.{environment}.json", optional: true, reloadOnChange: true)
			.AddEnvironmentVariables()
			.Build();
	}

	/// <summary>
	/// Key of the UU settings item.
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