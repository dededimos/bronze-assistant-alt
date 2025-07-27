using CommonInterfacesBronze;
using MirrorsModelsLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MirrorsModelsLibrary.StaticData.MirrorsStaticData;

namespace MirrorsModelsLibrary.Models
{
    public class Mirror : ICodeable
    {
        public const int MaxLength = 220;
        public const int MaxHeight = 220;
        public const int MinLength = 40;
        public const int MinHeight = 40;
        public const int MaxDiameter = 220;
        public const int MinDiameter = 40;
        public const int MaxMinFrameDimension = 180;
        //The Model always supposes a Mirror is Customized except if Explicitly told it is not.

        public MirrorSeries? Series { get; set; }
        public MirrorShape? Shape { get; set; }
        public int? Length { get; set; }
        public int? Height { get; set; }
        public int? Diameter {get; set;}

        public MirrorSandblast? Sandblast { get; set; }
        public SupportModel Support { get; set; } = new();
        public LightingModel Lighting { get; set; } = new();
        public List<MirrorExtra> Extras { get; set; } =[];
        public string Code { get; set; }
        public double Lengthmm {
            get
            {
                if (Length is not null)
                {
                    return (int)Length * 10d;
                }
                else
                {
                    return 0;
                }
            } 
        }
        public double Heightmm
        {
            get
            {
                if (Height is not null)
                {
                    return (int)Height * 10d;
                }
                else
                {
                    return 0;
                }
            }
        }
        public double Diametermm
        {
            get
            {
                if (Diameter is not null)
                {
                    return (int)Diameter * 10d;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Returns whether the Specified Option is Included in the Extras List of the Mirror
        /// </summary>
        /// <param name="option">The Mirror Option to check for</param>
        /// <returns>True if the Mirror Has the Extra , false if it doesn't</returns>
        public bool HasExtra(MirrorOption option)
        {
            bool hasExtra = Extras is not null && Extras.Any(e => e.Option == option);
            return hasExtra;
        }

        /// <summary>
        /// Returns wheather the Mirror contains any of the selected Extras
        /// </summary>
        /// <param name="option1">1st Extra to Check</param>
        /// <param name="option2">2nd</param>
        /// <param name="option3">3rd</param>
        /// <returns>true if Any or Multiple of the Extras exists in the Mirror</returns>
        public bool HasExtrasAny(MirrorOption option1,MirrorOption? option2 = null,MirrorOption? option3 = null,MirrorOption? option4=null,MirrorOption? option5 = null )
        {
            bool hasExtras = Extras is not null && Extras.Any(e => 
                e.Option == option1 || 
                e.Option == option2 ||
                e.Option == option3 ||
                e.Option == option4 ||
                e.Option == option5);
            return hasExtras;
        }
        /// <summary>
        /// Returns wheather the Mirror contains DOES NOT Contain ANy of the provided Extras
        /// <para>If at least one is present the method returns false</para>
        /// </summary>
        /// <param name="options"></param>
        /// <returns>True if the mirror has not any of the provided options , false if it has at least one of them</returns>
        public bool HasNotAny(params MirrorOption[] options)
        {
            if (Extras is null) return true;
            return !Extras.Any(e => options.Contains(e.Option));
        }

        /// <summary>
        /// Wheather the Mirror has a Visible Frame or Not
        /// </summary>
        /// <returns>True if it has a Visible Frame , false Otherwise</returns>
        public bool HasVisibleFrame()
        {
            if (Support.SupportType == MirrorSupport.Frame)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns Wheather the Mirror has a Magnifyer Extra
        /// </summary>
        /// <returns>True if it has -- Flase otherwise</returns>
        public bool HasMagnifyer()
        {
            bool hasMagnifyer = HasExtra(MirrorOption.Zoom) || HasExtra(MirrorOption.ZoomLed) || HasExtra(MirrorOption.ZoomLedTouch);
            return hasMagnifyer;
        }

        /// <summary>
        /// Returns Wheather the Mirror has a Magnifyer with Light Extra
        /// </summary>
        /// <returns>True if it has -- Flase otherwise</returns>
        public bool HasMagnifyerWithLight()
        {
            bool hasMagnifyerWithLight = HasExtra(MirrorOption.ZoomLed) || HasExtra(MirrorOption.ZoomLedTouch);
            return hasMagnifyerWithLight;
        }

        /// <summary>
        /// Returns weather the mirror has A Sandblast Design
        /// </summary>
        /// <returns>True if it has -- Flase otherwise</returns>
        public bool HasSandblast()
        {
            bool hasSandblast = Sandblast is not MirrorSandblast.H7 and not MirrorSandblast.N9 and not MirrorSandblast.Special and not null;
            return hasSandblast;
        }

        /// <summary>
        /// Returns weather the Mirror has Light or Not
        /// </summary>
        /// <returns></returns>
        public bool HasLight()
        {
            if (Lighting is not null && Lighting.Light is not null and not MirrorLight.WithoutLight)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns wheather the Mirror Has a Back Support
        /// </summary>
        /// <returns>True if there is a Back Support -- Flase if not any Support or Front Support Only</returns>
        public bool HasBackSupport()
        {
            if (Support is not null && Support.SupportType is not null and not MirrorSupport.Without and not MirrorSupport.FrontSupports)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns wheather the Mirror has Any Kind of Support
        /// </summary>
        /// <returns></returns>
        public bool HasSupport()
        {
            if (Support is not null && Support.SupportType is not null and not MirrorSupport.Without)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Matches the series with a certain Shape
        /// </summary>
        /// <param name="series"></param>
        /// <returns></returns>
        public static MirrorShape GetSeriesShape(MirrorSeries series)
        {
            return series switch
            {
                MirrorSeries.H7 => MirrorShape.Rectangular,
                MirrorSeries.H8 => MirrorShape.Rectangular,
                MirrorSeries.X6 => MirrorShape.Rectangular,
                MirrorSeries.X4 => MirrorShape.Rectangular,
                MirrorSeries._6000 => MirrorShape.Rectangular,
                MirrorSeries.M3 => MirrorShape.Rectangular,
                MirrorSeries.N9 => MirrorShape.Circular,
                MirrorSeries.N6 => MirrorShape.Circular,
                MirrorSeries.N7 => MirrorShape.Circular,
                MirrorSeries.A7 => MirrorShape.Circular,
                MirrorSeries.A9 => MirrorShape.Circular,
                MirrorSeries.M8 => MirrorShape.Rectangular,
                MirrorSeries.ND => MirrorShape.PebbleND,
                MirrorSeries.NC => MirrorShape.Capsule,
                MirrorSeries.NL => MirrorShape.Ellipse,
                MirrorSeries.Custom => MirrorShape.Special,
                MirrorSeries.P8 => MirrorShape.Rectangular,
                MirrorSeries.P9 => MirrorShape.Circular,
                MirrorSeries.R7 => MirrorShape.Rectangular,
                MirrorSeries.R9 => MirrorShape.Circular,
                MirrorSeries.NS => MirrorShape.StoneNS,
                MirrorSeries.N1 => MirrorShape.CircleSegment,
                MirrorSeries.N2 => MirrorShape.CircleSegment2,
                MirrorSeries.EL => MirrorShape.Rectangular,
                MirrorSeries.ES => MirrorShape.Rectangular,
                MirrorSeries.IM => MirrorShape.Rectangular,
                MirrorSeries.IA => MirrorShape.Rectangular,
                MirrorSeries.N9Custom => MirrorShape.Circular,
                MirrorSeries.NA => MirrorShape.Circular,
                MirrorSeries.NCCustom => MirrorShape.Capsule,
                MirrorSeries.IC => MirrorShape.Capsule,
                MirrorSeries.NLCustom => MirrorShape.Ellipse,
                MirrorSeries.IL => MirrorShape.Ellipse,
                _ => MirrorShape.Special,
            };
        }
    }
}
