using SVGDrawingLibrary.Models;
using SVGDrawingLibrary.Enums;
using SVGDrawingLibrary.Helpers;
using MirrorsModelsLibrary.DrawsBuilder.Models.DrawShapes;
using MirrorsModelsLibrary.DrawsBuilder.Models.DrawShapes.MirrorSpecificDraws;
using MirrorsModelsLibrary.DrawsBuilder.Models.MeasureObjects;
using MirrorsModelsLibrary.Enums;
using MirrorsModelsLibrary.Models;
using MirrorsModelsLibrary.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SVGDrawingLibrary.Models.ConcreteShapes;

namespace MirrorsModelsLibrary.DrawsBuilder.Models
{
    public class MirrorDraw
    {
        private const double SAFEDISTANCEFOG = 15; //Minimum Distance from The Fog of any other item
        private const double SAFEDISTANCETOUCH = 15; //Minimum Distance from the Touch of any other item

        private readonly Mirror mirror;
        private readonly MirrorDrawBuilder builder;

        /// <summary>
        /// The Box containing the Draw
        /// </summary>
        public MirrorDrawContainer ContainerBox { get; set; }
        
        /// <summary>
        /// The Extras Boundary Area
        /// </summary>
        public DrawShape ExtrasBoundaryArea { get; set; }
        public DrawShape SandblastBoundaryArea { get; set; }
        public DrawShape SupportBoundaryArea { get; set; }

        public MirrorDrawSide FrontDraw { get; set; }
        public MirrorDrawSide RearDraw { get; set; }
        public MirrorDrawSide SideDraw { get; set; }

        public List<string> OutOfBoundsShapesNames { get; set; } = new();

        /// <summary>
        /// Instantiates the Mirror Draw Object
        /// </summary>
        /// <param name="mirror">The Mirror Getting Drawn</param>
        /// <param name="measures">The Measures of the Mirror Getting Drawn</param>
        public MirrorDraw(Mirror mirror , double containerMargin)
        {
            this.mirror = mirror;
            ContainerBox = new(mirror, containerMargin);
            builder = new(mirror, ContainerBox);

            InitializeDraw();

            ExtrasBoundaryArea.Stroke = "red";
            FixOutOfBoundsShapes();

            FixTouchMagnifyerScreenCollisions();
            //If mirror has both Fog Switch && Any other Touch (without any screens)Rerun the Fix Touch Magnifyer Colision
            if (mirror.HasExtra(MirrorOption.TouchSwitchFog) &&
                mirror.HasExtrasAny(MirrorOption.TouchSwitch, MirrorOption.DimmerSwitch, MirrorOption.SensorSwitch) &&
                !mirror.HasExtrasAny(MirrorOption.DisplayRadio, MirrorOption.Display19, MirrorOption.Display20, MirrorOption.Clock))
            {
                FixTouchMagnifyerScreenCollisions();
            }
            FixFogCollisions(); //Check in the End For Fog Collisions
            MarkOutOfBoundsShapes();
        }

