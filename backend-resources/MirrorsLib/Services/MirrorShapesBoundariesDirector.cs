using CommonHelpers.Comparers;
using CommonHelpers.Exceptions;
using MirrorsLib.Enums;
using MirrorsLib.MirrorElements;
using MirrorsLib.MirrorElements.MirrorModules;
using MirrorsLib.MirrorElements.Sandblasts;
using MirrorsLib.MirrorElements.Supports;
using ShapesLibrary;
using ShapesLibrary.Interfaces;
using ShapesLibrary.Services;
using ShapesLibrary.ShapeInfoModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ShapesLibrary.Services.MathCalculations;

namespace MirrorsLib.Services
{
    /// <summary>
    /// Builds Boundaries and Shapes of the Mirror its Sandblasts and its Supports
    /// </summary>
    public class MirrorShapesBoundariesDirector
    {
        private MirrorSynthesis? _mirror;
        private MirrorSynthesis Mirror { get => _mirror ?? throw new Exception($"{nameof(Mirror)} has not been set"); }

        public void SetMirror(MirrorSynthesis mirror)
        {
            _mirror = mirror;
        }

        /// <summary>
        /// Generates all the Boundaries and Shapes for the Mirror's Sandblast and Supports
        /// </summary>
        /// <returns></returns>
        public MirrorSynthesis CreateMirrorShapesAndBoundaries()
        {
            ConfigureMirrorBoundaryOptions();
            FormulateShapesBoundariesSandblastsSupports();
            return Mirror;
        }

        /// <summary>
        /// Sets which Boundaries are to be Used for the Supports/Sandblasts/Modules
        /// </summary>
        private void ConfigureMirrorBoundaryOptions()
        {
            bool hasVisibleFrame = Mirror.HasVisibleFrame();
            bool hasSandblast = Mirror.Sandblast != null;
            bool hasBackSupport = Mirror.Support != null && !hasVisibleFrame;

            //Reset all boundaries to the Mirror Glass . If nothing is set otherwise it will use the Glass as its boundary
            Mirror.ModulesBoundary = MirrorBoundaryOption.BoundaryFormingFromMirrorGlass;
            Mirror.SupportBoundary = MirrorBoundaryOption.BoundaryFormingFromMirrorGlass;
            Mirror.SandblastBoundary = MirrorBoundaryOption.BoundaryFormingFromMirrorGlass;

            if (hasVisibleFrame)
            {
                //Mirror.SupportBoundary = MirrorBoundaryOption.BoundaryFormingFromMirrorGlass;
                Mirror.SandblastBoundary = MirrorBoundaryOption.BoundaryFormingFromSupport;
                Mirror.ModulesBoundary = MirrorBoundaryOption.BoundaryFormingFromSupport;
            }

            if (hasSandblast && hasVisibleFrame)
            {
                //Replace Visible Frames Boundary
                if (Mirror.Sandblast!.SandblastInfo.IsModulesBoundary) Mirror.ModulesBoundary = MirrorBoundaryOption.BoundaryFormingFromSandblast;
            }
            else if (hasSandblast && hasBackSupport)
            {
                //determine which is placed first

                //If the Sandblast is Boundary for the Supports and/or Modules
                //If the Supports are also boundary for Modules replace that of the Sandblast as the supports are placed last
                if (Mirror.Sandblast!.SandblastInfo.IsSupportsBoundary)
                {
                    Mirror.SupportBoundary = MirrorBoundaryOption.BoundaryFormingFromSandblast;
                    if (Mirror.Sandblast.SandblastInfo.IsModulesBoundary) Mirror.ModulesBoundary = MirrorBoundaryOption.BoundaryFormingFromSandblast;
                    if (Mirror.Support!.SupportInfo.IsModulesBoundary) Mirror.ModulesBoundary = MirrorBoundaryOption.BoundaryFormingFromSupport;
                }
                //Else Sandblast is not a boundary of supports .
                //If the Supports are boundary for the Modules its also for the Sandblast
                //If the sandblast is Modules Boundary replace the one forming from the Supports as the Sandblast is placed Last
                else if (Mirror.Support!.SupportInfo.IsModulesBoundary)
                {
                    Mirror.SandblastBoundary = MirrorBoundaryOption.BoundaryFormingFromSupport;
                    Mirror.ModulesBoundary = MirrorBoundaryOption.BoundaryFormingFromSupport;
                    if (Mirror.Sandblast.SandblastInfo.IsModulesBoundary) Mirror.ModulesBoundary = MirrorBoundaryOption.BoundaryFormingFromSandblast;
                }
            }
            //Only back Support without Sandblast
            else if (hasBackSupport)
            {
                if (Mirror.Support!.SupportInfo.IsModulesBoundary) Mirror.ModulesBoundary = MirrorBoundaryOption.BoundaryFormingFromSupport;
            }
            //Only Sandblast (Not Possible)
            else if (hasSandblast)
            {
                if (Mirror.Sandblast!.SandblastInfo.IsModulesBoundary) Mirror.ModulesBoundary = MirrorBoundaryOption.BoundaryFormingFromSandblast;
            }
        }
        /// <summary>
        /// Formulates the Shapes and Boundary Shapes of Sandblasts , Supports , Mirror
        /// </summary>
        private void FormulateShapesBoundariesSandblastsSupports()
        {
            //The Main Shape and Boundary
            Mirror.MirrorGlassShape = Mirror.GetGlassShape();

            //1st step Create Supports First if they are not being set by the Sandblast boundary
            if (Mirror.SupportBoundary != MirrorBoundaryOption.BoundaryFormingFromSandblast)
            {
                if (Mirror.Support != null) CreateSupportShapesAndBoundaries();
                if (Mirror.Sandblast != null) CreateSandblastShapeAndBoundaries();
            }
            //otherwise reverse
            else if (Mirror.SupportBoundary == MirrorBoundaryOption.BoundaryFormingFromSandblast)
            {
                if (Mirror.Sandblast != null) CreateSandblastShapeAndBoundaries();
                if (Mirror.Support != null) CreateSupportShapesAndBoundaries();
            }
        }
        
