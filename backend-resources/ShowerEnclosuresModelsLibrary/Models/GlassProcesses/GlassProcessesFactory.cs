using ShowerEnclosuresModelsLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ShowerEnclosuresModelsLibrary.Models.GlassProcesses.ConstantValues.GlassProcessesConstants;

namespace ShowerEnclosuresModelsLibrary.Models.GlassProcesses
{
    /// <summary>
    /// Contains Methods to get the Processes applied to a certain Glass
    /// </summary>
    public static class GlassProcessesFactory
    {
        /// <summary>
        /// Returns the Processes of the glass 
        /// </summary>
        /// <param name="glass">The Glass</param>
        /// <returns></returns>
        public static IEnumerable<GlassProcess> GetProcesses(this Glass glass)
        {
            var processes = GetDefaultGlassProcesses(glass.Draw);
            if (glass.HasStep)
            {
                var stepProcess = new StepProcess(glass.StepLength, glass.StepHeight);
                processes.Add(stepProcess);
            }
            return processes;
        }


        /// <summary>
        /// Gets the List of Processes Applied to a specific Glass.
        /// Current Version Gets the Processes based on the Provided Draw . The correct way would be to apply further conditions based on the Glass Dimensions Also
        /// </summary>
        /// <param name="draw"></param>
        /// <returns></returns>
        public static List<GlassProcess> GetDefaultGlassProcesses(GlassDrawEnum draw)
        {
            List<GlassProcess> processes = new();
            switch (draw)
            {
                case GlassDrawEnum.Draw9S:
                    processes.AddRange(Get9SHoles());
                    return processes;
                case GlassDrawEnum.Draw94:
                    processes.AddRange(Get94Holes());
                    return processes;
                case GlassDrawEnum.Draw9B:
                    processes.AddRange(Get9BHoles());
                    processes.AddRange(Get9BCuts());
                    return processes;
                case GlassDrawEnum.DrawVS:
                    processes.AddRange(GetVSHoles());
                    return processes;
                case GlassDrawEnum.DrawVA:
                    processes.AddRange(GetVAHoles());
                    return processes;
                case GlassDrawEnum.DrawVF:
                    processes.AddRange(GetVFHoles());
                    return processes;
                case GlassDrawEnum.DrawWS:
                    processes.AddRange(GetWSHoles());
                    return processes;
                case GlassDrawEnum.DrawHB2:
                    processes.AddRange(GetHB2Holes());
                    processes.AddRange(GetHB2Cuts());
                    return processes;
                case GlassDrawEnum.DrawDP1:
                    processes.AddRange(GetDP1Holes());
                    return processes;
                case GlassDrawEnum.DrawDP3:
                    processes.AddRange(GetDP3Holes());
                    return processes;
                case GlassDrawEnum.DrawNB:
                    processes.AddRange(GetNBHoles());
                    return processes;
                case GlassDrawEnum.DrawDB:
                    processes.AddRange(GetDBHoles());
                    processes.AddRange(GetDBCuts());
                    return processes;
                case GlassDrawEnum.DrawH1:
                    processes.AddRange(GetH1Holes());
                    return processes;
                case GlassDrawEnum.DrawHB1:
                    processes.AddRange(GetHB1Cuts());
                    return processes;
                case GlassDrawEnum.DrawFL:
                    processes.AddRange(GetWFlipperHoles());
                    return processes;
                case GlassDrawEnum.DrawF:
                case GlassDrawEnum.Draw9C:
                case GlassDrawEnum.DrawNV:
                case GlassDrawEnum.DrawNotSet:
                    return processes;
                default:
                    throw new InvalidOperationException($"Unrecognized Glass Draw - Cannot Get List of {nameof(GlassProcess)}");
            }
        }

        #region 1. Get Holes per GlassDraw Methods

