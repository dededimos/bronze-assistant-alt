using ClosedXML.Excel;
using CommonInterfacesBronze;
using XMLClosedReporting.StylesModels;

namespace XMLClosedReporting.AdvancedReportModels
{
    public class StylesInfo : IDeepClonable<StylesInfo>
    {
        /// <summary>
        /// The Format that forces the cell to be Text regardless of the value
        /// </summary>
        public const string forceTextNumberFormat = "@";
        /// <summary>
        /// The Number format that append "mm" to the number and keeps no decimal points
        /// </summary>
        public const string mmNumberFormat = "0 \"mm\"";

        /// <summary>
        /// Cells Horizontal Alignment
        /// </summary>
        public XLAlignmentHorizontalValues HorizontalAlignment { get; set; } = XLAlignmentHorizontalValues.Center;
        /// <summary>
        /// Cells Vertical Alignment
        /// </summary>
        public XLAlignmentVerticalValues VerticalAlignment { get; set; } = XLAlignmentVerticalValues.Center;
        /// <summary>
        /// Cells Text Indentation , (left padding)
        /// </summary>
        public int Indent { get; set; }
        /// <summary>
        /// Weather the cell's last Line is Justified or Not
        /// </summary>
        public bool JustifyLastLine { get; set; }
        /// <summary>
        /// Weather the cells's font size should decrease to fit contents
        /// </summary>
        public bool ShrinkToFit { get; set; }
        private int textRotation;
        /// <summary>
        /// The Rotation of the Text inside the Cell , Valid Values are -90 to 90 
        /// </summary>
        public int TextRotation
        {
            get => textRotation;
            set
            {
                //constraint the value between -90 and 90
                if (value < -90)
                    textRotation = -90;
                else if (value > 90)
                    textRotation = 90;
                else
                    textRotation = value;
            }
        }

        /// <summary>
        /// Weather the text should be wrapped inside the cell when it doesn't fit
        /// </summary>
        public bool WrapText { get; set; }
        public string NumberFormat { get; set; } = string.Empty;

        public FillStyle Fill { get; set; } = new();
        public BorderStyles Border { get; set; } = new();
        public FontStyle Font { get; set; } = new();

        public StylesInfo GetDeepClone()
        {
            var clone = (StylesInfo)MemberwiseClone();
            clone.Fill = Fill.GetDeepClone();
            clone.Border = Border.GetDeepClone();
            clone.Font = Font.GetDeepClone();
            return clone;
        }
    }
    public class FontStyle : IDeepClonable<FontStyle>
    {
        public bool Bold { get; set; }
        public bool Italic { get; set; }
        public XLFontUnderlineValues FontUnderline { get; set; } = XLFontUnderlineValues.None;
        public bool Strikethrough { get; set; } = false;
        public XLFontVerticalTextAlignmentValues FontVerticalAlignment { get; set; } = XLFontVerticalTextAlignmentValues.Baseline;
        public double FontSize { get; set; }
        public bool HasShadow { get; set; }
        public XLColor FontColor { get; set; } = XLColor.Black;

        public FontStyle GetDeepClone()
        {
            return (FontStyle)MemberwiseClone();
        }
    }
    public class BorderStyles : IDeepClonable<BorderStyles> 
    {
        private static XLColor excelThinDefaultBorder = XLColor.FromArgb(217, 217, 217);

        /// <summary>
        /// The Style of the Left Border
        /// </summary>
        public XLBorderStyleValues LeftBorder { get; set; } = XLBorderStyleValues.Thin;
        /// <summary>
        /// The Color of the Left Border
        /// </summary>
        public XLColor LeftBorderColor { get; set; } = excelThinDefaultBorder;
        /// <summary>
        /// The Style of the Right Border
        /// </summary>
        public XLBorderStyleValues RightBorder { get; set; } = XLBorderStyleValues.Thin;
        /// <summary>
        /// The Color of the Right Border
        /// </summary>
        public XLColor RightBorderColor { get; set; } = excelThinDefaultBorder;
        /// <summary>
        /// The Style of the Top Border
        /// </summary>
        public XLBorderStyleValues TopBorder { get; set; } = XLBorderStyleValues.Thin;
        /// <summary>
        /// The Color of the Top Border
        /// </summary>
        public XLColor TopBorderColor { get; set; } = excelThinDefaultBorder;
        /// <summary>
        /// The Style of the Bottom Border
        /// </summary>
        public XLBorderStyleValues BottomBorder { get; set; } = XLBorderStyleValues.Thin;
        /// <summary>
        /// The Color of the Bottom Border
        /// </summary>
        public XLColor BottomBorderColor { get; set; } = excelThinDefaultBorder;
        /// <summary>
        /// Weather there is the Diagonal Up Border
        /// </summary>
        public bool DiagonalUp { get; set; }
        /// <summary>
        /// Weather there is the Diagonal Down Border
        /// </summary>
        public bool DiagonalDown { get; set; }
        /// <summary>
        /// The Style of the Diagonal Border
        /// </summary>
        public XLBorderStyleValues DiagonalBorder { get; set; } = XLBorderStyleValues.None;
        /// <summary>
        /// The Color of the Diagonal Border
        /// </summary>
        public XLColor DiagonalBorderColor { get; set; } = XLColor.Black;