        #region 1. Sandblasts Shapes and Boundaries Formulation
        private void CreateSandblastShapeAndBoundaries()
        {
            //The Boundary options have been found So Now calculate accordingly
            if (Mirror.Sandblast == null) throw new Exception("Sandblast is Null");

            var boundary = Mirror.MirrorGlassShape;
            double minDistanceFromSupport = Mirror.Sandblast!.MinDistanceFromSupport;

            if (Mirror.SandblastBoundary == MirrorBoundaryOption.BoundaryFormingFromSupport)
            {
                boundary = Mirror.Support!.FormedBoundary ?? throw new Exception("Support Formed Boundary is not Set");
                minDistanceFromSupport = Math.Max(minDistanceFromSupport, Mirror.Support.MinDistanceFromSandblast);
            }


            switch (Mirror.Sandblast.SandblastInfo)
            {
                case HoledRectangleSandblast h8:
                    CreateHoledRectSandblastShapeAndBoundary(h8, boundary, minDistanceFromSupport);
                    break;
                case LineSandblast line:
                    CreateLineSandblastShapeAndBoundary(line, boundary, minDistanceFromSupport);
                    break;
                case TwoLineSandblast twoLine:
                    CreateTwoLineSandblastShapeAndBoundary(twoLine, boundary, minDistanceFromSupport);
                    break;
                case CircularSandblast cirlce:
                    //if the boundary is not Cricle info just use as boundary the Mirror (Circular Sandblast is only meant for Circles)
                    var boundaryAsCircle = boundary as CircleInfo ?? new CircleInfo(Mirror.MirrorGlassShape.GetTotalLength(), Mirror.MirrorGlassShape.LocationX, Mirror.MirrorGlassShape.LocationY);
                    CreateCircleSandblastShapeAndBoundary(cirlce, boundaryAsCircle, minDistanceFromSupport);
                    break;
                default:
                    throw new NotSupportedException($"{Mirror.Sandblast.SandblastInfo.GetType().Name} is not a Supported type for Shapes Creation");
            }
        }
        private void CreateCircleSandblastShapeAndBoundary(CircularSandblast circleSandblast, CircleInfo boundary, double minDistanceFromSupport)
        {
            var sandblastShape = circleSandblast.GetShapeInfo(boundary);

            if (Mirror.SandblastBoundary == MirrorBoundaryOption.BoundaryFormingFromSupport)
            {
                //check weather the distance from the boundary is not the desired and apply correction to size accordingly
                var diffR = boundary.Radius - sandblastShape.Radius;
                if (diffR < minDistanceFromSupport)
                {
                    sandblastShape.Radius -= minDistanceFromSupport - diffR;
                }
            }
            Mirror.Sandblast!.SandblastShape = sandblastShape;
            Mirror.Sandblast.FormedBoundary = sandblastShape.GetInnerRingWholeShape();
        }
        private void CreateTwoLineSandblastShapeAndBoundary(TwoLineSandblast twoLine, ShapeInfo boundary, double minDistanceFromSupport)
        {
            //Get the shape always from the Mirror's Glass !!
            var twoLineShape = twoLine.GetShapeInfo(Mirror.MirrorGlassShape);
            var boundaryBoundingBox = boundary.GetBoundingBox();
            if (Mirror.SandblastBoundary == MirrorBoundaryOption.BoundaryFormingFromSupport)
            {
                foreach (var lineShape in twoLineShape.Shapes)
                {
                    ConstraintLineSandblastOnBoundary(lineShape, boundaryBoundingBox, minDistanceFromSupport, twoLine.IsVertical);
                }
            }
            else if (Mirror.SandblastBoundary == MirrorBoundaryOption.BoundaryFormingFromMirrorGlass && boundary is RectangleInfo rectMirror)
            {
                foreach (var lineShape in twoLineShape.Shapes)
                {
                    double leftDistance = lineShape.LeftX - rectMirror.LeftX;
                    double rightDistance = rectMirror.RightX - lineShape.RightX;
                    double topDistance = lineShape.TopY - rectMirror.TopY;
                    double bottomDistance = rectMirror.BottomY - lineShape.BottomY;
                    double epsilon = DoubleSafeEqualityComparer.DefaultEpsilon;
                    //Apply The Mirrors Corner Radius to all Corners where the Corner Radius is Smaller . WHEN the Sandblast is on the Edges of the Mirror
                    if (leftDistance <= epsilon && topDistance <= epsilon && twoLine.CornerRadius < rectMirror.TopLeftRadius)
                    {
                        lineShape.TopLeftRadius = twoLine.Thickness >= rectMirror.TopLeftRadius ? rectMirror.TopLeftRadius : twoLine.Thickness;
                    }
                    if (leftDistance <= epsilon && bottomDistance <= epsilon && twoLine.CornerRadius < rectMirror.BottomLeftRadius)
                    {
                        lineShape.BottomLeftRadius = twoLine.Thickness >= rectMirror.BottomLeftRadius ? rectMirror.BottomLeftRadius : twoLine.Thickness;
                    }
                    if (rightDistance <= epsilon && topDistance <= epsilon && twoLine.CornerRadius < rectMirror.TopRightRadius)
                    {
                        lineShape.TopRightRadius = twoLine.Thickness >= rectMirror.TopRightRadius ? rectMirror.TopRightRadius : twoLine.Thickness;
                    }
                    if (rightDistance <= epsilon && bottomDistance <= epsilon && twoLine.CornerRadius < rectMirror.BottomRightRadius)
                    {
                        lineShape.BottomRightRadius = twoLine.Thickness >= rectMirror.BottomRightRadius ? rectMirror.BottomRightRadius : twoLine.Thickness;
                    }
                }
            }
            Mirror.Sandblast!.SandblastShape = twoLineShape;

            if (twoLine.IsVertical)
            {
                var leftLine = twoLineShape.Shapes.MinBy(l => l.RightX) ?? throw new Exception($"Unexpected empty Shapes on {nameof(twoLineShape)}");
                var rightLine = twoLineShape.Shapes.MaxBy(l => l.RightX) ?? throw new Exception($"Unexpected empty Shapes on {nameof(twoLineShape)}");
                Mirror.Sandblast.FormedBoundary = new RectangleInfo(leftLine.RightX, boundaryBoundingBox.TopY, rightLine.LeftX, boundaryBoundingBox.BottomY);
            }
            else
            {
                var topLine = twoLineShape.Shapes.MinBy(l => l.TopY) ?? throw new Exception($"Unexpected empty Shapes on {nameof(twoLineShape)}");
                var bottomLine = twoLineShape.Shapes.MaxBy(l => l.BottomY) ?? throw new Exception($"Unexpected empty Shapes on {nameof(twoLineShape)}");
                Mirror.Sandblast.FormedBoundary = new RectangleInfo(boundaryBoundingBox.LeftX, topLine.BottomY, boundaryBoundingBox.RightX, bottomLine.TopY);
            }

        }
        private void CreateLineSandblastShapeAndBoundary(LineSandblast line, ShapeInfo boundary, double minDistanceFromSupport)
        {
            //Get the Line Shape from the Mirror Glass always
            var lineShape = line.GetShapeInfo(Mirror.MirrorGlassShape);
            var boundaryBoundingBox = boundary.GetBoundingBox();

            //Check if the created Sandblasts falls within the boundary at the desired distance , otherwise shrink it to fit it
            if (Mirror.SandblastBoundary == MirrorBoundaryOption.BoundaryFormingFromSupport)
            {
                ConstraintLineSandblastOnBoundary(lineShape, boundaryBoundingBox, minDistanceFromSupport, line.IsVertical);
            }
            else if (Mirror.SandblastBoundary == MirrorBoundaryOption.BoundaryFormingFromMirrorGlass
                && boundary is RectangleInfo rectMirror)
            {
                double leftDistance = lineShape.LeftX - rectMirror.LeftX;
                double rightDistance = rectMirror.RightX - lineShape.RightX;
                double topDistance = lineShape.TopY - rectMirror.TopY;
                double bottomDistance = rectMirror.BottomY - lineShape.BottomY;
                double epsilon = DoubleSafeEqualityComparer.DefaultEpsilon;
                //Apply The Mirrors Corner Radius to all Corners where the Corner Radius is Smaller . WHEN the Sandblast is on the Edges of the Mirror
                if (leftDistance <= epsilon && topDistance <= epsilon && line.CornerRadius < rectMirror.TopLeftRadius)
                {
                    lineShape.TopLeftRadius = line.Thickness >= rectMirror.TopLeftRadius ? rectMirror.TopLeftRadius : line.Thickness;
                }
                if (leftDistance <= epsilon && bottomDistance <= epsilon && line.CornerRadius < rectMirror.BottomLeftRadius)
                {
                    lineShape.BottomLeftRadius = line.Thickness >= rectMirror.BottomLeftRadius ? rectMirror.BottomLeftRadius : line.Thickness;
                }
                if (rightDistance <= epsilon && topDistance <= epsilon && line.CornerRadius < rectMirror.TopRightRadius)
                {
                    lineShape.TopRightRadius = line.Thickness >= rectMirror.TopRightRadius ? rectMirror.TopRightRadius : line.Thickness;
                }
                if (rightDistance <= epsilon && bottomDistance <= epsilon && line.CornerRadius < rectMirror.BottomRightRadius)
                {
                    lineShape.BottomRightRadius = line.Thickness >= rectMirror.BottomRightRadius ? rectMirror.BottomRightRadius : line.Thickness;
                }
            }

            //Set the Shape
            Mirror.Sandblast!.SandblastShape = lineShape;

            //To Determine the Boundary FORMING from the Sandblast we check the center of the sandblast with the Boundary to which it is constrained to
            //And pick the left/right , top/bottom edges accordingly

            Mirror.Sandblast!.FormedBoundary = GetLineSandblastFormedBoundary(lineShape, boundaryBoundingBox, line.IsVertical);
        }
        private void CreateHoledRectSandblastShapeAndBoundary(HoledRectangleSandblast h8, ShapeInfo boundary, double minDistanceFromSupport)
        {
            //Create the Sandblast by the Glass' shape always
            var h8Shape = h8.GetShapeInfo(Mirror.MirrorGlassShape);
            var boundaryBoundingBox = boundary.GetBoundingBox();

            //Check if the created Sandblasts falls within the boundary at the desired distance , otherwise shrink it to fit it
            if (Mirror.SandblastBoundary == MirrorBoundaryOption.BoundaryFormingFromSupport)
            {
                //Calculate the Horizontal and Vertical Distances of the edges . 
                //If the values are negative it means the Sandblast is greater than the boundary on the designated Axis
                //Negative or positive the distance is added or subtracted from the min distance from support (correct)

                //Calculate the Distance from the Edge Horizontally and Vertically
                var diffH = h8Shape.LeftX - boundaryBoundingBox.LeftX;
                var diffV = boundaryBoundingBox.BottomY - h8Shape.BottomY;
                if (diffH < minDistanceFromSupport)
                {
                    //decrease the Length of the h8 Shape to match the cosntraint of Minimum Distance
                    double decreaseLengthBy = minDistanceFromSupport - diffH;
                    h8Shape.Length -= decreaseLengthBy * 2; //both sides so *2
                }
                if (diffV < minDistanceFromSupport)
                {
                    //decrease the Height of the h8 Shape to match the cosntraint of Minimum Distance
                    double decreaseHeightBy = minDistanceFromSupport - diffV;
                    h8Shape.Height -= decreaseHeightBy * 2; //both sides so *2
                }
            }
            if (Mirror.SandblastBoundary == MirrorBoundaryOption.BoundaryFormingFromMirrorGlass)
            {

                //Follow the corner Radius of the Parent Rectangle if there is one , Otherwise for other shapes does not follow
                if (h8.FollowsRectangleGlassCornerRadius && boundary is RectangleInfo rectMirror)
                {
                    //Make the Sandblast to follow the Corner Radius of the Parent
                    h8Shape.TopLeftRadius = RectangleRingInfo.CalculateInnerRadius(rectMirror.TopLeftRadius, h8.EdgeDistance);
                    h8Shape.TopRightRadius = RectangleRingInfo.CalculateInnerRadius(rectMirror.TopRightRadius, h8.EdgeDistance);
                    h8Shape.BottomLeftRadius = RectangleRingInfo.CalculateInnerRadius(rectMirror.BottomLeftRadius, h8.EdgeDistance);
                    h8Shape.BottomRightRadius = RectangleRingInfo.CalculateInnerRadius(rectMirror.BottomRightRadius, h8.EdgeDistance);
                }
            }
            Mirror.Sandblast!.SandblastShape = h8Shape;
            Mirror.Sandblast.FormedBoundary = h8Shape.GetInnerRingWholeShape();
        } 

