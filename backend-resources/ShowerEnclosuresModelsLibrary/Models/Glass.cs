using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonInterfacesBronze;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Helpers;
using ShowerEnclosuresModelsLibrary.Models.GlassProcesses;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using ShowerEnclosuresModelsLibrary.Validators;

namespace ShowerEnclosuresModelsLibrary.Models
{
    public class Glass : IDeepClonable<Glass>
    {//This Class is ONLY CREATED BY THE BUILDER CLASS

        public const int MinGlassLength = 100;
        public const int MinGlassHeight = 100;
        public const int MaxGlassLength = 2000;
        public const int MaxGlassHeight = 3000;

        public GlassDrawEnum Draw { get; set; }
        public GlassTypeEnum GlassType { get; set; }
        public GlassThicknessEnum? Thickness { get; set; }
        public GlassFinishEnum? Finish { get; set; }
        public int CornerRadiusTopRight { get; set; }
        public int CornerRadiusTopLeft { get; set; }
        public double Height { get; set; }
        
        private double length;
        public double Length { get => Math.Round(length,0,MidpointRounding.ToPositiveInfinity); set { if (value != length) length = value; } }
        public double StepHeight { get; set; }
        public double StepLength { get; set; }
        public bool HasStep { get=>IsWithStep();}
        public bool HasRounding { get => CornerRadiusTopRight > 0 || CornerRadiusTopLeft > 0; }

        /// <summary>
        /// The Effective SQM of a Glass , does not deduct any cuts or Holes e.t.c.
        /// </summary>
        public double EffectiveSQM { get => (Length * Height)/(1000*1000d); }

        public double EstimatedWeightKgr { get => GetWeight(); }

        /// <summary>
        /// Returns the Estimated Weight for this Glass (does not take Step Into Account)
        /// </summary>
        /// <returns></returns>
        private double GetWeight()
        {
            double weightPerSQM = 0;
            switch (Thickness)
            {
                case GlassThicknessEnum.Thick5mm:
                    weightPerSQM = 12.47d;
                    break;
                case GlassThicknessEnum.Thick6mm:
                    weightPerSQM = 15.00d;
                    break;
                case GlassThicknessEnum.Thick8mm:
                    weightPerSQM = 20.00d;
                    break;
                case GlassThicknessEnum.Thick10mm:
                    weightPerSQM = 25.00d;
                    break;
                case GlassThicknessEnum.ThickTenplex10mm:
                    weightPerSQM = 20.00d + 1d; //Equivalent to 4mm x 2pcs + 1Kgr
                    break;
                case null:
                case GlassThicknessEnum.GlassThicknessNotSet:
                default:
                    //Will return 0;
                    break;
            }
            return weightPerSQM * EffectiveSQM;
        }

        public override string ToString()
        {
            StringBuilder builder = new();
            return builder
                .Append(Draw)
                .Append(' ')
                .Append(Finish)
                .Append(' ')
                .Append(Length)
                .Append('x')
                .Append(Height)
                .Append('x')
                .Append(Thickness).ToString();
        }

