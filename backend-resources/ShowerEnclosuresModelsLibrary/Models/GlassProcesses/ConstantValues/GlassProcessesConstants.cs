using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.GlassProcesses.ConstantValues
{
    /// <summary>
    /// Contains the Constant Values of the Dimensions of all GlassProcesses
    /// </summary>
    public static class GlassProcessesConstants
    {
        public static class ProcessesB6000
        {
            public static readonly int WheelHoleDiameter = 10;
            public static readonly int WheelHoleLeftDistance9S = 105;
            public static readonly int WheelHoleLeftDistance94 = 60;
            public static readonly int WheelHoleRightDistance = 60;
            public static readonly int WheelHoleTopDistance = 16;
            public static readonly int WheelHoleBottomDistance = 16;
            public static readonly int WheelHoleBetweenDistance = 26;

            public static readonly int HandleHoleBetweenDistance = 145;
            public static readonly int HandleHoleRightDistance = 45;
            public static readonly int HandleHoleDiameter = 10;

            public static readonly int HingeHoleDiameter = 12;
            public static readonly int HingeHoleLeftDistance = 95;
            public static readonly int HingeHoleTopDistance = 23;
            public static readonly int HingeHoleBottomDistance = 23;

            public static readonly int HingeCutLength = 50;
            public static readonly int HingeCutHeight = 10;
            public static readonly int HingeCutLeftDistance = 70;
        }
        public static class ProcessesInox304
        {
            public static readonly int HandleHoleDiameter = 50;
            public static readonly int HandleHoleRightDistance = 80;

            public static readonly int WheelHoleRightDistanceVS = 80;
            public static readonly int WheelHoleLeftDistanceVS = 80;
            public static readonly int WheelHoleTopDistanceVS = 45;
            public static readonly int WheelHoleDiameterVS = 16;

            /// <summary>
            /// The Distance of the Lock Hole from the Right Side
            /// </summary>
            public static readonly int StopperHoleRightDistanceVS = 80;
            /// <summary>
            /// The Distance of the Lock Hole from the Left Side
            /// </summary>
            public static readonly int StopperHoleLeftDistanceVS = 80;
            /// <summary>
            /// The Diameter of the Lock Hole
            /// </summary>
            public static readonly int StopperHoleDiameterVS = 10;
            /// <summary>
            /// The Distance from the Lock Hole to the Center of the Wheel Hole
            /// </summary>
            public static readonly int WheelStopperBetweenDistanceVS = 56;

            public static readonly int BarHoleLeftDistanceVA = 120;
            public static readonly int BarHoleRightDistanceVA = 120;
            public static readonly int BarHoleTopDistanceVA = 76;
            public static readonly int BarHoleDiameterVA = 14;

            public static readonly int SupportHoleLeftDistanceVA = 25;
            public static readonly int SupportHoleTopDistanceVA = 350;
            public static readonly int SupportHoleBottomDistanceVA = 350;
            public static readonly int SupportHoleDiameterVA = 20;

            public static readonly int SupportHoleLeftDistanceVF = 25;
            public static readonly int SupportHoleTopDistanceVF = 350;
            public static readonly int SupportHoleBottomDistanceVF = 350;
            public static readonly int SupportHoleDiameterVF = 20;
            /// <summary>
            /// The Minimum Distance a Support Holde can Be From a Step
            /// </summary>
            public static readonly int SupportHoleMinDistanceFromStep = 60;
            /// <summary>
            /// The Maximum Height of a Step Cut that will not have a Support Hole
            /// </summary>
            public static readonly int StepMaxHeightWithoutSupportHole = 500;

            public static readonly int BarHoleRightDistanceVF = 25;
            public static readonly int BarHoleTopDistanceVF = 76;
            public static readonly int BarHoleDiameterVF = 14;
        }
        public static class ProcessesHB
        {
            public static readonly int HandleHoleDiameter = 10;
            public static readonly int HandleHoleRightDistance = 50;
            public static readonly int HandleHoleBetweenDistance = 145;
        }
        public static class ProcessesWS
        {
            public static readonly int WheelHoleBottomDistance = 27;
            public static readonly int WheelHoleDiameter = 15;
            public static readonly int WheelHoleLeftDistance = 100;
            public static readonly int WheelHoleRightDistance = 100;

            public static readonly int HandleHoleDiameter = 50;
            public static readonly int HandleHoleRightDistance = 55;
        }
        public static class ProcessesNP
        {
            public static readonly int HingeHoleDiameter = 24;
            public static readonly int HingeHoleBigDiameter = 32;
            
            public static readonly int HingeHoleLeftDistanceDP1 = 41;
            public static readonly int HingeHoleTopDistanceDP1 = 225;
            public static readonly int HingeHoleBottomDistanceDP1 = 225;

            public static readonly int HingeHoleRightDistanceDP3 = 41;
            public static readonly int HingeHoleTopDistanceDP3 = 225;
            public static readonly int HingeHoleBottomDistanceDP3 = 225;

            public static readonly int HandleHoleDiameter = 12;
            public static readonly int HandleHoleRightDistance = 55;
        }
        public static class ProcessesNB
        {
            public static readonly int HandleHoleDiameter = 10;
            public static readonly int HandleHoleRightDistance = 30;
        }
        public static class ProcessesDB
        {
            public static readonly int HandleHoleDiameter = 12;
            public static readonly int HandleHoleRightDistance = 45;
        }
        public static class ProcessesH1
        {
            public static readonly int SupportHoleDiameter = 20;
            public static readonly int SupportHoleTopDistance = 300;
            public static readonly int SupportHoleBottomDistance = 300;
            public static readonly int SupportHoleLeftDistance = 25;
        }
        public static class ProcessesHotel8000
        {
            public static readonly int CutHeight = 46;
            /// <summary>
            /// The Vertical Distance between the Two Semicircles in a Hinge Cut
            /// </summary>
            public static readonly int CutSemiCirclesCenterDistance = 49;
            /// <summary>
            /// The Distance of the Semicircles center from the Edge
            /// </summary>
            public static readonly int CutSemiCircleCenterEdgeDistance = 35;
            public static readonly int CutSemiCircleDiameter = 15;

            public static readonly int CutTopDistanceHB1 = 220;
            public static readonly int CutBottomDistanceHB1 = 220;

            public static readonly int CutTopDistanceHB2 = 220;
            public static readonly int CutBottomDistanceHB2 = 205;

            public static readonly int CutTopDistanceDB = 220;
            public static readonly int CutBottomDistanceDB = 220;
        }

        public static class ProcessesBathtubGlasses
        {
            /// <summary>
            /// The Radius of the Upper Corner Filleted Edge of a Bathtub Glass
            /// </summary>
            public static readonly double CornerCutRadius = 200;
        }

        public static class ProcessesW
        {
            public static readonly int FlipperHingeHoleTopDistance = 100;
            public static readonly int FlipperHingeHoleBottomDistance = 100;
        }
    }
}
