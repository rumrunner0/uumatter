using System;

namespace Rumrunner0.UuMatter.Console;

/// <summary>
/// Error that is related to the UU matter.
/// </summary>
internal sealed class UuException : Exception
{
	///
	/// <inheritdoc cref="UuException" />
	///
	internal UuException(string message) : base(message) { /* Empty. */  }

	///
	/// <inheritdoc cref="UuException" />
	///
	internal UuException(string? message, Exception? innerException) : base(message, innerException) { /* Empty. */ }
}