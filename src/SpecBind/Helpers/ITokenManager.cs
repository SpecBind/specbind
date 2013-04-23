// <copyright file="ITokenManager.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.Helpers
{
	/// <summary>
	/// A token manager for getting or setting tokens.
	/// </summary>
	public interface ITokenManager
	{
		/// <summary>
		/// Gets the token from the field if it exists and is on the context.
		/// </summary>
		/// <param name="fieldValue">The field value.</param>
		/// <returns>The original value if not a token; otherwise the parsed token.</returns>
		string GetToken(string fieldValue);

		/// <summary>
		/// Gets the token by key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>The token value; otherwise <c>null</c>.</returns>
		string GetTokenByKey(string key);

		/// <summary>
		/// Parses a token if it exists and sets the value on the context.
		/// </summary>
		/// <param name="fieldValue">The field value.</param>
		/// <returns>The original value if not a token; otherwise the parsed token.</returns>
		string SetToken(string fieldValue);

		/// <summary>
		/// Sets the token into the context.
		/// </summary>
		/// <param name="tokenName">Name of the token.</param>
		/// <param name="value">The value.</param>
		void SetToken(string tokenName, string value);
	}
}