        /// <summary>
        /// Moves the Shapes that are out Of Bounds Upper until Within Bounds
        /// If there Items are not put inBounds after 10Iterations returns Shapes as OutOfBounds
        /// </summary>
        private void FixOutOfBoundsShapes()
        {
            foreach (DrawShape shape in RearDraw.ExtrasDraws)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (MathCalc.IsShapeInsideBoundary(ExtrasBoundaryArea,shape) is false)
                    {
                        //Move Magnifyer and Sandblast Together
                        if (shape.Name == DrawShape.MAGNSAND)
                        {
                            shape.TranslateY(-10);
                            FrontDraw.ExtrasDraws.FirstOrDefault(s => s.Name == shape.Name)?.TranslateY(-10);
                            RearDraw.ExtrasDraws.FirstOrDefault(s=>s.Name == DrawShape.MAGN)?.TranslateY(-10);
                            FrontDraw.ExtrasDraws.FirstOrDefault(s => s.Name == DrawShape.MAGN)?.TranslateY(-10);
                        }
                        //Move Only Magnifyer when there is no sandblast
                        else if(shape.Name == DrawShape.MAGN && mirror.HasExtrasAny(MirrorOption.ZoomLedTouch,MirrorOption.ZoomLed) is false)
                        {
                            shape.TranslateY(-10);
                            FrontDraw.ExtrasDraws.FirstOrDefault(s => s.Name == shape.Name)?.TranslateY(-10);
                        }
                        //Move AnyOther Extra Except Fogs Upwards
                        else if (shape.Name != DrawShape.FOG16 && shape.Name != DrawShape.FOG24 && shape.Name != DrawShape.FOG55)
                        {
                            shape.TranslateY(-10);
                            FrontDraw.ExtrasDraws.FirstOrDefault(s => s.Name == shape.Name)?.TranslateY(-10);
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Initializes the Draw . Builds all Draw Sides/Dimensions and Draw VisualProperties
        /// </summary>
        private void InitializeDraw()
        {
            FrontDraw = builder.BuildFrontDraw();
            RearDraw = builder.BuildRearDraw();
            SideDraw = builder.BuildSideDraw();

            SandblastBoundaryArea = builder.BuildSandblastBoundary();
            SupportBoundaryArea = builder.BuildSupportBoundary();
            ExtrasBoundaryArea = builder.BuildExtrasBoundary();

            RemoveInconsistentDrawsRule();
        }

        /// <summary>
        /// Removes the Draws that Are Inconsistent (Touch when we Have Screens e.t.c.)
        /// </summary>
        private void RemoveInconsistentDrawsRule()
        {
            //1.When there is a Screen && TouchSwitch remove the TouchDraw
            if (mirror.HasExtrasAny(MirrorOption.Display20,MirrorOption.Display19,MirrorOption.DisplayRadio,MirrorOption.Clock) 
                && mirror.HasExtrasAny(MirrorOption.TouchSwitch,MirrorOption.DimmerSwitch,MirrorOption.SensorSwitch,MirrorOption.TouchSwitchFog))
            {
                //Find the Touch Draw and RemoveIt
                DrawShape switchDrawFront = FrontDraw.ExtrasDraws.FirstOrDefault(extraDraw => extraDraw.Name == DrawShape.TOUCH || extraDraw.Name == DrawShape.DIMMER || extraDraw.Name == DrawShape.SENSOR);
                DrawShape switchDrawRear  = RearDraw.ExtrasDraws.FirstOrDefault(extraDraw => extraDraw.Name == DrawShape.TOUCH || extraDraw.Name == DrawShape.DIMMER || extraDraw.Name == DrawShape.SENSOR);
                if (switchDrawFront != null) { FrontDraw.ExtrasDraws.Remove(switchDrawFront); }
                if (switchDrawRear != null) { RearDraw.ExtrasDraws.Remove(switchDrawRear); }
                // Find and Remove the Fog Switch (This must run after the removal of the normal touch switch because they share the same name)
                DrawShape switchFogFront = FrontDraw.ExtrasDraws.FirstOrDefault(switchFog => switchFog.Name == DrawShape.TOUCHFOG); //Has the Same Name with Touch Switch
                DrawShape switchFogRear = RearDraw.ExtrasDraws.FirstOrDefault(switchFog => switchFog.Name == DrawShape.TOUCHFOG); //Has the Same Name with Touch Switch
                if (switchFogFront != null) { FrontDraw.ExtrasDraws.Remove(switchFogFront); }
                if (switchFogRear != null) { RearDraw.ExtrasDraws.Remove(switchFogRear); }
            }
        }

        /// <summary>
        /// Checks Wheather the Anti-Fogs are Contained to the Boundary
        /// </summary>
        /// <returns>True if they are -- False if they Are Not</returns>
        private bool IsFogWithinBoundary()
        {
            bool isContained = true;
            DrawShape fog;
            if (mirror.HasExtra(MirrorOption.Fog16W))
            {
                fog = RearDraw.ExtrasDraws.FirstOrDefault(s => s.Name == DrawShape.FOG16);
            }
            else if (mirror.HasExtra(MirrorOption.Fog24W))
            {
                fog = RearDraw.ExtrasDraws.FirstOrDefault(s => s.Name == DrawShape.FOG24);
            }
            else if (mirror.HasExtra(MirrorOption.Fog55W))
            {
                fog = RearDraw.ExtrasDraws.FirstOrDefault(s => s.Name == DrawShape.FOG55);
            }
            else
            {
                fog = null;
            }

            if (fog is not null)
            {
                isContained = MathCalc.IsShapeInsideBoundary(ExtrasBoundaryArea, fog);
            }

            return isContained;
        }

        /// <summary>
        /// Changes Stroke to Red when one of the Extras Shapes is Out Of Bounds
        /// </summary>
        private void MarkOutOfBoundsShapes()
        {
            foreach (DrawShape shape in RearDraw.ExtrasDraws)
            {
                bool isContained = MathCalc.IsShapeInsideBoundary(ExtrasBoundaryArea, shape);
                if (isContained == false)
                {
                    OutOfBoundsShapesNames.Add($"{shape.Name}");
                    shape.Stroke = "red";
                }
            }
        }

        /// <summary>
        /// Returns the List of Colliding Sets of Extras
        /// </summary>
        /// <returns>Returns a Tuple List of Colliding Extras </returns>
        private List<(DrawShape,DrawShape)> FindCollidingSetsOfExtras()
        {
            //The Set of Extras Tha Collide
            List<(DrawShape, DrawShape)> collidingSets = new();

            //WITH THE BELOW METHOD EACH SET IS UNIQUE AND GIVES A COLLISION BETWEEN TWO EXTRAS ONCE

            //Search for Collisions only in the Rear Draw (Where Shapes are always Bigger
            if (RearDraw.ExtrasDraws.Count>1) 
                //When there are more than 1 extras
                //Check each extra with the rest for Collisions
                //Construct this way the List of Sets of Colliding Extras
            {
                for (int i = 0; i < RearDraw.ExtrasDraws.Count; i++)
                {
                    //Check each extra with the rest underneath it as long as there are elements underneath
                    if (RearDraw.ExtrasDraws.Count >= i+1) //At least one element underneath
                    {
                        for (int j = i + 1; j < RearDraw.ExtrasDraws.Count; j++) //start from the element underneath i+1
                        {
                            //Check for intersection
                            bool areIntersecting = MathCalc.AreIntersectingShapes(RearDraw.ExtrasDraws[i], RearDraw.ExtrasDraws[j]);
                            //If true add the Colliding set to the List
                            if (areIntersecting) { collidingSets.Add((RearDraw.ExtrasDraws[i], RearDraw.ExtrasDraws[j])); }
                        }
                    }
                }
            }
            collidingSets = collidingSets.Where(s => s.Item1 is not null && s.Item2 is not null).ToList();
            return collidingSets;
        }

        /// <summary>
        /// Gets the Colliding Sets that Contain a Fog
        /// </summary>
        /// <returns>A List of Sets that contain a Fog which Collides with another Extra</returns>
        private List<(DrawShape, DrawShape)> GetCollidingFogSets()
        {
            List<(DrawShape, DrawShape)> collidingFogSets = new();

            //Check if there is a Fog Shape Inside the Collision Sets and return it
            foreach (var collidingSet in FindCollidingSetsOfExtras())
            {
                if (collidingSet.Item1.Name == DrawShape.FOG16 ||
                    collidingSet.Item1.Name == DrawShape.FOG24 ||
                    collidingSet.Item1.Name == DrawShape.FOG55 ||
                    collidingSet.Item2.Name == DrawShape.FOG16 || 
                    collidingSet.Item2.Name == DrawShape.FOG24 || 
                    collidingSet.Item2.Name == DrawShape.FOG55)

                    collidingFogSets.Add(collidingSet);

            }
            
            return collidingFogSets;
        }

        #region 1.ScreenTouch / Magnifyer Collision Repositioning

        /// <summary>
        /// Gets the Touch/Screen Sets that Collide with a Magnifyer Mirror
        /// </summary>
        /// <returns> The List of Sets Screen-Touch/Magnifyer that Collide </returns>
        private List<(DrawShape,DrawShape)> GetCollidingMagnifyerSetWithoutFog()
        {
            List<(DrawShape, DrawShape)> collidingSets = new();
            
            //Get the Correct Magnifyer Shape to Check for Collision (Sandblast or Mirror)
            string magnifyerDrawName = "";
            if (mirror.HasExtrasAny(MirrorOption.ZoomLed, MirrorOption.ZoomLedTouch))
            {
                magnifyerDrawName = DrawShape.MAGNSAND;
                //IN THIS OCCASION WE HAVE TO REMOVE FROM THE COLLIDING SETS AFTERWARDS THE GLASSMAGGNIFYER BECAUSE IT COLLIDES WITH THE SANDBLAST
            }
            else if (mirror.HasExtra(MirrorOption.Zoom))
            {
                magnifyerDrawName = DrawShape.MAGN;
            }
            else
            {
                //There is no Magnifyer to Check For Collisions
                return null;
            }

            //Check if there is a Magnifyer Shape Inside the Collision Sets and return it the moment it is found
            //Remove Fogs from the Checking
            foreach (var collidingSet in FindCollidingSetsOfExtras())
            {
                if (
                    (collidingSet.Item1.Name.Equals(magnifyerDrawName) &&
                    collidingSet.Item2.Name !=  DrawShape.FOG16 &&
                    collidingSet.Item2.Name != DrawShape.FOG24 &&
                    collidingSet.Item2.Name != DrawShape.FOG55) 
                    ||
                    (collidingSet.Item2.Name.Equals(magnifyerDrawName) &&
                    collidingSet.Item1.Name != DrawShape.FOG16 &&
                    collidingSet.Item1.Name != DrawShape.FOG24 &&
                    collidingSet.Item1.Name != DrawShape.FOG55)
                    )

                    collidingSets.Add(collidingSet);
            }

            //If we have a Magnifyer with Sandblast -- Then we have to remove from the Coliding sets the Collision of Magnifyer Sandblast with the GlassMagnifyer
            if (magnifyerDrawName.Equals(DrawShape.MAGNSAND))
            {
                //remove from colliding sets the Magnifyer Glass that Colides with the Sandblast Glass 
                (DrawShape, DrawShape) setToRemove = collidingSets.Where(s => 
                    s.Item1.Name.Equals(DrawShape.MAGNSAND) && s.Item2.Name.Equals(DrawShape.MAGN) ||
                    s.Item2.Name.Equals(DrawShape.MAGNSAND) && s.Item1.Name.Equals(DrawShape.MAGN))
                    .FirstOrDefault();
                if (setToRemove is not (null,null)) { collidingSets.Remove(setToRemove); }
            }

            return collidingSets;
        }

        /// <summary>
        /// Repositions the Touch/Screens when Colliding with the Magnifyer
        /// </summary>
        private void FixTouchMagnifyerScreenCollisions()
        {
            List<(DrawShape, DrawShape)> collidingSets = GetCollidingMagnifyerSetWithoutFog();
            if (collidingSets is null || collidingSets.Count < 1) { return; }

            //The Set should Always be One or None if Correctly Done
            DrawShape touchOrScreen = null;
            DrawShape magnifyer = null;

            //Get the Order Correct (Which is Which)
            if (collidingSets.FirstOrDefault().Item1.Name.Equals(DrawShape.MAGN) ||
                collidingSets.FirstOrDefault().Item1.Name.Equals(DrawShape.MAGNSAND))
            {
                magnifyer = collidingSets.FirstOrDefault().Item1;
                touchOrScreen = collidingSets.FirstOrDefault().Item2;
            }
            else
            {
                magnifyer = collidingSets.FirstOrDefault().Item2;
                touchOrScreen = collidingSets.FirstOrDefault().Item1;
            }

            //SOMETHING IS WRONG HERE AND THERE ARE MANY NULL COLLIDING SETS AND ADDED THIS HACK TO AVOID MISTAKES
            if (touchOrScreen is null || magnifyer is null)
            {
                return;
            }

            //Keep Default Position Before Moving
            double defaultCenterX = touchOrScreen.ShapeCenterX;

            //Move the Screen or Touch to the Left (In Both Rear and Front Draws)
            MathCalc.RepositionCollidingShapes(touchOrScreen, magnifyer, DrawMoveDirection.Left, SAFEDISTANCETOUCH);
            Console.WriteLine("Moved Touch Left from Magnifyer");
            DrawShape touchScreenDrawInFront = FrontDraw.ExtrasDraws.FirstOrDefault(d => d.Name == touchOrScreen.Name);
            if (touchScreenDrawInFront != null)
            {
                touchScreenDrawInFront.SetCenterOrStartX(touchOrScreen.ShapeCenterX, DrawShape.CSCoordinate.Center);
            }

            //Check for containment
            bool isContained = MathCalc.IsShapeInsideBoundary(ExtrasBoundaryArea, touchOrScreen);
            if (isContained) { return; }

            //If not Contained Revert to Default (Both Rear and Front)
            touchOrScreen.SetCenterOrStartX(defaultCenterX, DrawShape.CSCoordinate.Center);
            Console.WriteLine("Moved Touch ToDefault");
            if (touchScreenDrawInFront != null)
            {
                touchScreenDrawInFront.SetCenterOrStartX(defaultCenterX, DrawShape.CSCoordinate.Center);
            }

            //move the sandblastMagnifyer OR the Mirror Magnifyer (Of Both Front and Rear Draws)
            MathCalc.RepositionCollidingShapes(magnifyer, touchOrScreen, DrawMoveDirection.Up, SAFEDISTANCETOUCH);
            Console.WriteLine("Moved Magnifyer Up from Touch");
            DrawShape magnDrawInFront = FrontDraw.ExtrasDraws.FirstOrDefault(d => d.Name == magnifyer.Name);
            if (magnDrawInFront != null)
            {
                magnDrawInFront.SetCenterOrStartY(magnifyer.ShapeCenterY, DrawShape.CSCoordinate.Center);
            }
            

            //Both Sandblast and Magnifyer must be Moved IF Mirror has Magnifyer with Light then we must move the Mirror Also because the above call moved only the Sandblast
            if (mirror.HasExtra(MirrorOption.ZoomLed) || mirror.HasExtra(MirrorOption.ZoomLedTouch))
            {
                //Get the Magnifyer Mirror to Move Also
                DrawShape magnifyerOnlyMirror = RearDraw.ExtrasDraws.FirstOrDefault(e => e.Name.Equals(DrawShape.MAGN));
                //Move it at the same CenterX as the Sandblast
                magnifyerOnlyMirror.SetCenterOrStartY(magnifyer.ShapeCenterY, DrawShape.CSCoordinate.Center);
                Console.WriteLine("Moved GlassMagnifyer to same Center as Sandblast Magnifyer");
                //Move the Same Draw in the Front
                DrawShape magnifyerOnlyMirrorFront = FrontDraw.ExtrasDraws.FirstOrDefault(d => d.Name == magnifyerOnlyMirror.Name);
                if (magnDrawInFront != null)
                {
                    magnifyerOnlyMirrorFront.SetCenterOrStartY(magnifyerOnlyMirror.ShapeCenterY, DrawShape.CSCoordinate.Center);
                }
            }
        }

        #endregion

        #region 2.Fog Collisions Repositioning

        /// <summary>
        /// Moves the Fog Draws - So that they Do not Collide with the Magnifyer
        /// </summary>
        private void FixFogCollisions()
        {
            List<(DrawShape, DrawShape)> collidingFogSets = GetCollidingFogSets();
            if (collidingFogSets is null || collidingFogSets.Count<1) { return; }

            DrawShape fog = null;

            #region 1.Fog with Magnifyer Collision Reposition

            DrawShape magnifyer = null;
            string magnifyerName = "";
            
            //if the Mirror has A magnifyer with Light check for collision only with the MagnifyerSandblast
            //otherwise check only with the MagnifyerGlass
            if (mirror.HasExtrasAny(MirrorOption.ZoomLed, MirrorOption.ZoomLedTouch))
            {
                magnifyerName = DrawShape.MAGNSAND;
            }
            else if (mirror.HasExtra(MirrorOption.Zoom))
            {
                magnifyerName = DrawShape.MAGN;
            }
            else //There is no Magnifyer
            {
                magnifyerName = string.Empty;
            }

            foreach (var set in collidingFogSets)
            {
                if (set.Item1.Name == magnifyerName)
                {
                    fog = set.Item2;
                    magnifyer = set.Item1;
                }
                else if (set.Item2.Name == magnifyerName)
                {
                    fog= set.Item1;
                    magnifyer=set.Item2;
                }
            }

            if (fog is not null && magnifyer is not null)
            {
                RemoveFogCollisionWithMagnifyer(fog,magnifyer);
            }
            #endregion


            //After Repositioning the Fog from the Magnifyer reposition the Fog with the Touch Or Screen
            //If there was a magnifyer this step will be Skipped as always there will be no Colliding sets

            #region 2.Fog with Touch Or Screen Collision Reposition
            collidingFogSets = GetCollidingFogSets(); //To Reset
            fog = null; //Reset to Null to recheck below
            DrawShape touchOrScreen = null;

            string[] touchOrScreenNames = new string[8] { DrawShape.DISPLAY11,
                                                          DrawShape.DISPLAY19,
                                                          DrawShape.DISPLAY20,
                                                          DrawShape.DIMMER,
                                                          DrawShape.TOUCH,
                                                          DrawShape.TOUCHFOG,
                                                          DrawShape.SENSOR,
                                                          DrawShape.CLOCK};

            foreach (var set in collidingFogSets)
            {
                //If the Name of the Colliding Set Item1 Matches any Touch or Screen
                if (touchOrScreenNames.Any(n=>n==set.Item1.Name))
                {
                    touchOrScreen = set.Item1;
                    fog = set.Item2;
                }//The Opposite
                else if (touchOrScreenNames.Any(n=>n==set.Item2.Name))
                {
                    fog = set.Item1;
                    touchOrScreen= set.Item2;
                }
            }

            if (fog is not null && touchOrScreen is not null)
            {
                RemoveFogCollisionWithTouchOrScreen(fog,touchOrScreen);
            }
            #endregion


        }

        /// <summary>
        /// Moves the Colliding Fog Upwards (so that the Y Distance of the Fog center with the mirror Center is >FogHeight/2 +R )
        /// </summary>
        /// <param name="fog">The Fog Shape</param>
        /// <param name="magnifyer">The MagnifyerShape</param>
        private void RemoveFogCollisionWithMagnifyer(DrawShape fog,DrawShape magnifyer)
        {
            //The Starting Deistances must be Kept so to Revert to the Starting Position Later in the Method if Needed
            double fogDefaultCenterX = fog.ShapeCenterX;
            double fogDefaultCenterY = fog.ShapeCenterY;

            //Reposition the Fog Upwards and Check again for Containment
            MathCalc.RepositionCollidingShapes(fog, magnifyer,DrawMoveDirection.Up, SAFEDISTANCEFOG);
            Console.WriteLine("Moved Fog Up From Magnifyer");

            //If not Contained -- Move back to Default Position and Move it again to the Left
            //If Contained return
            bool isFogWithinBoundary = MathCalc.IsShapeInsideBoundary(ExtrasBoundaryArea, fog);
            if (isFogWithinBoundary) { return; }
            
            //Revert to Original Position
            fog.SetCenterOrStartY(fogDefaultCenterY, DrawShape.CSCoordinate.Center);
            Console.WriteLine("Moved Fog ToDefault");

            MathCalc.RepositionCollidingShapes(fog,magnifyer,DrawMoveDirection.Left, SAFEDISTANCEFOG);
            Console.WriteLine("Moved Fog Left from Magnifyer");

            //Check for Containment
            isFogWithinBoundary = MathCalc.IsShapeInsideBoundary(ExtrasBoundaryArea, fog);
            if(isFogWithinBoundary) { return; }

            //iF NOT WITHIN Boundary again Reposition again to Default X 
            fog.SetCenterOrStartX(fogDefaultCenterX, DrawShape.CSCoordinate.Center);
            Console.WriteLine("Moved Fog ToDefault Again");
            //Then Move Again Up to Stay There to Visualize to User as Out Of Bounds
            //Reposition the Fog Upwards Again
            MathCalc.RepositionCollidingShapes(fog, magnifyer, DrawMoveDirection.Up, SAFEDISTANCEFOG);
            Console.WriteLine("Moved Fog Up from Magnifyer Again");
        }

        /// <summary>
        /// Moves the CollidingFog Upwards From the Screen or Touch Extra
        /// </summary>
        /// <param name="fog">The Fog Shape</param>
        /// <param name="touchScreen">The Screen or Touch Extra</param>
        private void RemoveFogCollisionWithTouchOrScreen(DrawShape fog,DrawShape touchScreen)
        {
            //Reposition the Fog Upwards
            MathCalc.RepositionCollidingShapes(fog, touchScreen, DrawMoveDirection.Up, SAFEDISTANCEFOG);
            Console.WriteLine("Moved Fog Up from Touch-Screen");
        }

        #endregion

    }
}
