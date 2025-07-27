using ClosedXML.Excel;
using CommunityToolkit.Diagnostics;
using System.Windows.Media;

namespace BronzeFactoryApplication.ApplicationServices.ExcelXlsService;

public static class ClosedXMLExtensions
{
    public static System.Windows.Media.Color ToMediaColor(this XLColor color) 
    {
        return Color.FromArgb(color.Color.A, color.Color.R, color.Color.G, color.Color.B);
    }

    public static XLColor ToXLColor(this SolidColorBrush brush)
    {
        return XLColor.FromArgb(brush.Color.A, brush.Color.R, brush.Color.G, brush.Color.B);
    }

    public static XLColor ToXLColor(this System.Windows.Media.Brush brush)
    {
        SolidColorBrush solidBrush = brush as SolidColorBrush ?? throw new Exception("Brush Provided was not a SolidColorBrush or it was Null");
        if (solidBrush is null) { ThrowHelper.ThrowInvalidOperationException("Cannot Cast Brush to SolidColorBrush when not of a Solid Color"); }
        return solidBrush.ToXLColor();
    }

    public static XLColor ToXLColor(this System.Windows.Media.Color color)
    {
        return XLColor.FromArgb(color.A, color.R, color.G, color.B);
    }
}