        /// <summary>
        /// Constraints a Rectangle Line within a Rectangle Boundary 
        /// </summary>
        /// <param name="lineShape">The Line Rectangle</param>
        /// <param name="boundaryBoundingBox">The Boundary</param>
        /// <param name="minDistanceFromBoundary">The minimum Distance the Line must have from the Boundary</param>
        /// <param name="isLineVertical">Weather the Line is Vertical</param>
        /// <returns></returns>
        private static RectangleInfo ConstraintLineSandblastOnBoundary(RectangleInfo lineShape, RectangleInfo boundaryBoundingBox, double minDistanceFromBoundary, bool isLineVertical)
        {
            //If the line is horizontal , Shrink Length to Fit and Move on Y to fit
            //If the line is vertical , Shrink Height to Fit and Move Y to fit

            //A line will solve intersections with its thickness side by moving only in one direction always .
            //As the thickness side is always very small comparing to the parent
            //so solving the intersection by moving wont trigger a new intersection on the other side

            double diffLeft = lineShape.LeftX - boundaryBoundingBox.LeftX;
            double diffRight = boundaryBoundingBox.RightX - lineShape.RightX;
            double diffTop = lineShape.TopY - boundaryBoundingBox.TopY;
            double diffBottom = boundaryBoundingBox.BottomY - lineShape.BottomY;

            //vertical
            if (isLineVertical)
            {
                //only one of the two will be true , or none
                if (diffLeft < minDistanceFromBoundary) lineShape.TranslateX(minDistanceFromBoundary - diffLeft);
                else if (diffRight < minDistanceFromBoundary) lineShape.TranslateX(-(minDistanceFromBoundary - diffRight));

                //Both can be true or none
                if (diffTop < minDistanceFromBoundary)
                {
                    lineShape.Height -= minDistanceFromBoundary - diffTop;
                    //Translate shape so the other end remains where it was from before shrinking (half the shrinkage moving to negative Ys)
                    lineShape.TranslateY(-(minDistanceFromBoundary - diffTop) / 2d);
                }
                if (diffBottom < minDistanceFromBoundary)
                {
                    lineShape.Height -= (minDistanceFromBoundary - diffBottom);
                    //Translate shape so the other end remains where it was from before shrinking (half the shrinkage moving to positive Ys)
                    lineShape.TranslateY((minDistanceFromBoundary - diffBottom) / 2d);
                }
            }
            //horizontal
            else
            {
                //Only one can be true or none
                if (diffTop < minDistanceFromBoundary) lineShape.TranslateY(minDistanceFromBoundary - diffTop); //move to positive Ys
                else if (diffBottom < minDistanceFromBoundary) lineShape.TranslateY(-(minDistanceFromBoundary - diffBottom)); //move to negative Ys

                //Both can be true or none
                if (diffLeft < minDistanceFromBoundary)
                {
                    lineShape.Length -= minDistanceFromBoundary - diffLeft;
                    //Translate shape so the other end remains where it was from before shrinking (half the shrinkage moving to negative Xs)
                    lineShape.TranslateX(-(minDistanceFromBoundary - diffLeft) / 2d);
                }
                if (diffRight < minDistanceFromBoundary)
                {
                    lineShape.Length -= minDistanceFromBoundary - diffRight;
                    //Translate shape so the other end remains where it was from before shrinking (half the shrinkage moving to positive Xs)
                    lineShape.TranslateX((minDistanceFromBoundary - diffRight) / 2d);
                }
            }
            return lineShape;
        }
        /// <summary>
        /// Returns the Boundary formed by a Rectangle Line when combined with a rectangle Boundary
        /// </summary>
        /// <param name="lineShape">The Line Rectangle</param>
        /// <param name="boundaryBoundingBox">The Rectangle Boundary</param>
        /// <param name="isLineVertical">Weather the line is Vertical</param>
        /// <returns></returns>
        private static RectangleInfo GetLineSandblastFormedBoundary(RectangleInfo lineShape, RectangleInfo boundaryBoundingBox, bool isLineVertical)
        {
            if (isLineVertical)
            {
                if (lineShape.LocationX < boundaryBoundingBox.LocationX) //Line is to the left 
                {
                    //form a rectangle starting from the rightX of line and finishing to the boundary's bounding box 
                    return new RectangleInfo(lineShape.RightX, boundaryBoundingBox.TopY, boundaryBoundingBox.RightX, boundaryBoundingBox.BottomY);
                }
                else
                {
                    //the opposite from above , now line is on the right side of boundary's bounding box
                    return new RectangleInfo(boundaryBoundingBox.LeftX, boundaryBoundingBox.TopY, lineShape.LeftX, boundaryBoundingBox.BottomY);
                }
            }
            else
            {
                if (lineShape.LocationY < boundaryBoundingBox.LocationY) //Line is to the Top
                {
                    return new RectangleInfo(boundaryBoundingBox.LeftX, lineShape.BottomY, boundaryBoundingBox.RightX, boundaryBoundingBox.BottomY);
                }
                else //line is to the bottom
                {
                    return new RectangleInfo(boundaryBoundingBox.LeftX, boundaryBoundingBox.TopY, boundaryBoundingBox.RightX, lineShape.TopY);
                }
            }
        }
        #endregion

