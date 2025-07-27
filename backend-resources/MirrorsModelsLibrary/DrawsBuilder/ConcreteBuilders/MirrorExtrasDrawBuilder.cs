using SVGDrawingLibrary.Enums;
using SVGDrawingLibrary.Helpers;
using SVGDrawingLibrary.Models;
using SVGDrawingLibrary.Models.ConcreteShapes;
using MirrorsModelsLibrary.Enums;
using MirrorsModelsLibrary.StaticData;
using System;
using MirrorsModelsLibrary.Models;

namespace MirrorsModelsLibrary.DrawsBuilder.ConcreteBuilders
{
    /// <summary>
    /// The Builder for all the Mirrors Extras -- Buidls and Places an Extra at a Default Position
    /// </summary>
    public class MirrorExtrasDrawBuilder : IDrawShapeBuilder
    {
        DrawShape extraDraw;
        DrawShape extrasBoundary;
        Mirror mirror;
        private double extrasBoundaryLength;
        private double extrasBoundaryHeight;
        private double extrasBoundaryDiameter;
        private double extrasBoundaryCenterX;
        private double extrasBoundaryCenterY;

        private bool isAdditiveDraw;
        private MirrorOption? extraToDraw;
        private DrawnSide? sideToDraw;

        /// <summary>
        /// Builds the Draw of an Extra
        /// </summary>
        /// <param name="extraToDraw">The Extra for which to build the draw</param>
        /// <param name="sideToDraw">the Side that is beiong drawn</param>
        /// <param name="isAdditiveDraw">if this is a supplementary Draw for the extra (ex.Magnifyers Sandblast)</param>
        public MirrorExtrasDrawBuilder(MirrorOption? extraToDraw,
                                          DrawnSide sideToDraw,
                                          DrawShape extrasBoundary,
                                          Mirror mirror,
                                          bool isAdditiveDraw = false)
        {
            this.mirror = mirror;
            this.extrasBoundary = extrasBoundary;
            if (extrasBoundary is RectangleDraw r)
            {
                extrasBoundaryLength = r.Length;
                extrasBoundaryHeight = r.Height;
                extrasBoundaryCenterX = r.ShapeCenterX;
                extrasBoundaryCenterY = r.ShapeCenterY;
            }
            else if (extrasBoundary is CircleDraw c)
            {
                extrasBoundaryDiameter = c.Diameter;
                extrasBoundaryLength = c.Diameter; //Same as the Dimaeter anyways
                extrasBoundaryHeight = c.Diameter; //Same as the Diameter anyways
                extrasBoundaryCenterX = c.ShapeCenterX;
                extrasBoundaryCenterY = c.ShapeCenterY;
            }
            else if (extrasBoundary is EllipseDraw e)
            {
                extrasBoundaryLength = e.Length;
                extrasBoundaryHeight = e.Height;
                extrasBoundaryCenterX = e.ShapeCenterX;
                extrasBoundaryCenterY = e.ShapeCenterY;
            }
            else
            {
                throw new InvalidOperationException("Invalid ExtrasBoundary Shape -- Expected Only Circle - Rectangle or Ellipse");
            }

            this.extraToDraw = extraToDraw;
            this.sideToDraw = sideToDraw;
            this.isAdditiveDraw = isAdditiveDraw;
        }

