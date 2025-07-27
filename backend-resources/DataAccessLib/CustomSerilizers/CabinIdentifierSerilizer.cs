using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLib.CustomSerilizers
{
    /// <summary>
    /// Serilizes and Deserilizes a CabinIdentifier , Use : BsonSerilizer.RegisterSerilizer(new CabinIdentifierSerilizer)
    /// </summary>
    public class CabinIdentifierSerilizer : StructSerializerBase<CabinIdentifier>
    {
        //THIS IS HOW IT SHOULD BE Deserilized WHEN NOT DICTIONARY KEY
        //public override CabinIdentifier Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        //{
        //    //Go to the Start of the CabinIdentifier Document
        //    context.Reader.ReadStartDocument();
        //    //Go to the Model Property
        //    context.Reader.ReadName(nameof(CabinIdentifier.Model));
        //    //Read its Value
        //    var model = context.Reader.ReadString();

        //    //Go to the Draw Property
        //    context.Reader.ReadName(nameof(CabinIdentifier.DrawNumber));
        //    //Read its Value
        //    var draw = context.Reader.ReadString();

        //    //Go to the SynthesisModel Property
        //    context.Reader.ReadName(nameof(CabinIdentifier.SynthesisModel));
        //    //Read its Value
        //    var synthesis = context.Reader.ReadString();

        //    //Read to the End of Document
        //    context.Reader.ReadEndDocument();

        //    CabinModelEnum modelEnum = (CabinModelEnum)Enum.Parse(typeof(CabinModelEnum), model);
        //    CabinDrawNumber drawEnum = (CabinDrawNumber)Enum.Parse(typeof(CabinDrawNumber), draw);
        //    CabinSynthesisModel synthesisEnum = (CabinSynthesisModel)Enum.Parse(typeof(CabinSynthesisModel), synthesis);
        //    return new CabinIdentifier(modelEnum, drawEnum, synthesisEnum);
        //}

        /// <summary>
        /// This is Deserilized for being a Dictionary Key
        /// </summary>
        /// <param name="context"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public override CabinIdentifier Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            //The Identifier is represented as a single String => read it
            var identifierString = context.Reader.ReadString();

            //Split the String into its Enum Components
            var enums = identifierString.Split('|');
            CabinModelEnum model = (CabinModelEnum)Enum.Parse(typeof(CabinModelEnum), enums[0]);
            CabinDrawNumber draw = (CabinDrawNumber)Enum.Parse(typeof(CabinDrawNumber), enums[1]);
            CabinSynthesisModel synthesisModel = (CabinSynthesisModel)Enum.Parse(typeof(CabinSynthesisModel), enums[2]);

            return new CabinIdentifier(model,draw,synthesisModel);
        }

        //THIS IS HOW IT SHOULD BE SERILIZED WHEN NOT DICTIONARY KEY
        //public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, CabinIdentifier value)
        //{
        //    //Write the Start of the Document
        //    context.Writer.WriteStartDocument();

        //    //Write the PropName of Model
        //    context.Writer.WriteName(nameof(CabinIdentifier.Model));
        //    //The Value
        //    context.Writer.WriteString(value.Model.ToString());

        //    //e.t.c...
        //    context.Writer.WriteName(nameof(CabinIdentifier.DrawNumber));
        //    context.Writer.WriteString(value.DrawNumber.ToString());
        //    context.Writer.WriteName(nameof(CabinIdentifier.SynthesisModel));
        //    context.Writer.WriteString(value.SynthesisModel.ToString());

        //    //Write the Close of the Document
        //    context.Writer.WriteEndDocument();
        //}

        /// <summary>
        /// This is Serilized for being a Dictionary Key
        /// </summary>
        /// <param name="context"></param>
        /// <param name="args"></param>
        /// <param name="value"></param>
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, CabinIdentifier value)
        {
            context.Writer.WriteString(value.ToString());
        }
    }
}
