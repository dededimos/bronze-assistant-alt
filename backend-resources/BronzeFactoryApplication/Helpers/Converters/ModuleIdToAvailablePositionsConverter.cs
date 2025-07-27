using MirrorsLib.Enums;
using MirrorsLib.MirrorElements;
using MirrorsLib.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BronzeFactoryApplication.Helpers.Converters
{
    public class ModuleIdToAvailablePositionsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var moduleId = values[0] as string ?? string.Empty;
            MirrorOrientedShape shape = values[1] as MirrorOrientedShape? ?? MirrorOrientedShape.Undefined;
            if (string.IsNullOrEmpty(moduleId)) return new List<MirrorElementPosition>();

            var dataProvider = App.AppHost?.Services.GetRequiredService<IMirrorsDataProvider>();
            if (dataProvider is null)
            {
                Log.Warning("The returned Mirrors Data Provider was null");
                return new List<MirrorElementPosition>();
            }
            else
            {
                var posOptions = dataProvider.GetPositionOptionsOfElement(moduleId);
                return posOptions.GetAllAvailablePositionElements(shape);
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"{nameof(ModuleIdToAvailablePositionsConverter)} does not support two-way Binding");
        }
    }
}