        public DrawShape BuildShape()
        {
            switch (extraToDraw)
            {
                case MirrorOption.TouchSwitchFog:
                case MirrorOption.TouchSwitch:
                case MirrorOption.DimmerSwitch:
                case MirrorOption.SensorSwitch:
                    BuildTouchDraw();
                    if (extraDraw is not null)
                    {
                        extraDraw.Name = extraToDraw != MirrorOption.TouchSwitchFog ? DrawShape.TOUCH : DrawShape.TOUCHFOG;
                        extraDraw.Stroke = "lightslategray";
                    }
                    break;
                case MirrorOption.DisplayRadio:
                    BuildDisplay11Draw();
                    if (extraDraw is not null)
                    {
                        extraDraw.Name = DrawShape.DISPLAY11;
                        extraDraw.Stroke = "lightslategray";
                    }
                    break;
                case MirrorOption.Clock:
                    BuildClockDraw();
                    if (extraDraw is not null)
                    {
                        extraDraw.Name = DrawShape.CLOCK;
                        extraDraw.Stroke = "lightslategray";
                    }
                    break;
                case MirrorOption.Display20:
                    BuildDisplay20Draw();
                    if (extraDraw is not null)
                    {
                        extraDraw.Name = DrawShape.DISPLAY20;
                        extraDraw.Stroke = "lightslategray";
                    }
                    break;
                case MirrorOption.Display19:
                    BuildDisplay19Draw();
                    if (extraDraw is not null)
                    {
                        extraDraw.Name = DrawShape.DISPLAY19;
                        extraDraw.Stroke = "lightslategray";
                    }
                    break;
                case MirrorOption.Fog16W:
                    BuildFogDraw(MirrorOption.Fog16W);
                    if (extraDraw is not null)
                    {
                        extraDraw.Name = DrawShape.FOG16;
                        extraDraw.Stroke = "lightslategray";
                    }
                    break;
                case MirrorOption.Fog24W:
                    BuildFogDraw(MirrorOption.Fog24W);
                    if (extraDraw is not null)
                    {
                        extraDraw.Name = DrawShape.FOG24;
                        extraDraw.Stroke = "lightslategray";
                    }
                    break;
                case MirrorOption.Fog55W:
                    BuildFogDraw(MirrorOption.Fog55W);
                    if (extraDraw is not null)
                    {
                        extraDraw.Name = DrawShape.FOG55;
                        extraDraw.Stroke = "lightslategray";
                    }
                    break;
                case MirrorOption.Zoom:
                case MirrorOption.ZoomLed:
                case MirrorOption.ZoomLedTouch:
                    BuildMagnifyerDraw(isAdditiveDraw);
                    break;
                case MirrorOption.BlueTooth:
                case MirrorOption.IPLid:
                case MirrorOption.RoundedCorners:
                default:
                    extraDraw = null;
                    //Do Nothing
                    break;
            }
            if (extraDraw is not null)
            {
                DrawShape shapeToReturn = extraDraw.CloneSelf();
                ResetBuilder();
                return shapeToReturn;
            }
            else
            {
                ResetBuilder();
                return null;
            }
        }

        private void BuildMagnifyerDraw(bool isAdditiveDraw)
        {
            if (isAdditiveDraw)
            {
                // Fix the Sandblast
                HoledCircleDraw sandblast = BuildMagnifyerSandblastDraw();
                sandblast.Stroke = "lightslategray";
                sandblast.Fill = "lightgray";
                sandblast.Opacity = "0.9";
                sandblast.Name = DrawShape.MAGNSAND;
                extraDraw = sandblast;
            }
            else
            {
                // Fix the Magnifyer Based on the Drawn Side
                CircleDraw magnifyer = new();
                if (sideToDraw is DrawnSide.Front) { magnifyer.Diameter = MirrorStandards.Magnifyer.FrontDiametermm; magnifyer.Filter = "URL(#inset-shadow)"; }
                else if (sideToDraw is DrawnSide.Rear) { magnifyer.Diameter = MirrorStandards.Magnifyer.RearDiametermm; }
                else if (sideToDraw is DrawnSide.Side) { throw new NotImplementedException("Side Draw is Not Implemented -- Magnifyer Draw could not be Drawn "); }
                else { throw new InvalidOperationException($"Could not Recognize DrawnSide : -{sideToDraw}- or Null -- Magnifyer Draw could not be drawn "); }
                magnifyer.Fill = "aliceblue";
                magnifyer.Stroke = "lightslategray";
                magnifyer.Name = DrawShape.MAGN;
                extraDraw = magnifyer;
            }
            double x,y; //The Center of the Magnifyer
            
            if (extrasBoundaryDiameter is not 0) //Means the Boundary is Circular
            {
                //We have to find the Point of Placement of the Mirror Inisde the Boundary (We need it at PI/4)
                (x , y) = MathCalc.GetPointInsideCircle(
                            extrasBoundaryCenterX, 
                            extrasBoundaryCenterY, 
                            extrasBoundaryDiameter/2d - MirrorStandards.Magnifyer.CenterFromEdgeCmm, 
                            MirrorStandards.Magnifyer.DefaultAnglePositionInCircle);
            }
            else if (extrasBoundary is CapsuleRectangleDraw capsule)
            {
                //As per the Circular Mirror we always place the mirror at 45degrees and at a certain distance from the Edge
                //First Get the Bottom-Right Semicircle we do not need to know the orientation .In both situations we will place the mirror at 45degrees
                SemicircleRectangleDraw capsuleSemicircle = capsule.GetBottomRightSemicircle();
                (x, y) = MathCalc.GetPointInsideCircle(
                            capsuleSemicircle.GetCircleCenter().X,
                            capsuleSemicircle.GetCircleCenter().Y,
                            capsuleSemicircle.CornerRadius - MirrorStandards.Magnifyer.CenterFromEdgeCapsulemm,
                            MirrorStandards.Magnifyer.DefaultAnglePositionInCircle);
            }
            else if (extrasBoundary is EllipseDraw ellipse)
            {
                //Make a new Ellipse Smaller so that the center of the Magnifyer is at the desired Distance from the Boundary (MirrorStandards.EllipseDistance e.t.c)
                EllipseDraw newEllipse = new(ellipse.Length - MirrorStandards.Magnifyer.CenterFromEdgeEllipse * 2 ,
                                             ellipse.Height - MirrorStandards.Magnifyer.CenterFromEdgeEllipse * 2,
                                             extrasBoundaryCenterX , extrasBoundaryCenterY );
                //Place the Magnifyer at the new ellipse in the Chord from the Right Focci
                
                double ratio = mirror.Lengthmm/mirror.Heightmm;
                DrawPoint point;
                (double y1, double y2) = MathCalc.GetYSolutionOfEllipseEquation(newEllipse.ShapeCenterX + newEllipse.Length / 4d, newEllipse.ShapeCenterX, newEllipse.ShapeCenterY, newEllipse.Length / 2d, newEllipse.Height / 2d);
                point = new(newEllipse.ShapeCenterX + newEllipse.Length / 4d, Math.Max(y1, y2));
                x = point.X;
                y = point.Y;
            }
            else //When the Boundary is Rectangular
            {
                //Find the Center of the Magnifyer
                x = extrasBoundaryCenterX + extrasBoundaryLength / 2d - MirrorStandards.Magnifyer.CenterFromEdgeXmm;
                y = extrasBoundaryCenterY + extrasBoundaryHeight / 2d - MirrorStandards.Magnifyer.CenterFromEdgeYmm;
            }
            //Set the Center of the Magnifyer
            extraDraw.SetCenterOrStartX(x, DrawShape.CSCoordinate.Center);
            extraDraw.SetCenterOrStartY(y, DrawShape.CSCoordinate.Center);
        }

