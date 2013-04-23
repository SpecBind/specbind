// <copyright file="TokenManager.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.Helpers
{
	using System;
	using System.Globalization;

	/// <summary>
	/// A token manager class that will parse out tokens and save or get them from the context.
	/// </summary>
	public class TokenManager : ITokenManager
	{
		private static readonly TokenManager Manager = new TokenManager(new ScenarioContextHelper());
		private readonly IScenarioContextHelper context;

		/// <summary>
		/// Initializes a new instance of the <see cref="TokenManager" /> class.
		/// </summary>
		/// <param name="context">The context.</param>
		public TokenManager(IScenarioContextHelper context)
		{
			this.context = context;
		}

		/// <summary>
		/// Gets the current token manager.
		/// </summary>
		/// <value>
		/// The current token manager.
		/// </value>
		public static ITokenManager Current
		{
			get
			{
				return Manager;
			}
		}

		/// <summary>
		/// Gets the token from the field if it exists and is on the context.
		/// </summary>
		/// <param name="fieldValue">The field value.</param>
		/// <returns>The original value if not a token; otherwise the parsed token.</returns>
		public string GetToken(string fieldValue)
		{
			string contextValue = null;
			TokenData tokenData;
			if (TryParseToken(fieldValue, out tokenData))
			{
				contextValue = this.GetTokenByKey(tokenData.Name);
			}

			return contextValue ?? fieldValue;
		}

		/// <summary>
		/// Gets the token by key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>The token value; otherwise <c>null</c>.</returns>
		public string GetTokenByKey(string key)
		{
			return this.context.GetValue<string>(GetTokenName(key));
		}

		/// <summary>
		/// Parses a token if it exists and sets the value on the context.
		/// </summary>
		/// <param name="fieldValue">The field value.</param>
		/// <returns>The original value if not a token; otherwise the parsed token.</returns>
		public string SetToken(string fieldValue)
		{
			TokenData tokenData;
			if (!TryParseToken(fieldValue, out tokenData))
			{
				return fieldValue;
			}

			var tokenName = GetTokenName(tokenData.Name);

			if (string.IsNullOrWhiteSpace(tokenData.Value))
			{
				// Try to get the value from the context if no value is provided
				return this.context.GetValue<string>(tokenName);
			}

			this.context.SetValue(tokenData.Value, tokenName);

			return tokenData.Value;
		}

		/// <summary>
		/// Sets the token into the context.
		/// </summary>
		/// <param name="tokenName">Name of the token.</param>
		/// <param name="value">The value.</param>
		public void SetToken(string tokenName, string value)
		{
			if (string.IsNullOrWhiteSpace(tokenName))
			{
				throw new ArgumentNullException("tokenName");
			}

			this.context.SetValue(value, GetTokenName(tokenName));
		}

		/// <summary>
		/// Gets the name of the token.
		/// </summary>
		/// <param name="baseName">Name of the base.</param>
		/// <returns>The qualified token name.</returns>
		private static string GetTokenName(string baseName)
		{
			return string.Format("TOKEN:{0}", baseName.ToUpperInvariant().Trim());
		}

		/// <summary>
		/// Gets the random string.
		/// </summary>
		/// <param name="length">The length.</param>
		/// <returns>
		/// A random string.
		/// </returns>
		private static string GetRandomString(int length)
		{
			var generator = new Random();
			var randomChars = new char[length];
			
			for (var i = 0; i < length; i++)
			{
				randomChars[i] = Convert.ToChar(generator.Next(32, 127));
			}
			
			return new string(randomChars);
		}

		/// <summary>
		/// Tries to parse the field as a token.
		/// </summary>
		/// <param name="fieldValue">The field value.</param>
		/// <param name="data">The data.</param>
		/// <returns><c>true</c> if it is a token; otherwise <c>false</c>.</returns>
		private static bool TryParseToken(string fieldValue, out TokenData data)
		{
			fieldValue = (fieldValue != null) ? fieldValue.Trim() : null;

			data = null;
			if (string.IsNullOrWhiteSpace(fieldValue) || !fieldValue.StartsWith("{") || !fieldValue.EndsWith("}"))
			{
				return false;
			}

			var innerToken = fieldValue.Substring(1, fieldValue.Length - 2).Trim();
			var parts = innerToken.Split(new[] { ':' }, 2);
			
			data = new TokenData { Name = parts[0] };
			if (parts.Length == 2)
			{
				var lengthPart = parts[1].Split(new[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
				var operationType = lengthPart[0].ToLowerInvariant().Trim();

				switch (operationType)
				{
					case "randomint":
						data.Value = new Random().Next().ToString(CultureInfo.InvariantCulture);
						break;
					case "randomguid":
						data.Value = Guid.NewGuid().ToString();
						break;
					case "randomstring":
						int parseInt;
						var length = (lengthPart.Length == 2 && int.TryParse(lengthPart[1], out parseInt)) ? parseInt : 30;
						data.Value = GetRandomString(length);
						break;
					default:
						data.Value = parts[1];
						break;
				}
			}
			
			return true;
		}

		/// <summary>
		/// A class to hold any parsed data.
		/// </summary>
		private class TokenData
		{
			/// <summary>
			/// Gets or sets the name.
			/// </summary>
			/// <value>
			/// The name.
			/// </value>
			public string Name { get; set; }

			/// <summary>
			/// Gets or sets the value.
			/// </summary>
			/// <value>
			/// The value.
			/// </value>
			public string Value { get; set; }
		}
	}
}