using MirrorsLib.Enums;
using MirrorsLib.MirrorElements;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BronzeFactoryApplication.Helpers.Converters
{
    public class MirrorPositionWithConcerningShapeToTupleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2)
            {
                throw new InvalidOperationException($"{this.GetType().Name} supports Two Values {nameof(MirrorOrientedShape)} and {nameof(MirrorElementPosition)} objects");
            }

            MirrorOrientedShape shape = (values[0] as MirrorOrientedShape?) ?? throw new InvalidOperationException($"First value in {this.GetType().Name} must be of type {typeof(MirrorOrientedShape).Name}");
            MirrorElementPosition position = (values[1] as MirrorElementPosition) ?? throw new InvalidOperationException($"Second value in {this.GetType().Name} must be of type {typeof(MirrorElementPosition).Name}");

            return (shape, position);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"{this.GetType().Name} does not support Two-Way Binding");
        }
    }
}