        #region 2. Supports Shapes and Boundaries Formulation
        private void CreateSupportShapesAndBoundaries()
        {
            //The Boundary options have been found So Now calculate accordingly
            if (Mirror.Support == null) throw new Exception("Support is Null");

            switch (Mirror.Support.SupportInfo)
            {
                case MirrorVisibleFrameSupport visibleFrame:
                    CreateVisibleFrameShapeAndBoundary(visibleFrame);
                    break;
                case MirrorMultiSupports multiSupports:
                    CreateMultiSupportsShapeAndBoundary(multiSupports);
                    break;
                case MirrorBackFrameSupport backFrameSupport:
                    CreateBackFrameSupportShapeAndBoundary(backFrameSupport);
                    break;
                default:
                    throw new NotSupportedException($"{Mirror.Support.SupportInfo.GetType().Name} is not a Supported type for Shapes Creation");
            }
        }
        private void CreateBackFrameSupportShapeAndBoundary(MirrorBackFrameSupport backFrameSupport)
        {
            //Create the Sandblast by the Glass' shape always
            var frameShape = backFrameSupport.GetFrameRectangleRingShape(Mirror.MirrorGlassShape);

            //Check if the created Support falls within the boundary at the desired distance , otherwise shrink it to fit it
            if (Mirror.SupportBoundary == MirrorBoundaryOption.BoundaryFormingFromSandblast)
            {
                //Calculate the Horizontal and Vertical Distances of the edges . 
                //If the values are negative it means the Support is greater than the boundary on the designated Axis
                //Negative or positive the distance is added or subtracted from the min distance from Sandblast (correct)
                var boundaryBox = Mirror.Sandblast?.FormedBoundary?.GetBoundingBox() ?? throw new Exception("Uncreated Sandblast or Sandblast Formed Boundary");
                double minDistanceFromSandblast = Math.Max(Mirror.Sandblast!.MinDistanceFromSupport, Mirror.Support!.MinDistanceFromSandblast);


                double diffLeft = frameShape.LeftX - boundaryBox.LeftX;
                double diffRight = boundaryBox.RightX - frameShape.RightX;
                double diffTop = frameShape.TopY - boundaryBox.TopY;
                double diffBottom = boundaryBox.BottomY - frameShape.BottomY;

                if (diffLeft < minDistanceFromSandblast)
                {
                    double shrinkage = minDistanceFromSandblast - diffLeft;
                    //Shrink left side and translate to bring at the same place
                    frameShape.Length -= shrinkage;
                    //Move to the Left to bring at the same place as before
                    frameShape.TranslateX(-shrinkage / 2d);
                }
                if (diffRight < minDistanceFromSandblast)
                {
                    double shrinkage = minDistanceFromSandblast - diffRight;
                    //Shrink right side and translate to bring at the same place
                    frameShape.Length -= shrinkage;
                    //Move to the Right to bring at the same place as before
                    frameShape.TranslateX(shrinkage / 2d);
                }
                if (diffTop < minDistanceFromSandblast)
                {
                    double shrinkage = minDistanceFromSandblast - diffTop;
                    //Shrink Top side and translate to bring at the same place
                    frameShape.Height -= shrinkage;
                    //Move to the Top to bring at the same place as before
                    frameShape.TranslateY(-shrinkage / 2d);
                }
                if (diffBottom < minDistanceFromSandblast)
                {
                    double shrinkage = minDistanceFromSandblast - diffBottom;
                    //Shrink Bottom side and translate to bring at the same place
                    frameShape.Height -= shrinkage;
                    //Move to the Bottom to bring at the same place as before
                    frameShape.TranslateY(shrinkage / 2d);
                }
            }

            var mirrorGlassSideView = Mirror.GetMirrorGlassSideView();
            Mirror.Support!.SupportRearShape = new CompositeShapeInfo([frameShape, .. backFrameSupport.GetDiagonalConnectionLinesShapes(frameShape)]);
            Mirror.Support!.SupportSideShape = new RectangleInfo(backFrameSupport.Depth, frameShape.Height, 0, mirrorGlassSideView.LeftX - backFrameSupport.Depth / 2d, mirrorGlassSideView.LocationY);
            Mirror.Support.SupportFrontShape = null;
            Mirror.Support.FormedBoundary = frameShape.GetInnerRingWholeShape();
        }
        private void CreateVisibleFrameShapeAndBoundary(MirrorVisibleFrameSupport visibleFrame)
        {
            List<ShapeInfo> rearShapesOfFrame = [];
            if (Mirror.DimensionsInformation is IRingableShape ringable)
            {
                var frameMainBackShape = ringable.GetRingShape(visibleFrame.RearThickness1);
                rearShapesOfFrame.Add((ShapeInfo)frameMainBackShape);
                Mirror.Support!.FormedBoundary = (ShapeInfo)frameMainBackShape;
                if (visibleFrame.RearThickness2 != 0)
                {
                    var extraBackShape = frameMainBackShape.GetInnerRingWholeShape().GetRingShape(visibleFrame.RearThickness2);
                    rearShapesOfFrame.Add((ShapeInfo)extraBackShape);
                    Mirror.Support.FormedBoundary = (ShapeInfo)extraBackShape; //replace the main shape with the secondary if there is a secondary shape
                }

                var mirrorGlassSideView = Mirror.GetMirrorGlassSideView();
                Mirror.Support.SupportRearShape = new CompositeShapeInfo(rearShapesOfFrame);
                Mirror.Support.SupportFrontShape = (ShapeInfo)ringable.GetRingShape(visibleFrame.FrontThickness);
                Mirror.Support.SupportSideShape = new RectangleInfo(visibleFrame.Depth, frameMainBackShape.GetTotalHeight(), 0, mirrorGlassSideView.LocationX, mirrorGlassSideView.LocationY);
            }
            else throw new NotSupportedException($"{Mirror.DimensionsInformation.GetType().Name} is not a {typeof(IRingableShape).Name} thus does not support Visible Frame Creation");
        }
        private void CreateMultiSupportsShapeAndBoundary(MirrorMultiSupports multiSupports)
        {
            Mirror.Support!.SupportFrontShape = null;
            //Find the Boundary for which to create the Supports
            ShapeInfo boundary = Mirror.MirrorGlassShape;
            if (Mirror.SupportBoundary == MirrorBoundaryOption.BoundaryFormingFromSandblast)
            {
                boundary = Mirror.Sandblast!.FormedBoundary ?? throw new Exception("Sandblast Formed Boundary is not Set");
                //check weather there should be a greater distance between the sandblast and the Support:
                var additionalDistance = Math.Max(Mirror.Sandblast.MinDistanceFromSupport, Mirror.Support.MinDistanceFromSandblast);
                if (additionalDistance > 0)
                {
                    boundary = boundary.GetReducedPerimeterClone(additionalDistance, true);
                }
            }
            var bBoxBoundary = boundary.GetBoundingBox();
            var topSupportsShape = multiSupports.TopSupportsInstructions.GetSupportShapes(bBoxBoundary);
            var bottomSupportsShape = multiSupports.BottomSupportsInstructions.GetSupportShapes(bBoxBoundary);

            Mirror.Support.SupportRearShape = new CompositeShapeInfo<RectangleInfo>([.. topSupportsShape, .. bottomSupportsShape]);

            //To the formed boundary from the Multi Supports will always be the initial Boundary , constrained by the TopY and BottomY of the Supports
            //The Xs are constrained to that of the initial boundary
            var formedBoundary = boundary.GetDeepClone();
            var formedBoundaryBoundyBox = formedBoundary.GetBoundingBox();
            //find the new Height which is the diff of the in between Ys (Top Bottom) of the top and bottom Supports
            var topY = formedBoundaryBoundyBox.TopY;
            var bottomY = formedBoundaryBoundyBox.BottomY;
            if (topSupportsShape.Count >= 1) topY = topSupportsShape[0].BottomY;
            if (bottomSupportsShape.Count >= 1) bottomY = bottomSupportsShape[0].TopY;

            var diff = bottomY - topY;
            //Change the Height of the Formed Boundary to the new Height (The x location will be the same)
            formedBoundary.SetTotalHeight(diff);
            formedBoundary.LocationY = topY + diff / 2d; //center of new rectangle
            Mirror.Support.FormedBoundary = formedBoundary;
            Mirror.Support.SupportFrontShape = null;

            var mirrorGlassSideView = Mirror.GetMirrorGlassSideView();
            List<RectangleInfo> sideShapes = [];
            if (topSupportsShape.Count > 0) sideShapes.Add(new RectangleInfo(multiSupports.TopSupportsInstructions.Depth, multiSupports.TopSupportsInstructions.Thickness, 0, mirrorGlassSideView.LeftX - multiSupports.TopSupportsInstructions.Depth / 2d, topSupportsShape[0].LocationY));
            if (bottomSupportsShape.Count > 0) sideShapes.Add(new RectangleInfo(multiSupports.BottomSupportsInstructions.Depth, multiSupports.BottomSupportsInstructions.Thickness, 0, mirrorGlassSideView.LeftX - multiSupports.BottomSupportsInstructions.Depth / 2d, bottomSupportsShape[0].LocationY));
            Mirror.Support.SupportSideShape = sideShapes.Count > 0 ? new CompositeShapeInfo<RectangleInfo>(sideShapes) : null;
        } 
        #endregion
    }

}