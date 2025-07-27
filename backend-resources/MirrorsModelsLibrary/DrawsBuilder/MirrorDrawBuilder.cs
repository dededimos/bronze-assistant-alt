using MirrorsModelsLibrary.DrawsBuilder.ConcreteBuilders;
using SVGDrawingLibrary.Models;
using SVGDrawingLibrary.Models.ConcreteShapes;
using SVGDrawingLibrary.Enums;
using MirrorsModelsLibrary.DrawsBuilder.Models;
using MirrorsModelsLibrary.DrawsBuilder.Models.MeasureObjects;
using MirrorsModelsLibrary.Enums;
using MirrorsModelsLibrary.Models;
using MirrorsModelsLibrary.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsModelsLibrary.DrawsBuilder
{
    /// <summary>
    /// The Builder Director Class Used to Get Any of the Various Draws Needed to Construct a Mirror Draw
    /// </summary>
    public class MirrorDrawBuilder
    {
        /*NOTES
         The Bulder has the Next Tasks
        1.Builds the Front and Rear Draws
        2.Builds the Boundaries that Contain the Extras/Sandblast/Supports
        3.Builds the various Pieces of a Draw

        a.MirrDraw Class Calls the Builder at its creation to
        -Create All The Draws and set their Starting Position
        -Set the Dimensions where applicable
        -Set the Boundaries for the Draws
         */

        // The Drawing are done in Steps 1st Glass - 2nd Sandblast - 3rd Supports - 4th Extras
        private IDrawShapeBuilder _builder;

        /// <summary>
        /// The Mirror That is Being Drawn
        /// </summary>
        private readonly Mirror mirror;

        // The Boundary Properties defining the Rectangle/Circle in which the Mirror Extras can be Placed
        private double extrasBoundaryLength;
        private double extrasBoundaryHeight;
        private double extrasBoundaryDiameter;

        // The Boundary Properties defining the Rectangle/Circle in which the Mirror Sandblast can be Placed
        private double sandblastBoundaryLength;
        private double sandblastBoundaryHeight;
        private double sandblastBoundaryDiameter;

        // The Boundary Properties defining the Rectangle/Circle in which the Mirror Supports can be Placed (Valid for All Supports except Visible Frame)
        private double supportBoundaryLength;
        private double supportBoundaryHeight;
        private double supportBoundaryDiameter;

        private double dimArrowLength = 20;
        private double dimArrowThick = 10;

        // The Draw Container Properties that Define the container that encases the Mirror Draw
        private double containerMargin;
        private double containerLength;
        private double containerHeight;
        
        /// <summary>
        /// Instantiates a MirrorDraw Builder to construct all the Parts of a Mirror Draw
        /// </summary>
        /// <param name="mirrorToDraw">The Mirror which will be Drawn</param>
        /// <param name="container">The Container that will be encasing the Draw -- The Mirror Draw is Always Placed at the Center of the Container</param>
        public MirrorDrawBuilder(Mirror mirrorToDraw , MirrorDrawContainer container)
        {
            this.mirror = mirrorToDraw;
            this.containerMargin = container.MarginX; //Same for X or Y
            this.containerLength = container.Length;
            this.containerHeight = container.Height;
            InitilizeBuilderDimensioning();
        }

        /// <summary>
        /// Initializes the Builder by calculating the Boundary Areas
        /// </summary>
        private void InitilizeBuilderDimensioning()
        {
            CalculateSandblastBoundaryDimensions();
            CalculateSupportBoundaryDimensions();
            CalculateExtrasBoundaryDimensions();
        }

        /// <summary>
        /// Builds the Front Draw of the Mirror
        /// </summary>
        /// <returns>a MirrorDrawSide Object of the Front Draw</returns>
        public MirrorDrawSide BuildFrontDraw()
        {
            MirrorDrawSide frontDraw = new();
            frontDraw.GlassDraw = BuildMirrorGlass(DrawnSide.Front);
            frontDraw.SupportDraw = BuildMirrorSupport(DrawnSide.Front);
            if (frontDraw.SupportDraw != null)
            {
                frontDraw.SupportDraw.Fill = GetMirrorFrameColor();
            }
            frontDraw.SandblastDraw = BuildMirrorSandblastDraw();

            foreach (MirrorExtra extra in mirror.Extras)
            {
                DrawShape extraDraw = BuildMirrorExtra(DrawnSide.Front, extra.Option, false);
                if (extraDraw != null){frontDraw.ExtrasDraws.Add(extraDraw);}
                
                // Add the Sandblast Magnifyer Draw 
                if (extra.Option is MirrorOption.ZoomLed or MirrorOption.ZoomLedTouch)
                {
                    DrawShape additionalDraw = BuildMirrorExtra(DrawnSide.Front, extra.Option, true);
                    if (additionalDraw is not null) { frontDraw.ExtrasDraws.Add (additionalDraw); };
                }
            }

            // Build Glass Dimensions
            frontDraw.DimensionsDraws.AddRange(BuildGlassDimensionsDraws());

            return frontDraw;
        }

        /// <summary>
        /// Builds the Rear Draw of the Mirror
        /// </summary>
        /// <returns>a MirrorDrawSide Object of the Rear Draw</returns>
        public MirrorDrawSide BuildRearDraw()
        {
            MirrorDrawSide rearDraw = new();
            
            // Build the Glass
            rearDraw.GlassDraw = BuildMirrorGlass(DrawnSide.Rear);
            
            // Build the Support
            rearDraw.SupportDraw = BuildMirrorSupport(DrawnSide.Rear);
            
            // Build the Sandblast
            rearDraw.SandblastDraw = BuildMirrorSandblastDraw();

            // Build All Extras
            foreach (MirrorExtra extra in mirror.Extras)
            {
                DrawShape extraDraw = BuildMirrorExtra(DrawnSide.Rear, extra.Option, false);
                if (extraDraw is not null) {rearDraw.ExtrasDraws.Add(extraDraw);}

                // Add the Sandblast Magnifyer Draw 
                if (extra.Option is MirrorOption.ZoomLed or MirrorOption.ZoomLedTouch)
                {
                    DrawShape additionalDraw = BuildMirrorExtra(DrawnSide.Rear, extra.Option, true);
                    if (additionalDraw is not null) { rearDraw.ExtrasDraws.Add(additionalDraw); };
                }
            }

            // Build Glass Dimensions
            rearDraw.DimensionsDraws.AddRange(BuildGlassDimensionsDraws());

            return rearDraw;
        }

        /// <summary>
        /// Builds a Side Draw of the Mirror
        /// </summary>
        /// <returns>a MirrorDrawSide Object of the Side Draw of the Mirror</returns>
        public MirrorDrawSide BuildSideDraw()
        {
            return null;
        }

        #region 1.ConcreteBuilders Products

        /// <summary>
        /// Returns the Glass Draw of the Chose Draw Side
        /// </summary>
        /// <param name="sideToDraw">The Side which will be Drawn</param>
        /// <returns>Null when the Mirror Shape is null - otherwise the Glass's Shape</returns>
        /// <exception cref="NotImplementedException">For Not implemented Options</exception>
        private DrawShape BuildMirrorGlass(DrawnSide sideToDraw)
        {
            if (sideToDraw is DrawnSide.Front or DrawnSide.Rear)
            {
                switch (mirror.Shape)
                {
                    case MirrorShape.Rectangular:
                        _builder = new MirrorGlassDrawBuilder(
                            mirror.Lengthmm,
                            mirror.Heightmm,
                            containerMargin,
                            mirror.HasExtra(MirrorOption.RoundedCorners),
                            mirror.HasVisibleFrame());
                        break;
                    case MirrorShape.Capsule:
                        _builder = new MirrorGlassDrawBuilder(
                            mirror.Lengthmm,
                            mirror.Heightmm,
                            containerMargin,MirrorShape.Capsule);
                        break;
                    case MirrorShape.Ellipse:
                        _builder = new MirrorGlassDrawBuilder(
                            mirror.Lengthmm,
                            mirror.Heightmm,
                            containerMargin, MirrorShape.Ellipse);
                        break;
                    case MirrorShape.Circular:
                        _builder = new MirrorGlassDrawBuilder(
                            mirror.Diametermm,
                            containerMargin,
                            mirror.HasVisibleFrame());
                        break;
                    case MirrorShape.Special:
                        throw new NotImplementedException("Special Mirror Shape Draw Not Implemented");
                    default:
                        return null;
                }
            }
            else
            {
                throw new NotImplementedException("SideDraw Not Implemented");
            }
            return _builder.BuildShape();
        }

        /// <summary>
        /// Returns the Support Draw of the Chosen Drawn Side
        /// </summary>
        /// <param name="sideToDraw">Side that will be drawn</param>
        /// <returns>The SupportDraw Object or Null if there is not One</returns>
        /// <exception cref="NotImplementedException">When Mirror Shape is Special</exception>
        private DrawShape BuildMirrorSupport(DrawnSide sideToDraw)
        {
            if (mirror.Shape is MirrorShape.Rectangular or MirrorShape.Capsule or MirrorShape.Ellipse)
            {
                //If the Mirror has Visible Frame the Inner Support Boundary is of No Use -- we must Pass the Boundary for the outer Frame which is the Mirror Itself
                _builder = new MirrorSupportDrawBuilder(
                            mirror.Support.SupportType,
                            sideToDraw,
                            mirror.HasVisibleFrame() ? BuildVisibleFrameDrawBoundary() : BuildSupportBoundary(),
                            (MirrorShape)mirror.Shape);
            }
            else if (mirror.Shape is MirrorShape.Circular)
            {
                //If the Mirror has Visible Frame the Inner Support Boundary is of No Use -- we must Pass the Boundary for the outer Frame which is the Mirror Itself
                _builder = new MirrorSupportDrawBuilder(
                            mirror.Support.SupportType,
                            sideToDraw,
                            mirror.HasVisibleFrame() ? BuildVisibleFrameDrawBoundary() : BuildSupportBoundary(),
                            mirror.HasLight());
            }
            else if (mirror.Shape is MirrorShape.Special)
            {
                throw new NotImplementedException("Special Mirror Shape not Implemented -- Support Draw could not be Built");
            }
            else
            {
                _builder = null;
            }

            if (_builder is not null)
            {
                return _builder.BuildShape();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the Sandblast Draw of the Mirror (The Draw is the Same for both drawn Sides
        /// </summary>
        /// <returns>The Sandblast Draw</returns>
        /// <exception cref="NotImplementedException">When the Mirror is of a Special Shape</exception>
        private DrawShape BuildMirrorSandblastDraw()
        {
            if (mirror.Shape is MirrorShape.Rectangular)
            {
                _builder = new MirrorSandblastDrawBuilder(
                    mirror.Sandblast,
                    BuildSandblastBoundary(),
                    mirror.HasExtra(MirrorOption.RoundedCorners),
                    mirror.HasVisibleFrame());
            }
            else if (mirror.Shape is MirrorShape.Circular)
            {
                _builder = new MirrorSandblastDrawBuilder(
                    mirror.Sandblast,
                    BuildSandblastBoundary());
            }
            else if (mirror.Shape is MirrorShape.Capsule or MirrorShape.Ellipse)
            {
                return null;//Capsule Cannot have Sandblast
            }
            else if (mirror.Shape is MirrorShape.Special)
            {
                throw new NotImplementedException("Special Mirror Shape not Implemented -- Support Draw could not be Built");
            }
            else
            {
                _builder = null;
            }

            if (_builder is not null)
            {
                return _builder.BuildShape();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Builds the Shape of the Extra 
        /// </summary>
        /// <param name="sideToDraw">The Side that is Being Drawn</param>
        /// <param name="extra">The Extra that we want to Draw</param>
        /// <param name="isAdditiveDraw">If this is the Additional Part of the extra (Ex.Magnifyer Sandblast)</param>
        /// <returns></returns>
        private DrawShape BuildMirrorExtra(DrawnSide sideToDraw , MirrorOption extra ,bool isAdditiveDraw)
        {
            _builder = new MirrorExtrasDrawBuilder(extra, 
                                                   sideToDraw,
                                                   BuildExtrasBoundary(),
                                                   mirror,
                                                   isAdditiveDraw);
            return _builder.BuildShape();            
        }

        #endregion

        #region 2.BoundariesCalculations

        /*Notes
         * The Builder Acts as Follows
         * 1.Calculates the Sandblast Boundary
         * 2.With the Sandblast Boundary it Calculates the Supports Boundary
         * 3.With the Supports Boundary it calculates the Extras Boundary 
         * 
         * Each of these Boundaries are arguments in the Respective Builders of (Sandblast/Support or Extra)
         * This way the Builder can place the items inside their Boundaries and try to prevent as possible initial collisions
         * Any Collisions thought that might happen between the items are then Resolved in the Mirror Draw Class , which can reposition its internal Elements(Extras) 
         */

        /// <summary>
        /// When there is a Support Calculates the Available Area on the Back for Extras
        /// The Extras Area is Defined with the Combo of Support Boundary and Support Type
        /// </summary>
        private void CalculateExtrasBoundaryDimensions()
        {
            //When there is not Back Support -- there is not Any Area available for Extras
            if (mirror.HasBackSupport() == false)
            {
                extrasBoundaryDiameter = 0;
                extrasBoundaryLength = 0;
                extrasBoundaryHeight = 0;
            }
            else if (mirror.Shape is MirrorShape.Rectangular)
            {
                //Check Support Type
                switch (mirror.Support.SupportType)
                {

                    case MirrorSupport.Double:
                        //The Extrass Boundary Area is the Support Boundary Area Minus the Double Supports (Height Constraint - -Same DIstance as Perimetrical Support)
                        extrasBoundaryLength = supportBoundaryLength;
                        extrasBoundaryHeight = supportBoundaryHeight - 2 * (MirrorStandards.Frames.PerimetricalDistanceFromEdge + MirrorStandards.Frames.PerimetricalThickness);
                        break;
                    case MirrorSupport.Perimetrical:
                        //The Extrass Boundary Area is the Support Boundary Area Minus the Perimetrical Supports Properties
                        extrasBoundaryLength = supportBoundaryLength - 2 * (MirrorStandards.Frames.PerimetricalDistanceFromEdge + MirrorStandards.Frames.PerimetricalThickness);
                        extrasBoundaryHeight = supportBoundaryHeight - 2 * (MirrorStandards.Frames.PerimetricalDistanceFromEdge + MirrorStandards.Frames.PerimetricalThickness);
                        break;
                    case MirrorSupport.Frame:
                        //When we have Frame the Extras Boundary is Defined by the Sandblasts - because they are inside of the Frame
                        extrasBoundaryDiameter = supportBoundaryDiameter;
                        extrasBoundaryLength = supportBoundaryLength;
                        extrasBoundaryHeight = supportBoundaryHeight;
                        break;
                    case MirrorSupport.Without:
                    case MirrorSupport.FrontSupports:
                    default:
                        throw new InvalidOperationException("Something Went Wrong -- Mirror Should Have a Back Support and appears as it Does not");
                }

            }
            else if (mirror.Shape is MirrorShape.Capsule)
            {
                //Check Support Type
                switch (mirror.Support.SupportType)
                {

                    case MirrorSupport.Double:
                    case MirrorSupport.Perimetrical:
                        //Capsule Boundary does not take any HeightDistance from Edges when Vertical or LengthDistance from Edges when Horizontal
                        extrasBoundaryLength = supportBoundaryLength - 2 * (MirrorStandards.Frames.PerimetricalCapsuleThickness + MirrorStandards.Frames.PerimetricalCapsuleDistanceFromEdge);
                        extrasBoundaryHeight = supportBoundaryHeight - 2 * (MirrorStandards.Frames.PerimetricalCapsuleThickness + MirrorStandards.Frames.PerimetricalCapsuleDistanceFromEdge);
                        break;
                    case MirrorSupport.Frame:
                        throw new NotSupportedException("Frame is Not Supported for Capsule Mirrors - Could not Calculate Extras Boundary");
                    case MirrorSupport.Without:
                    case MirrorSupport.FrontSupports:
                    default:
                        throw new InvalidOperationException("Something Went Wrong -- Mirror Should Have a Back Support and appears as it Does not");
                }
            }
            else if (mirror.Shape is MirrorShape.Ellipse)
            {
                //Check Support Type
                switch (mirror.Support.SupportType)
                {
                    case MirrorSupport.Double:
                    case MirrorSupport.Perimetrical:
                        //Capsule Boundary does not take any HeightDistance from Edges when Vertical or LengthDistance from Edges when Horizontal
                        extrasBoundaryLength = supportBoundaryLength - 2 * (MirrorStandards.Frames.PerimetricalEllipseThickness + MirrorStandards.Frames.PerimetricalEllipseDistanceFromEdge);
                        extrasBoundaryHeight = supportBoundaryHeight - 2 * (MirrorStandards.Frames.PerimetricalEllipseThickness + MirrorStandards.Frames.PerimetricalEllipseDistanceFromEdge);
                        break;
                    case MirrorSupport.Frame:
                        throw new NotSupportedException("Frame is Not Supported for Ellipse Mirrors - Could not Calculate Extras Boundary");
                    case MirrorSupport.Without:
                    case MirrorSupport.FrontSupports:
                    default:
                        throw new InvalidOperationException("Something Went Wrong -- Mirror Should Have a Back Support and appears as it Does not");
                }
            }
            else if (mirror.Shape is MirrorShape.Circular)
            {
                //Check Support Type
                switch (mirror.Support.SupportType)
                {
                    case MirrorSupport.Perimetrical:
                        extrasBoundaryDiameter = supportBoundaryDiameter - MirrorStandards.Frames.PerimetricalCircularThickness - 2*MirrorStandards.Frames.PerimetricalCircularDistanceFromEdge;
                        break;
                    case MirrorSupport.Double:
                    case MirrorSupport.Frame:
                        extrasBoundaryDiameter = supportBoundaryDiameter;
                        extrasBoundaryLength = supportBoundaryLength;
                        extrasBoundaryHeight = supportBoundaryHeight;
                        break;
                    case MirrorSupport.Without:
                    case MirrorSupport.FrontSupports:
                    default:
                        throw new InvalidOperationException("Something Went Wrong -- Mirror Should Have a Back Support and appears as it Does not");
                }
            }
            else
            {
                extrasBoundaryDiameter = 0;
                extrasBoundaryLength = 0;
                extrasBoundaryHeight = 0;
            }
        }

        /// <summary>
        /// When there is Frame Around all the Sandblasts are Made witing some distance of the Frame--
        /// When there is no frame the sandblasts are made at their default positions from the glass--
        /// The Boundary Area Defines where the Rear Frame Stops and Glass Starts
        /// </summary>
        private void CalculateSandblastBoundaryDimensions()
        {
            if (mirror.Shape is MirrorShape.Rectangular)
            {
                //We only need to Calculate the Height and Length of the Available Area after putting the Frame or Just of the Glass
                //Then when there is Frame the Sandblast will be drawn at a certain distance from it. When there is not the Sandblasts will be Drawn at their Default positions
                sandblastBoundaryLength = mirror.HasVisibleFrame()
                    ? mirror.Lengthmm - 2 * (MirrorStandards.Frames.FrameFrontThickness + MirrorStandards.Frames.FrameRearThickness)
                    : mirror.Lengthmm;
                sandblastBoundaryHeight = mirror.HasVisibleFrame()
                    ? mirror.Heightmm - 2 * (MirrorStandards.Frames.FrameFrontThickness + MirrorStandards.Frames.FrameRearThickness)
                    : mirror.Heightmm;
            }
            else if (mirror.Shape is MirrorShape.Circular)
            {
                sandblastBoundaryDiameter = mirror.HasVisibleFrame()
                    ? mirror.Diametermm - 2 * MirrorStandards.Frames.FrameFrontCircularThickness
                    : mirror.Diametermm;
            }
            else if(mirror.Shape is MirrorShape.Capsule or MirrorShape.Ellipse)
            {
                if (mirror.HasVisibleFrame())
                {
                    throw new NotSupportedException("Capsules/Ellipses with Frame are not Supported for Drawing");
                }
                sandblastBoundaryLength = mirror.Lengthmm;
                sandblastBoundaryHeight = mirror.Heightmm;
            }
            else
            {
                sandblastBoundaryLength = 0;
                sandblastBoundaryHeight = 0;
                sandblastBoundaryDiameter = 0;
                //There is no Boundary
            }
        }

        /// <summary>
        /// When there is a Sandblast the Back Supports are Confined to a certaian smaller area than the glass (Except wqhen there is a visible Frame)--
        /// This is the area that gets calculated by this method
        /// </summary>
        private void CalculateSupportBoundaryDimensions()
        {
            //When there is no Sandblast the Support Boundary Matches that of the Sandblast Boundary Area
            if (mirror.HasSandblast() == false)
            {
                supportBoundaryLength = sandblastBoundaryLength;
                supportBoundaryHeight = sandblastBoundaryHeight;
                supportBoundaryDiameter = sandblastBoundaryDiameter;
            }

            // Else check the Shape Cases (Where in all Cases the Mirror Has Sandblast
            else if (mirror.Shape is MirrorShape.Rectangular)
            {
                //Check the which Sandblast
                switch (mirror.Sandblast)
                {
                    //When there is Visible Frame this Boundary is Used to Calculate the ExtrasBoundary so we have to add where needed the SandblastFrameMargins
                    case MirrorSandblast.H8:
                        supportBoundaryLength = sandblastBoundaryLength - 2 * (MirrorStandards.Sandblasts.DistanceFromSideH8 + MirrorStandards.Sandblasts.ThicknessH8);
                        supportBoundaryHeight = sandblastBoundaryHeight - 2 * (MirrorStandards.Sandblasts.DistanceFromTopH8 + MirrorStandards.Sandblasts.ThicknessH8);
                        break;
                    case MirrorSandblast.X6:
                        supportBoundaryLength = sandblastBoundaryLength;
                        supportBoundaryHeight = sandblastBoundaryHeight - 2 * (MirrorStandards.Sandblasts.ThicknessX6 + (mirror.HasVisibleFrame() ? MirrorStandards.Sandblasts.SandblastFrameMargin : 0));
                        break;
                    case MirrorSandblast.X4:
                        supportBoundaryLength = sandblastBoundaryLength - 2 * (MirrorStandards.Sandblasts.ThicknessX4 + (mirror.HasVisibleFrame() ? MirrorStandards.Sandblasts.SandblastFrameMargin : 0));
                        supportBoundaryHeight = sandblastBoundaryHeight;
                        break;
                    case MirrorSandblast._6000:
                        supportBoundaryLength = sandblastBoundaryLength - 2 * (MirrorStandards.Sandblasts.DistanceFromSide6000 + MirrorStandards.Sandblasts.Thickness6000);
                        supportBoundaryHeight = sandblastBoundaryHeight;
                        break;
                    case MirrorSandblast.M3:
                        supportBoundaryLength = sandblastBoundaryLength;
                        supportBoundaryHeight = sandblastBoundaryHeight - (MirrorStandards.Sandblasts.DistanceFromTopM3 + MirrorStandards.Sandblasts.ThicknessM3);
                        //The Arranging Method should place the Support Shifted in the Y Axis for this Kind of Sandblast
                        break;
                    case MirrorSandblast.N6:
                        throw new InvalidOperationException("Something Went Wrong -- Mirror Supposed to Be Rectangular");
                    case MirrorSandblast.N9:
                    case MirrorSandblast.H7:
                    case MirrorSandblast.Special:
                    default:
                        throw new InvalidOperationException("Something went Wrong -- Mirror Supposed to Have Sandblast");
                }

            }

            else if (mirror.Shape is MirrorShape.Circular)
            {
                //Check the which Sandblast
                switch (mirror.Sandblast)
                {

                    case MirrorSandblast.N6:
                        supportBoundaryDiameter = sandblastBoundaryDiameter - MirrorStandards.Sandblasts.ThicknessN6;
                        break;
                    case MirrorSandblast.H8:
                    case MirrorSandblast.X6:
                    case MirrorSandblast.X4:
                    case MirrorSandblast._6000:
                    case MirrorSandblast.M3:
                        throw new InvalidOperationException("Something Went Wrong -- Mirror Supposed to Be Circular");
                    case MirrorSandblast.N9:
                    case MirrorSandblast.H7:
                    case MirrorSandblast.Special:
                    default:
                        throw new InvalidOperationException("Something went Wrong -- Mirror Supposed to Have Sandblast");
                }
            }
            else
            {
                supportBoundaryDiameter = 0;
                supportBoundaryHeight = 0;
                supportBoundaryLength = 0;
            }
        }

        /// <summary>
        /// Builds the Shape of the Area that the Sandblast can be drawn into
        /// </summary>
        /// <returns>the Boundary Shape</returns>
        public DrawShape BuildSandblastBoundary()
        {
            DrawShape boundary;
            if (sandblastBoundaryDiameter is not 0)
            {
                CircleDraw sandblastBoundaryCircle = new();
                sandblastBoundaryCircle.Diameter = sandblastBoundaryDiameter;
                sandblastBoundaryCircle.SetCenterOrStartX(containerLength / 2d);
                sandblastBoundaryCircle.SetCenterOrStartY(containerHeight / 2d);
                boundary = sandblastBoundaryCircle;
            }
            else if (sandblastBoundaryHeight is not 0)
            {
                if (mirror.Shape is MirrorShape.Rectangular)
                {
                    RectangleDraw sandblastBoundaryRect = new();
                    sandblastBoundaryRect.Length = sandblastBoundaryLength;
                    sandblastBoundaryRect.Height = sandblastBoundaryHeight;
                    sandblastBoundaryRect.SetCenterOrStartX(containerLength / 2d, DrawShape.CSCoordinate.Center);
                    sandblastBoundaryRect.SetCenterOrStartY(containerHeight / 2d, DrawShape.CSCoordinate.Center);
                    boundary = sandblastBoundaryRect;
                }
                else if (mirror.Shape is MirrorShape.Capsule)
                {
                    CapsuleRectangleDraw sandblastBoundaryCapsule = new(sandblastBoundaryLength, sandblastBoundaryHeight);
                    sandblastBoundaryCapsule.SetCenterOrStartX(containerLength / 2d, DrawShape.CSCoordinate.Center);
                    sandblastBoundaryCapsule.SetCenterOrStartY(containerHeight / 2d, DrawShape.CSCoordinate.Center);
                    boundary = sandblastBoundaryCapsule;
                }
                else if (mirror.Shape is MirrorShape.Ellipse)
                {
                    EllipseDraw sandblastBoundaryEllipse = new(sandblastBoundaryLength, sandblastBoundaryHeight);
                    sandblastBoundaryEllipse.SetCenterOrStartX(containerLength / 2d, DrawShape.CSCoordinate.Center);
                    sandblastBoundaryEllipse.SetCenterOrStartY(containerHeight / 2d, DrawShape.CSCoordinate.Center);
                    boundary = sandblastBoundaryEllipse;
                }
                else
                {
                    throw new NotSupportedException("Sandblast Boundary with Height can only be a Rectangle-Capsule or Ellipse");
                }
            }
            else
            {
                boundary = mirror.Shape switch
                {
                    MirrorShape.Circular => new CircleDraw(),
                    MirrorShape.Capsule => new CapsuleRectangleDraw(0, 0),
                    MirrorShape.Ellipse => new EllipseDraw(0, 0),
                    _ => new RectangleDraw(),
                };
            }
            return boundary;
        }

        /// <summary>
        /// Builds the Shape of the Area that the Support can be drawn into (Except Visible Frame which is always drawn in the whole mirror)
        /// </summary>
        /// <returns>The Boundary shape</returns>
        public DrawShape BuildSupportBoundary()
        {
            //The Support Boundary on the Back is Defined by the Sandblasts of the Glass

            DrawShape boundary;
            if (supportBoundaryDiameter is not 0)
            {
                CircleDraw supportBoundaryCircle = new();
                supportBoundaryCircle.Diameter = supportBoundaryDiameter;
                supportBoundaryCircle.SetCenterOrStartX(containerLength / 2d);
                supportBoundaryCircle.SetCenterOrStartY(containerHeight / 2d);
                boundary = supportBoundaryCircle;
            }
            else if (supportBoundaryHeight is not 0)
            {
                if (mirror.Shape is MirrorShape.Rectangular)
                {
                    RectangleDraw supportBoundaryRect = new();
                    supportBoundaryRect.Length = supportBoundaryLength;
                    supportBoundaryRect.Height = supportBoundaryHeight;

                    supportBoundaryRect.SetCenterOrStartX(containerLength / 2d, DrawShape.CSCoordinate.Center);
                    //Shifted from Center only when there is M3 Sandblast
                    if (mirror.Sandblast is not MirrorSandblast.M3)
                    {
                        supportBoundaryRect.SetCenterOrStartY(containerHeight / 2d, DrawShape.CSCoordinate.Center);
                    }
                    else
                    {
                        supportBoundaryRect.SetCenterOrStartY(containerHeight / 2d + (MirrorStandards.Sandblasts.ThicknessM3 + MirrorStandards.Sandblasts.DistanceFromTopM3) / 2d, DrawShape.CSCoordinate.Center);
                    }

                    boundary = supportBoundaryRect; 
                }
                else if (mirror.Shape is MirrorShape.Capsule)
                {
                    CapsuleRectangleDraw supportBoundaryCapsule = new(supportBoundaryLength, supportBoundaryHeight);
                    supportBoundaryCapsule.SetCenterOrStartX(containerLength / 2d, DrawShape.CSCoordinate.Center);
                    supportBoundaryCapsule.SetCenterOrStartY(containerHeight / 2d, DrawShape.CSCoordinate.Center);
                    boundary = supportBoundaryCapsule;
                }
                else if (mirror.Shape is MirrorShape.Ellipse)
                {
                    EllipseDraw supportBoundaryEllipse = new(supportBoundaryLength, supportBoundaryHeight);
                    supportBoundaryEllipse.SetCenterOrStartX(containerLength / 2d, DrawShape.CSCoordinate.Center);
                    supportBoundaryEllipse.SetCenterOrStartY(containerHeight / 2d, DrawShape.CSCoordinate.Center);
                    boundary = supportBoundaryEllipse;
                }
                else
                {
                    throw new NotSupportedException("Support Boundary with Height can only be a Rectangle,Capsule or Ellipse");
                }
            }
            else
            {
                boundary = mirror.Shape switch
                {
                    MirrorShape.Circular => new CircleDraw(),
                    MirrorShape.Capsule => new CapsuleRectangleDraw(0, 0),
                    MirrorShape.Ellipse => new EllipseDraw(0, 0),
                    _ => new RectangleDraw(),
                };
            }
            return boundary;
        }

        /// <summary>
        /// Builds the Boundary for the Visible Frame (Used only to Construct the VisibleFrame)
        /// </summary>
        /// <returns>the Draw Boundary of the Visible Frame</returns>
        private DrawShape BuildVisibleFrameDrawBoundary()
        {
            if (mirror.Shape is MirrorShape.Rectangular)
            {
                RectangleDraw boundary = new();
                boundary.Length = mirror.Lengthmm;
                boundary.Height = mirror.Heightmm;
                boundary.SetCenterOrStartX(containerLength / 2d, DrawShape.CSCoordinate.Center);
                boundary.SetCenterOrStartY(containerHeight / 2d, DrawShape.CSCoordinate.Center);
                return boundary;
            }
            else if (mirror.Shape is MirrorShape.Circular)
            {
                CircleDraw boundary = new();
                boundary.Diameter = mirror.Diametermm;
                boundary.SetCenterOrStartX(containerLength / 2d, DrawShape.CSCoordinate.Center);
                boundary.SetCenterOrStartY(containerHeight / 2d, DrawShape.CSCoordinate.Center);
                return boundary;
            }
            else
            {
                return new RectangleDraw();
            }
        }

        /// <summary>
        /// Builds the Shape of the Area that the Extras can be drawn into
        /// </summary>
        /// <returns>The Boundary shape</returns>
        public DrawShape BuildExtrasBoundary()
        {
            //The Extras Boundary is Always calculated from the Numbers of the SupportBoundary
            //The Extras Boundary Shifts together with the Support Boundary so it can be placed at the Same Center

            DrawShape boundary;
            if (extrasBoundaryDiameter is not 0)
            {
                CircleDraw extrasBoundaryCircle = new();
                extrasBoundaryCircle.Diameter = extrasBoundaryDiameter;
                boundary = extrasBoundaryCircle;
            }
            else if (supportBoundaryHeight is not 0)
            {
                if (mirror.Shape is MirrorShape.Rectangular)
                {
                    RectangleDraw extrasBoundaryRect = new();
                    extrasBoundaryRect.Length = extrasBoundaryLength;
                    extrasBoundaryRect.Height = extrasBoundaryHeight;
                    boundary = extrasBoundaryRect;
                }
                else if (mirror.Shape is MirrorShape.Capsule) 
                {
                    CapsuleRectangleDraw extrasBoundaryCapsule = new(extrasBoundaryLength,extrasBoundaryHeight);
                    boundary = extrasBoundaryCapsule;
                }
                else if (mirror.Shape is MirrorShape.Ellipse)
                {
                    EllipseDraw extrasBoundaryEllipse = new(extrasBoundaryLength, extrasBoundaryHeight);
                    boundary = extrasBoundaryEllipse;
                }
                else
                {
                    throw new NotSupportedException("Extras Boundary with Height can only be a Rectangle,Capsule or Ellipse");
                }
                
            }
            else
            {
                boundary = new RectangleDraw();
            }
            boundary.SetCenterOrStartX(BuildSupportBoundary().ShapeCenterX, DrawShape.CSCoordinate.Center);
            boundary.SetCenterOrStartY(BuildSupportBoundary().ShapeCenterY, DrawShape.CSCoordinate.Center);
            return boundary;
        }

        #endregion

        #region 3.Dimensions and Visuals
  
        /// <summary>
        /// Gets the List of Dimensions Draws for the Glass of the Mirror
        /// </summary>
        /// <returns>The List of Dimensions Draw Shapes</returns>
        private List<DrawShape> BuildGlassDimensionsDraws()
        {
            List<DrawShape> dimensions = new();
            if (mirror.Shape is MirrorShape.Rectangular or MirrorShape.Capsule or MirrorShape.Ellipse)
            {
                DimensionLineDraw heightDimension = new();
                heightDimension.StartX = containerMargin / 2d;
                heightDimension.EndX = heightDimension.StartX;
                heightDimension.StartY = containerMargin;
                heightDimension.EndY = heightDimension.StartY + mirror.Heightmm;
                heightDimension.ArrowLength = dimArrowLength;
                heightDimension.ArrowThickness = dimArrowThick;
                heightDimension.Name = DrawShape.HEIGHTDIM;
                heightDimension.Stroke = "lightslategray";
                heightDimension.Fill = "lightslategray";

                DimensionLineDraw lengthDimension = new();
                lengthDimension.StartX = containerMargin;
                lengthDimension.EndX = lengthDimension.StartX + mirror.Lengthmm;
                lengthDimension.StartY = containerMargin / 2d;
                lengthDimension.EndY = lengthDimension.StartY;
                lengthDimension.ArrowLength = dimArrowLength;
                lengthDimension.ArrowThickness = dimArrowThick;
                lengthDimension.Name = DrawShape.LENGTHDIM;
                lengthDimension.Stroke = "lightslategray";
                lengthDimension.Fill = "lightslategray";

                dimensions.Add(heightDimension);
                dimensions.Add(lengthDimension);
            }
            else if (mirror.Shape == MirrorShape.Circular)
            {
                //not implemented
            }
            else
            {
                // do nothing
            }
            return dimensions;
        }

        /// <summary>
        /// Sets the Stroke-Fill-Filter-Opacity Properties to the Mirror Draw
        /// </summary>
        /// <param name="mirrorDraw">The Mirror Draw to Modify</param>
        private void SetDrawVisualProperties(MirrorDraw mirrorDraw)
        {
           // mirrorDraw.GlassDraw.Stroke = "lightslategray";
           // mirrorDraw.GlassDraw.Fill = "aliceblue";
           // mirrorDraw.SandblastDraw.Stroke = "lightslategray";
           // mirrorDraw.SandblastDraw.Fill = "lightgray";
           // mirrorDraw.SandblastDraw.Opacity = "0.9";
           //
           // mirrorDraw.MagnifyerSandblastDraw.Stroke = "lightslategray";
           // mirrorDraw.MagnifyerSandblastDraw.Fill = "lightgray";
           // mirrorDraw.MagnifyerSandblastDraw.Opacity = "0.9";
           // mirrorDraw.MagnifyerDraw.Stroke = "lightslategray";
           // mirrorDraw.MagnifyerDraw.Fill = "aliceblue";
           // mirrorDraw.MagnifyerDraw.Filter = sideToDraw == DrawnSide.Front ? "URL(#inset-shadow)" : "";
           //
           // foreach (var draw in mirrorDraw.ExtrasDraws)
           // {
           //     draw.Stroke = "lightslategray";
           // }
           //
           // if (sideToDraw == DrawnSide.Rear)
           // {
           //     mirrorDraw.SupportDraw.Fill = "URL(#hatchPattern)";
           //     mirrorDraw.SupportDraw.Stroke = "black";
           // }
           // else
           // {
           //     mirrorDraw.SupportDraw.Fill = GetMirrorFrameColor();
           //     mirrorDraw.SupportDraw.Stroke = "ghostWhite";
           // }
           //
           // foreach (var draw in mirrorDraw.GlassDimensionsDraws)
           // {
           //     draw.Stroke = "lightslategray";
           //     draw.Fill = "lightslategray";
           // }
        }

        /// <summary>
        /// Gets the Color of the Frame
        /// </summary>
        /// <returns>the String representing the Color of the Frame</returns>
        private string GetMirrorFrameColor()
        {
            string mirrorFrameFill = "black";
            if (mirror?.Support?.PaintFinish != null)
            {
                switch (mirror.Support.PaintFinish)
                {
                    case SupportPaintFinish.Black:
                        mirrorFrameFill = "url(#FrameBlackGradient)";
                        break;
                    case SupportPaintFinish.ChromeMat:
                        mirrorFrameFill = "var(--BackgroundColor)";
                        break;
                    case SupportPaintFinish.GoldMat:
                        mirrorFrameFill = "url(#FrameGoldGradient)";
                        break;
                    case SupportPaintFinish.GraphiteMat:
                        mirrorFrameFill = "url(#FrameGraphiteGradient)";
                        break;
                    case SupportPaintFinish.BronzeMat:
                        mirrorFrameFill = "url(#FrameBronzeGradient)";
                        break;
                    case SupportPaintFinish.CopperMat:
                        mirrorFrameFill = "url(#FrameCopperGradient)";
                        break;
                    case SupportPaintFinish.Silver:
                        mirrorFrameFill = "url(#FrameChromeGradient)";
                        break;
                    case SupportPaintFinish.RalColor:
                    default:
                        mirrorFrameFill = "url(#FrameBlackGradient)";
                        break;
                }
            }
            else if (mirror?.Support?.ElectroplatedFinish != null)
            {
                switch (mirror.Support.ElectroplatedFinish)
                {
                    case SupportElectroplatedFinish.RoseGoldBrushed:
                    case SupportElectroplatedFinish.RoseGoldMirror:
                        mirrorFrameFill = "url(#FrameCopperGradient)";
                        break;
                    case SupportElectroplatedFinish.GoldSimilarMirror:
                    case SupportElectroplatedFinish.GoldSimilarBrushed:
                        mirrorFrameFill = "url(#FrameGoldGradient)";
                        break;
                    case SupportElectroplatedFinish.RealGoldMirror:
                    case SupportElectroplatedFinish.RealGoldBrushed:
                        mirrorFrameFill = "url(#FrameGoldGradient)";
                        break;
                    case SupportElectroplatedFinish.GraphiteBrushed:
                    case SupportElectroplatedFinish.GraphiteMirror:
                        mirrorFrameFill = "url(#FrameGraphiteGradient)";
                        break;
                    case SupportElectroplatedFinish.NickelBrushed:
                        mirrorFrameFill = "var(--BackgroundColor)";
                        break;
                    case SupportElectroplatedFinish.NickelMirror:
                        mirrorFrameFill = "url(#FrameChromeGradient)";
                        break;
                    case SupportElectroplatedFinish.CopperBrushed:
                    case SupportElectroplatedFinish.CopperMirror:
                        mirrorFrameFill = "url(#FrameCopperGradient)";
                        break;
                    case SupportElectroplatedFinish.BronzeBrushed:
                    case SupportElectroplatedFinish.BronzeMirror:
                        mirrorFrameFill = "url(#FrameBronzeGradient)";
                        break;
                    default:
                        mirrorFrameFill = "url(#FrameBlackGradient)";
                        break;
                }
            }

            return mirrorFrameFill;
        }

        #endregion
    }
}
