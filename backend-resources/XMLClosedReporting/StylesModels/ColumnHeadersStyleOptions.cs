using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLClosedReporting.StylesModels
{
    public class StyleOptions
    {
        public double RowHeight { get; set; } = double.NaN;
        public double ColumnWidth { get; set; } = double.NaN;
        public double FontSize { get; set; } = 18;
        public bool IsFontItalic { get; set; } = false;
        public bool IsFontBold { get; set; } = false;
        public XLAlignmentHorizontalValues HorizontalAlignment { get; set; } = XLAlignmentHorizontalValues.Center;
        public XLAlignmentVerticalValues VerticalAlignment { get; set; } = XLAlignmentVerticalValues.Center;
        public XLBorderStyleValues RightBorderThickness { get; set; } = XLBorderStyleValues.None;
        public XLColor RightBorderColor { get; set; } = XLColor.Transparent;
        public XLBorderStyleValues LeftBorderThickness { get; set; } = XLBorderStyleValues.None;
        public XLColor LeftBorderColor { get; set; } = XLColor.Transparent;
        public XLBorderStyleValues BottomBorderThickness { get; set; } = XLBorderStyleValues.Thin;
        public XLColor BottomBorderColor { get; set; } = XLColor.Black;
        public XLBorderStyleValues TopBorderThickness { get; set; } = XLBorderStyleValues.Thin;
        public XLColor TopBorderColor { get; set; } = XLColor.Black;

        public XLColor FontColor { get; set; } = XLColor.White;
        public XLColor BackgroundColor { get; set; } = XLColor.FromArgb(68, 114, 196);

        public StyleOptions()
        {
            
        }

        #region Default Styles
        public static StyleOptions ColumnHeadersDefaultStyles() => new()
        {
            RowHeight = 20d,
            FontSize = 12,
            IsFontBold = true,
            IsFontItalic = false,
            HorizontalAlignment = XLAlignmentHorizontalValues.Center,
            VerticalAlignment = XLAlignmentVerticalValues.Center,
            FontColor = XLColor.White,
            BackgroundColor = XLColor.FromArgb(68, 114, 196),
            TopBorderColor = XLColor.Black,
            TopBorderThickness = XLBorderStyleValues.Thin,
            BottomBorderColor = XLColor.Black,
            BottomBorderThickness = XLBorderStyleValues.Thin,
            RightBorderColor = XLColor.Transparent,
            RightBorderThickness = XLBorderStyleValues.None,
            LeftBorderColor = XLColor.Transparent,
            LeftBorderThickness = XLBorderStyleValues.None
        };
        public static StyleOptions ReportTitleDefaultStyles() => new()
        {
            RowHeight = 39.75d,
            FontSize = 18,
            IsFontBold = false,
            IsFontItalic = false,
            HorizontalAlignment = XLAlignmentHorizontalValues.Left,
            VerticalAlignment = XLAlignmentVerticalValues.Center,
            FontColor = XLColor.Black,
            BackgroundColor = XLColor.FromArgb(231, 230, 230),
            TopBorderColor = XLColor.Black,
            TopBorderThickness = XLBorderStyleValues.Thin,
            BottomBorderColor = XLColor.Black,
            BottomBorderThickness = XLBorderStyleValues.Thin,
            RightBorderColor = XLColor.Transparent,
            RightBorderThickness = XLBorderStyleValues.None,
            LeftBorderColor = XLColor.Transparent,
            LeftBorderThickness = XLBorderStyleValues.None
        };
        public static StyleOptions ReportNotesDefaultStyles() => new()
        {
            RowHeight = 39.75d,
            FontSize = 12,
            IsFontBold = false,
            IsFontItalic = true,
            HorizontalAlignment = XLAlignmentHorizontalValues.Left,
            VerticalAlignment = XLAlignmentVerticalValues.Center,
            FontColor = XLColor.FromArgb(68, 84, 106),
            BackgroundColor = XLColor.White,
            TopBorderColor = XLColor.Black,
            TopBorderThickness = XLBorderStyleValues.Thin,
            BottomBorderColor = XLColor.Black,
            BottomBorderThickness = XLBorderStyleValues.Thin,
            RightBorderColor = XLColor.Black,
            RightBorderThickness = XLBorderStyleValues.None,
            LeftBorderColor = XLColor.Black,
            LeftBorderThickness = XLBorderStyleValues.None
        };
        public static StyleOptions ReportSumsTableValuesDefaultStyles() => new()
        {
            RowHeight = 20,
            FontSize = 12,
            IsFontBold = true,
            IsFontItalic = false,
            HorizontalAlignment = XLAlignmentHorizontalValues.Right,
            VerticalAlignment = XLAlignmentVerticalValues.Center,
            FontColor = XLColor.FromArgb(68, 84, 106),
            BackgroundColor = XLColor.White,
            TopBorderColor = XLColor.Black,
            TopBorderThickness = XLBorderStyleValues.Thin,
            BottomBorderColor = XLColor.Black,
            BottomBorderThickness = XLBorderStyleValues.Thin,
            RightBorderColor = XLColor.Black,
            RightBorderThickness = XLBorderStyleValues.Thin,
            LeftBorderColor = XLColor.Black,
            LeftBorderThickness = XLBorderStyleValues.None
        };
        public static StyleOptions ReportSumsTableTitlesDefaultStyles() => new()
        {
            RowHeight = 20,
            FontSize = 12,
            IsFontBold = true,
            IsFontItalic = false,
            HorizontalAlignment = XLAlignmentHorizontalValues.Left,
            VerticalAlignment = XLAlignmentVerticalValues.Center,
            FontColor = XLColor.FromArgb(68, 84, 106),
            BackgroundColor = XLColor.White,
            TopBorderColor = XLColor.Black,
            TopBorderThickness = XLBorderStyleValues.Thin,
            BottomBorderColor = XLColor.Black,
            BottomBorderThickness = XLBorderStyleValues.Thin,
            RightBorderColor = XLColor.Black,
            RightBorderThickness = XLBorderStyleValues.None,
            LeftBorderColor = XLColor.Black,
            LeftBorderThickness = XLBorderStyleValues.Thin
        };
        public static StyleOptions ValueCellsDefaultStyles() => new()
        {
            RowHeight = double.NaN,
            FontSize = 11,
            IsFontBold = false,
            IsFontItalic = false,
            HorizontalAlignment = XLAlignmentHorizontalValues.Center,
            VerticalAlignment = XLAlignmentVerticalValues.Center,
            FontColor = XLColor.FromArgb(68, 84, 106),
            BackgroundColor = XLColor.White,
            TopBorderColor = XLColor.Black,
            TopBorderThickness = XLBorderStyleValues.Thin,
            BottomBorderColor = XLColor.Black,
            BottomBorderThickness = XLBorderStyleValues.Thin,
            RightBorderColor = XLColor.Transparent,
            RightBorderThickness = XLBorderStyleValues.None,
            LeftBorderColor = XLColor.Transparent,
            LeftBorderThickness = XLBorderStyleValues.None
        };
        #endregion

        public void ApplyStyle(IXLStyle style)
        {
            style.Font.FontSize = FontSize;
            style.Font.Bold = IsFontBold;
            style.Font.Italic = IsFontItalic;
            style.Font.FontColor = FontColor;
            style.Fill.SetBackgroundColor(BackgroundColor);
            style.Alignment.Horizontal = HorizontalAlignment;
            style.Alignment.Vertical = VerticalAlignment;
            
            style.Border.BottomBorder = BottomBorderThickness;
            style.Border.BottomBorderColor = BottomBorderColor;
            style.Border.TopBorder = TopBorderThickness;
            style.Border.TopBorderColor = TopBorderColor;

            style.Border.RightBorder = RightBorderThickness;
            style.Border.RightBorderColor = RightBorderColor;
            style.Border.LeftBorder = LeftBorderThickness;
            style.Border.LeftBorderColor = LeftBorderColor;
        }
        public void ApplyDimensions(IXLCell cell)
        {
            if (!double.IsNaN(RowHeight))
            {
                cell.WorksheetRow().Height = RowHeight;
            }

            if (!double.IsNaN(ColumnWidth))
            {
                cell.WorksheetColumn().Width = ColumnWidth;
            }
        }
        public void ApplyDimensions(IXLRange range)
        {
            if (!double.IsNaN(RowHeight))
            {
                foreach (var row in range.Rows())
                {
                    row.WorksheetRow().Height = RowHeight;
                }
            }

            if (!double.IsNaN(ColumnWidth))
            {
                foreach (var column in range.Columns())
                {
                    column.WorksheetColumn().Width = ColumnWidth;
                }
            }
        }

    }
}
