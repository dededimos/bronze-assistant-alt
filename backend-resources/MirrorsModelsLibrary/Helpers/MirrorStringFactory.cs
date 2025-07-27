using MirrorsModelsLibrary.Enums;
using MirrorsModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsModelsLibrary.Helpers
{
    /// <summary>
    /// Generates a Mirror from a String or String from a Mirror
    /// </summary>
    public static class MirrorStringFactory
    {
        /// <summary>
        /// The Fallback String value When the Value is Null
        /// </summary>
        private static readonly string fallbackValue = "null";
        /// <summary>
        /// The various Mirror Attributes Seperator
        /// </summary>
        private static readonly char seperator = '-';

        public static string GenerateStringFromMirror(Mirror mirror)
        {
            StringBuilder builder = new();
            
            //1.MirrorSeries
            builder.Append(mirror.Series is not null ? (int)mirror.Series : fallbackValue)
                   .Append(seperator);

            //2.Shape
            builder.Append(mirror.Shape is not null ? (int)mirror.Shape : fallbackValue)
                   .Append(seperator);

            //3.Length
            builder.Append(mirror.Length is not null ? mirror.Length : fallbackValue)
                   .Append(seperator);
            //4.Height
            builder.Append(mirror.Height is not null  ? mirror.Height : fallbackValue)
                   .Append(seperator);
            //5.Diameter
            builder.Append(mirror.Diameter is not null ? mirror.Diameter : fallbackValue)
                   .Append(seperator);

            //6.Sandblast
            builder.Append(mirror.Sandblast is not null ? (int)mirror.Sandblast : fallbackValue)
                   .Append(seperator);

            //Support
            if (mirror.Support != null)
            {
                //7.SupportType
                builder.Append(mirror.Support.SupportType is not null ? (int)mirror.Support.SupportType : fallbackValue)
                       .Append(seperator);
                //8.SupportFinish
                builder.Append(mirror.Support.FinishType is not null ? (int)mirror.Support.FinishType : fallbackValue)
                       .Append(seperator);
                //9.SupportPaintFinish
                builder.Append(mirror.Support.PaintFinish is not null ? (int)mirror.Support.PaintFinish : fallbackValue)
                       .Append(seperator);
                //10.SupportElectroplatedFinish
                builder.Append(mirror.Support.ElectroplatedFinish is not null ? (int)mirror.Support.ElectroplatedFinish : fallbackValue)
                       .Append(seperator);
            }
            else
            {
                //If Support is null - everything else is null
                builder.Append(fallbackValue).Append(seperator)
                       .Append(fallbackValue).Append(seperator)
                       .Append(fallbackValue).Append(seperator)
                       .Append(fallbackValue).Append(seperator);
            }

            //11.Lighting
            builder.Append(mirror.Lighting?.Light is not null ? (int)mirror.Lighting.Light : fallbackValue)
                   .Append(seperator);

            //12+Extras
            foreach (MirrorExtra extra in mirror.Extras)
            {
                builder.Append((int)extra.Option).Append(seperator);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Generates a Mirror from a String with Values Coming From this FactoryClass
        /// </summary>
        /// <param name="mirrorString">The Mirror String</param>
        /// <returns>The Mirror</returns>
        /// <exception cref="ArgumentException">Invalid String</exception>
        public static Mirror GenerateMirrorFromString(string mirrorString)
        {
            Mirror mirror = new();
            string[] values = mirrorString.Split(seperator);
            if (values.Length < 11) { throw new ArgumentException("Invalid Link string"); }
            string series = values[0];
            string shape = values[1];
            string length = values[2];
            string height = values[3];
            string diameter = values[4];
            string sandblast = values[5];
            string supportType = values[6];
            string supportFinishType = values[7];
            string supportPaintFinish = values[8];
            string supportElectroplatedFinish = values[9];
            string lighting = values[10];
            //Extras to Mirror
            string[] extras = values.Skip(11).ToArray(); //Seperate the Extras 
            foreach (string extra in extras)
            {
                bool isParsable = Enum.TryParse(extra, out MirrorOption option);
                if (isParsable)
                {
                    MirrorExtra mirrorExtra = new(option);
                    mirror.Extras.Add(mirrorExtra);
                }
            }
            //Rest Values
            mirror.Series = Enum.TryParse(series, out MirrorSeries mirrorSeries) ? mirrorSeries : null;
            mirror.Shape = Enum.TryParse(shape, out MirrorShape mirrorShape) ? mirrorShape: null;
            mirror.Length  = int.TryParse(length, out int mirrorLength) ? mirrorLength : null;
            mirror.Height = int.TryParse(height, out int mirrorHeight) ? mirrorHeight : null;
            mirror.Diameter = int.TryParse(diameter, out int mirrorDiameter) ? mirrorDiameter : null;
            mirror.Sandblast = Enum.TryParse(sandblast, out MirrorSandblast mirrorSandblast) ? mirrorSandblast : null;
            mirror.Support.SupportType = Enum.TryParse(supportType, out MirrorSupport mirrorSupportType) ? mirrorSupportType : null;
            mirror.Support.FinishType = Enum.TryParse(supportFinishType, out SupportFinishType mirrorSupportFinishType) ? mirrorSupportFinishType : null;
            mirror.Support.PaintFinish = Enum.TryParse(supportPaintFinish, out SupportPaintFinish mirrorSupportPaintFinish) ? mirrorSupportPaintFinish : null;
            mirror.Support.ElectroplatedFinish = Enum.TryParse(supportElectroplatedFinish, out SupportElectroplatedFinish mirrorSupportElectroplatedFinish) ? mirrorSupportElectroplatedFinish : null;
            mirror.Lighting.Light = Enum.TryParse(lighting, out MirrorLight mirrorLight) ? mirrorLight : null;

            return mirror;
        }
    }
}
