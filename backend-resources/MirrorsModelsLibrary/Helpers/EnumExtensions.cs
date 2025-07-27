using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsModelsLibrary.Helpers
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Get the Enums Description Attribute from its Value
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetEnumDescription(this Enum enumValue)
        {
            if (enumValue != null)
            {
                var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

                var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

                return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : enumValue.ToString();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get Description from enum Value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescFromEnumValue(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return value.ToString();
            }
        }

        public static T GetValueFromDescription<T>(string description) where T : Enum
        {
            
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
            //If T is Nullable it returns Null otherwise it returns the Enum Value at 0
            throw new ArgumentException($"Description {description} is Not Present for {nameof(T)}");
        }
    }
}
