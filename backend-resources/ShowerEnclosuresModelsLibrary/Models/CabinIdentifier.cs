using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models
{

    /// <summary>
    /// The struct with which a Model of a Cabin can be Identified , based on its Model/DrawNumber/SynthesisModel
    /// </summary>
    public struct CabinIdentifier
    {
#nullable enable
        public CabinModelEnum Model { get; set; }
        public CabinDrawNumber DrawNumber { get; set; }
        public CabinSynthesisModel SynthesisModel { get; set; }

        public CabinIdentifier(CabinModelEnum m, CabinDrawNumber d, CabinSynthesisModel sm)
        {
            Model = m;
            DrawNumber = d;
            SynthesisModel = sm;
        }

        public override string ToString()
        {
            return string.Join('|',Model.ToString(),DrawNumber.ToString(),SynthesisModel.ToString());
        }

        public override bool Equals(object? obj)
        {
            if (obj is not CabinIdentifier i) return false;

            return (this.Model == i.Model && this.DrawNumber == i.DrawNumber && this.SynthesisModel == i.SynthesisModel);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Model, DrawNumber, SynthesisModel);
        }

        public static bool operator ==(CabinIdentifier left, CabinIdentifier right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CabinIdentifier left, CabinIdentifier right)
        {
            return !(left == right);
        }
    }
}
