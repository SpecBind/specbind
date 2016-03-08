// <copyright file="TableFormater.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Helpers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	/// <summary>
	/// Assists in printing the results as a specflow table.
	/// </summary>
	/// <typeparam name="TItem">The type of the item to be printed.</typeparam>
	internal class TableFormater<TItem>
	{
		private readonly List<IColumnInformation> columns;

		private bool excludeIfNoRows;

		/// <summary>
		/// Initializes a new instance of the <see cref="TableFormater{TItem}"/> class.
		/// </summary>
		public TableFormater()
		{
			this.columns = new List<IColumnInformation>(3);
		}

		#region IColumnInformation interface

		/// <summary>
		/// Represents the column information so the concrete type can hold cell types.
		/// </summary>
		private interface IColumnInformation
		{
			/// <summary>
			/// Gets the header.
			/// </summary>
			/// <value>The header.</value>
			string Header { get; }

			/// <summary>
			/// Gets the index.
			/// </summary>
			/// <value>The index.</value>
			int Index { get; }

			/// <summary>
			/// Gets the length of the max.
			/// </summary>
			/// <value>The length of the max.</value>
			int MaxLength { get; }

			/// <summary>
			/// Adds a cell for column.
			/// </summary>
			/// <param name="item">The item.</param>
			/// <param name="formatter">The function used to format the cell value.</param>
			void AddCellForItem(TItem item, Func<string, bool, string, string> formatter);

			/// <summary>
			/// Gets the cell value.
			/// </summary>
			/// <param name="rowIndex">Index of the row.</param>
			/// <returns>The value of the cell.</returns>
			string GetCellValue(int rowIndex);
		}

		#endregion

		/// <summary>
		/// Gets the column count.
		/// </summary>
		/// <value>The column count.</value>
		public int ColumnCount
		{
			get
			{
				return this.columns.Count;
			}
		}

		/// <summary>
		/// Excludes the printing of the table if no rows exist..
		/// </summary>
		/// <returns>The table formatter class for additional configuration.</returns>
		public TableFormater<TItem> ExcludePrintingIfNoRows()
		{
			this.excludeIfNoRows = true;
			return this;
		}

		/// <summary>
		/// Adds the column.
		/// </summary>
		/// <typeparam name="TField">The type of the T field.</typeparam>
		/// <param name="header">The header.</param>
		/// <param name="cellSelector">The cell selector.</param>
		/// <param name="valueSelector">The value selector.</param>
		/// <param name="validationSelector">The validation selector to indicate if the cell is valid and it's actual value otherwise.</param>
		/// <param name="index">The index.</param>
		/// <returns>The table formatter class for additional configuration.</returns>
		/// <exception cref="System.ArgumentNullException">cellSelector
		/// or
		/// valueSelector</exception>
		public TableFormater<TItem> AddColumn<TField>(
													string header,
													Func<TItem, TField> cellSelector,
													Func<TField, string> valueSelector,
													Func<TField, Tuple<bool, string>> validationSelector = null,
													int? index = null)
		{
			if (cellSelector == null)
			{
				throw new ArgumentNullException("cellSelector");
			}

			if (valueSelector == null)
			{
				throw new ArgumentNullException("valueSelector");
			}

			this.columns.Add(new ColumnInformation<TField>
			                 {
								 CellSelector = cellSelector,
				                 Header = header,
								 Index = index.GetValueOrDefault(this.columns.Count),
								 ValueSelector = valueSelector,
								 ValidationSelector = validationSelector
			                 });
			return this;
		}

		/// <summary>
		/// Creates the comparison table.
		/// </summary>
		/// <param name="items">The items to convert into a table.</param>
		/// <returns>The formatted comparison table.</returns>
		public string CreateTable(IEnumerable<TItem> items)
		{
			var rowCount = this.LoadCellData(items);

			if (this.excludeIfNoRows && rowCount == 0)
			{
				return null;
			}

			var sortedColumns = this.columns.OrderBy(c => c.Index).ToList();
			var stringBuilder = new StringBuilder();

			stringBuilder.Append(this.PrintTableItem(TableItemType.TableStart, false));

			for (var index = -1; index < rowCount; index++)
			{
				var isHeader = index < 0;
				stringBuilder.Append(this.PrintTableItem(TableItemType.RowStart, isHeader));

				foreach (var column in sortedColumns)
				{
					stringBuilder.Append(this.PrintTableItem(TableItemType.CellStart, isHeader))
								 .Append(this.PrintCell(column.MaxLength, column.GetCellValue(index), isHeader))
								 .Append(this.PrintTableItem(TableItemType.CellEnd, isHeader));
				}

				stringBuilder.Append(this.PrintTableItem(TableItemType.RowEnd, isHeader));
			}

			stringBuilder.Append(this.PrintTableItem(TableItemType.TableEnd, false));

			return stringBuilder.ToString();
		}

		/// <summary>
		/// Gets the formatted cell value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="isValid">if set to <c>true</c> the cell data is valid.</param>
		/// <param name="actualValue">The actual value.</param>
		/// <returns>The cell value formatted as a string.</returns>
		protected virtual string GetFormattedCell(string value, bool isValid, string actualValue)
		{
			const string NullPrintString = "<EMPTY>";

			if (string.IsNullOrEmpty(value))
			{
				value = NullPrintString;
			}

			if (string.IsNullOrEmpty(actualValue) && !isValid)
			{
				actualValue = NullPrintString;
			}

			return isValid ? value : string.Format("{0} [{1}]", value, actualValue);
		}

		/// <summary>
		/// Prints the cell value including any additional padding or spacing.
		/// </summary>
		/// <param name="maxLength">Length of the max.</param>
		/// <param name="value">The value.</param>
		/// <param name="isHeader">if set to <c>true</c> this is a header cell.</param>
		/// <returns>The formatted cell value with padding and formatting.</returns>
		protected virtual string PrintCell(int maxLength, string value, bool isHeader)
		{
			var paddingTotal = maxLength - ((value != null) ? value.Length : 0);
			var padding = (paddingTotal > 0) ? new string(' ', paddingTotal) : string.Empty;

			return string.Format("{0}{1}", value, padding);
		}

		/// <summary>
		/// Prints the table item.
		/// </summary>
		/// <param name="tableItemType">Type of the table item.</param>
		/// <param name="isHeader">if set to <c>true</c> this is the header row.</param>
		/// <returns>The formatting for the cell.</returns>
		protected virtual string PrintTableItem(TableItemType tableItemType, bool isHeader)
		{
			switch (tableItemType)
			{
				case TableItemType.RowStart:
					return isHeader ? string.Empty : Environment.NewLine;
				case TableItemType.RowEnd:
					return "|";
				case TableItemType.CellStart:
					return "| ";
				case TableItemType.CellEnd:
					return " ";
				default:
					return string.Empty;
			}
		}

		/// <summary>
		/// Loads the cells into each column.
		/// </summary>
		/// <param name="itemList">The item list.</param>
		/// <returns>The number of rows loaded.</returns>
		private int LoadCellData(IEnumerable<TItem> itemList)
		{
			var rowCount = 0;
			foreach (var listItem in itemList)
			{
				var item = listItem;
				this.columns.AsParallel().ForAll(c => c.AddCellForItem(item, this.GetFormattedCell));
				rowCount++;
			}

			return rowCount;
		}

		#region ColumnInformation class

		/// <summary>
		/// Holds the processing column information for the cell.
		/// </summary>
		/// <typeparam name="TField">The type of the field.</typeparam>
		private class ColumnInformation<TField> : IColumnInformation
		{
			private readonly List<string> cells;
			private readonly Lazy<int> maxLength;

			/// <summary>
			/// Initializes a new instance of the <see cref="ColumnInformation{TField}"/> class.
			/// </summary>
			public ColumnInformation()
			{
				this.cells = new List<string>();
				this.maxLength = new Lazy<int>(this.GetMaxLength);
			}

			/// <summary>
			/// Sets the cell selector.
			/// </summary>
			/// <value>The cell selector.</value>
			public Func<TItem, TField> CellSelector { private get; set; }

			/// <summary>
			/// Gets or sets the header.
			/// </summary>
			/// <value>The header.</value>
			public string Header { get; set; }

			/// <summary>
			/// Gets or sets the index.
			/// </summary>
			/// <value>The index.</value>
			public int Index { get; set; }

			/// <summary>
			/// Gets the maximum length of the column content.
			/// </summary>
			/// <value>The maximum length of the column content.</value>
			public int MaxLength
			{
				get
				{
					return this.maxLength.Value;
				}
			}

			/// <summary>
			/// Sets the validation selector.
			/// </summary>
			/// <value>The validation selector.</value>
			public Func<TField, Tuple<bool, string>> ValidationSelector { private get; set; }

			/// <summary>
			/// Sets the value selector.
			/// </summary>
			/// <value>The value selector.</value>
			public Func<TField, string> ValueSelector { private get; set; }

			/// <summary>
			/// Adds a cell for column.
			/// </summary>
			/// <param name="item">The item.</param>
			/// <param name="formatter">The function used to format the cell value.</param>
			public void AddCellForItem(TItem item, Func<string, bool, string, string> formatter)
			{
				var cellValue = this.CellSelector(item);

				var cellData = this.ValueSelector(cellValue);
				var isValid = true;
				string actualValue = null;

				if (this.ValidationSelector != null)
				{
					var validationResult = this.ValidationSelector(cellValue);
					if (validationResult != null)
					{
						isValid = validationResult.Item1;
						actualValue = validationResult.Item2;
					}
				}

				var formattedValue = formatter(cellData, isValid, actualValue);
				this.cells.Add(formattedValue);
			}

			/// <summary>
			/// Gets the cell value.
			/// </summary>
			/// <param name="rowIndex">Index of the row.</param>
			/// <returns>The value of the cell.</returns>
			public string GetCellValue(int rowIndex)
			{
				if (rowIndex < 0)
				{
					return this.Header;
				}

				return rowIndex < this.cells.Count ? this.cells[rowIndex] : null;
			}

			/// <summary>
			/// Gets the max length of the cells.
			/// </summary>
			/// <returns>The maximum length of the cells.</returns>
			private int GetMaxLength()
			{
				var headerLength = (this.Header != null) ? this.Header.Length : 0;
				return Math.Max(headerLength, this.cells.Count > 0 ? this.cells.Max(cell => cell != null ? cell.Length : 0) : 0);
			}
		}

		#endregion
	}
}