        public BorderStyles GetDeepClone()
        {
            return (BorderStyles)MemberwiseClone();
        }
    }
    public class FillStyle : IDeepClonable<FillStyle>
    {
        public XLColor BackgroundColor { get; set; } = XLColor.White;

        public FillStyle GetDeepClone()
        {
            return (FillStyle)MemberwiseClone();
        }
    }


    public static class StylesProvider
    {
        public static StylesInfo DefaultColumnHeaderStyle()
        {
            var style = new StylesInfo();
            style.Font.FontSize = 11;
            style.Font.Bold = true;
            style.Font.FontColor = XLColor.White;
            style.Fill.BackgroundColor = XLColor.FromArgb(68, 114, 196);
            return style;
        }
        public static StylesInfo DefaultValueCellStyle()
        {
            var style = new StylesInfo();
            style.Font.FontSize = 11;
            style.Font.Bold = true;
            style.Font.FontColor = XLColor.FromArgb(68, 84, 106);
            style.Border.BottomBorder = XLBorderStyleValues.Medium;
            return style;
        }
        public static StylesInfo DefaultTableHeaderStyle()
        {
            var style = new StylesInfo();
            style.Font.FontSize = 18;
            style.HorizontalAlignment = XLAlignmentHorizontalValues.Left;
            style.Fill.BackgroundColor = XLColor.FromArgb(231, 230, 230);
            style.Border.TopBorder = XLBorderStyleValues.Thin;
            style.Border.BottomBorder = XLBorderStyleValues.None;
            return style;
        }
        public static StylesInfo DefaultNotesTitleStyle()
        {
            var style = new StylesInfo();
            style.Font.FontSize = 18;
            style.Font.Italic = true;
            style.Fill.BackgroundColor = XLColor.FromArgb(221, 217, 196);
            style.HorizontalAlignment = XLAlignmentHorizontalValues.Left;
            style.Font.FontColor = XLColor.Black;
            return style;
        }
        public static StylesInfo DefaultNotesTableStyle()
        {
            var style = new StylesInfo
            {
                WrapText = true
            };
            style.Font.FontSize = 16;
            style.Font.Italic = true;
            style.HorizontalAlignment = XLAlignmentHorizontalValues.Left;
            style.Font.FontColor = XLColor.FromArgb(68, 84, 106);
            return style;
        }
        public static StylesInfo ReportTitleDefaultStyles()
        {
            var style = new StylesInfo();
            style.Font.FontSize = 18;
            style.Font.Bold = true;
            style.Fill.BackgroundColor = XLColor.FromArgb(221, 217, 196);
            style.HorizontalAlignment = XLAlignmentHorizontalValues.Left;
            style.VerticalAlignment = XLAlignmentVerticalValues.Center;
            return style;
        }
        /// <summary>
        /// The Default Borders on the Perimeter of the Values of a table
        /// </summary>
        /// <returns></returns>
        public static BorderStyles DefaultBordersTableValues()
        {
            BorderStyles borders = new();
            return borders;
        }

        /// <summary>
        /// Applies the Style Options to the given Style
        /// </summary>
        /// <param name="style"></param>
        /// <param name="options"></param>
        public static void ApplyStyle(this IXLStyle style, StylesInfo options)
        {
            style.Alignment.Horizontal = options.HorizontalAlignment;
            style.Alignment.Vertical = options.VerticalAlignment;
            style.Alignment.Indent = options.Indent;
            style.Alignment.JustifyLastLine = options.JustifyLastLine;
            style.Alignment.ShrinkToFit = options.ShrinkToFit;
            style.Alignment.TextRotation = options.TextRotation;
            style.Alignment.WrapText = options.WrapText;
            style.NumberFormat.Format = options.NumberFormat;

            //Omit the Pattern and Type and set only the background which will set the pattern type as solid 
            style.Fill.SetBackgroundColor(options.Fill.BackgroundColor);

            style.Font.Bold = options.Font.Bold;
            style.Font.Italic = options.Font.Italic;
            style.Font.Underline = options.Font.FontUnderline;
            style.Font.Strikethrough = options.Font.Strikethrough;
            style.Font.VerticalAlignment = options.Font.FontVerticalAlignment;
            style.Font.FontSize = options.Font.FontSize;
            style.Font.Shadow = options.Font.HasShadow;
            style.Font.FontColor = options.Font.FontColor;

            style.ApplyBorders(options.Border);
        }
        public static void ApplyBorders(this IXLStyle style, BorderStyles borders)
        {
            style.Border.BottomBorder = borders.BottomBorder;
            style.Border.BottomBorderColor = borders.BottomBorderColor;
            style.Border.TopBorder = borders.TopBorder;
            style.Border.TopBorderColor = borders.TopBorderColor;
            style.Border.LeftBorder = borders.LeftBorder;
            style.Border.LeftBorderColor = borders.LeftBorderColor;
            style.Border.RightBorder = borders.RightBorder;
            style.Border.RightBorderColor = borders.RightBorderColor;
            style.Border.DiagonalUp = borders.DiagonalUp;
            style.Border.DiagonalDown = borders.DiagonalDown;
            style.Border.DiagonalBorder = borders.DiagonalBorder;
            style.Border.DiagonalBorderColor = borders.DiagonalBorderColor;
        }
    }
}