using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.SupportsModels;
using ShowerEnclosuresModelsLibrary.Models.ModelsInterfaces;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.Inox304Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.WSSmartModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Builder.GlassBuilderHelperMethods
{
    public static class GlassBuilderHelpers
    {
#nullable disable
        #region 1.DoorGlassLength CalcHelpers 94-9A-9S-V4-VS-VA-WS

        public static double DoorGlassLength94Calculation(Cabin94 cabin)
        {
            //Constraint for StepLength = 0 When both Step Height/Length are not Set
            int stepLength = 0;
            if (cabin.HasStep)
            {
                stepLength = cabin.GetStepCut().StepLength;
            }

            //When the Profile is a Connector the Calculation of Thickness should be that of the front Side and not the Side that goes to the 9F Profile
            //The Connector Profiles have a ThicknessView for the 9F Part and an Inner Thickness View for the Part that is in front
            int profile1Thickness;
            int profile2Thickness;
            if (cabin.Parts.WallProfile1?.ProfileType == CabinProfileType.ConnectorProfile)
            {
                profile1Thickness = cabin.Parts.WallProfile1.InnerThicknessView;
            }
            else
            {
                profile1Thickness = cabin.Parts.WallProfile1?.ThicknessView ?? 0;
            }
            if (cabin.Parts.WallProfile2?.ProfileType == CabinProfileType.ConnectorProfile)
            {
                profile2Thickness = cabin.Parts.WallProfile2.InnerThicknessView;
            }
            else
            {
                profile2Thickness = cabin.Parts.WallProfile2?.ThicknessView ?? 0;
            }

            double doorCalculationsLength = (cabin.LengthMin 
                - stepLength // -stepLength2 if we Add Second Step! 
                - profile1Thickness
                - profile2Thickness
                + 2 * (cabin.Parts.Handle?.GetSlidingDoorAirDistance() ?? 0)
                + 2 * cabin.Constraints.CoverDistance
                - 2 * (cabin.Parts.CloseStrip?.OutOfGlassLength ?? 0)
                + 2 * cabin.Constraints.Overlap)
                / 4d ;
            //The Average GlassInProfileDepth from both Profiles , practically they would never be different though

            return doorCalculationsLength;
        }
        public static double DoorGlassLength9ACalculation(Cabin9A cabin)
        {
            //Constraint for StepLength = 0 When both Step Height/Length are not Set
            double stepLength = 0;
            if (cabin.HasStep)
            {
                stepLength = cabin.GetStepCut().StepLength;
            }

            //When the Profile is a Connector the Calculation of Thickness should be that of the front Side and not the Side that goes to the 9F Profile
            //The Connector Profiles have a ThicknessView for the 9F Part and an Inner Thickness View for the Part that is in front
            int profile1Thickness;
            if (cabin.Parts.WallProfile1?.ProfileType == CabinProfileType.ConnectorProfile)
            {
                profile1Thickness = cabin.Parts.WallProfile1.InnerThicknessView;
            }
            else
            {
                profile1Thickness = cabin.Parts.WallProfile1?.ThicknessView ?? 0;
            }

            double doorCalculationsLength = ( cabin.LengthMin 
                - stepLength 
                - cabin.Parts.Angle.AngleDistanceFromDoor
                - profile1Thickness
                + cabin.Constraints.CoverDistance
                + cabin.Parts.Handle.GetSlidingDoorAirDistance()
                + cabin.Constraints.Overlap
                - cabin.Parts.CloseStrip.OutOfGlassLength ) 
                / 2d;

            return doorCalculationsLength;
        }
        public static double DoorGlassLength9SCalculation(Cabin9S cabin) 
        {
            //Constraint for StepLength = 0 When both Step Height/Length are not Set
            double stepLength = 0;
            if (cabin.HasStep)
            {
                stepLength = cabin.GetStepCut().StepLength;
            }

            //When the Profile is a Connector the Calculation of Thickness should be that of the front Side and not the Side that goes to the 9F Profile
            //The Connector Profiles have a ThicknessView for the 9F Part and an Inner Thickness View for the Part that is in front
            int profile1Thickness;
            int profile2Thickness;
            if (cabin.Parts.WallProfile1?.ProfileType == CabinProfileType.ConnectorProfile)
            {
                profile1Thickness = cabin.Parts.WallProfile1.InnerThicknessView;
            }
            else
            {
                profile1Thickness = cabin.Parts.WallProfile1?.ThicknessView ?? 0;
            }
            if (cabin.Parts.WallProfile2?.ProfileType == CabinProfileType.ConnectorProfile)
            {
                profile2Thickness = cabin.Parts.WallProfile2.InnerThicknessView;
            }
            else
            {
                profile2Thickness = cabin.Parts.WallProfile2?.ThicknessView ?? 0;
            }

            double doorCalculationsLength = (cabin.LengthMin
                - stepLength
                - profile1Thickness
                - profile2Thickness
                + cabin.Constraints.CoverDistance
                + (cabin.Parts.Handle?.GetSlidingDoorAirDistance() ?? throw new InvalidOperationException("Cannot Calculate 9S Glass Without Handle"))
                + cabin.Constraints.Overlap
                - cabin.Parts.CloseStrip.OutOfGlassLength)
                / 2d;
            
            return doorCalculationsLength;
        }
        public static double DoorGlassLengthV4Calculation(CabinV4 cabin) 
        {
            //Constraint for StepLength = 0 When both Step Height/Length are not Set
            double stepLength = 0;
            if (cabin.HasStep)
            {
                stepLength = cabin.GetStepCut().StepLength;
            }

            // Find the ALST - AL1 If an Aluminium is There else glass Gap Only
            (int wallFixer1, _) = cabin.Parts.WallSideFixer switch
            {
                Profile p => (p.ThicknessView, p.GlassInProfileDepth),
                CabinSupport s => (s.GlassGapAER, 0),
                _ => (0, 0),
            };
            (int wallFixer2, _) = cabin.Parts.WallFixer2 switch
            {
                Profile p => (p.ThicknessView, p.GlassInProfileDepth),
                CabinSupport s => (s.GlassGapAER, 0),
                _ => (0, 0),
            };

            double doorCalculationLength = (cabin.LengthMin
                - stepLength // -stepLength2 if we add Second Step!
                - wallFixer1
                - wallFixer2
                + 2 * (cabin.Parts.Handle?.GetSlidingDoorAirDistance() ?? throw new InvalidOperationException("Cannot Calculate V4 Glass Without Handle"))
                + 2 * cabin.Constraints.CoverDistance
                - 2 * (cabin.Parts.CloseStrip?.OutOfGlassLength ?? throw new InvalidOperationException("Cannot Calculate V4 Glass Without Close Strip"))
                + 2 * cabin.Constraints.Overlap)
                / 4d;

            return doorCalculationLength;
        }
        public static double DoorGlassLengthVSCalculation(CabinVS cabin) 
        {
            //Constraint for StepLength = 0 When both Step Height/Length are not Set
            double stepLength = 0;
            if (cabin.HasStep)
            {
                stepLength = cabin.GetStepCut().StepLength;
            }

            // Find the ALST - AL1 If an Aluminium is There else glass Gap Only
            (int wallFixer, _) = cabin.Parts.WallSideFixer switch
            {
                Profile p => (p.ThicknessView, p.GlassInProfileDepth),
                CabinSupport s => (s.GlassGapAER, 0),
                _ => (0, 0),
            };

            double glassDoorCalculationLength = (cabin.LengthMin
                - stepLength
                - wallFixer
                - (cabin.Parts.CloseProfile?.ThicknessView ?? 0)
                + cabin.Constraints.CoverDistance
                + (cabin.Parts.Handle?.GetSlidingDoorAirDistance() ?? throw new InvalidOperationException("Cannot Calculate VS Glass Without Handle"))
                + cabin.Constraints.Overlap
                - (cabin.Parts.CloseStrip?.OutOfGlassLength ?? throw new InvalidOperationException("Cannot Calculate VS Glass Without CloseStrip")))
                / 2d;

            return glassDoorCalculationLength;
        }
        public static double DoorGlassLengthVACalculation(CabinVA cabin)
        {
            //Constraint for StepLength = 0 When both Step Height/Length are not Set
            double stepLength = 0;
            if (cabin.HasStep)
            {
                stepLength = cabin.GetStepCut().StepLength;
            }

            // Find the ALST - AL1 If an Aluminium is There else glass Gap Only
            (int wallFixer, _) = cabin.Parts.WallSideFixer switch
            {
                Profile p => (p.ThicknessView, p.GlassInProfileDepth),
                CabinSupport s => (s.GlassGapAER, 0),
                _ => (0, 0),
            };

            double glassDoorCalculationLength = (cabin.LengthMin 
                - stepLength
                - wallFixer
                - cabin.Parts.Angle.AngleDistanceFromDoor
                + cabin.Constraints.CoverDistance
                + cabin.Parts.Handle.GetSlidingDoorAirDistance()
                + cabin.Constraints.Overlap
                - cabin.Parts.CloseStrip.OutOfGlassLength) 
                / 2d;

            return glassDoorCalculationLength;
        }
        public static double DoorGlassLengthWSCalculation(CabinWS cabin)
        {
            int stepLength = 0;
            // NEVER WITH STEP
            //if (cabin.HasStep)
            //{
            //    stepLength = cabin.GetStepCut().StepLength;
            //}
            
            //factor in If : CloseProfile - Handle - CloseStrip
            double doorCalculationsLength = (cabin.LengthMin 
                - stepLength
                - cabin.Parts.WallFixer.ThicknessView
                - (cabin.Parts.CloseProfile?.ThicknessView ?? 0)
                + cabin.Constraints.CoverDistance
                + (cabin.Parts.Handle?.GetSlidingDoorAirDistance() ?? 0)
                + cabin.Constraints.Overlap
                - (cabin.Parts.CloseStrip?.OutOfGlassLength ?? 0))
                / 2d;

            return doorCalculationsLength;
        }

        #endregion

        #region 2.Cabin Deductible Glasses Length

        /// <summary>
        /// Returns the Total Deductable Length for a Cabin
        /// This number is how much Length must be Deducted from LengthMin in order to find the Total Glasses Length plus any overlapping between them.
        /// </summary>
        /// <param name="cabin"></param>
        /// <returns>The Total Deductable Length</returns>
        /// <exception cref="ArgumentNullException">When the Model of the Cabin is Null</exception>
        public static double GetDeductableGlassesLength(Cabin cabin)
        {
            CabinModelEnum model = cabin.Model ?? throw new ArgumentNullException(nameof(cabin.Model));
            //Get all the Positioned Parts
            var deductableLengthParts = cabin.Parts.GetPositionedParts().Values.OfType<IDeductableGlassesLength>();
            //Extract the Deductable Length from all of them .
            //Deducting this Length from the Length Min of a Cabin gives us the Total Glasses Length plus any Overlapping between them
            return deductableLengthParts.Sum(p => p.GetDeductableLength(model));
        }

        /// <summary>
        /// Returns the Total Length of Glasses for a Cabin .
        /// Takes into Account Overlaps 
        /// </summary>
        /// <param name="cabin"></param>
        /// <returns>The Total Length of All Glasses of the Cabin</returns>
        public static double GetGlassesTotalLength(Cabin cabin)
        {
            double deductibleLength = GetDeductableGlassesLength(cabin);
            double overlap = 0;
            
            // If This is a Slider then add also the Overlap to the Total Glasses
            if (cabin.Constraints is ISlidingConstraints constraints)
            {
                overlap = constraints.GetTotalOverlap();
            }
            
            // return total length
            return (cabin.LengthMin - deductibleLength + overlap);
        }

        #endregion

    }
}
