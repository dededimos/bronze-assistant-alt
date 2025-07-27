using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace BronzeFactoryApplication.Helpers.MarkupExtensions
{
    /// <summary>
    /// Markup Extension - Use this to Bind a Collection Field to the List of Values of an Enumeration
    /// </summary>
    public class BindingSourceEnumValues : MarkupExtension
    {
        public Type EnumType { get; private set; }

        public BindingSourceEnumValues(Type enumType)
        {
            if (enumType is null || enumType.IsEnum is false)
            {
                throw new InvalidOperationException($"{nameof(enumType)} must be not Null and Of Type Enum");
            }
            EnumType = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            //Provide the List of Values to Bind From
            return Enum.GetValues(EnumType);
        }
    }

    /// <summary>
    /// Markup Extension - Use this to Bind a Collection Field to the List of Values of an Enumeration
    /// </summary>
    public class BindingSourceEnumValuesNoZero : MarkupExtension
    {
        public Type EnumType { get; private set; }

        public BindingSourceEnumValuesNoZero(Type enumType)
        {
            if (enumType is null || enumType.IsEnum is false)
            {
                throw new InvalidOperationException($"{nameof(enumType)} must be not Null and Of Type Enum");
            }
            EnumType = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            // Retrieve all enum values and filter out those with an underlying value of zero using LINQ
            var values = Enum.GetValues(EnumType).Cast<Enum>();
            var nonZeroValues = values.Where(value => Convert.ToInt32(value) != 0).ToArray();
            return nonZeroValues;
        }
    }
}
