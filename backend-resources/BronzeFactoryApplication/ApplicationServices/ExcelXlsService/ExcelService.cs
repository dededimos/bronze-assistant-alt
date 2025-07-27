using BronzeRulesPricelistLibrary.Models.Priceables;
using BronzeRulesPricelistLibrary.Models.Priceables.CabinsPriceables;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using GlassesOrdersModels.Models;
using Microsoft.Win32;
using ReportingHelperLibrary;
using ShowerEnclosuresModelsLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BronzeFactoryApplication.ApplicationServices.ExcelXlsService.ExcelGenerationExceptions;

namespace BronzeFactoryApplication.ApplicationServices.ExcelXlsService;

public static class ExcelService
{
    /// <summary>
    /// Creates a Dialog to Save an xls File
    /// </summary>
    /// <param name="fileName">The Initial File Name Appended with Current Date</param>
    /// <returns>The FileName FULL path</returns>
    /// <exception cref="EmptyWorkBookException"></exception>
    public static string SaveXlsFile(XLWorkbook workBook,string fileName = "")
    {
        if (string.IsNullOrEmpty(fileName))
        {
            fileName = workBook.Worksheets.FirstOrDefault()?.Name ?? throw new EmptyWorkBookException();
        }
        SaveFileDialog saveDialog = new()
        {
            FileName = string.IsNullOrEmpty(fileName) ? $"ExcelFile {DateTime.Now:dd-MM-yyyy}" : $"{fileName} {DateTime.Now:dd-MM-yyyy}",
            DefaultExt = ".xlsx",
            Filter = @"Excel Document (.xlsx)|*.xlsx",
        };
        if (saveDialog.ShowDialog() is true)
        {
            workBook.SaveAs(saveDialog.FileName);
            return saveDialog.FileName;
        }
        //When dialog closes without a save
        throw new SaveCancelledException();
    }

