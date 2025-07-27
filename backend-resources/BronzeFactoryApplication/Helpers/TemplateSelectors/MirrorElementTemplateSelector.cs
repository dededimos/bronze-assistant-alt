using BronzeFactoryApplication.ViewModels.MirrorsViewModels.EntitiesViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BronzeFactoryApplication.Helpers.TemplateSelectors
{
    public class MirrorElementEntityTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (item != null && container is FrameworkElement element)
            {
                //Get the type of the ViewModel and traverse its inheritance tree
                Type? itemType = item.GetType();
                while (itemType != null && itemType != typeof(object))
                {
                    //The first iteration will return the Concerte type and in the end its base Type will be the Inherited class
                    //this will go on until it finds an inherited MirrorElementEntityBaseEditor or not 
                    if (itemType.IsGenericType && itemType.GetGenericTypeDefinition() == typeof(MirrorElementEntityBaseEditorViewModel<>))
                    {
                        return (DataTemplate)element.FindResource("MirrorElementEntityTemplate");
                    }
                    //Set the base type so next iteration can find weather its the correct type
                    itemType = itemType.BaseType;
                }
                return (DataTemplate)element.FindResource("EmptyDataTemplate");
            }
            return null;
        }
    }
    public class MirrorElementTraitEntityTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (item != null && container is FrameworkElement element)
            {
                //Get the type of the ViewModel and traverse its inheritance tree
                Type? itemType = item.GetType();
                while (itemType != null && itemType != typeof(object))
                {
                    //The first iteration will return the Concerte type and in the end its base Type will be the Inherited class
                    //this will go on until it finds an inherited MirrorElementEntityBaseEditor or not 
                    if (itemType.IsGenericType && itemType.GetGenericTypeDefinition() == typeof(MirrorElementTraitEntityEditorBaseViewModel<>))
                    {
                        return (DataTemplate)element.FindResource("MirrorElementTraitEntityTemplate");
                    }
                    //Set the base type so next iteration can find weather its the correct type
                    itemType = itemType.BaseType;
                }
                return (DataTemplate)element.FindResource("EmptyDataTemplate");
            }
            return null;
        }
    }
}
