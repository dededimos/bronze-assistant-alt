using SVGDrawingLibrary.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace BronzeFactoryApplication.Helpers.Converters
{
    public class GlassToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) { return "Undefined Glass"; }
            else if (value is Glass glass)
            {
                string draw = glass.Draw.ToString().TryTranslateKey();
                if (draw.Length < 3)
                {
                    int drawLengthDiff = 3 - draw.Length;
                    var spaces = new string(' ', drawLengthDiff);
                    draw += spaces;
                }
                string finish = glass.Finish?.ToString().TryTranslateKey() ?? "UndefinedFinish";
                if (finish.Length < 12)
                {
                    int finishLengthDiff = 12 - finish.Length;
                    var spaces = new string(' ', finishLengthDiff);
                    finish += spaces;
                }
                string thickness = glass.Thickness?.ToString().TryTranslateKey() ?? "Undefinied Thickness";
                if (thickness.Length < 4)
                {
                    int thicknessLengthDiff = 4 - thickness.Length;
                    var spaces = new string(' ', thicknessLengthDiff);
                    thickness = spaces + thickness;
                }
                string length = glass.Length.ToString();
                if (length.Length < 4)
                {
                    int lengthLengthDiff = 4 - length.Length;
                    var spaces = new string(' ', lengthLengthDiff);
                    length = spaces + length;
                }
                string height = glass.Height.ToString();
                if (height.Length < 4)
                {
                    int heightLengthDiff = 4 - height.Length;
                    var spaces = new string(' ', heightLengthDiff);
                    height = spaces + height;
                }
                StringBuilder builder = new();
                builder.Append(draw).Append(' ')
                       .Append(finish).Append(' ')
                       .Append(length).Append(' ').Append('x').Append(' ')
                       .Append(height).Append(' ').Append('x').Append(' ')
                       .Append(thickness);
                return builder.ToString();
            }
            else
            {
                throw new Exception($"{nameof(GlassToStringConverter)} can Only Convert Glass Objects");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