    public static class GlassOrdersXls
    {
        /// <summary>
        /// Saves a Glasses Order to an XLS file
        /// </summary>
        /// <param name="fileName">The filename with which the File Dialog Starts</param>
        /// <param name="order">The Order to Save</param>
        /// <param name="xlsSettings">Settings of the Xls generation</param>
        /// <returns>The File Name of the Saved Xls</returns>
        /// <exception cref="Exception">When the Generation fails for any Reason</exception>
        public static string SaveGlassesOrderToXls(GlassesOrder order, XlsSettingsGlasses xlsSettings)
        {
            using (var wb = new XLWorkbook())
            {
                CreateGlassesXls(wb, order, xlsSettings);
                var fileName = SaveXlsFile(wb);
                return fileName;
            }
        }
        /// <summary>
        /// Creates an xls File from a Glasses Order
        /// </summary>
        /// <param name="wb">The Workbook object to Create the tables</param>
        /// <param name="order">The Order</param>
        /// <param name="s">Settings of xls Generation</param>
        /// <exception cref="InvalidOperationException"></exception>
        private static void CreateGlassesXls(IXLWorkbook wb, GlassesOrder order, XlsSettingsGlasses s)
        {
            var ws = wb.Worksheets.Add(s.WorksheetName);                    // Add a new WorkSheet
            int currentRow = s.FirstRowIndex;                               // Add a Row Incrementor to know which is the Current Row (Starts from the first Row)
            int glassLine = 1;                                              // Add a Glass Index for each glass Written 
            var groups = order.GetGlassRowGroups(true);     // Get The Grouped Glasses from the Glasses Order

            ApplyGeneralStylesGlassesTables(ws, s);

            #region 1. Create All the Glasses Tables

            foreach (var group in groups)
            {
                // Get the First Glass of the Group to Read the Description
                Glass firstGlass = group.FirstOrDefault()?.OrderedGlass ?? throw new InvalidOperationException("A Group of Glasses was Empty without a Glass");

                // Capture the Row that the table starts
                int tableStartingRow = currentRow;

                var tableHeader = s.GlassDescriptions.FullDescription(firstGlass, string.IsNullOrEmpty(group.First().SpecialDrawString) ? "" : $"{group.First().SpecialDrawString}**");
                // Create the Header of the Group
                CreateGlassTableHeader(ws, currentRow, s.FirstColumnIndex, s.TotalColumns, tableHeader, s.TableHeaderSettings);

                // Move one Row
                currentRow++;

                CreateGlassTableTitles(ws, currentRow, s);

                // Move one Row
                currentRow++;

                // Write all the Glasses
                foreach (var glassRow in group)
                {
                    WriteGlassRow(ws, currentRow, glassLine, s, glassRow);
                    glassLine++;
                    currentRow++;
                }

                //Capture the Last Line of the Table
                int tableLastRow = currentRow - 1;

                // Add Borders to the Table
                AddBordersToGlassTable(ws, tableStartingRow, tableLastRow, s);

                // Add One Row as Spacing between this and the Next Table
                currentRow++;
            }

            #endregion

            ws.Columns().AdjustToContents();                                // Adjust All Columns

            foreach (var row in ws.Rows())
            {
                //Adjust only the rows that have Glass Data (Avoiding Header Rows and Table Titles
                if (row.Height == s.NormalRowHeight)
                {
                    row.AdjustToContents();
                    //If the Height Changed to less than normal From the Adjustment , revert it back to normal , else leave it as is
                    row.Height = (row.Height < s.NormalRowHeight) ? s.NormalRowHeight : row.Height;
                }
            }

            CreateGeneralHeadersGlasses(ws, order.OrderId, s);              // Create the Main Header
            currentRow += 2;                                                // Move Two Rows
            double totalSqm = order.GlassRows.Sum(r => r.OrderedGlass.EffectiveSQM * r.Quantity); //Get Total Sqm
            double totalWeight = order.GlassRows.Sum(r => r.OrderedGlass.EstimatedWeightKgr * r.Quantity);
            CreateGlassesTotalsBox(ws, currentRow, order.GlassesCount, totalSqm, totalWeight, s);  // Create the Totals Box
            currentRow += 2;                                                // Move Two Rows
            CreateNotesBoxGlasses(ws, currentRow, order.Notes, s);          // Create the Notes Box

            ws.PageSetup.PageOrientation = XLPageOrientation.Landscape;     // Define Print Area as Landscape for the Columns to Fit
            ws.PageSetup.FitToPages(1, 0);                // Fit All The Columns in a Single Page
        }
        /// <summary>
        /// Creates the Header for a Glass Table
        /// </summary>
        /// <param name="ws">The Worksheet</param>
        /// <param name="row">The Row at which the header will be placed</param>
        /// <param name="column">The First Column at which the header will be placed</param>
        /// <param name="columnsNo">The Number of Columns of the Table</param>
        /// <param name="tableHeader">The Header of the Table</param>
        /// <param name="s">Settings</param>
        private static void CreateGlassTableHeader(IXLWorksheet ws, int row, int column, int columnsNo, string tableHeader, XlsGlassTableHeaderSettings s)
        {
            //Style and Set the Header
            ws.Row(row).Height = s.TableHeaderRowHeight;

            var headerCell = ws.Cell(row, column);

            headerCell.Value = tableHeader;
            headerCell.Style.Font.FontSize = s.FontSize;
            headerCell.Style.Font.Bold = s.IsFontBold;
            headerCell.Style.Alignment.Horizontal = s.HorizontalAlignment;
            headerCell.Style.Alignment.Vertical = s.VerticalAlignment;
            headerCell.Style.Font.FontColor = s.FontColor;
            headerCell.Style.Fill.SetBackgroundColor(s.BackgroundColor);

            //Merge the cells (first row - first column -----up to----- first row - last column)
            ws.Range(row, column, row, columnsNo + column).Merge();
        }
        /// <summary>
        /// Applies General Styles to the Glasses Tables
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="s"></param>
        private static void ApplyGeneralStylesGlassesTables(IXLWorksheet ws, XlsSettingsGlasses s)
        {
            //Set the Fonts
            ws.Style.Font.FontSize = s.FontSize;
            ws.Style.Font.Bold = s.IsFontBold;
            ws.Style.Font.FontName = s.FontFamily;
            ws.Style.Font.FontColor = s.FontColor;

            //Set the Initial Height of all the Rows to be Used
            ws.Rows(s.FirstRowIndex, s.TotalRows).Height = s.NormalRowHeight;

            //Set the Alignment for the Range of the Cells to Be Used
            ws.Range(s.FirstRowIndex, s.FirstColumnIndex, s.LastRowIndex, s.LastColumnIndex).Style.Alignment.Horizontal = s.HorizontalAlignment;
            ws.Range(s.FirstRowIndex, s.FirstColumnIndex, s.LastRowIndex, s.LastColumnIndex).Style.Alignment.Vertical = s.VerticalAlignment;
        }
        /// <summary>
        /// Creates the Title and SubTitles for Glass Tables
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="currentRow"></param>
        /// <param name="s"></param>
        private static void CreateGlassTableTitles(IXLWorksheet ws, int currentRow, XlsSettingsGlasses s)
        {
            //Write the Columns' Titles
            int index = 0;
            foreach (var title in s.GetColumnNames())
            {
                ws.Cell(currentRow, s.FirstColumnIndex + index).Value = title;
                //Move to next column after the First
                index++;
            }

            // Not sure yet why we need this
            ws.Row(currentRow).Height = s.TitlesRowHeight;
            var titleRange = ws.Range(currentRow, s.FirstColumnIndex, currentRow, s.TotalColumns + s.FirstColumnIndex);
            titleRange.Style.Fill.BackgroundColor = s.TitlesBackgroundColor;
            titleRange.Style.Font.FontColor = s.TitlesFontColor;
            titleRange.Style.Font.Bold = s.IsTitlesFontBold;
        }
        /// <summary>
        /// Writes a GlassRow into an excel row
        /// </summary>
        /// <param name="ws">The Worksheet</param>
        /// <param name="rowToWrite">The Row to Write the glass into</param>
        /// <param name="glassLine">The GlassLine No aka 'A/A'</param>
        /// <param name="s">settings of xls generation</param>
        /// <param name="glassRow">The GlassRow to write into the File</param>
        private static void WriteGlassRow(IXLWorksheet ws, int rowToWrite, int glassLine, XlsSettingsGlasses s, GlassOrderRow glassRow)
        {
            // Write all the Glasses Properties
            var columnIndex = s.FirstColumnIndex;
            var glass = glassRow.OrderedGlass;

            // A/A
            ws.Cell(rowToWrite, columnIndex).Value = glassLine;
            columnIndex++;

            // Glass Draw (Spaces to the front and back this way its always a string)
            var numberingLettering = string.IsNullOrEmpty(glassRow.SpecialDrawString) ? "" : $"{glassRow.SpecialDrawString}{glassRow.SpecialDrawNumber}";
            ws.Cell(rowToWrite, columnIndex).Value = $"{s.GlassDescriptions.DrawSimpleString(glass, numberingLettering)}";
            columnIndex++;

            // Length
            ws.Cell(rowToWrite, columnIndex).Value = glass.Length;
            columnIndex++;

            // Height
            ws.Cell(rowToWrite, columnIndex).Value = glass.Height;
            columnIndex++;

            // Glass Thickness
            ws.Cell(rowToWrite, columnIndex).Value = s.GlassDescriptions.SimpleThicknessString(glass);
            columnIndex++;

            // Glass Finish
            ws.Cell(rowToWrite, columnIndex).Value = s.GlassDescriptions.SimpleFinishString(glass);
            columnIndex++;

            // Step Length
            ws.Cell(rowToWrite, columnIndex).Value = glass.StepLength != 0 ? glass.StepLength : "-";
            columnIndex++;

            // Step Height
            ws.Cell(rowToWrite, columnIndex).Value = glass.StepHeight != 0 ? glass.StepHeight : "-";
            columnIndex++;

            // Quantity
            ws.Cell(rowToWrite, columnIndex).Value = glassRow.Quantity;
            columnIndex++;

            // PA0 Number
            // If Explicitly we need to set as text without the PA* prefix we append only in front a '
            ws.Cell(rowToWrite, columnIndex).Value = "PA-" + glassRow.ReferencePA0;
            columnIndex++;

            // Cabin Code Parent
            ws.Cell(rowToWrite, columnIndex).Value = glassRow.ParentCabinRow?.OrderedCabin.Code ?? "Code Not Available";
            columnIndex++;

            // Notes (Write both Cabin and Glass Notes)
            StringBuilder builder = new(glassRow.ParentCabinRow?.Notes ?? "");
            if (!string.IsNullOrEmpty(builder.ToString())) builder.Append(Environment.NewLine).Append(Environment.NewLine);
            builder.Append(glassRow.Notes);
            var notes = builder.ToString();
            ws.Cell(rowToWrite, columnIndex).Value = string.IsNullOrWhiteSpace(notes) ? "-" : notes;
            columnIndex++;

            // Cost
            ws.Cell(rowToWrite, columnIndex).Value = 0;
            ws.Cell(rowToWrite, columnIndex).Style.NumberFormat.Format = "0.00\u20AC";
            columnIndex++;

            // Sqm
            ws.Cell(rowToWrite, columnIndex).Value = glassRow.OrderedGlass.EffectiveSQM * glassRow.Quantity;
            ws.Cell(rowToWrite, columnIndex).Style.NumberFormat.Format = "0.00";
            columnIndex++;
            // Weight
            ws.Cell(rowToWrite, columnIndex).Value = glassRow.OrderedGlass.EstimatedWeightKgr * glassRow.Quantity;
            ws.Cell(rowToWrite, columnIndex).Style.NumberFormat.Format = "0.00";
        }
        /// <summary>
        /// Adds borders and Styles them to a glasses Table
        /// </summary>
        /// <param name="ws">The Worksheet</param>
        /// <param name="tableRowStart">The Row that the Table Starts At</param>
        /// <param name="lastTableRow">The Last row of the Table</param>
        /// <param name="s">Settings of xls Generation</param>
        private static void AddBordersToGlassTable(IXLWorksheet ws, int tableRowStart, int lastTableRow, XlsSettingsGlasses s)
        {
            // Always start from two rows down the Table (Pass Header and Titles)
            tableRowStart += 2;
            XlsGlassTablesSettings tableSettings = s.GlassTableSettings;

            //Change The Border Inbetween to Smoother Blue Lines Horizontally and Without Vertical Lines
            for (int i = tableRowStart; i < lastTableRow; i++) //Apply border to All except the Last Row 
            {
                for (int j = s.FirstColumnIndex; j < s.LastColumnIndex + 1; j++)
                {
                    ws.Cell(i, j).Style.Border.BottomBorder = tableSettings.HorizontalBorderThickness;
                    ws.Cell(i, j).Style.Border.BottomBorderColor = tableSettings.HorizontalBorderColor;
                    ws.Cell(i, j).Style.Border.RightBorder = tableSettings.VerticalBorderThickness;
                    ws.Cell(i, j).Style.Border.RightBorderColor = tableSettings.VerticalBorderColor;

                    //When the division of (i-Rowstart)/2 leaves a modulus , meaning every second line AFTER the first , we fill the cell color
                    if ((i - tableRowStart) % 2 != 0)
                    {
                        ws.Cell(i, j).Style.Fill.BackgroundColor = tableSettings.AlternatingTableRowBackground;
                    }

                }
            }

            //Putting Border Around All table , -2 to Include Sub Headers and General Header
            ws.Range(tableRowStart - 2, s.FirstColumnIndex, lastTableRow, s.LastColumnIndex).Style.Border.OutsideBorder = tableSettings.TablePerimetricalBorderThickness;
            ws.Range(tableRowStart - 2, s.FirstColumnIndex, lastTableRow, s.LastColumnIndex).Style.Border.OutsideBorderColor = tableSettings.TablePerimetricalBorderColor;
        }
        /// <summary>
        /// Creates the General Header for a Glasses Order xls File
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="orderNo"></param>
        /// <param name="s"></param>
        private static void CreateGeneralHeadersGlasses(IXLWorksheet ws, string orderNo, XlsSettingsGlasses s)
        {
            var headerSettings = s.GeneralHeaderSettings;
            // Add 2-Row on top to insert the Headers (the first row index from now on will be that row)
            ws.Row(s.FirstRowIndex).InsertRowsAbove(2);

            ws.Row(s.FirstRowIndex).Height = headerSettings.RowHeight;

            //All Styles Happen at the first cell and then merges with the rest
            var cell = ws.Cell(s.FirstRowIndex, s.FirstColumnIndex);
            cell.Value = $"{"lngOrder".TryTranslateKey()}: {orderNo} -- {"lngDate".TryTranslateKey()}: {DateTime.Now:dd-MM-yyyy}";
            cell.Style.Font.FontSize = headerSettings.FontSize;
            cell.Style.Font.Bold = headerSettings.IsFontBold;
            cell.Style.Alignment.Horizontal = headerSettings.HorizontalAlignment;
            cell.Style.Alignment.Vertical = headerSettings.VerticalAlignment;
            cell.Style.Font.FontColor = headerSettings.FontColor;
            cell.Style.Fill.SetBackgroundColor(headerSettings.BackgroundColor);

            //Merge the row
            ws.Range(s.FirstColumnIndex, s.FirstColumnIndex, s.FirstRowIndex, s.LastColumnIndex).Merge();
        }
        /// <summary>
        /// Creates a Box that Contains the Totals from a Glasses Order
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="rowToWriteBox"></param>
        /// <param name="totalGlasses"></param>
        /// <param name="totalSqm"></param>
        /// <param name="totalKgr"></param>
        /// <param name="s"></param>
        private static void CreateGlassesTotalsBox(IXLWorksheet ws, int rowToWriteBox, int totalGlasses, double totalSqm, double totalKgr, XlsSettingsGlasses s)
        {
            var boxSettings = s.TotalGlassesBoxSettings;
            // Write All to a Single Cell and then Merge it
            var cell = ws.Cell(rowToWriteBox, s.FirstColumnIndex);
            string totalGlassesString = $"{"lngTotalGlassesString".TryTranslateKey()}: {totalGlasses}{"lngPcs".TryTranslateKey()}";
            string totalSqmGlassesString = $"{"lngTotalGlassesSQM".TryTranslateKey()}: {totalSqm:0.00m²}";
            string totalWeightString = $"{"lngTotalGlassesKgr".TryTranslateKey()}: {totalKgr:0.00kgr}";

            cell.Value = $"{totalGlassesString} ---- {totalSqmGlassesString} ---- {totalWeightString}";
            cell.Style.Alignment.Horizontal = boxSettings.HorizontalAlignment;
            cell.Style.Alignment.Vertical = boxSettings.VerticalAlignment;
            cell.Style.Fill.SetBackgroundColor(boxSettings.BackgroundColor);
            cell.Style.Font.FontSize = boxSettings.FontSize;
            cell.Style.Font.Bold = boxSettings.IsFontBold;
            cell.Style.Font.FontColor = boxSettings.FontColor;

            //Merge
            ws.Range(rowToWriteBox, s.FirstColumnIndex, rowToWriteBox, s.LastColumnIndex).Merge();
        }
        /// <summary>
        /// Creates a Box to Contain the Notes of a Glasses Order
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="rowToWriteBox"></param>
        /// <param name="notes"></param>
        /// <param name="s"></param>
        private static void CreateNotesBoxGlasses(IXLWorksheet ws, int rowToWriteBox, string notes, XlsSettingsGlasses s)
        {
            var notesSettings = s.NotesBoxSettings;

            //Cell to Write then Merge
            var cell = ws.Cell(rowToWriteBox, s.FirstColumnIndex);
            cell.Value = $"{"lngOrderNotes".TryTranslateKey()}:{Environment.NewLine}{notes}";
            cell.Style.Alignment.Horizontal = notesSettings.HorizontalAlignment;
            cell.Style.Alignment.Vertical = notesSettings.VerticalAlignment;
            cell.Style.Fill.SetBackgroundColor(notesSettings.BackgroundColor);
            cell.Style.Font.FontSize = notesSettings.FontSize;
            cell.Style.Font.Bold = notesSettings.IsFontBold;
            cell.Style.Font.FontColor = notesSettings.FontColor;

            //Merge
            ws.Range(rowToWriteBox, s.FirstColumnIndex, rowToWriteBox + (notesSettings.NumberOfRowsForNotes - 1), s.LastColumnIndex).Merge();
        }
    }

