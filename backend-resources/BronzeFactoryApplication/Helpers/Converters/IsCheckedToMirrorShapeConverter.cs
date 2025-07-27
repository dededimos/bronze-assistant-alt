using MirrorsLib.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BronzeFactoryApplication.Helpers.Converters
{
    /// <summary>
    /// Converts a Boolean value to a the passed Parameter BronzeMirrorShape if the boolean is true
    /// </summary>
    public class IsCheckedToMirrorShapeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BronzeMirrorShape shape = value as BronzeMirrorShape? ?? BronzeMirrorShape.UndefinedMirrorShape;
            if (shape is BronzeMirrorShape.UndefinedMirrorShape)
            {
                return false;
            }
            else
            {
                BronzeMirrorShape param = parameter as BronzeMirrorShape? ?? BronzeMirrorShape.UndefinedMirrorShape;
                if (param is BronzeMirrorShape.UndefinedMirrorShape) return false;
                else return shape == param;
            }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isChecked && isChecked)
            {
                if (parameter is BronzeMirrorShape mirrorShape)
                {
                    return mirrorShape;
                }
            }
            return Binding.DoNothing;
        }
    }

}
