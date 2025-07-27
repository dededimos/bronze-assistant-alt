using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsModelsLibrary.StaticData
{
    /// <summary>
    /// Default Values and Positioning of Mirror Parts & Extras
    /// </summary>
    public static class MirrorStandards
    {
        public static class Sandblasts
        {
            public static readonly int ThicknessX4 = 25;
            public static readonly int ThicknessX6 = 25;
            public static readonly int ThicknessM3 = 30;
            public static readonly int ThicknessH8 = 30;
            public static readonly int Thickness6000 = 30;
            public static readonly int ThicknessN6 = 30;

            public static readonly int DistanceFromTopM3 = 60;
            public static readonly int DistanceFromTopH8 = 10;
            public static readonly int DistanceFromTop6000 = 0;

            public static readonly int DistanceFromSideM3 = 60;
            public static readonly int DistanceFromSideH8 = 10;
            public static readonly int DistanceFromSide6000 = 50;

            public static readonly int SandblastFrameMargin = 25;
        }
        public static class Magnifyer
        {
            public static readonly int FrontDiametermm = 120;
            public static readonly int RearDiametermm = 140;
            public static readonly double DefaultAnglePositionInCircle = Math.PI / 4;

            public static readonly int CenterFromEdgeXmm = 110; // When Inside a Rectangular Mirror -- Previously 175
            public static readonly int CenterFromEdgeYmm = 110; // When Inside a Rectangular Mirror -- Previously 175
            public static readonly int CenterFromEdgeCmm = 110; // When Inside a Circular Mirror    -- Previously 175
            public static readonly int CenterFromEdgeCapsulemm = 110; // When Inisde a Capsule Mirror
            public static readonly int CenterFromEdgeEllipse = 110;

            public static readonly int SandblastThickness = 20;
            public static readonly int SandblastDiametermm = 190;
        }
        public static class Touch
        {
            public static readonly int DistanceFromBottom = 5; //Previously 60
            public static readonly int DistanceFromSide = 0;
            public static readonly int OuterSquare = 15;
            public static readonly int InnerSquare = 9;

            public static readonly int BoxLength = 65;
            public static readonly int BoxHeight = 58;
        }

        public static class Screens
        {
            public static readonly int Display11Length = 116;
            public static readonly int Display11Height = 60;
            public static readonly int Display11DistanceFromBottom = 60;
            public static readonly int Display11RearLength = 175;
            public static readonly int Display11RearHeight = 120;

            public static readonly int Display19Length = 185;
            public static readonly int Display19Height = 88;
            public static readonly int Display19DistanceFromBottom = 60;
            public static readonly int Display19RearLength = 185;
            public static readonly int Display19RearHeight = 88;

            public static readonly int Display20Length = 100;
            public static readonly int Display20Height = 88;
            public static readonly int Display20DistanceFromBottom = 60;
            public static readonly int Display20RearLength = 100;
            public static readonly int Display20RearHeight = 88;

            public static readonly int ClockLength = 90;
            public static readonly int ClockHeight = 16;
            public static readonly int ClockDistanceFromBottom = 60;
            public static readonly int ClockDistanceFromSide = 60;
            public static readonly int ClockRearLength = 130;
            public static readonly int ClockRearHeight = 78;

        }

        public static class Frames 
        {
            public static readonly int FrameFrontThickness = 20;
            public static readonly int FrameRearThickness = 15;
            public static readonly int FrameFrontCircularThickness = 15;

            public static readonly int PerimetricalDistanceFromEdge = 22;
            public static readonly int PerimetricalThickness = 15;

            public static readonly int PerimetricalCircularThickness = 10;
            public static readonly int PerimetricalCircularDistanceFromEdge = 22;

            public static readonly int PerimetricalCapsuleThickness = 10;
            public static readonly int PerimetricalCapsuleDistanceFromEdge = 22;

            public static readonly int PerimetricalEllipseThickness = 10;
            public static readonly int PerimetricalEllipseDistanceFromEdge = 22;

            public static readonly double DoubleSupportLengthPercent = 0.4d;
        }

        public static class Fogs
        {
            public static readonly int Fog16Length = 290;
            public static readonly int Fog16Height = 290;
            public static readonly int Fog24Length = 410;
            public static readonly int Fog24Height = 290;
            public static readonly int Fog55Length = 510;
            public static readonly int Fog55Height = 510;
        }

        public static readonly int RoundedCornersRadius = 20;

    }
}
