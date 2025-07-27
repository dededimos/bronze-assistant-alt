using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BronzeFactoryApplication.ApplicationServices.JsonXmlSerilizers
{
    /// <summary>
    /// A Helper class to Serilize and Save Json and XML files
    /// </summary>
    public static class ObjectSerilizer
    {
        public static string GetJsonString<T>(T item)
        {
            return JsonSerializer.Serialize(item);
        }
        public static string GetXmlString<T>(T item)
        {
            XmlSerializer serilizer = new(typeof(T));

            using StringWriter writer = new();
            serilizer.Serialize(writer, item);
            return writer.ToString();
        }
        public static void SaveAsXml<T>(T item, string fullFileName)
        {
            XmlSerializer serilizer = new(typeof(T));
            using FileStream file = new(fullFileName, FileMode.Create);
            serilizer.Serialize(file, item);
        }
        public static void SaveAsJson<T>(T item, string fullFileName)
        {
            using FileStream file = new(fullFileName, FileMode.Create);
            using Utf8JsonWriter writer = new(file);
            JsonSerializer.Serialize(writer, item,new JsonSerializerOptions() { WriteIndented = true});
        }
    }

}