    public static class PriceListsXls
    {
        /// <summary>
        /// Saves a Set of Cabins along with its Prices and a Discount in an XLS File
        /// </summary>
        /// <param name="priceList">The Cabin-Prices List</param>
        /// <param name="discount100">The Discount in format ex. 58 for 58%</param>
        /// <returns></returns>
        public static string SaveCabinPricingToXls(List<PriceableCabin> cabinPriceables)
        {
            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("CabinsList");
                int firstRow = 2;
                int currentRow = firstRow;
                int firstColumn = 2;

                ws.Cell(currentRow, firstColumn).Value = "lngCode".TryTranslateKey();
                ws.Cell(currentRow, firstColumn + 1).Value = "lngModel".TryTranslateKey();
                ws.Cell(currentRow, firstColumn + 2).Value = "lngSeries".TryTranslateKey();
                ws.Cell(currentRow, firstColumn + 3).Value = "lngNominalLength".TryTranslateKey() + Environment.NewLine + "(mm)";
                ws.Cell(currentRow, firstColumn + 4).Value = "lngMaxLength".TryTranslateKey() + Environment.NewLine + "(mm)";
                ws.Cell(currentRow, firstColumn + 5).Value = "lngMinLength".TryTranslateKey() + Environment.NewLine + "(mm)";
                ws.Cell(currentRow, firstColumn + 6).Value = "lngHeight".TryTranslateKey() + Environment.NewLine + "(mm)";
                ws.Cell(currentRow, firstColumn + 7).Value = "lngThickness".TryTranslateKey() + Environment.NewLine + "(mm)";
                ws.Cell(currentRow, firstColumn + 8).Value = "lngGlass".TryTranslateKey();
                ws.Cell(currentRow, firstColumn + 9).Value = "lngMetalFinish".TryTranslateKey();
                ws.Cell(currentRow, firstColumn + 10).Value = "lngOpening".TryTranslateKey() + Environment.NewLine + "(mm)";
                ws.Cell(currentRow, firstColumn + 11).Value = "lngReversible".TryTranslateKey();


                ws.Cell(currentRow, firstColumn + 12).Value = "lngStartingPrice".TryTranslateKey();
                ws.Cell(currentRow, firstColumn + 13).Value = "lngDiscountPercent".TryTranslateKey();
                ws.Cell(currentRow, firstColumn + 14).Value = "lngNetPrice".TryTranslateKey();
                ws.Cell(currentRow, firstColumn + 15).Value = "lngPhotoPath".TryTranslateKey();

                //Style the Titles
                var titleRange = ws.Range(currentRow, firstColumn, currentRow, firstColumn + 15);
                titleRange.Style.Fill.BackgroundColor = XLColor.FromArgb(68, 114, 196);
                titleRange.Style.Font.FontSize = 12;
                titleRange.Style.Font.FontColor = XLColor.White;
                titleRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                titleRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                titleRange.Style.Font.Bold = true;

                currentRow++;

                foreach (var priceable in cabinPriceables)
                {
                    ws.Cell(currentRow, firstColumn).Value = priceable.Code;

                    ws.Cell(currentRow, firstColumn + 1).Value = priceable.CabinProperties.Model?.ToString().TryTranslateKey() ?? "Undefinied Model";
                    ws.Cell(currentRow, firstColumn + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Cell(currentRow, firstColumn + 2).Value = priceable.CabinProperties.Series.ToString();
                    ws.Cell(currentRow, firstColumn + 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Cell(currentRow, firstColumn + 3).Value = priceable.CabinProperties.NominalLength;
                    ws.Cell(currentRow, firstColumn + 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Cell(currentRow, firstColumn + 4).Value = priceable.CabinProperties.LengthMax;
                    ws.Cell(currentRow, firstColumn + 5).Value = priceable.CabinProperties.LengthMin;
                    ws.Cell(currentRow, firstColumn + 6).Value = priceable.CabinProperties.Height;

                    ws.Cell(currentRow, firstColumn + 7).Value = priceable.CabinProperties.Thicknesses?.ToString().TryTranslateKey() ?? "Undefined Thickness";
                    ws.Cell(currentRow, firstColumn + 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Cell(currentRow, firstColumn + 8).Value = priceable.CabinProperties.GlassFinish?.ToString().TryTranslateKey() ?? "Undefined GlassFinish";
                    ws.Cell(currentRow, firstColumn + 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Cell(currentRow, firstColumn + 9).Value = priceable.CabinProperties.MetalFinish?.ToString().TryTranslateKey() ?? "Undefined MetalFinish";
                    ws.Cell(currentRow, firstColumn + 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Cell(currentRow, firstColumn + 10).Value = priceable.CabinProperties.Opening;
                    ws.Cell(currentRow, firstColumn + 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Cell(currentRow, firstColumn + 11).Value = priceable.CabinProperties.IsReversible ? "lngYes".TryTranslateKey() : "lngNo".TryTranslateKey();
                    ws.Cell(currentRow, firstColumn + 11).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Cell(currentRow, firstColumn + 12).Value = priceable.StartingPrice;
                    ws.Cell(currentRow, firstColumn + 12).Style.NumberFormat.Format = "0.00\u20AC";

                    var discount = 1 - ((IPriceable)priceable).GetTotalDiscountFactor();
                    ws.Cell(currentRow, firstColumn + 13).Value = discount * 100;
                    ws.Cell(currentRow, firstColumn + 13).Style.NumberFormat.Format = "0.00%";

                    ws.Cell(currentRow, firstColumn + 14).FormulaR1C1 = $"=R[0]C[-2]*(1-R[0]C[-1])";
                    ws.Cell(currentRow, firstColumn + 14).Style.NumberFormat.Format = "0.00\u20AC";

                    ws.Cell(currentRow, firstColumn + 15).Value = "lngPhoto".TryTranslateKey();
                    ws.Cell(currentRow, firstColumn + 15).SetHyperlink(new(priceable.ThumbnailPhotoPath));
                    ws.Cell(currentRow, firstColumn + 15).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    //Alternate Row Background Color if Column Number is even
                    if (currentRow % 2 is 0)
                    {
                        var range = ws.Range(currentRow, firstColumn, currentRow, firstColumn + 15);
                        range.Style.Fill.BackgroundColor = XLColor.FromArgb(242, 242, 242);
                    }

                    currentRow++;
                }
                ws.Columns().AdjustToContents();
                return SaveXlsFile(wb);
            }
        }

    }

    public static class ReportXls
    {
        /// <summary>
        /// Saves a List of Objects as an XLS file (Each Property as a Column)
        /// </summary>
        /// <typeparam name="T">The Type of the Object being reported</typeparam>
        /// <param name="reportList">The List of Items</param>
        /// <returns>The Saved File</returns>
        public static string SaveAsXlsReport<T>(IEnumerable<T> reportList)
        {
            ReportOptions options = ReportsOptionsBuilder.GetReportOptions(typeof(T));

            Report<T> report = new(reportList, options);

            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add($"Report-{report.GetType().GetGenericArguments().FirstOrDefault()?.Name ?? "????"}");
                int firstRow = 2;
                int firstColumn = 2;
                int currentRow = firstRow;
                int currentColumn = firstColumn;

                int namesCounter = 1;
                // Write Column Headers
                foreach (var columnName in report.AlteredColumnNames)
                {
                    // check if the column name is duplicated inside the Column Names and if it is add an index Number to the end of it
                    // this way any Table Creations do not throw Exceptions because they have same header names
                    if (report.AlteredColumnNames.Where(n => n == columnName).Skip(1).Any())
                    {
                        var columnNameWithNumber = $"{columnName}-{namesCounter++}";
                        ws.Cell(currentRow, currentColumn++).Value = columnNameWithNumber;
                    }
                    else
                    {
                        ws.Cell(currentRow, currentColumn++).Value = columnName;
                    }
                }

                //Go Back one Column
                currentColumn--;

                //Style the Titles
                var titleRange = ws.Range(currentRow, firstColumn, currentRow, currentColumn);
                titleRange.Style.Font.FontSize = 12;
                titleRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                titleRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                titleRange.Style.Font.Bold = true;

                //Write Below
                currentRow++;
                //Reset the Column to the first
                currentColumn = firstColumn;
                //Save the First Row where values are being written
                int firstRowOfValues = currentRow;

                foreach (var column in report.Columns)
                {
                    //Start each Iteration at the first Column
                    currentRow = firstRowOfValues;

                    foreach (var columnValue in column.GetAlteredValues())
                    {
                        // Get the Cell
                        var cell = ws.Cell(currentRow, currentColumn);

                        // Write each item to the Next Row but the Current Column
                        if (double.TryParse(columnValue, out var doubleValue))
                        {
                            cell.Value = doubleValue;
                            //Check if there are decimals and Set accordingly the decimals
                            if (doubleValue % 1 != 0) //has decimals
                            {
                                // Set it to a number with up to two decimals so that there are not squiggles in excel
                                cell.Style.NumberFormat.NumberFormatId = (int)XLPredefinedFormat.Number.Precision2;
                            }
                            else
                            {
                                // Set it to a number Integer
                                cell.Style.NumberFormat.NumberFormatId = (int)XLPredefinedFormat.Number.Integer;
                            }
                        }
                        else
                        {
                            cell.Value = columnValue;
                        }
                        cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        cell.Style.Font.FontColor = XLColor.Black;
                        cell.Style.Font.FontSize = 12;
                        currentRow++;
                    }

                    // Go To Next Column
                    currentColumn++;
                }
                // Go Back one row and One Column
                currentColumn--;
                currentRow--;

                // Adjust the Contents of the Reports Cells
                ws.Rows().AdjustToContents();
                ws.Columns().AdjustToContents();

                //Add a little padding in each Column
                foreach (var col in ws.Columns())
                {
                    col.Width += 5d;
                }


                //Get the Range of the Data and Sort it based on the first Column
                var rangeOfWrittenDataNoHeaders = ws.Range(firstRow + 1, firstColumn, currentRow, currentColumn);
                rangeOfWrittenDataNoHeaders.Sort(1, XLSortOrder.Ascending, false, true);

                // Create a Table on the Data so user can sort it when opened in excel
                // Select ALL Data Headers must be included to the Table Creation as when Excel is used (works the same way ?)
                var table = ws.RangeUsed()?.CreateTable("Table");
                if (table is not null)
                {
                    table.Theme = XLTableTheme.TableStyleMedium16;
                }

                return SaveXlsFile(wb);
            }
        }
    }
}

public class XlsSettingsGlasses
{
    public int Id { get; set; }
    public bool IsSelected { get; set; }
    public string SettingsName { get; set; } = string.Empty;
    /// <summary>
    /// The Worksheet Name
    /// </summary>
    public string WorksheetName { get; set; } = "GlassesOrder";
    /// <summary>
    /// The Font Size
    /// </summary>
    public double FontSize { get; set; } = 11;
    /// <summary>
    /// Weather the Sheet Main Font will be Bold or Not
    /// </summary>
    public bool IsFontBold { get; set; } = true;
    /// <summary>
    /// The Font Family to be Used by the Excel Document
    /// </summary>
    public string FontFamily { get; set; } = "Calibri";
    /// <summary>
    /// The Color of the Fonts (The Default is 68,84,106 as the Initial First Excel Made)
    /// </summary>
    public XLColor FontColor { get; set; } = XLColor.FromArgb(68, 84, 106);
    /// <summary>
    /// The Height of each NormalRow
    /// </summary>
    public double NormalRowHeight { get; set; } = 19.50d;
    /// <summary>
    /// The Total Columns that will be Used , Default for Glasses is 13
    /// </summary>
    public int TotalColumns { get => ColumnsTitlesGR.Count; }
    /// <summary>
    /// The Maximum Number of Rows that might be Used , Just a big Number as Default
    /// </summary>
    public int TotalRows { get; set; } = 2000;
    /// <summary>
    /// The Index of the First Row to be Used
    /// </summary>
    public int FirstRowIndex { get; set; } = 1;
    /// <summary>
    /// The Index of the First Column to be Used
    /// </summary>
    public int FirstColumnIndex { get; set; } = 1;
    /// <summary>
    /// The Last Row Index Number
    /// </summary>
    public int LastRowIndex { get => TotalRows + FirstRowIndex; }
    /// <summary>
    /// The Last Column Index Number
    /// </summary>
    public int LastColumnIndex { get => TotalColumns + FirstColumnIndex; }
    /// <summary>
    /// The Horizontal Alignment of the Cells
    /// </summary>
    public XLAlignmentHorizontalValues HorizontalAlignment { get; set; } = XLAlignmentHorizontalValues.Center;
    /// <summary>
    /// The Vertical Alignment of the Cells
    /// </summary>
    public XLAlignmentVerticalValues VerticalAlignment { get; set; } = XLAlignmentVerticalValues.Center;
    public string SelectedCultureString { get; set; }
    public List<string> ColumnsTitlesGR { get; private set; }
    public List<string> ColumnsTitlesEN { get; private set; }
    public double TitlesRowHeight { get; set; } = 20d;
    public XLColor TitlesBackgroundColor { get; set; } = XLColor.FromArgb(68, 114, 196);
    public XLColor TitlesFontColor { get; set; } = XLColor.White;
    public bool IsTitlesFontBold { get; set; } = true;

    /// <summary>
    /// Settings for the Header
    /// </summary>
    public XlsGlassTableHeaderSettings TableHeaderSettings { get; set; } = new();
    /// <summary>
    /// Contains the Glass Descriptions
    /// </summary>
    public XlsGlassDescriptions GlassDescriptions { get; private set; }
    public XlsGlassTablesSettings GlassTableSettings { get; set; } = new();
    public XlsGeneralHeaderSettings GeneralHeaderSettings { get; set; } = new();
    public XlsTotalGlassesBoxSettings TotalGlassesBoxSettings { get; set; } = new();
    public XlsNotesBoxGlassesSettings NotesBoxSettings { get; set; } = new();

    public XlsSettingsGlasses(string cultureString = "el-GR")
    {
        SelectedCultureString = cultureString;

        ColumnsTitlesGR = new()
        {
            "A/A",
            "Σχέδιο",
            "Μήκος(mm)",
            "Ύψος(mm)",
            "Πάχος(mm)",
            "Τύπος",
            "CUT Μήκος(mm)",
            "CUT Ύψος(mm)",
            "Τεμάχια",
            "BronzeUse PA0",
            "BronzeUse Item",
            "Σημειώσεις",
            "Τιμή EUR",
            "SQM(m\u00b2)",
            "Βάρος(kgr)"
        };
        ColumnsTitlesEN = new()
        {
            "A/A",
            "Draw",
            "Length(mm)",
            "Height(mm)",
            "Thickness(mm)",
            "Type",
            "CUT Length(mm)",
            "CUT Height(mm)",
            "Pieces",
            "BronzeUse PA0",
            "BronzeUse Item",
            "Notes",
            "Price EUR",
            "SQM(m\u00b2)",
            "Weight(kgr)"
        };

        GlassDescriptions = new(SelectedCultureString);
    }

    /// <summary>
    /// Gets the Column Names Based on the Selected Culture
    /// </summary>
    /// <returns></returns>
    public List<string> GetColumnNames()
    {
        return SelectedCultureString is "en-EN" ? ColumnsTitlesEN : ColumnsTitlesGR;
    }

}
public class XlsGlassTableHeaderSettings
{
    /// <summary>
    /// The Height of the Row where the Header is Placed for each Glasses Group
    /// </summary>
    public double TableHeaderRowHeight { get; set; } = 39.75d;
    /// <summary>
    /// The Font Size of the Header Text
    /// </summary>
    public double FontSize { get; set; } = 16;
    /// <summary>
    /// Weather the Header is in Bold
    /// </summary>
    public bool IsFontBold { get; set; } = false;
    /// <summary>
    /// The Font Color of the Header
    /// </summary>
    public XLColor FontColor { get; set; } = XLColor.Black;
    /// <summary>
    /// The Background Color of the Header , Default 231,230,230 (Grayish)
    /// </summary>
    public XLColor BackgroundColor { get; set; } = XLColor.FromArgb(231, 230, 230);
    /// <summary>
    /// The Horizontal Alignment of the Header
    /// </summary>
    public XLAlignmentHorizontalValues HorizontalAlignment { get; set; } = XLAlignmentHorizontalValues.Left;
    /// <summary>
    /// The Vertical Alignment of the Header
    /// </summary>
    public XLAlignmentVerticalValues VerticalAlignment { get; set; } = XLAlignmentVerticalValues.Center;
}
public class XlsGlassDescriptions
{
    public string SelectedCultureString { get; set; } = "el-GR";

    /// <summary>
    /// The Descriptions of the Glass Thickness
    /// </summary>
    public Dictionary<GlassThicknessEnum, string> ThicknessDescriptions { get; set; }
    /// <summary>
    /// The Finish Descriptions of the Glass
    /// </summary>
    public Dictionary<GlassFinishEnum, string> FinishDescriptionsGR { get; set; }
    public Dictionary<GlassFinishEnum, string> FinishDescriptionsEN { get; set; }
    /// <summary>
    /// The Descriptions for the Holes of the Glass
    /// </summary>
    public Dictionary<GlassDrawEnum, string> HolesDescriptionsGR { get; set; }
    public Dictionary<GlassDrawEnum, string> HolesDescriptionsEN { get; set; }
    /// <summary>
    /// The Descriptions for the Draws of the Glass
    /// </summary>
    public Dictionary<GlassDrawEnum, string> DrawDescriptions { get; set; }

    public XlsGlassDescriptions(string cultureString = "el-GR")
    {
        SelectedCultureString = cultureString;

        #region 1. Thickness Descriptions
        ThicknessDescriptions = GetDefaultThicknessDescriptions();

        //If more values are added later it will add their string representation
        foreach (GlassThicknessEnum value in Enum.GetValues(typeof(GlassThicknessEnum)))
        {
            ThicknessDescriptions.TryAdd(value, value.ToString());
        }
        #endregion

        #region 2. Finish Descriptions
        FinishDescriptionsGR = GetDefaultFinishDescriptions();
        FinishDescriptionsEN = GetDefaultFinishDescriptionsEN();

        //If more values are added later it will add their string representation
        foreach (GlassFinishEnum value in Enum.GetValues(typeof(GlassFinishEnum)))
        {
            FinishDescriptionsGR.TryAdd(value, value.ToString());
            FinishDescriptionsEN.TryAdd(value, value.ToString());
        }
        #endregion

        #region 3. Holes Descriptions

        HolesDescriptionsGR = GetDefaultHoleDescriptions();
        HolesDescriptionsEN = GetDefaultHoleDescriptionsEN();

        //If more values are added later it will add Undefined Description
        foreach (GlassDrawEnum value in Enum.GetValues(typeof(GlassDrawEnum)))
        {
            HolesDescriptionsGR.TryAdd(value, "UndefinedHolesDescription");
            HolesDescriptionsEN.TryAdd(value, "UndefinedHolesDescription");
        }

        #endregion

        #region 4. Draw Descriptions

        DrawDescriptions = GetDefaultDrawDescriptions();

        //If more values are added later it will add their string representation
        foreach (GlassDrawEnum value in Enum.GetValues(typeof(GlassDrawEnum)))
        {
            DrawDescriptions.TryAdd(value, value.ToString());
        }

        #endregion
    }

    /// <summary>
    /// Gets the Full String Description of the Glass Without Dimensions (Thickness/Finish/FullDrawDescription)
    /// </summary>
    /// <param name="glass"></param>
    /// <param name="lettering">Extra Letters Numbers (FK e.t.c)</param>
    /// <returns></returns>
    public string FullDescription(Glass glass, string lettering)
    {
        StringBuilder builder = new();
        builder.Append(SimpleThicknessString(glass))
               .Append(" Tempered -- ")
               .Append(SimpleFinishString(glass))
               .Append(" -- -- -- ")
               .Append(DrawFullDescription(glass, lettering));
        return builder.ToString();
    }
    /// <summary>
    /// Gets the String Representation of the Glass Dimensions (ex. 1000 x 2000 x 6mm)
    /// </summary>
    /// <param name="glass"></param>
    /// <returns></returns>
    public string DimensionsDescription(Glass glass)
    {
        StringBuilder builder = new();
        builder.Append(glass.Length)
               .Append(" x ")
               .Append(glass.Height)
               .Append(" x ")
               .Append(SimpleThicknessString(glass));

        return builder.ToString();
    }
    /// <summary>
    /// Gets the String Representation of the Steps Dimensions (ex. 100 x 200mm)
    /// </summary>
    /// <param name="glass"></param>
    /// <returns></returns>
    public string StepDimensionString(Glass glass)
    {
        string dimensionString;
        if (glass.HasStep)
        {
            dimensionString = $"{glass.StepLength} x {glass.StepHeight}mm";
        }
        else
        {
            dimensionString = "-";
        }

        return dimensionString;
    }
    /// <summary>
    /// Gets the Description of the Draw in its simplest Form (ex. 9S)
    /// </summary>
    /// <param name="glass"></param>
    /// <returns></returns>
    public string DrawSimpleString(Glass glass, string lettering)
    {
        if (DrawDescriptions.TryGetValue(glass.Draw, out string? draw))
        {
            return string.IsNullOrEmpty(lettering)
                ? draw
                : $"{draw}{lettering}";
        }
        return "Undefined-Draw";
    }
    /// <summary>
    /// Gets the Simple Description String of Thickness (ex. 6mm)
    /// </summary>
    /// <param name="glass"></param>
    /// <returns></returns>
    public string SimpleThicknessString(Glass glass)
    {
        var thickness = glass.Thickness ?? GlassThicknessEnum.GlassThicknessNotSet;
        if (ThicknessDescriptions.TryGetValue(thickness, out string? thicknessDesc))
        {
            return thicknessDesc;
        }
        return GlassThicknessEnum.GlassThicknessNotSet.ToString();
    }
    /// <summary>
    /// Gets the Simple Finish Description string (Ex. Διάφανο)
    /// </summary>
    /// <param name="glass"></param>
    /// <returns></returns>
    public string SimpleFinishString(Glass glass)
    {
        var descriptionsDictionary = SelectedCultureString is "en-EN" ? FinishDescriptionsEN : FinishDescriptionsGR;

        var finish = glass.Finish ?? GlassFinishEnum.GlassFinishNotSet;
        if (descriptionsDictionary.TryGetValue(finish, out string? finishDesc))
        {
            return finishDesc;
        }
        return GlassFinishEnum.GlassFinishNotSet.ToString();
    }

    private string DrawString(Glass glass, string lettering)
    {
        var drawDescriptor = SelectedCultureString is "en-EN" ? "Draw" : "Σχέδιο";
        string drawString = $"{drawDescriptor}:'{DrawSimpleString(glass, lettering)}'";
        return drawString;
    }

    private string DrawFullDescription(Glass glass, string lettering)
    {
        var descriptionsDictionary = SelectedCultureString is "en-EN" ? HolesDescriptionsEN : HolesDescriptionsGR;

        if (!descriptionsDictionary.TryGetValue(glass.Draw, out string? holesDesc))
        {
            holesDesc = "UndefinedHolesDescription";
        }

        StringBuilder builder = new();
        builder.Append(DrawString(glass, lettering))
               .Append("    ")
               .Append(holesDesc);
        return builder.ToString();
    }

    /// <summary>
    /// Returns the Default Draw Descriptions
    /// </summary>
    /// <returns></returns>
    public static Dictionary<GlassDrawEnum, string> GetDefaultDrawDescriptions()
    {
        return new Dictionary<GlassDrawEnum, string>()
        {
            { GlassDrawEnum.Draw9S ,    "9S"},
            { GlassDrawEnum.DrawF  ,    "F"},
            { GlassDrawEnum.Draw9B ,    "9B"},
            { GlassDrawEnum.Draw94 ,    "94"},
            { GlassDrawEnum.DrawVS ,    "VS"},
            { GlassDrawEnum.DrawVA ,    "VA"},
            { GlassDrawEnum.DrawVF ,    "VF"},
            { GlassDrawEnum.DrawHB1,    "HB1"},
            { GlassDrawEnum.DrawHB2,    "HB2"},
            { GlassDrawEnum.DrawDP1,    "DP1"},
            { GlassDrawEnum.DrawDP3,    "DP3"},
            { GlassDrawEnum.DrawNB ,    "NB"},
            { GlassDrawEnum.DrawDB ,    "DB"},
            { GlassDrawEnum.DrawWS ,    "WS"},
            { GlassDrawEnum.DrawH1 ,    "H1"},
            { GlassDrawEnum.DrawFL ,    "FL"},
            { GlassDrawEnum.Draw9C ,    "9C"},
            { GlassDrawEnum.DrawNV ,    "NV"}
        };
    }
    public static Dictionary<GlassDrawEnum, string> GetDefaultHoleDescriptions()
    {
        return new Dictionary<GlassDrawEnum, string>()
        {
            { GlassDrawEnum.Draw9S ,    "10-Τρύπες--Φ10mm" },
            { GlassDrawEnum.DrawF,      "Χωρίς Τρύπες" },
            { GlassDrawEnum.Draw94,     "10-Τρύπες--Φ10mm" },
            { GlassDrawEnum.Draw9B,     "4-Τρύπες--Φ10mm & 2-Κοψίματα" },
            { GlassDrawEnum.DrawVS,     "2-Τρύπες--Φ16mm & 2-Τρύπες--Φ10mm & 1-Τρύπα--Φ50mm"},
            { GlassDrawEnum.DrawVA,     "2-Τρύπες--Φ20mm & 2-Τρύπες Φ14mm" },
            { GlassDrawEnum.DrawVF,     "1-Τρύπα--Φ14mm & 2-Τρύπες Φ25mm" },
            { GlassDrawEnum.DrawHB1,    "2-CUT--Μεντεσέ"},
            { GlassDrawEnum.DrawHB2,    "2-CUT--Μεντεσέ & 1-Τρύπα--Φ12mm" },
            { GlassDrawEnum.DrawDP1,    "2-Φρεζάτες--Φ24/Φ32mm & 1-Τρύπα--Φ12mm"},
            { GlassDrawEnum.DrawDP3,    "2-Φρεζάτες--Φ24/Φ32mm"},
            { GlassDrawEnum.DrawNB ,    "1-Τρύπα--Φ10mm"},
            { GlassDrawEnum.DrawDB ,    "2-DG--CUT & 1-Τρύπα--Φ10mm"},
            { GlassDrawEnum.DrawWS ,    "2-Τρύπες--Φ15mm & 1-Τρύπα--Φ50mm"},
            { GlassDrawEnum.DrawH1 ,    "2-Τρύπες--Φ20mm"},
            { GlassDrawEnum.DrawFL ,    "2-Tρυπες--Φ14mm" }, //Fixed on 10-12-23
            { GlassDrawEnum.Draw9C ,    "??-Τρυπες--Φ??mm"},
            { GlassDrawEnum.DrawNV ,    "Χωρίς Τρύπες -- με R-200" }
        };
    }
    public static Dictionary<GlassDrawEnum, string> GetDefaultHoleDescriptionsEN()
    {
        return new Dictionary<GlassDrawEnum, string>()
        {
            { GlassDrawEnum.Draw9S ,    "10-Holes--Φ10mm" },
            { GlassDrawEnum.DrawF,      "Without Holes" },
            { GlassDrawEnum.Draw94 ,    "10-Holes--Φ10mm" },
            { GlassDrawEnum.Draw9B,     "4-Holes--Φ10mm & 2-CUTS" },
            { GlassDrawEnum.DrawVS,     "2-Holes--Φ16mm & 2-Holes--Φ10mm & 1-Hole--Φ50mm"},
            { GlassDrawEnum.DrawVA,     "2-Holes--Φ20mm & 2-Holes Φ14mm" },
            { GlassDrawEnum.DrawVF,     "1-Hole--Φ14mm & 2-HolesΦ25mm" },
            { GlassDrawEnum.DrawHB1,    "2-CUT--Hinge"},
            { GlassDrawEnum.DrawHB2,    "2-CUT--Hinge & 1-Hole--Φ12mm" },
            { GlassDrawEnum.DrawDP1,    "2-Conical Holes--Φ24/Φ32mm & 1-Hole--Φ12mm"},
            { GlassDrawEnum.DrawDP3,    "2-Conical Holes--Φ24/Φ32mm"},
            { GlassDrawEnum.DrawNB ,    "1-Hole--Φ10mm"},
            { GlassDrawEnum.DrawDB ,    "2-DG--CUT & 1-Hole--Φ10mm"},
            { GlassDrawEnum.DrawWS ,    "2-Holes--Φ15mm & 1-Hole--Φ50mm"},
            { GlassDrawEnum.DrawH1 ,    "2-Holes--Φ20mm"},
            { GlassDrawEnum.DrawFL ,    "2-Holes--Φ14mm" }, // Fixed on 10-12-23
            { GlassDrawEnum.Draw9C ,    "??-Holes--Φ??mm"},
            { GlassDrawEnum.DrawNV ,    "Withou Holes -- with R-200" }
        };
    }
    public static Dictionary<GlassFinishEnum, string> GetDefaultFinishDescriptions()
    {
        return new Dictionary<GlassFinishEnum, string>()
        {
            {GlassFinishEnum.Fume , "Φιμέ" },
            {GlassFinishEnum.Frosted, "Frosted" },
            {GlassFinishEnum.GlassFinishNotSet , "Undefined" },
            {GlassFinishEnum.Satin , "Σατινέ" },
            {GlassFinishEnum.Serigraphy, "Σεριγραφεία" },
            {GlassFinishEnum.Transparent, "Διάφανο" },
            {GlassFinishEnum.Special , "Ειδικό" }
        };
    }
    public static Dictionary<GlassFinishEnum, string> GetDefaultFinishDescriptionsEN()
    {
        return new Dictionary<GlassFinishEnum, string>()
        {
            {GlassFinishEnum.Fume , "Fume" },
            {GlassFinishEnum.Frosted, "Frosted" },
            {GlassFinishEnum.GlassFinishNotSet , "Undefined" },
            {GlassFinishEnum.Satin , "Satin" },
            {GlassFinishEnum.Serigraphy, "Serigraphy" },
            {GlassFinishEnum.Transparent, "Transparent" },
            {GlassFinishEnum.Special , "Special" }
        };
    }
    public static Dictionary<GlassThicknessEnum, string> GetDefaultThicknessDescriptions()
    {
        return new Dictionary<GlassThicknessEnum, string>()
        {
            {GlassThicknessEnum.Thick5mm , "5mm" },
            {GlassThicknessEnum.Thick6mm , "6mm" },
            {GlassThicknessEnum.Thick8mm , "8mm" },
            {GlassThicknessEnum.Thick10mm, "10mm" },
            {GlassThicknessEnum.ThickTenplex10mm , "10mm Tenplex" },
            {GlassThicknessEnum.GlassThicknessNotSet , "Undefined" }
        };
    }

}
public class XlsGlassTablesSettings
{
    /// <summary>
    /// The Thickness of the Border in the Horizontal Lines between the Glasses
    /// </summary>
    public XLBorderStyleValues HorizontalBorderThickness { get; set; } = XLBorderStyleValues.Medium;
    /// <summary>
    /// The Color of the Border in the Horizontal Lines between the Glasses Properties
    /// </summary>
    public XLColor HorizontalBorderColor { get; set; } = XLColor.FromArgb(142, 169, 219);
    /// <summary>
    /// The Thickness of the Border in the Vertical Lines between the Borders
    /// </summary>
    public XLBorderStyleValues VerticalBorderThickness { get; set; } = XLBorderStyleValues.None;
    /// <summary>
    /// The Color of the Border in the Vertical Lines between the Glasses Properties
    /// </summary>
    public XLColor VerticalBorderColor { get; set; } = XLColor.FromArgb(142, 169, 219);
    /// <summary>
    /// The Color of the Alternating Row in the Glasses Tables
    /// </summary>
    public XLColor AlternatingTableRowBackground { get; set; } = XLColor.FromArgb(242, 242, 242);
    /// <summary>
    /// The Thickness of the Perimetrical Border of each of the Glasses Tables
    /// </summary>
    public XLBorderStyleValues TablePerimetricalBorderThickness { get; set; } = XLBorderStyleValues.Medium;
    /// <summary>
    /// The Color of the Perimetrical Border of each of the Glasses Tables
    /// </summary>
    public XLColor TablePerimetricalBorderColor { get; set; } = XLColor.Black;
}
public class XlsGeneralHeaderSettings
{
    public double RowHeight { get; set; } = 39.75d;
    public double FontSize { get; set; } = 22;
    public bool IsFontBold { get; set; } = true;
    public XLAlignmentHorizontalValues HorizontalAlignment { get; set; } = XLAlignmentHorizontalValues.Center;
    public XLAlignmentVerticalValues VerticalAlignment { get; set; } = XLAlignmentVerticalValues.Center;
    public XLColor FontColor { get; set; } = XLColor.Black;
    public XLColor BackgroundColor { get; set; } = XLColor.FromArgb(231, 230, 230);
}
public class XlsTotalGlassesBoxSettings
{
    public XLAlignmentHorizontalValues HorizontalAlignment { get; set; } = XLAlignmentHorizontalValues.Center;
    public XLAlignmentVerticalValues VerticalAlignment { get; set; } = XLAlignmentVerticalValues.Center;
    public XLColor BackgroundColor { get; set; } = XLColor.FromArgb(242, 242, 242);
    public double FontSize { get; set; } = 16;
    public bool IsFontBold { get; set; } = true;
    public XLColor FontColor { get; set; } = XLColor.Black;
}
public class XlsNotesBoxGlassesSettings
{
    public XLAlignmentHorizontalValues HorizontalAlignment { get; set; } = XLAlignmentHorizontalValues.Center;
    public XLAlignmentVerticalValues VerticalAlignment { get; set; } = XLAlignmentVerticalValues.Center;
    public XLColor BackgroundColor { get; set; } = XLColor.FromArgb(242, 242, 242);
    public double FontSize { get; set; } = 12;
    public bool IsFontBold { get; set; } = true;
    public XLColor FontColor { get; set; } = XLColor.Black;
    public int NumberOfRowsForNotes { get; set; } = 5;
}