        private static List<GlassHole> Get9SHoles()
        {
            List<GlassHole> holes9S = new();

            GlassHole topLeftWheelHole1 = new()
            {
                Diameter = ProcessesB6000.WheelHoleDiameter,
                VerticalDistancing = VertDistancing.FromTop,
                VerticalDistance = ProcessesB6000.WheelHoleTopDistance,
                HorizontalDistancing = HorizDistancing.FromLeft,
                HorizontalDistance = ProcessesB6000.WheelHoleLeftDistance9S
            };
            GlassHole topLeftWheelHole2 = new()
            {
                Diameter = ProcessesB6000.WheelHoleDiameter,
                VerticalDistancing = VertDistancing.FromTop,
                VerticalDistance = ProcessesB6000.WheelHoleTopDistance,
                HorizontalDistancing = HorizDistancing.FromLeft,
                HorizontalDistance = ProcessesB6000.WheelHoleLeftDistance9S + ProcessesB6000.WheelHoleBetweenDistance
            };
            GlassHole topRightWheelHole1 = new()
            {
                Diameter = ProcessesB6000.WheelHoleDiameter,
                VerticalDistancing = VertDistancing.FromTop,
                VerticalDistance = ProcessesB6000.WheelHoleTopDistance,
                HorizontalDistancing = HorizDistancing.FromRight,
                HorizontalDistance = ProcessesB6000.WheelHoleRightDistance
            };
            GlassHole topRightWheelHole2 = new()
            {
                Diameter = ProcessesB6000.WheelHoleDiameter,
                VerticalDistancing = VertDistancing.FromTop,
                VerticalDistance = ProcessesB6000.WheelHoleTopDistance,
                HorizontalDistancing = HorizDistancing.FromRight,
                HorizontalDistance = ProcessesB6000.WheelHoleRightDistance + ProcessesB6000.WheelHoleBetweenDistance
            };
            GlassHole bottomLeftWheelHole1 = new()
            {
                Diameter = ProcessesB6000.WheelHoleDiameter,
                VerticalDistancing = VertDistancing.FromBottom,
                VerticalDistance = ProcessesB6000.WheelHoleBottomDistance,
                HorizontalDistancing = HorizDistancing.FromLeft,
                HorizontalDistance = ProcessesB6000.WheelHoleLeftDistance9S
            };
            GlassHole bottomLeftWheelHole2 = new()
            {
                Diameter = ProcessesB6000.WheelHoleDiameter,
                VerticalDistancing = VertDistancing.FromBottom,
                VerticalDistance = ProcessesB6000.WheelHoleBottomDistance,
                HorizontalDistancing = HorizDistancing.FromLeft,
                HorizontalDistance = ProcessesB6000.WheelHoleLeftDistance9S + ProcessesB6000.WheelHoleBetweenDistance
            };
            GlassHole bottomRightWheelHole1 = new()
            {
                Diameter = ProcessesB6000.WheelHoleDiameter,
                VerticalDistancing = VertDistancing.FromBottom,
                VerticalDistance = ProcessesB6000.WheelHoleBottomDistance,
                HorizontalDistancing = HorizDistancing.FromRight,
                HorizontalDistance = ProcessesB6000.WheelHoleRightDistance
            };
            GlassHole bottomRightWheelHole2 = new()
            {
                Diameter = ProcessesB6000.WheelHoleDiameter,
                VerticalDistancing = VertDistancing.FromBottom,
                VerticalDistance = ProcessesB6000.WheelHoleBottomDistance,
                HorizontalDistancing = HorizDistancing.FromRight,
                HorizontalDistance = ProcessesB6000.WheelHoleRightDistance + ProcessesB6000.WheelHoleBetweenDistance
            };
            GlassHole middleUpRightHandleHole = new()
            {
                Diameter = ProcessesB6000.HandleHoleDiameter,
                VerticalDistancing = VertDistancing.FromMiddleUp,
                VerticalDistance = ProcessesB6000.HandleHoleBetweenDistance / 2,
                HorizontalDistancing = HorizDistancing.FromRight,
                HorizontalDistance = ProcessesB6000.HandleHoleRightDistance
            };
            GlassHole middleDownRightHandleHole = new()
            {
                Diameter = ProcessesB6000.HandleHoleDiameter,
                VerticalDistancing = VertDistancing.FromMiddleDown,
                VerticalDistance = ProcessesB6000.HandleHoleBetweenDistance / 2,
                HorizontalDistancing = HorizDistancing.FromRight,
                HorizontalDistance = ProcessesB6000.HandleHoleRightDistance
            };
            holes9S.Add(topLeftWheelHole1);
            holes9S.Add(topLeftWheelHole2);
            holes9S.Add(topRightWheelHole1);
            holes9S.Add(topRightWheelHole2);
            holes9S.Add(bottomLeftWheelHole1);
            holes9S.Add(bottomLeftWheelHole2);
            holes9S.Add(bottomRightWheelHole1);
            holes9S.Add(bottomRightWheelHole2);
            holes9S.Add(middleUpRightHandleHole);
            holes9S.Add(middleDownRightHandleHole);

            return holes9S;
        }
        private static List<GlassHole> Get94Holes()
        {
            List<GlassHole> holes94 = new();

            GlassHole topLeft1 = new()
            {
                Diameter = ProcessesB6000.WheelHoleDiameter,
                VerticalDistancing = VertDistancing.FromTop,
                VerticalDistance = ProcessesB6000.WheelHoleTopDistance,
                HorizontalDistancing = HorizDistancing.FromLeft,
                HorizontalDistance = ProcessesB6000.WheelHoleLeftDistance94
            };
            GlassHole topLeft2 = new()
            {
                Diameter = ProcessesB6000.WheelHoleDiameter,
                VerticalDistancing = VertDistancing.FromTop,
                VerticalDistance = ProcessesB6000.WheelHoleTopDistance,
                HorizontalDistancing = HorizDistancing.FromLeft,
                HorizontalDistance = ProcessesB6000.WheelHoleLeftDistance94 + ProcessesB6000.WheelHoleBetweenDistance
            };
            GlassHole topRight1 = new()
            {
                Diameter = ProcessesB6000.WheelHoleDiameter,
                VerticalDistancing = VertDistancing.FromTop,
                VerticalDistance = ProcessesB6000.WheelHoleTopDistance,
                HorizontalDistancing = HorizDistancing.FromRight,
                HorizontalDistance = ProcessesB6000.WheelHoleRightDistance
            };
            GlassHole topRight2 = new()
            {
                Diameter = ProcessesB6000.WheelHoleDiameter,
                VerticalDistancing = VertDistancing.FromTop,
                VerticalDistance = ProcessesB6000.WheelHoleTopDistance,
                HorizontalDistancing = HorizDistancing.FromRight,
                HorizontalDistance = ProcessesB6000.WheelHoleRightDistance + ProcessesB6000.WheelHoleBetweenDistance
            };
            GlassHole bottomLeft1 = new()
            {
                Diameter = ProcessesB6000.WheelHoleDiameter,
                VerticalDistancing = VertDistancing.FromBottom,
                VerticalDistance = ProcessesB6000.WheelHoleBottomDistance,
                HorizontalDistancing = HorizDistancing.FromLeft,
                HorizontalDistance = ProcessesB6000.WheelHoleLeftDistance94
            };
            GlassHole bottomLeft2 = new()
            {
                Diameter = ProcessesB6000.WheelHoleDiameter,
                VerticalDistancing = VertDistancing.FromBottom,
                VerticalDistance = ProcessesB6000.WheelHoleBottomDistance,
                HorizontalDistancing = HorizDistancing.FromLeft,
                HorizontalDistance = ProcessesB6000.WheelHoleLeftDistance94 + ProcessesB6000.WheelHoleBetweenDistance
            };
            GlassHole bottomRight1 = new()
            {
                Diameter = ProcessesB6000.WheelHoleDiameter,
                VerticalDistancing = VertDistancing.FromBottom,
                VerticalDistance = ProcessesB6000.WheelHoleBottomDistance,
                HorizontalDistancing = HorizDistancing.FromRight,
                HorizontalDistance = ProcessesB6000.WheelHoleRightDistance
            };
            GlassHole bottomRight2 = new()
            {
                Diameter = ProcessesB6000.WheelHoleDiameter,
                VerticalDistancing = VertDistancing.FromBottom,
                VerticalDistance = ProcessesB6000.WheelHoleBottomDistance,
                HorizontalDistancing = HorizDistancing.FromRight,
                HorizontalDistance = ProcessesB6000.WheelHoleRightDistance + ProcessesB6000.WheelHoleBetweenDistance
            };
            GlassHole middleUpRight = new()
            {
                Diameter = ProcessesB6000.HandleHoleDiameter,
                VerticalDistancing = VertDistancing.FromMiddleUp,
                VerticalDistance = ProcessesB6000.HandleHoleBetweenDistance / 2,
                HorizontalDistancing = HorizDistancing.FromRight,
                HorizontalDistance = ProcessesB6000.HandleHoleRightDistance
            };
            GlassHole middleDownRight = new()
            {
                Diameter = ProcessesB6000.HandleHoleDiameter,
                VerticalDistancing = VertDistancing.FromMiddleDown,
                VerticalDistance = ProcessesB6000.HandleHoleBetweenDistance / 2,
                HorizontalDistancing = HorizDistancing.FromRight,
                HorizontalDistance = ProcessesB6000.HandleHoleRightDistance
            };
            holes94.Add(topLeft1);
            holes94.Add(topLeft2);
            holes94.Add(topRight1);
            holes94.Add(topRight2);
            holes94.Add(bottomLeft1);
            holes94.Add(bottomLeft2);
            holes94.Add(bottomRight1);
            holes94.Add(bottomRight2);
            holes94.Add(middleUpRight);
            holes94.Add(middleDownRight);

            return holes94;
        }
        private static List<GlassHole> GetVSHoles()
        {
            List<GlassHole> holesVS = new();

            GlassHole topLeftWheel = new()
            {
                Diameter = ProcessesInox304.WheelHoleDiameterVS,
                VerticalDistancing = VertDistancing.FromTop,
                VerticalDistance = ProcessesInox304.WheelHoleTopDistanceVS,
                HorizontalDistancing = HorizDistancing.FromLeft,
                HorizontalDistance = ProcessesInox304.WheelHoleLeftDistanceVS
            };
            GlassHole topRightWheel = new()
            {
                Diameter = ProcessesInox304.WheelHoleDiameterVS,
                VerticalDistancing = VertDistancing.FromTop,
                VerticalDistance = ProcessesInox304.WheelHoleTopDistanceVS,
                HorizontalDistancing = HorizDistancing.FromRight,
                HorizontalDistance = ProcessesInox304.WheelHoleRightDistanceVS
            };
            GlassHole topRightStopper = new()
            {
                Diameter = ProcessesInox304.StopperHoleDiameterVS,
                VerticalDistancing = VertDistancing.FromTop,
                VerticalDistance = ProcessesInox304.WheelHoleTopDistanceVS + ProcessesInox304.WheelStopperBetweenDistanceVS,
                HorizontalDistancing = HorizDistancing.FromRight,
                HorizontalDistance = ProcessesInox304.StopperHoleRightDistanceVS
            };
            GlassHole topLeftStopper = new()
            {
                Diameter = ProcessesInox304.StopperHoleDiameterVS,
                VerticalDistancing = VertDistancing.FromTop,
                VerticalDistance = ProcessesInox304.WheelHoleTopDistanceVS + ProcessesInox304.WheelStopperBetweenDistanceVS,
                HorizontalDistancing = HorizDistancing.FromLeft,
                HorizontalDistance = ProcessesInox304.StopperHoleLeftDistanceVS
            };
            GlassHole middleHandle = new()
            {
                Diameter = ProcessesInox304.HandleHoleDiameter,
                VerticalDistancing = VertDistancing.FromMiddleUp,
                VerticalDistance = 0,
                HorizontalDistancing = HorizDistancing.FromRight,
                HorizontalDistance = ProcessesInox304.HandleHoleRightDistance
            };

            holesVS.Add(topLeftWheel);
            holesVS.Add(topRightWheel);
            holesVS.Add(topRightStopper);
            holesVS.Add(topLeftStopper);
            holesVS.Add(middleHandle);

            return holesVS;
        }
        private static List<GlassHole> GetVFHoles()
        {
            List<GlassHole> holesVF = new();

            GlassHole topRightBarHole = new()
            {
                Diameter = ProcessesInox304.BarHoleDiameterVF,
                VerticalDistancing = VertDistancing.FromTop,
                VerticalDistance = ProcessesInox304.BarHoleTopDistanceVF,
                HorizontalDistancing = HorizDistancing.FromRight,
                HorizontalDistance = ProcessesInox304.BarHoleRightDistanceVF
            };
            GlassHole topLeftSupportHole = new()
            {
                Diameter = ProcessesInox304.SupportHoleDiameterVF,
                VerticalDistancing = VertDistancing.FromTop,
                VerticalDistance = ProcessesInox304.SupportHoleTopDistanceVF,
                HorizontalDistancing = HorizDistancing.FromLeft,
                HorizontalDistance = ProcessesInox304.SupportHoleLeftDistanceVF
            };
            GlassHole bottomLeftSupportHole = new()
            {
                Diameter = ProcessesInox304.SupportHoleDiameterVF,
                VerticalDistancing = VertDistancing.FromBottom,
                VerticalDistance = ProcessesInox304.SupportHoleBottomDistanceVF,
                HorizontalDistancing = HorizDistancing.FromLeft,
                HorizontalDistance = ProcessesInox304.SupportHoleLeftDistanceVF
            };
            holesVF.Add(topRightBarHole);
            holesVF.Add(topLeftSupportHole);
            holesVF.Add(bottomLeftSupportHole);
            return holesVF;
        }
        private static List<GlassHole> GetVAHoles()
        {
            List<GlassHole> holesVA = new();

            GlassHole topLeftBarHole = new()
            {
                Diameter = ProcessesInox304.BarHoleDiameterVA,
                VerticalDistancing = VertDistancing.FromTop,
                VerticalDistance = ProcessesInox304.BarHoleTopDistanceVA,
                HorizontalDistancing = HorizDistancing.FromLeft,
                HorizontalDistance = ProcessesInox304.BarHoleLeftDistanceVA
            };
            GlassHole topRightBarHole = new()
            {
                Diameter = ProcessesInox304.BarHoleDiameterVA,
                VerticalDistancing = VertDistancing.FromTop,
                VerticalDistance = ProcessesInox304.BarHoleTopDistanceVA,
                HorizontalDistancing = HorizDistancing.FromRight,
                HorizontalDistance = ProcessesInox304.BarHoleRightDistanceVA
            };
            GlassHole topLeftSupportHole = new()
            {
                Diameter = ProcessesInox304.SupportHoleDiameterVA,
                VerticalDistancing = VertDistancing.FromTop,
                VerticalDistance = ProcessesInox304.SupportHoleTopDistanceVA,
                HorizontalDistancing = HorizDistancing.FromLeft,
                HorizontalDistance = ProcessesInox304.SupportHoleLeftDistanceVA
            };
            GlassHole bottomLeftSupportHole = new()
            {
                Diameter = ProcessesInox304.SupportHoleDiameterVA,
                VerticalDistancing = VertDistancing.FromBottom,
                VerticalDistance = ProcessesInox304.SupportHoleBottomDistanceVA,
                HorizontalDistancing = HorizDistancing.FromLeft,
                HorizontalDistance = ProcessesInox304.SupportHoleLeftDistanceVA
            };

            holesVA.Add(topLeftBarHole);
            holesVA.Add(topRightBarHole);
            holesVA.Add(topLeftSupportHole);
            holesVA.Add(bottomLeftSupportHole);

            return holesVA;
        }
        private static List<GlassHole> GetHB2Holes()
        {
            List<GlassHole> holesHB2 = new();

            GlassHole topHandleHole = new()
            {
                Diameter = ProcessesHB.HandleHoleDiameter,
                VerticalDistancing = VertDistancing.FromMiddleUp,
                VerticalDistance = ProcessesHB.HandleHoleBetweenDistance / 2,
                HorizontalDistancing = HorizDistancing.FromRight,
                HorizontalDistance = ProcessesHB.HandleHoleRightDistance
            };
            GlassHole bottomHandleHole = new()
            {
                Diameter = ProcessesHB.HandleHoleDiameter,
                VerticalDistancing = VertDistancing.FromMiddleDown,
                VerticalDistance = ProcessesHB.HandleHoleBetweenDistance / 2,
                HorizontalDistancing = HorizDistancing.FromRight,
                HorizontalDistance = ProcessesHB.HandleHoleRightDistance
            };


            holesHB2.Add(topHandleHole);
            holesHB2.Add(bottomHandleHole);

            return holesHB2;
        }
        private static List<GlassHole> GetWSHoles()
        {
            List<GlassHole> holesWS = new();

            GlassHole handleHole = new()
            {
                Diameter = ProcessesWS.HandleHoleDiameter,
                VerticalDistancing = VertDistancing.FromMiddleUp,
                VerticalDistance = 0,
                HorizontalDistancing = HorizDistancing.FromRight,
                HorizontalDistance = ProcessesWS.HandleHoleRightDistance
            };
            GlassHole bottomLeftWheelHole = new()
            {
                Diameter = ProcessesWS.WheelHoleDiameter,
                VerticalDistancing = VertDistancing.FromBottom,
                VerticalDistance = ProcessesWS.WheelHoleBottomDistance,
                HorizontalDistancing = HorizDistancing.FromLeft,
                HorizontalDistance = ProcessesWS.WheelHoleLeftDistance
            };
            GlassHole bottomRightWheelHole = new()
            {
                Diameter = ProcessesWS.WheelHoleDiameter,
                VerticalDistancing = VertDistancing.FromBottom,
                VerticalDistance = ProcessesWS.WheelHoleBottomDistance,
                HorizontalDistancing = HorizDistancing.FromRight,
                HorizontalDistance = ProcessesWS.WheelHoleRightDistance
            };

            holesWS.Add(handleHole);
            holesWS.Add(bottomLeftWheelHole);
            holesWS.Add(bottomRightWheelHole);

            return holesWS;
        }
        private static List<GlassHole> GetDP1Holes()
        {
            List<GlassHole> holesDP1 = new();

            ConicalHole topConicalHole = new()
            {
                Diameter = ProcessesNP.HingeHoleDiameter,
                BigDiameter = ProcessesNP.HingeHoleBigDiameter,
                VerticalDistancing = VertDistancing.FromTop,
                VerticalDistance = ProcessesNP.HingeHoleTopDistanceDP1,
                HorizontalDistancing = HorizDistancing.FromLeft,
                HorizontalDistance = ProcessesNP.HingeHoleLeftDistanceDP1
            };
            ConicalHole bottomConicalHole = new()
            {
                Diameter = ProcessesNP.HingeHoleDiameter,
                BigDiameter = ProcessesNP.HingeHoleBigDiameter,
                VerticalDistancing = VertDistancing.FromBottom,
                VerticalDistance = ProcessesNP.HingeHoleBottomDistanceDP1,
                HorizontalDistancing = HorizDistancing.FromLeft,
                HorizontalDistance = ProcessesNP.HingeHoleLeftDistanceDP1
            };
            GlassHole handleHole = new()
            {
                Diameter = ProcessesNP.HandleHoleDiameter,
                VerticalDistancing = VertDistancing.FromMiddleUp,
                VerticalDistance = 0,
                HorizontalDistancing = HorizDistancing.FromRight,
                HorizontalDistance = ProcessesNP.HandleHoleRightDistance
            };

            holesDP1.Add(topConicalHole);
            holesDP1.Add(bottomConicalHole);
            holesDP1.Add(handleHole);

            return holesDP1;
        }
        private static List<GlassHole> GetDP3Holes()
        {
            List<GlassHole> holesDP3 = new();

            ConicalHole topConicalHole = new()
            {
                Diameter = ProcessesNP.HingeHoleDiameter,
                BigDiameter = ProcessesNP.HingeHoleBigDiameter,
                VerticalDistancing = VertDistancing.FromTop,
                VerticalDistance = ProcessesNP.HingeHoleTopDistanceDP3,
                HorizontalDistancing = HorizDistancing.FromRight,
                HorizontalDistance = ProcessesNP.HingeHoleRightDistanceDP3
            };
            ConicalHole bottomConicalHole = new()
            {
                Diameter = ProcessesNP.HingeHoleDiameter,
                BigDiameter = ProcessesNP.HingeHoleBigDiameter,
                VerticalDistancing = VertDistancing.FromBottom,
                VerticalDistance = ProcessesNP.HingeHoleBottomDistanceDP3,
                HorizontalDistancing = HorizDistancing.FromRight,
                HorizontalDistance = ProcessesNP.HingeHoleRightDistanceDP3
            };

            holesDP3.Add(topConicalHole);
            holesDP3.Add(bottomConicalHole);

            return holesDP3;
        }
        private static List<GlassHole> GetNBHoles()
        {
            List<GlassHole> holesNB = new();

            GlassHole handleHole = new()
            {
                Diameter = ProcessesNB.HandleHoleDiameter,
                VerticalDistancing = VertDistancing.FromMiddleUp,
                VerticalDistance = 0,
                HorizontalDistancing = HorizDistancing.FromRight,
                HorizontalDistance = ProcessesNB.HandleHoleRightDistance
            };

            holesNB.Add(handleHole);

            return holesNB;
        }
        private static List<GlassHole> GetDBHoles()
        {
            List<GlassHole> holesDB = new();

            GlassHole handleHole = new()
            {
                Diameter = ProcessesDB.HandleHoleDiameter,
                VerticalDistancing = VertDistancing.FromMiddleUp,
                VerticalDistance = 0,
                HorizontalDistancing = HorizDistancing.FromRight,
                HorizontalDistance = ProcessesDB.HandleHoleRightDistance
            };

            holesDB.Add(handleHole);

            return holesDB;
        }
        private static List<GlassHole> GetH1Holes()
        {
            List<GlassHole> holesH1 = new();

            GlassHole topLeftSupportHole = new()
            {
                Diameter = ProcessesH1.SupportHoleDiameter,
                VerticalDistancing = VertDistancing.FromTop,
                VerticalDistance = ProcessesH1.SupportHoleTopDistance,
                HorizontalDistancing = HorizDistancing.FromLeft,
                HorizontalDistance = ProcessesH1.SupportHoleLeftDistance
            };
            GlassHole bottomLeftSupportHole = new()
            {
                Diameter = ProcessesH1.SupportHoleDiameter,
                VerticalDistancing = VertDistancing.FromBottom,
                VerticalDistance = ProcessesH1.SupportHoleBottomDistance,
                HorizontalDistancing = HorizDistancing.FromLeft,
                HorizontalDistance = ProcessesH1.SupportHoleLeftDistance
            };
            holesH1.Add(topLeftSupportHole);
            holesH1.Add(bottomLeftSupportHole);

            return holesH1;
        }
        private static List<GlassHole> Get9BHoles()
        {
            List<GlassHole> holes9B = new();

            GlassHole topHingeHole = new()
            {
                Diameter = ProcessesB6000.HingeHoleDiameter,
                VerticalDistancing = VertDistancing.FromTop,
                VerticalDistance = ProcessesB6000.HingeHoleTopDistance,
                HorizontalDistancing = HorizDistancing.FromLeft,
                HorizontalDistance = ProcessesB6000.HingeHoleLeftDistance
            };
            GlassHole bottomHingeHole = new()
            {
                Diameter = ProcessesB6000.HingeHoleDiameter,
                VerticalDistancing = VertDistancing.FromBottom,
                VerticalDistance = ProcessesB6000.HingeHoleBottomDistance,
                HorizontalDistancing = HorizDistancing.FromLeft,
                HorizontalDistance = ProcessesB6000.HingeHoleLeftDistance
            };
            GlassHole upMiddleHandleHole = new()
            {
                Diameter = ProcessesB6000.HandleHoleDiameter,
                VerticalDistancing = VertDistancing.FromMiddleUp,
                VerticalDistance = ProcessesB6000.HandleHoleBetweenDistance / 2,
                HorizontalDistancing = HorizDistancing.FromRight,
                HorizontalDistance = ProcessesB6000.HandleHoleRightDistance
            };
            GlassHole downMiddleHandleHole = new()
            {
                Diameter = ProcessesB6000.HandleHoleDiameter,
                VerticalDistancing = VertDistancing.FromMiddleDown,
                VerticalDistance = ProcessesB6000.HandleHoleBetweenDistance / 2,
                HorizontalDistancing = HorizDistancing.FromRight,
                HorizontalDistance = ProcessesB6000.HandleHoleRightDistance
            };

            holes9B.Add(topHingeHole);
            holes9B.Add(bottomHingeHole);
            holes9B.Add(upMiddleHandleHole);
            holes9B.Add(downMiddleHandleHole);

            return holes9B;
        }
        private static List<GlassHole> GetWFlipperHoles()
        {
            return new();
        }

