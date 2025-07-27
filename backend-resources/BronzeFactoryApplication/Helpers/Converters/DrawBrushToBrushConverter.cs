using BronzeFactoryApplication.Helpers.Other;
using CommunityToolkit.Diagnostics;
using DrawingLibrary.Models.PresentationOptions;
using MirrorsRepositoryMongoDB.Entities;
using ShapesLibrary.Services;
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
    public class DrawBrushToBrushConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Return Red if it Fails , return Brush in Solid othwerwise create a Gradient
            if (value is DrawBrush brush)
            {
                if (brush.IsSolidColor)
                {
                    if (string.IsNullOrWhiteSpace(brush.Color)) return Brushes.Transparent;
                    return new SolidColorBrush(WPFHelpers.ConvertHexadecimalStringToColor(brush.Color));
                }
                else
                {
                    LinearGradientBrush gradient = new();
                    (PointXY start, PointXY end) = DrawBrush.GetGradientAngleNormalizedPoints(brush.GradientAngleDegrees);
                    gradient.StartPoint = new(start.X, start.Y);
                    gradient.EndPoint = new(end.X, end.Y);

                    foreach (var stop in brush.GradientStops)
                    {
                        var wpfStop = new GradientStop(WPFHelpers.ConvertHexadecimalStringToColor(stop.Color), stop.Offset);
                        gradient.GradientStops.Add(wpfStop);
                    }
                    return gradient;
                }
            }
            else if (value is DrawBrushDTO brushDto)
            {
                brush = brushDto.ToDrawBrush();
                if (brush.IsSolidColor)
                {
                    if (string.IsNullOrWhiteSpace(brush.Color)) return Brushes.Transparent;
                    return new SolidColorBrush(WPFHelpers.ConvertHexadecimalStringToColor(brush.Color));
                }
                else
                {
                    LinearGradientBrush gradient = new();
                    (PointXY start, PointXY end) = DrawBrush.GetGradientAngleNormalizedPoints(brush.GradientAngleDegrees);
                    gradient.StartPoint = new(start.X, start.Y);
                    gradient.EndPoint = new(end.X, end.Y);

                    foreach (var stop in brush.GradientStops)
                    {
                        var wpfStop = new GradientStop(WPFHelpers.ConvertHexadecimalStringToColor(stop.Color), stop.Offset);
                        gradient.GradientStops.Add(wpfStop);
                    }
                    return gradient;
                }
            }
            else
            {
                return Brushes.Transparent;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"{nameof(DrawBrushToBrushConverter)} does not Support two Way Binding");
        }
    }
}
