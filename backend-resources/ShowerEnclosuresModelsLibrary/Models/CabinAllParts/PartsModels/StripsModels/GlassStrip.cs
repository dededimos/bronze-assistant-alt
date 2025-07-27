using ShowerEnclosuresModelsLibrary.Attributes;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.StripsModels
{
    /// <summary>
    /// A Strip - Polycarbonic or Metallic that fits onto one of the Glass Sides
    /// </summary>
    public class GlassStrip : CabinPart , IWithCutLength , IDeductableGlassesLength
    {
        public override CabinPartType Part { get => CabinPartType.Strip; }
        public CabinStripType StripType { get; private set; }

        public GlassStrip(CabinStripType stripType)
        {
            StripType = stripType;
        }

        /// <summary>
        /// How Long the Polycarbonic is Cut -- Usually takes the Height of the Glass
        /// </summary>
        public double CutLength { get; set; }
        /// <summary>
        /// How much it Protrudes from the Glass it is placed on (How much it extends the glasses Length)
        /// </summary>
        [Impact(ImpactOn.Glasses)]
        public int OutOfGlassLength { get; set; }

        /// <summary>
        /// How much its inside the Glass , this is Useful to Draw the Metal Magnet and then the Polycarbonic
        /// </summary>
        public int InGlassLength { get; set; }

        /// <summary>
        /// The Length of the Metal On the Strip
        /// </summary>
        public int MetalLength { get; set; }
        /// <summary>
        /// The Length of the Polycarbonic Plastic on the Strip
        /// When there is no Metal , then this is the Total Length of the Polycarbonic Strip
        /// </summary>
        public int PolycarbonicLength { get; set; }

        /// <summary>
        /// Returns the Length that will be Deducted from the Structure's LengthMin to determine the Total Glasses Length
        /// </summary>
        /// <param name="model">The Model of the Structure</param>
        /// <returns>The Deductible Length</returns>
        public double GetDeductableLength(CabinModelEnum model)
        {
            return OutOfGlassLength;
        }

        public override GlassStrip GetDeepClone()
        {
            return (GlassStrip)base.GetDeepClone();
        }

        //public override int GetHashCode()
        //{
        //    return HashCode.Combine(base.GetHashCode(), StripType, CutLength, OutOfGlassLength, InGlassLength, MetalLength, PolycarbonicLength);
        //}

        //public override bool Equals(object obj)
        //{
        //    if (obj is GlassStrip otherStrip)
        //    {
        //        return base.Equals(otherStrip) &&
        //            StripType == otherStrip.StripType &&
        //            CutLength == otherStrip.CutLength &&
        //            OutOfGlassLength == otherStrip.OutOfGlassLength &&
        //            InGlassLength == otherStrip.InGlassLength &&
        //            MetalLength == otherStrip.MetalLength &&
        //            PolycarbonicLength == otherStrip.PolycarbonicLength;
        //    }
        //    return false;
        //}
    }
}
