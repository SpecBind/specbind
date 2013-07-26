// <copyright file="TableItemType.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Helpers
{
	/// <summary>
	/// Enumerated the type of printable item to format.
	/// </summary>
	public enum TableItemType
	{
		/// <summary>
		/// The table start
		/// </summary>
		TableStart = 0,

		/// <summary>
		/// The table end
		/// </summary>
		TableEnd = 1,

		/// <summary>
		/// The row start
		/// </summary>
		RowStart = 2,

		/// <summary>
		/// The row end
		/// </summary>
		RowEnd = 3,

		/// <summary>
		/// The cell start
		/// </summary>
		CellStart = 4,

		/// <summary>
		/// The cell end
		/// </summary>
		CellEnd = 5
	}
}