        private HoledCircleDraw BuildMagnifyerSandblastDraw()
        {
            HoledCircleDraw sandblast = new();
            sandblast.Diameter = MirrorStandards.Magnifyer.SandblastDiametermm;
            sandblast.InnerDiameter = sandblast.Diameter - MirrorStandards.Magnifyer.SandblastThickness * 2;
            return sandblast;
        }

        /// <summary>
        /// Builds a Fog Draw at the Default Position
        /// </summary>
        /// <param name="fogOption">The FogOption to be Drawn</param>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        private void BuildFogDraw(MirrorOption fogOption)
        {
            RectangleDraw fog = new();
            double fogLength;
            double fogHeight;

            switch (fogOption)
            {
                case MirrorOption.Fog16W:
                    fogHeight = MirrorStandards.Fogs.Fog16Height;
                    fogLength = MirrorStandards.Fogs.Fog16Length;
                    break;
                case MirrorOption.Fog24W:
                    fogHeight = MirrorStandards.Fogs.Fog24Height;
                    fogLength = MirrorStandards.Fogs.Fog24Length;
                    break;
                case MirrorOption.Fog55W:
                    fogHeight = MirrorStandards.Fogs.Fog55Height;
                    fogLength = MirrorStandards.Fogs.Fog55Length;
                    break;
                default:
                    fogHeight = 0;
                    fogLength = 0;
                    break;
            }

            if (sideToDraw is DrawnSide.Front)
            {
                extraDraw = null;
                return;
            }
            else if (sideToDraw is DrawnSide.Rear)
            {
                fog.Length = fogLength;
                fog.Height = fogHeight;

                fog.SetCenterOrStartX(extrasBoundaryCenterX, DrawShape.CSCoordinate.Center);
                fog.SetCenterOrStartY(extrasBoundaryCenterY, DrawShape.CSCoordinate.Center);
            }
            else if (sideToDraw is DrawnSide.Side)
            {
                throw new NotImplementedException("Side Draw is Not Implemented -- Touch Draw could not be Drawn ");
            }
            else
            {
                throw new InvalidOperationException($"Could not Recognize DrawnSide : -{sideToDraw}- or Null -- Touch Draw could not be drawn ");
            }

            extraDraw = fog;
        }

