using HandyControl.Tools.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BronzeFactoryApplication.Helpers.TemplateSelectors
{
    public class ComboBoxTemplateSelector : DataTemplateSelector
    {
#nullable disable
        public DataTemplate SelectedItemTemplate { get; set; }
        public DataTemplate DropDownTemplate { get; set; }
#nullable enable
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            //It stops as soon as it finds a Combobox item otherwise when it finds the comboBox
            while (container is not null and not ComboBox and not ComboBoxItem)
                container = VisualTreeHelper.GetParent(container);

            bool isDropDownMenu = container is ComboBoxItem;
            //If it is a comobbox Item it returns the combobox item Template otherwiose the ComboBox
            return isDropDownMenu
                ? (DropDownTemplate ?? throw new InvalidOperationException($"{nameof(DropDownTemplate)} was Null"))
                : (SelectedItemTemplate ?? throw new InvalidOperationException($"{nameof(SelectedItemTemplate)} was Null"));
        }
    }
}