        /// <summary>
        /// Gets if the Glass Has Step
        /// </summary>
        /// <returns></returns>
        private bool IsWithStep()
        {
            if (StepHeight > 0 && StepLength > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Compares this Glass to another, Returns true if they are the same false if not
        /// Comparison Done for All Properties Except QueueNo
        /// </summary>
        /// <param name="glass"></param>
        /// <returns></returns>
        public bool IsEqualGlass(Glass glassToCompare)
        {
            //if all properties are the same , glasses are the same 
            bool areEqual = (
                Draw == glassToCompare.Draw &&
                GlassType == glassToCompare.GlassType &&
                Thickness == glassToCompare.Thickness &&
                Finish == glassToCompare.Finish &&
                Height == glassToCompare.Height &&
                Length == glassToCompare.Length &&
                StepHeight == glassToCompare.StepHeight &&
                StepLength == glassToCompare.StepLength &&
                CornerRadiusTopLeft == glassToCompare.CornerRadiusTopLeft &&
                CornerRadiusTopRight == glassToCompare.CornerRadiusTopRight
                );
            return areEqual;
        }

        public override bool Equals(object obj)
        {
            if ((obj is null) || obj.GetType() != typeof(Glass) )
            {
                return false;
            }
            else
            {
                return IsEqualGlass(obj as Glass);
            }
        }

        /// <summary>
        /// Returns the HashCode for this Item
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int prime = 31;
                int hash = 17;

                hash = hash * prime + Draw.GetHashCode();
                hash = hash * prime + GlassType.GetHashCode();
                hash = hash * prime + (Thickness.HasValue ? Thickness.Value.GetHashCode() : 0);
                hash = hash * prime + (Finish.HasValue ? Finish.Value.GetHashCode() : 0);
                hash = hash * prime + CornerRadiusTopRight.GetHashCode();
                hash = hash * prime + CornerRadiusTopLeft.GetHashCode();
                hash = hash * prime + Height.GetHashCode();
                hash = hash * prime + Length.GetHashCode();
                hash = hash * prime + StepHeight.GetHashCode();
                hash = hash * prime + StepLength.GetHashCode();

                return hash;
            }
        }

        /// <summary>
        /// Returns a DeepClone of the Glass
        /// </summary>
        /// <returns></returns>
        public Glass GetDeepClone()
        {
            return (Glass)this.MemberwiseClone();
        }
    }

    /// <summary>
    /// An Object representing a Grouping Key so that Glasses can be placed in seperate groups based on their Draw/Thickness/Step Properties
    /// </summary>
    public class GlassGroupKey : IComparable<GlassGroupKey>
    {
        public GlassDrawEnum Draw { get; set; }
        public GlassThicknessEnum Thickness { get; set; }
        public GlassFinishEnum Finish { get; set; }
        public bool HasStep { get; set; }
        public string Lettering { get; set; }

        public GlassGroupKey(GlassDrawEnum draw, GlassThicknessEnum thickness, bool hasStep, GlassFinishEnum glassFinish,string lettering)
        {
            Draw = draw;
            Thickness = thickness;
            HasStep = hasStep;
            Finish = glassFinish;
            Lettering = lettering ?? "";
        }

        public override bool Equals(object obj)
        {
            if (obj is not GlassGroupKey other) return false;
            return Draw == other.Draw 
                && Thickness == other.Thickness 
                && Finish == other.Finish 
                && HasStep == other.HasStep
                && Lettering == other.Lettering;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Draw, Thickness, HasStep,Finish,Lettering);
        }

        public int CompareTo(GlassGroupKey other)
        {
            if (other is null)
            {
                return -1;
            }
            else if (this.Equals(other))
            {
                return 0;
            }
            else
            {
                //Seperate those with Different Step First the ones that do not Have
                if (this.HasStep && !other.HasStep)
                {
                    return 1;
                }
                else if (!this.HasStep && other.HasStep)
                {
                    return -1;
                }
                else
                {
                    //If Step is the Same Go with Thickness First the less Thickness
                    if (Thickness > other.Thickness)
                    {
                        return 1;
                    }
                    else if (Thickness < other.Thickness)
                    {
                        return -1;
                    }
                    else
                    {
                        //Then by Finish
                        if (Finish > other.Finish)
                        {
                            return 1;
                        }
                        else if (Finish < other.Finish)
                        {
                            return -1;
                        }
                        else
                        {
                            //Then By Draw
                            if (Draw > other.Draw)
                            {
                                return 1;
                            }
                            else if (Draw < other.Draw)
                            {
                                return -1;
                            }
                            else
                            {
                                //Then by Lettering (Only if are of the Same Draw and Lettering they will be at the same table)
                                return string.Compare(Lettering, other.Lettering, StringComparison.InvariantCulture);
                            }
                        }
                    }
                }
            }
        }
    }
}
