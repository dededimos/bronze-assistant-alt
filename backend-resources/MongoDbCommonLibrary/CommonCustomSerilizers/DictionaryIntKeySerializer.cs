using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbCommonLibrary.CommonCustomSerilizers
{
    /// <summary>
    /// A Mongo Serilizer for Dictionaries with an Int as Key instead of a string
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class DictionaryIntKeySerializer<TValue> : SerializerBase<Dictionary<int,TValue>>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Dictionary<int, TValue> value)
        {
            if (value == null) 
            {
                context.Writer.WriteNull();
                return;
            }

            context.Writer.WriteStartDocument();
            foreach (var kvp in value)
            {
                context.Writer.WriteName(kvp.Key.ToString());
                //Serilize the value with its default or registered serilizer ?!
                BsonSerializer.Serialize(context.Writer, kvp.Value); 
            }
            context.Writer.WriteEndDocument();
        }
        public override Dictionary<int, TValue> Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var dictionary = new Dictionary<int, TValue>();
            var reader = context.Reader;

            // Handle null
            if (reader.CurrentBsonType == BsonType.Null)
            {
                reader.ReadNull();
                return dictionary;
            }

            // 1. Start reading the document
            reader.ReadStartDocument();

            // 2. Keep reading until we encounter EndOfDocument
            while (true)
            {
                var bsonType = reader.ReadBsonType();

                // 3. Break if BsonType is EndOfDocument
                if (bsonType == BsonType.EndOfDocument)
                {
                    break;
                }

                // 4. Read the field name (key)
                var keyString = reader.ReadName();
                if (!int.TryParse(keyString, out var key))
                {
                    throw new FormatException($"Invalid key format: {keyString}. Expected an integer.");
                }

                // 5. Deserialize the value
                var value = BsonSerializer.Deserialize<TValue>(reader);
                dictionary[key] = value;
            }

            // 6. End reading the document
            reader.ReadEndDocument();

            return dictionary;
        }
    }
}
