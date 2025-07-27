using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ShowerEnclosuresModelsLibrary.Models.CabinAllParts;

/// <summary>
/// Contains information about the Dimensions of all the Various Cabin Parts
/// </summary>
public static class CabinPartsDimensions
{
    public static class Aluminiums
    {
        /// <summary>
        /// The Total Height of the FloorAluminium
        /// </summary>
        public static readonly int FloorAluminiumHeight = 15;
        public static readonly int FloorAluminiumOutOfGlassHeight = 7;

        /// <summary>
        /// The Distance of the NV Rotating Aluminium from the Top of the Glass
        /// </summary>
        public static readonly int NVAluminiumFromTopOfGlass = 20;
        /// <summary>
        /// The Distance of the NV Rotating Aluminium from the Bottom of the Glass
        /// </summary>
        public static readonly int NVAluminiumFromBottomOfGlass = 20;

    }

    /// <summary>
    /// Various Dimensions Concerning the Handles of the Cabins
    /// </summary>
    public static class Handles
    {
        /// <summary>
        /// How Long is the B6000 Handle
        /// </summary>
        public static readonly int B6000HandleHeight = 165;
        /// <summary>
        /// How Wide is the B6000 Handle
        /// </summary>
        public static readonly int B6000HandleWidth = 20;
        /// <summary>
        /// The CornerRadius of the Upper and Bottom Part of the B6000 Handle
        /// </summary>
        public static readonly int B6000HandleCornerRadius = 10;
        /// <summary>
        /// The Diameter of the Simple ABSKnob Handle
        /// </summary>
        public static readonly int KnobABSHandleDiameter = 50;
        /// <summary>
        /// The Diameter of the Holed Inox304 Handle
        /// </summary>
        public static readonly int HoledHandleDiameter = 60;
        /// <summary>
        /// The Diameter of the Hole in Holed Inox304 Handle
        /// </summary>
        public static readonly int HoledHandleInnerDiameter = 40;
    }

    /// <summary>
    /// Various Dimensions Concerning the Hinges of the Cabins
    /// </summary>
    public static class Hinges
    {
        /// <summary>
        /// The Total Height of the Hinge
        /// </summary>
        public static readonly int WallToGlassHingeHeight = 76;
        /// <summary>
        /// The Height of the Hinge After the Wall Plate
        /// </summary>
        public static readonly int WallToGlassHingeInnerHeight = 68;
        /// <summary>
        /// The Total Width of the Hinge
        /// </summary>
        public static readonly int WallToGlassHingeWidth = 59;
        /// <summary>
        /// The Height of the Small Angular Supports that Connect the Glass with another Glass
        /// </summary>
        public static readonly int GlassToGlassSupportHeight = 45;
        /// <summary>
        /// The Length of the Small Angular Supports that Connect the Glass with another Glass
        /// </summary>
        public static readonly int GlassToGlassSupportLength = 45;

        /// <summary>
        /// The Length of the Flipper Hinge
        /// </summary>
        public static readonly int FlipperHingeLength = 70;
        /// <summary>
        /// The Height of the Flipper Hinge
        /// </summary>
        public static readonly int FlipperHingeHeight = 40;
        /// <summary>
        /// The Distance that the Hinge is Inside the Fixed Glass
        /// </summary>
        public static readonly int FlipperHingeInFixed = 20;
        /// <summary>
        /// The Distance that the Hinge is Inside the Door Glass
        /// </summary>
        public static readonly int FlipperHingeInDoor = 45;

        /// <summary>
        /// The height of the HB Glass to Glass Hinge
        /// </summary>
        public static readonly int GlassToGlassHingeHeight = 76;
        /// <summary>
        /// The Width of the HB Glass to Glass Hinge
        /// </summary>
        public static readonly int GlassToGlassHingeWidth = 110;

        /// <summary>
        /// The Height of the NP Glass to Glass Hinge
        /// </summary>
        public static readonly int GlassToGlassNPHingeHeight = 60;
        /// <summary>
        /// The Width of the NP Glass to Glass Hinge
        /// </summary>
        public static readonly int GlassToGlassNPHingeWidth = 130;
    }

    /// <summary>
    /// Various Dimensions Concerning the Polycarbonic Seals/Magnets of the Cabins
    /// </summary>
    public static class Polycarbonics
    {
        public static readonly int MagnetStripWidth = 10;
        public static readonly int MagnetStripWidth9BOnly = 8;
    }

    /// <summary>
    /// Various Dimensions Concerning the Support Bars of Cabins
    /// </summary>
    public static class SupportBars
    {
        /// <summary>
        /// The Height of the Small Clamp in a Support Bar (Looking it from the Front)
        /// </summary>
        public static readonly int SupportBarClampFrontHeight = 40;
        /// <summary>
        /// The Width of the Small Clamp in a Support Bar (Looking it from the Front)
        /// </summary>
        public static readonly int SupportBarClampFrontWidth = 30;
        /// <summary>
        /// How far from the Edge of the Glass ,should the Clamp be placed ,to grip it
        /// </summary>
        public static readonly int ClampCenterDefaultDistanceFromGlass = 80;

        /// <summary>
        /// How much Height does the Support Bar Have , that does not overlap the Glass
        /// </summary>
        public static readonly int SupportBarOutOfGlassHeight = 20;
    }