        /// <summary>
        /// Builds a Display19 Draw at the Default Position
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        private void BuildDisplay19Draw()
        {
            RectangleDraw display = new();

            if (sideToDraw is DrawnSide.Front)
            {
                display.Length = MirrorStandards.Screens.Display19Length;
                display.Height = MirrorStandards.Screens.Display19Height;
                display.SetCenterOrStartX(extrasBoundaryCenterX, DrawShape.CSCoordinate.Center);
                display.SetCenterOrStartY(extrasBoundaryCenterY + extrasBoundaryHeight / 2d - MirrorStandards.Screens.Display19DistanceFromBottom - MirrorStandards.Screens.Display19Height / 2d,
                    DrawShape.CSCoordinate.Center);
            }
            else if (sideToDraw is DrawnSide.Rear)
            {
                display.Length = MirrorStandards.Screens.Display19RearLength;
                display.Height = MirrorStandards.Screens.Display19RearHeight;

                display.SetCenterOrStartX(extrasBoundaryCenterX, DrawShape.CSCoordinate.Center);
                display.SetCenterOrStartY(extrasBoundaryCenterY + extrasBoundaryHeight / 2d - MirrorStandards.Screens.Display19DistanceFromBottom - MirrorStandards.Screens.Display19RearHeight / 2d,
                    DrawShape.CSCoordinate.Center);
            }
            else if (sideToDraw is DrawnSide.Side)
            {
                throw new NotImplementedException("Side Draw is Not Implemented -- Touch Draw could not be Drawn ");
            }
            else
            {
                throw new InvalidOperationException($"Could not Recognize DrawnSide : -{sideToDraw}- or Null -- Touch Draw could not be drawn ");
            }

            extraDraw = display;
        }

        /// <summary>
        /// Builds a Display20 Draw at the Default Position
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        private void BuildDisplay20Draw()
        {
            RectangleDraw display = new();

            if (sideToDraw is DrawnSide.Front)
            {
                display.Length = MirrorStandards.Screens.Display20Length;
                display.Height = MirrorStandards.Screens.Display20Height;
                display.SetCenterOrStartX(extrasBoundaryCenterX, DrawShape.CSCoordinate.Center);
                display.SetCenterOrStartY(extrasBoundaryCenterY + extrasBoundaryHeight / 2d - MirrorStandards.Screens.Display20DistanceFromBottom - MirrorStandards.Screens.Display20Height / 2d,
                    DrawShape.CSCoordinate.Center);
            }
            else if (sideToDraw is DrawnSide.Rear)
            {
                display.Length = MirrorStandards.Screens.Display20RearLength;
                display.Height = MirrorStandards.Screens.Display20RearHeight;

                display.SetCenterOrStartX(extrasBoundaryCenterX, DrawShape.CSCoordinate.Center);
                display.SetCenterOrStartY(extrasBoundaryCenterY + extrasBoundaryHeight / 2d - MirrorStandards.Screens.Display20DistanceFromBottom - MirrorStandards.Screens.Display20RearHeight / 2d,
                    DrawShape.CSCoordinate.Center);
            }
            else if (sideToDraw is DrawnSide.Side)
            {
                throw new NotImplementedException("Side Draw is Not Implemented -- Touch Draw could not be Drawn ");
            }
            else
            {
                throw new InvalidOperationException($"Could not Recognize DrawnSide : -{sideToDraw}- or Null -- Touch Draw could not be drawn ");
            }

            extraDraw = display;
        }

        /// <summary>
        /// Builds a Clock Draw at the Default Position
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        private void BuildClockDraw()
        {
            RectangleDraw clock = new();

            if (sideToDraw is DrawnSide.Front)
            {
                clock.Length = MirrorStandards.Screens.ClockLength;
                clock.Height = MirrorStandards.Screens.ClockHeight;
                clock.SetCenterOrStartX(extrasBoundaryCenterX, DrawShape.CSCoordinate.Center);
                clock.SetCenterOrStartY(extrasBoundaryCenterY + extrasBoundaryHeight / 2d - MirrorStandards.Screens.ClockDistanceFromBottom - MirrorStandards.Screens.ClockHeight / 2d,
                    DrawShape.CSCoordinate.Center);
            }
            else if (sideToDraw is DrawnSide.Rear)
            {
                clock.Length = MirrorStandards.Screens.ClockRearLength;
                clock.Height = MirrorStandards.Screens.ClockRearHeight;

                clock.SetCenterOrStartX(extrasBoundaryCenterX, DrawShape.CSCoordinate.Center);
                clock.SetCenterOrStartY(extrasBoundaryCenterY + extrasBoundaryHeight / 2d - MirrorStandards.Screens.ClockDistanceFromBottom - MirrorStandards.Screens.ClockRearHeight / 2d,
                    DrawShape.CSCoordinate.Center);
            }
            else if (sideToDraw is DrawnSide.Side)
            {
                throw new NotImplementedException("Side Draw is Not Implemented -- Touch Draw could not be Drawn ");
            }
            else
            {
                throw new InvalidOperationException($"Could not Recognize DrawnSide : -{sideToDraw}- or Null -- Touch Draw could not be drawn ");
            }

            extraDraw = clock;
        }

