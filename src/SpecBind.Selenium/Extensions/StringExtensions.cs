// <copyright file="StringExtensions.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.Extensions
{
    using System;
    using System.Globalization;

    /// <summary>
    /// String Extensions.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Converts an IntPtr to a hexadecimal string.
        /// </summary>
        /// <param name="value">The IntPtr.</param>
        /// <returns>A hexadecimal string.</returns>
        public static string ToHex(this IntPtr value)
        {
            return value.ToInt32().ToString("x");
        }

        /// <summary>
        /// Converts a hexadecimal string to an integer.
        /// </summary>
        /// <param name="value">The hexadecimal string.</param>
        /// <returns>The integer.</returns>
        public static int FromHex(this string value)
        {
            // strip the leading 0x
            if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(2);
            }

            return int.Parse(value, NumberStyles.HexNumber);
        }
    }
}
