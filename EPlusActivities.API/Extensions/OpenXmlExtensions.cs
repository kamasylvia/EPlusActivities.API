using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace EPlusActivities.API.Extensions
{
    public static class OpenXmlExtensions
    {
        public static void InsertCell(
            this WorksheetPart workSheetPart,
            string cellValue,
            (string, uint) coordinate
        )
        {
            var cell = workSheetPart.InsertCell(coordinate);
            cell.CellValue = new CellValue(cellValue);
            cell.DataType = new EnumValue<CellValues>(CellValues.String);
        }

        public static void InsertCell(
            this WorksheetPart workSheetPart,
            int cellValue,
            (string, uint) coordinate
        )
        {
            var cell = workSheetPart.InsertCell(coordinate);
            cell.CellValue = new CellValue(cellValue);
            cell.DataType = new EnumValue<CellValues>(CellValues.Number);
        }

        public static void InsertCell(
            this WorksheetPart workSheetPart,
            decimal cellValue,
            (string, uint) coordinate
        )
        {
            var cell = workSheetPart.InsertCell(coordinate);
            cell.CellValue = new CellValue(cellValue);
            cell.DataType = new EnumValue<CellValues>(CellValues.Number);
        }

        public static void InsertCell(
            this WorksheetPart workSheetPart,
            double cellValue,
            (string, uint) coordinate
        )
        {
            var cell = workSheetPart.InsertCell(coordinate);
            cell.CellValue = new CellValue(cellValue);
            cell.DataType = new EnumValue<CellValues>(CellValues.Number);
        }

        public static void InsertCell(
            this WorksheetPart workSheetPart,
            bool cellValue,
            (string, uint) coordinate
        )
        {
            var cell = workSheetPart.InsertCell(coordinate);
            cell.CellValue = new CellValue(cellValue);
            cell.DataType = new EnumValue<CellValues>(CellValues.Number);
        }

        public static void InsertCell(
            this WorksheetPart workSheetPart,
            DateTime cellValue,
            (string, uint) coordinate
        )
        {
            var cell = workSheetPart.InsertCell(coordinate);
            cell.CellValue = new CellValue(cellValue);
            cell.DataType = new EnumValue<CellValues>(CellValues.Date);
        }

        public static void InsertCell(
            this WorksheetPart workSheetPart,
            DateTimeOffset cellValue,
            (string, uint) coordinate
        )
        {
            var cell = workSheetPart.InsertCell(coordinate);
            cell.CellValue = new CellValue(cellValue);
            cell.DataType = new EnumValue<CellValues>(CellValues.Date);
        }

        // Given a column name, a row index, and a WorksheetPart, inserts a cell into the worksheet.

        // If the cell already exists, returns it.

        public static Cell InsertCell(this WorksheetPart worksheetPart, (string, uint) coordinate)
        {
            var (columnName, rowIndex) = coordinate;
            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
            string cellReference = columnName + rowIndex;

            // If the worksheet does not contain a row with the specified row index, insert one.
            Row row;
            if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
            {
                row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
            }
            else
            {
                row = new Row() { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            // If there is not a cell with the specified column name, insert one.

            if (
                row.Elements<Cell>()
                    .Where(c => c.CellReference.Value == columnName + rowIndex)
                    .Count() > 0
            )
            {
                return row.Elements<Cell>()
                    .Where(c => c.CellReference.Value == cellReference)
                    .First();
            }
            else
            {
                // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
                Cell refCell = null;
                foreach (Cell cell in row.Elements<Cell>())
                {
                    if (cell.CellReference.Value.Length == cellReference.Length)
                    {
                        if (string.Compare(cell.CellReference.Value, cellReference, true) > 0)
                        {
                            refCell = cell;
                            break;
                        }
                    }
                }

                Cell newCell = new Cell() { CellReference = cellReference };
                row.InsertBefore(newCell, refCell);

                worksheet.Save();
                return newCell;
            }
        }
    }
}
