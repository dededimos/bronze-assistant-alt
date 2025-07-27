using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BronzeFactoryApplication.Helpers.TemplateSelectors
{
    /// <summary>
    /// Returns a Certain Predefined Template when the DataContext is a FilterViewModel based on the Selected Property to Filter
    /// </summary>
    public class FilterBoxDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TextBoxTemplate { get; set; } = new();
        public DataTemplate ComboBoxEnumTemplate { get; set; } = new();
        public DataTemplate ComboBoxBoolTemplate { get; set; } = new();
        public DataTemplate DatePickerTemplate { get; set; } = new();

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is PropertyInfo property && property is not null)
            {
                var type = property.PropertyType;
                if (type == typeof(int) || type == typeof(double) || type == typeof(decimal) || type == typeof(string))
                {
                    //Return a Predefined TextBox Template for Input
                    //Numbers and should be only string without Constraining so to be able to input Operators (%>< e.t.c.)
                    return TextBoxTemplate;
                }
                //Some enums are Nullables so we check if nullable and Enum
                else if ((type != null && type.IsEnum)
                      || (type != null && Nullable.GetUnderlyingType(type)?.IsEnum is true ))
                {
                    //Return a Predefined ComboBox Template for Input of Enum
                    return ComboBoxEnumTemplate;
                }
                else if(type == typeof(bool))
                {
                    //Return a Predefined ComboBox Template for Input of Boolean
                    return ComboBoxBoolTemplate;
                }
                else if (type == typeof(DateTime))
                {
                    return DatePickerTemplate;
                }
            }
            return base.SelectTemplate(item, container);
        }
    }
}