    /// <summary>
    /// Various Dimensions of Mixed Cabin Parts
    /// </summary>
    public static class VariousParts
    {
        /// <summary>
        /// The Length of the Floor Stopper used in 8W Glasses
        /// </summary>
        public static readonly int FloorStopperLength = 30;
        /// <summary>
        /// The Height of the Floor Stopper used in 8W Glasses
        /// </summary>
        public static readonly int FloorStopperHeight = 12;
        /// <summary>
        /// How long is the part that is out of the Glass
        /// </summary>
        public static readonly int FloorStopperOutOfGlassLength = 3;
    }

    /// <summary>
    /// Various Dimension of Parts and Distances for Inox304 Models
    /// </summary>
    public static class Inox304Parts
    {
        /// <summary>
        /// Diameter of the Inox 304 Wheel
        /// </summary>
        public static readonly int WheelDiameter = 45;
        
        /// <summary>
        /// Diameter of the Inox 304 Wheel Lock
        /// </summary>
        public static readonly int WheelLockDiameter = 15;
        
        /// <summary>
        /// The Distance of the Outer Circle of the Lock from the Main Bar
        /// </summary>
        public static readonly int WheelLockDistanceFromBar = 1;
        
        /// <summary>
        /// The Diameter of the Fixed Glass's Lock
        /// </summary>
        public static readonly int FixedLockDiameter = 25;
        
        /// <summary>
        /// The Height of the Inox304 Guide Looking from the Front of the Cabin
        /// </summary>
        public static readonly int GuideFrontHeight = 20;
        /// <summary>
        /// The Width of the Inox304 Guide Looking from the Front of the Cabin
        /// </summary>
        public static readonly int GuideFrontWidth = 20;
        
        /// <summary>
        /// The Inox304's Main Bar Height
        /// </summary>
        public static readonly int BarHeight = 30;
        
        /// <summary>
        /// The Height of the Inox304's Wall Support
        /// </summary>
        public static readonly int WallSupportHeight = 45;
        /// <summary>
        /// The Width of the Inox304's Wall Support
        /// </summary>
        public static readonly int WallSupportWidth = 45;
        
        /// <summary>
        /// The Height of the Door Stopper
        /// </summary>
        public static readonly int DoorStopperHeight = 55;
        /// <summary>
        /// The Width of the Door Stopper
        /// </summary>
        public static readonly int DoorStopperWidth = 20;
        /// <summary>
        /// The Min Distance of the Door Stopper from the Wall
        /// </summary>
        public static readonly int DoorStopperMinDistanceFromWall = 30;
        /// <summary>
        /// How much Height of the Door Stopper is Below the Main Bar
        /// </summary>
        public static readonly int DoorStopperExtraLengthBelowBar = 5;
        
        /// <summary>
        /// The Height of the Small Bumper Rubber on the Stopper
        /// </summary>
        public static readonly int StopperBumperRubberHeight = 5;
        /// <summary>
        /// The Height of the Small Bumper Rubber on the Stopper
        /// </summary>
        public static readonly int StopperBumperRubberLength = 5;
        /// <summary>
        /// The Distance of the Rubber Bumper from the Top of the Stopper
        /// </summary>
        public static readonly int StopperBumperDistanceFromStopperTop = 5;

        /// <summary>
        /// The Door Stoppers Bumper Width
        /// </summary>
        public static readonly int BumperWidth = 5;
        /// <summary>
        /// The Door Stoppers Bumper Height
        /// </summary>
        public static readonly int BumperHeight = 5;
        /// <summary>
        /// The Bumpers Distance from the Top of the Door Stopper
        /// </summary>
        public static readonly int BumperDistanceFromTopOfStopper = 5;

        /// <summary>
        /// The Width of the Bar's Base
        /// </summary>
        public static readonly int BarBaseWidth = 20;
        /// <summary>
        /// The Height of the Bar's Base
        /// </summary>
        public static readonly int BarBaseHeight = 40;
        /// <summary>
        /// The Length of the Base that Touches the Wall or the VF Glass
        /// </summary>
        public static readonly int BarBaseDepth = 20;
    }

    /// <summary>
    /// Various Dimension of Parts and Distances for SmartInox Models
    /// </summary>
    public static class SmartInoxParts
    {
        /// <summary>
        /// The Width of the DoorStopper Attached to the Door
        /// </summary>
        public static readonly int DoorStopperWidth = 45;
        /// <summary>
        /// The Height of the DoorStopper Attached to the Door
        /// </summary>
        public static readonly int DoorStopperHeight = 40;
        /// <summary>
        /// The Width of the Rubber part of the Door Stopper
        /// </summary>
        public static readonly int DoorStopperBumperWidth = 10;
        /// <summary>
        /// The Height of the Rubber part of the Door Stopper
        /// </summary>
        public static readonly int DoorStopperBumperHeight = 5;
        /// <summary>
        /// The Bumpers Distance from the Top of the Door Stopper
        /// </summary>
        public static readonly int BumperDistanceFromTopOfStopper = 5;

        /// <summary>
        /// The Height of the Bottom Driver Aluminium
        /// </summary>
        public static readonly int BottomDriverHeight = 8;

        /// <summary>
        /// The Diameter of the Bottom Wheels
        /// </summary>
        public static readonly int WheelDiameter = 60;

    }
}
