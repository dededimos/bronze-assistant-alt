using CommonInterfacesBronze;
using MirrorsModelsLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsModelsLibrary.Models
{
    public class SupportModel : ICodeable
    {
        [Obsolete("Not used anymore", true)]
        public const decimal PaintExtraPriceCoefficient = 1m;
        [Obsolete("Not used anymore", true)]
        public const decimal ElectroplatedRealGoldCoefficient = 1.3m;

        public const decimal ChannelPlacementWhenH7WithFramePrice = 30m;
        public const decimal DoubleSupportPrice = 29m;
        public const decimal PerimetricalSupportMeterPrice = 15m;
        public const decimal PreimetricalSupportMinPrice = 50m;
        public const decimal FrontSupportsSetPrice = 16m;
        public const decimal RalColorAdditionalPrice = 80m;

        public MirrorSupport? SupportType { get; set; }
        public SupportFinishType? FinishType { get; set; }
        public SupportPaintFinish? PaintFinish { get; set; }
        public SupportElectroplatedFinish? ElectroplatedFinish { get; set; }
        public string Code { get => GetCode(); }

        /// <summary>
        /// Returns the Code of the Support
        /// </summary>
        /// <returns></returns>
        private string GetCode()
        {
            string code = SupportType switch
            {
                MirrorSupport.Double =>        "0000-ST",
                MirrorSupport.Perimetrical =>  "0050-FRAME",
                MirrorSupport.Frame =>         $"0050-{GetVisibleFinishCode()}-FRAME",
                MirrorSupport.FrontSupports => "0050-10",
                _ =>                           "Without",
            };
            return code;
        }

        /// <summary>
        /// Returns the sub Code of the Frame's Finish
        /// </summary>
        /// <returns></returns>
        private string GetVisibleFinishCode()
        {
            if (FinishType == SupportFinishType.Painted)
            {
                return PaintFinish switch
                {
                    SupportPaintFinish.Black =>       "MM",
                    SupportPaintFinish.ChromeMat =>   "A1",
                    SupportPaintFinish.GoldMat =>     "A2",
                    SupportPaintFinish.GraphiteMat => "A8",
                    SupportPaintFinish.BronzeMat =>   "A4",
                    SupportPaintFinish.CopperMat =>   "A5",
                    SupportPaintFinish.Silver =>      "S1",
                    SupportPaintFinish.RalColor =>    "RAL",
                    _ =>                              "XXX",
                };
            }
            else if (FinishType == SupportFinishType.Electroplated)
            {
                return ElectroplatedFinish switch
                {
                    SupportElectroplatedFinish.GraphiteBrushed =>    "81",
                    SupportElectroplatedFinish.GraphiteMirror =>     "80",
                    SupportElectroplatedFinish.NickelBrushed =>      "31",
                    SupportElectroplatedFinish.NickelMirror =>       "30",
                    SupportElectroplatedFinish.CopperBrushed =>      "51",
                    SupportElectroplatedFinish.CopperMirror =>       "50",
                    SupportElectroplatedFinish.BronzeBrushed =>      "41",
                    SupportElectroplatedFinish.BronzeMirror =>       "40",
                    SupportElectroplatedFinish.RoseGoldBrushed =>    "R1",
                    SupportElectroplatedFinish.RoseGoldMirror =>     "R0",
                    SupportElectroplatedFinish.GoldSimilarMirror =>  "G0",
                    SupportElectroplatedFinish.GoldSimilarBrushed => "G1",
                    SupportElectroplatedFinish.RealGoldMirror =>     "20",
                    SupportElectroplatedFinish.RealGoldBrushed =>    "21",
                    _ =>                                             "XXX",
                };
            }
            else
            {
                return "XXX";
            }
        }
    }
}