        /// <summary>
        /// Builds a Display11 Draw at the Default Position
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        private void BuildDisplay11Draw()
        {
            RectangleDraw display = new();

            if (sideToDraw is DrawnSide.Front)
            {
                display.Length = MirrorStandards.Screens.Display11Length;
                display.Height = MirrorStandards.Screens.Display11Height;
                display.SetCenterOrStartX(extrasBoundaryCenterX, DrawShape.CSCoordinate.Center);
                display.SetCenterOrStartY(extrasBoundaryCenterY + extrasBoundaryHeight / 2d - MirrorStandards.Screens.Display11DistanceFromBottom - MirrorStandards.Screens.Display11Height / 2d,
                    DrawShape.CSCoordinate.Center);
            }
            else if (sideToDraw is DrawnSide.Rear)
            {
                display.Length = MirrorStandards.Screens.Display11RearLength;
                display.Height = MirrorStandards.Screens.Display11RearHeight;

                display.SetCenterOrStartX(extrasBoundaryCenterX, DrawShape.CSCoordinate.Center);
                display.SetCenterOrStartY(extrasBoundaryCenterY + extrasBoundaryHeight / 2d - MirrorStandards.Screens.Display11DistanceFromBottom - MirrorStandards.Screens.Display11RearHeight / 2d,
                    DrawShape.CSCoordinate.Center);
            }
            else if (sideToDraw is DrawnSide.Side)
            {
                throw new NotImplementedException("Side Draw is Not Implemented -- Touch Draw could not be Drawn ");
            }
            else
            {
                throw new InvalidOperationException($"Could not Recognize DrawnSide : -{sideToDraw}- or Null -- Touch Draw could not be drawn ");
            }

            extraDraw = display;
        }

        /// <summary>
        /// Builds a Touch Draw at the Default Position
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        private void BuildTouchDraw()
        {
            if (sideToDraw is DrawnSide.Front)
            {
                ConcentricRectangles touch = new();
                touch.Length = MirrorStandards.Touch.OuterSquare;
                touch.Height = MirrorStandards.Touch.OuterSquare;
                touch.InnerRectangle.Length = MirrorStandards.Touch.InnerSquare;
                touch.InnerRectangle.Height = MirrorStandards.Touch.InnerSquare;

                touch.SetCenterOrStartX(extrasBoundaryCenterX, DrawShape.CSCoordinate.Center);
                touch.SetCenterOrStartY(extrasBoundaryCenterY + extrasBoundaryHeight / 2d - MirrorStandards.Touch.DistanceFromBottom - MirrorStandards.Touch.OuterSquare / 2d,
                    DrawShape.CSCoordinate.Center);

                extraDraw = touch;
            }
            else if (sideToDraw is DrawnSide.Rear)
            {
                RectangleDraw touch = new();
                touch.Length = MirrorStandards.Touch.BoxLength;
                touch.Height = MirrorStandards.Touch.BoxHeight;

                touch.SetCenterOrStartX(extrasBoundaryCenterX, DrawShape.CSCoordinate.Center);
                touch.SetCenterOrStartY(extrasBoundaryCenterY + extrasBoundaryHeight / 2d - MirrorStandards.Touch.DistanceFromBottom - MirrorStandards.Touch.BoxHeight / 2d,
                    DrawShape.CSCoordinate.Center);

                extraDraw = touch;
            }
            else if (sideToDraw is DrawnSide.Side)
            {
                throw new NotImplementedException("Side Draw is Not Implemented -- Touch Draw could not be Drawn ");
            }
            else
            {
                throw new InvalidOperationException($"Could not Recognize DrawnSide : -{sideToDraw}- or Null -- Touch Draw could not be drawn ");
            }
        }

        public void ResetBuilder()
        {
            extraDraw = null;
            isAdditiveDraw = false;
            extraToDraw = null;
            sideToDraw = null;
        }
    }
}
