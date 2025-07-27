using DocumentFormat.OpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BronzeFactoryApplication.Helpers.Converters
{
    //THIS BREAKS NOW THAT DEFAULT LIST HAS CHANGED -- RETRIEVED PARTS HAVE TO CHECK DEFAULT LIST BUT ALSO USE THEIR OWN OLD OPTION
    //TO INCLUDE TO THEIR SELECTABLES ALSO THE VALUES THAT HAVE BEEN STORED TO THE DATABASE
    public class CabinPartAndPartsListToEnabledConverter : IMultiValueConverter
    {
        //The First Value is Binded to a Part
        //The Second to the PartsList
        //The Third is Binded to the Tag (usually of a comboBox) so to pass the Spot of the part
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //Check Weather the Passed in Part is able to Fit in the Designated Spot  based on the Parts List that is into
            if (values.Length == 3 && values[0] is CabinPart cabinPart
                && values[1] is PartsViewModel partsList)
            {
                if (values[2] is PartSpot spot)
                {
                    var defaults = partsList.GetDefaults();
                    if (defaults is not null && defaults.SpotDefaults.ContainsKey(spot))
                    {
                        return defaults.SpotDefaults[spot].IsPartCodeValid(cabinPart.Code);
                    }
                    return false;
                }
                else if (values[2] is DoublePartSpot spots)
                {
                    var defaults = partsList.GetDefaults();
                    if (defaults is not null && defaults.SpotDefaults.ContainsKey(spots.PartSpot1) && defaults.SpotDefaults.ContainsKey(spots.PartSpot2))
                    {
                        return (defaults.SpotDefaults[spots.PartSpot1].IsPartCodeValid(cabinPart.Code) 
                            || defaults.SpotDefaults[spots.PartSpot2].IsPartCodeValid(cabinPart.Code));
                    }
                    return false;
                }
                else return false;
            }

            // Else return that is not 
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public struct DoublePartSpot
    {
        public PartSpot PartSpot1 { get; set; }
        public PartSpot PartSpot2 { get; set; }
    }
}
