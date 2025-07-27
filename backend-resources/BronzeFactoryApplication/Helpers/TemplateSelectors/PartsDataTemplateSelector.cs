using BronzeFactoryApplication.ViewModels.ComponentsUCViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BronzeFactoryApplication.Helpers.TemplateSelectors
{
    /// <summary>
    /// When set in Xaml returns a certain Data template according to the Below Method
    /// </summary>
    public class PartsDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement? element = container as FrameworkElement;
            Log.Information(item.GetType().Name);
            if (element != null)
            {
                //switch (item)
                //{
                //    case Parts9SViewModel:
                //        return element.FindResource("Parts9SDataTemplate") as DataTemplate ?? throw new InvalidOperationException("Resource was not a DataTemplate");
                //    default:
                //        break;
                //}
            }

            return base.SelectTemplate(item, container);
        }
    }
}