        #endregion

        #region 2. Get Hotel8000 Hinge

        private static List<CutHotel8000> GetHB1Cuts()
        {
            List<CutHotel8000> Cuts = new();

            CutHotel8000 topCut = new(false)
            {
                Height = ProcessesHotel8000.CutHeight,
                SemiCircleDiameter = ProcessesHotel8000.CutSemiCircleDiameter,
                SemiCircleDistanceFromEdge = ProcessesHotel8000.CutSemiCircleCenterEdgeDistance,
                SemiCirclesCentersDistance = ProcessesHotel8000.CutSemiCirclesCenterDistance,
                VerticalDistancing = VertDistancing.FromTop,
                VerticalDistance = ProcessesHotel8000.CutTopDistanceHB1,
                HorizontalDistancing = HorizDistancing.FromRight,
                HorizontalDistance = 0
            };
            CutHotel8000 bottomCut = new(false)
            {
                Height = ProcessesHotel8000.CutHeight,
                SemiCircleDiameter = ProcessesHotel8000.CutSemiCircleDiameter,
                SemiCircleDistanceFromEdge = ProcessesHotel8000.CutSemiCircleCenterEdgeDistance,
                SemiCirclesCentersDistance = ProcessesHotel8000.CutSemiCirclesCenterDistance,
                VerticalDistancing = VertDistancing.FromBottom,
                VerticalDistance = ProcessesHotel8000.CutBottomDistanceHB1,
                HorizontalDistancing = HorizDistancing.FromRight,
                HorizontalDistance = 0
            };


            Cuts.Add(topCut);
            Cuts.Add(bottomCut);

            return Cuts;
        }
        private static List<CutHotel8000> GetHB2Cuts()
        {
            List<CutHotel8000> Cuts = new();

            CutHotel8000 topCut = new(true)
            {
                Height = ProcessesHotel8000.CutHeight,
                SemiCircleDiameter = ProcessesHotel8000.CutSemiCircleDiameter,
                SemiCircleDistanceFromEdge = ProcessesHotel8000.CutSemiCircleCenterEdgeDistance,
                SemiCirclesCentersDistance = ProcessesHotel8000.CutSemiCirclesCenterDistance,
                VerticalDistancing = VertDistancing.FromTop,
                VerticalDistance = ProcessesHotel8000.CutTopDistanceHB2,
                HorizontalDistancing = HorizDistancing.FromLeft,
                HorizontalDistance = 0
            };
            CutHotel8000 bottomCut = new(true)
            {
                Height = ProcessesHotel8000.CutHeight,
                SemiCircleDiameter = ProcessesHotel8000.CutSemiCircleDiameter,
                SemiCircleDistanceFromEdge = ProcessesHotel8000.CutSemiCircleCenterEdgeDistance,
                SemiCirclesCentersDistance = ProcessesHotel8000.CutSemiCirclesCenterDistance,
                VerticalDistancing = VertDistancing.FromBottom,
                VerticalDistance = ProcessesHotel8000.CutBottomDistanceHB2,
                HorizontalDistancing = HorizDistancing.FromLeft,
                HorizontalDistance = 0
            };


            Cuts.Add(topCut);
            Cuts.Add(bottomCut);

            return Cuts;
        }
        private static List<CutHotel8000> GetDBCuts()
        {
            List<CutHotel8000> Cuts = new();

            CutHotel8000 topCut = new(true)
            {
                Height = ProcessesHotel8000.CutHeight,
                SemiCircleDiameter = ProcessesHotel8000.CutSemiCircleDiameter,
                SemiCircleDistanceFromEdge = ProcessesHotel8000.CutSemiCircleCenterEdgeDistance,
                SemiCirclesCentersDistance = ProcessesHotel8000.CutSemiCirclesCenterDistance,
                VerticalDistancing = VertDistancing.FromTop,
                VerticalDistance = ProcessesHotel8000.CutTopDistanceDB,
                HorizontalDistancing = HorizDistancing.FromLeft,
                HorizontalDistance = 0
            };
            CutHotel8000 bottomCut = new(true)
            {
                Height = ProcessesHotel8000.CutHeight,
                SemiCircleDiameter = ProcessesHotel8000.CutSemiCircleDiameter,
                SemiCircleDistanceFromEdge = ProcessesHotel8000.CutSemiCircleCenterEdgeDistance,
                SemiCirclesCentersDistance = ProcessesHotel8000.CutSemiCirclesCenterDistance,
                VerticalDistancing = VertDistancing.FromBottom,
                VerticalDistance = ProcessesHotel8000.CutBottomDistanceDB,
                HorizontalDistancing = HorizDistancing.FromLeft,
                HorizontalDistance = 0
            };


            Cuts.Add(topCut);
            Cuts.Add(bottomCut);

            return Cuts;
        }

        #endregion

        #region 3. Get 9BHinge Cut

        private static List<Cut9B> Get9BCuts()
        {
            List<Cut9B> Cuts = new();

            Cut9B topCut = new()
            {
                Height = ProcessesB6000.HingeCutHeight,
                Length = ProcessesB6000.HingeCutLength,
                VerticalDistancing = VertDistancing.FromTop,
                VerticalDistance = 0,
                HorizontalDistancing = HorizDistancing.FromLeft,
                HorizontalDistance = ProcessesB6000.HingeCutLeftDistance
            };
            Cut9B bottomCut = new()
            {
                Height = ProcessesB6000.HingeCutHeight,
                Length = ProcessesB6000.HingeCutLength,
                VerticalDistancing = VertDistancing.FromBottom,
                VerticalDistance = 0,
                HorizontalDistancing = HorizDistancing.FromLeft,
                HorizontalDistance = ProcessesB6000.HingeCutLeftDistance
            };
            Cuts.Add(topCut);
            Cuts.Add(bottomCut);
            return Cuts;
        }

        #endregion

    }
}
