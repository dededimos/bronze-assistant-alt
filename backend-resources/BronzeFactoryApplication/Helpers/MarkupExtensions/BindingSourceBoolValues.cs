using System.Windows.Markup;

namespace BronzeFactoryApplication.Helpers.MarkupExtensions
{
    /// <summary>
    /// Markup Extension - Use this to Bind a Collection Field to the List of Boolean Values True or False
    /// </summary>
    public class BindingSourceBoolValues : MarkupExtension
    {
        public BindingSourceBoolValues()
        {
            
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            //Provide the List of Values to Bind From
            return new List<bool>(){ true , false };
        }
    }
}
