using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Helpers;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.StripsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.SupportsModels;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.DBModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.HBModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.Inox304Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.NBModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.NPModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.WSSmartModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Builder
{
    /// <summary>
    /// Runs Calculations for the Various Profiles and Glass Strips -- Glass Calculations must  have been made first
    /// </summary>
    public static class PartsDimensionsCalculator
    {
#nullable disable
        public static void CalculatePartsDimensions(Cabin cabin)
        {

            switch (cabin.Model)
            {
                case CabinModelEnum.Model9S:
                    Calculate9SPartsDimensions((Cabin9S)cabin);
                    break;
                case CabinModelEnum.Model94:
                    Calculate94PartsDimensions((Cabin94)cabin);
                    break;
                case CabinModelEnum.Model9A:
                    Calculate9APartsDimensions((Cabin9A)cabin);
                    break;
                case CabinModelEnum.Model9B:
                    Calculate9BPartsDimensions((Cabin9B)cabin);
                    break;
                case CabinModelEnum.Model9F:
                    Calculate9FPartsDimensions((Cabin9F)cabin);
                    break;
                case CabinModelEnum.ModelW:
                case CabinModelEnum.Model8W40:
                    CalculateWPartsDimensions((CabinW)cabin);
                    break;
                case CabinModelEnum.ModelHB:
                    CalculateHBPartsDimensions((CabinHB)cabin);
                    break;
                case CabinModelEnum.ModelNP:
                case CabinModelEnum.ModelQP:
                case CabinModelEnum.ModelMV2:
                case CabinModelEnum.ModelNV2:
                    CalculateNPPartsDimensions((CabinNP)cabin);
                    break;
                case CabinModelEnum.ModelVS:
                    CalculateVSPartsDimensions((CabinVS)cabin);
                    break;
                case CabinModelEnum.ModelVF:
                    CalculateVFPartsDimensions((CabinVF)cabin);
                    break;
                case CabinModelEnum.ModelV4:
                    CalculateV4PartsDimensions((CabinV4)cabin);
                    break;
                case CabinModelEnum.ModelVA:
                    CalculateVAPartsDimensions((CabinVA)cabin);
                    break;
                case CabinModelEnum.ModelWS:
                    CalculateWSPartsDimensions((CabinWS)cabin);
                    break;
                case CabinModelEnum.ModelE:
                    CalculateEPartsDimensions((CabinE)cabin);
                    break;
                case CabinModelEnum.ModelWFlipper:
                    CalculateWFlipperPartsDimensions((CabinWFlipper)cabin);
                    break;
                case CabinModelEnum.ModelDB:
                    CalculateDBPartsDimensions((CabinDB)cabin);
                    break;
                case CabinModelEnum.ModelNB:
                case CabinModelEnum.ModelQB:
                case CabinModelEnum.ModelNV:
                    CalculateNBPartsDimensions((CabinNB)cabin);
                    break;
                case CabinModelEnum.Model9C:
                    Calculate9CPartsDimensions((Cabin9C)cabin);
                    break;
                case CabinModelEnum.Model6WA:
                    throw new NotImplementedException();
                case null:
                case CabinModelEnum.ModelGlassContainer:
                default:
                    break;
            }
        }
        private static void Calculate9CPartsDimensions(Cabin9C cabin)
        {
            var glasses = SetGlassPolycarbonicsLengths(cabin);
            //CANNOT HAVE STEP
            //var step = cabin.GetStepCut();
            //int stepHeight = step?.StepHeight ?? 0;
            //int stepLength = step?.StepLength ?? 0;

            //This should be combined as one piece with the Profile of The second Piece
            cabin.Parts.HorizontalProfileTop.CutLength = cabin.LengthMin
                - cabin.Parts.WallProfile1.ThicknessView;

            cabin.Parts.HorizontalProfileBottom.CutLength = cabin.Parts.HorizontalProfileTop.CutLength;

            //If there is no Step then Simply the stepDimensions are Zero
            cabin.Parts.WallProfile1.CutLength = cabin.Height;

            //Pass any Cut Length to Siblings or to placed Spots
            cabin.Parts.WallProfile1.PassCutLengthToSiblings();
            var innerWallProfile1 = cabin.Parts.GetPartOrNull(PartSpot.InternalProfileWall1) as IWithCutLength;
            if (innerWallProfile1 is not null) innerWallProfile1.CutLength = cabin.Parts.WallProfile1.CutLength;
            var innerWallProfile1Alt = cabin.Parts.GetPartOrNull(PartSpot.InternalProfileWall1Alt) as IWithCutLength;
            if (innerWallProfile1Alt is not null) innerWallProfile1Alt.CutLength = cabin.Parts.WallProfile1.CutLength;
            
            cabin.Parts.WallProfile1.CutLengthStepPart = 0;

            var sealerL0Top = cabin.Parts.GetPartOrNull(PartSpot.HorizontalGuideSealerTop1) as IWithCutLength;
            var sealerL0Bottom = cabin.Parts.GetPartOrNull(PartSpot.HorizontalGuideSealerTop1) as IWithCutLength;

            if (glasses.fixed1 is not null)
            {
                if (sealerL0Top is not null) sealerL0Top.CutLength = glasses.fixed1.Length + cabin.Constraints.SealerL0LengthCorrection;
                if (sealerL0Bottom is not null) sealerL0Bottom.CutLength = glasses.fixed1.Length - glasses.fixed1.StepLength + cabin.Constraints.SealerL0LengthCorrection;
            }
            //Correct the Fixed Sealers Height
            if (cabin.Parts.GetPartOrNull(PartSpot.FixedGlass1SideSealer) is IWithCutLength sideSeal)
            {
                sideSeal.CutLength -= cabin.Parts.HorizontalProfileTop?.GlassInProfileDepth * 2 ?? 0;
            }
        }
        private static void CalculateNBPartsDimensions(CabinNB cabin)
        {
            var glasses = SetGlassPolycarbonicsLengths(cabin);

            cabin.Parts.WallHinge.CutLength = cabin.Height;
            double correctionLength = cabin.Height - (glasses.door1?.Height ?? 0);
            //Pass the Difference inner alluminums should be smaller by the difference (equal to the glass)
            cabin.Parts.WallHinge.PassCutLengthToSiblings(-correctionLength);
            
            //Correct Length of Bottom Door Sealer
            var bottomSealerDoor1 = cabin.Parts.GetPartOrNull(PartSpot.DoorGlass1BottomSealer) as IWithCutLength;
            if (bottomSealerDoor1 is not null) bottomSealerDoor1.CutLength -= cabin.Parts.WallHinge?.GlassInProfileDepth ?? 0;

            if (cabin.Parts.CloseProfile is not null)
            {
                //deduct the upper aluminium part as well as any corner Radius
                cabin.Parts.CloseProfile.CutLength = cabin.Height - cabin.Parts.WallHinge.TopHeightAboveGlass - cabin.Constraints.CornerRadiusTopEdge;
                cabin.Parts.CloseProfile.PassCutLengthToSiblings();
            }

            //Correct the Close Strip Height if there is Corner Radius
            if (cabin.Parts.CloseStrip is not null)
            {
                cabin.Parts.CloseStrip.CutLength -= cabin.Constraints.CornerRadiusTopEdge;
            }

            //Find Length of Sealer
            var frontDoorSealer = cabin.Parts.GetPartOrNull(PartSpot.Door1FrontSealer) as IWithCutLength;
            if (frontDoorSealer is not null && glasses.door1 is not null)
            {
                frontDoorSealer.CutLength = glasses.door1.Length + cabin.Constraints.DoorSealerLengthCorrection;
            }

        }
        private static void CalculateDBPartsDimensions(CabinDB cabin)
        {
            var glasses = SetGlassPolycarbonicsLengths(cabin);

            if (cabin.Parts.CloseProfile is not null)
            {
                cabin.Parts.CloseProfile.CutLength = cabin.Height - cabin.Constraints.FinalHeightCorrection - cabin.Constraints.CornerRadiusTopEdge;
                cabin.Parts.CloseProfile.PassCutLengthToSiblings();
            }
            if (cabin.Parts.CloseStrip is not null)
            {
                cabin.Parts.CloseStrip.CutLength -= cabin.Constraints.CornerRadiusTopEdge;
            }

            //Find Length of Sealer
            var frontDoorSealer = cabin.Parts.GetPartOrNull(PartSpot.Door1FrontSealer) as IWithCutLength;
            if (frontDoorSealer is not null && glasses.door1 is not null)
            {
                frontDoorSealer.CutLength = glasses.door1.Length + cabin.Constraints.DoorSealerLengthCorrection;
            }
        }
        private static void CalculateWFlipperPartsDimensions(CabinWFlipper cabin)
        {
            _ = SetGlassPolycarbonicsLengths(cabin);
            
            return;
        }
        private static void CalculateEPartsDimensions(CabinE cabin)
        {
            var glasses = SetGlassPolycarbonicsLengths(cabin);
            
            if (cabin.Parts.BottomFixer is Profile floorProfile)
            {
                floorProfile.CutLength = glasses.fixed1?.Length ?? 0;
            }

            //Alter the Suport Bar distance from the Side , if there is Corner Radius
            //otherwise use the Default Value
            if (cabin.Parts.SupportBar is not null)
            {
                //The Clamp get placed in the Default position or 10% back from the Corner Radius
                cabin.Parts.SupportBar.ClampCenterDistanceFromGlass =
                    cabin.Constraints.CornerRadiusTopEdge >= 0.9d * cabin.Parts.SupportBar.ClampCenterDistanceFromGlassDefault
                    ? (cabin.Constraints.CornerRadiusTopEdge * 1.1d)
                    : (cabin.Parts.SupportBar.ClampCenterDistanceFromGlassDefault);
            }

        }
        private static void CalculateWSPartsDimensions(CabinWS cabin)
        {
            var glasses = SetGlassPolycarbonicsLengths(cabin);

            //If there is no Step then Simply the stepDimensions are Zero
            cabin.Parts.WallFixer.CutLength = cabin.Height - cabin.Constraints.FinalHeightCorrection;
            cabin.Parts.WallFixer.CutLengthStepPart = 0;

            if (cabin.Parts.CloseProfile is not null && glasses.door1 is not null)
            {
                cabin.Parts.CloseProfile.CutLength = glasses.door1.Height + cabin.Constraints.DoorDistanceFromBottom;
                cabin.Parts.CloseProfile.PassCutLengthToSiblings();
            }

            cabin.Parts.SupportBar.ClampCenterDistanceFromGlass = cabin.Parts.SupportBar.ClampCenterDistanceFromGlassDefault;
        }
        private static void CalculateVAPartsDimensions(CabinVA cabin)
        {
            var glasses = SetGlassPolycarbonicsLengths(cabin);

            var step = cabin.GetStepCut();
            int stepHeight = step?.StepHeight ?? 0;
            int stepLength = step?.StepLength ?? 0;

            Profile wallProfile = cabin.Parts.WallSideFixer as Profile;
            Profile floorProfile = cabin.Parts.BottomFixer as Profile;

            #region 1.Wall Fixer
            //Main Wall Profile , that can also have Step Cut
            if (wallProfile is not null)
            {
                wallProfile.CutLength = cabin.Height - cabin.Constraints.FinalHeightCorrection - stepHeight;
                wallProfile.CutLengthStepPart = stepHeight;
                
                //Pass Cut Length to any Siblings or to the Spots containing the extra profiles
                wallProfile.PassCutLengthToSiblings();
                var innerWallProfile1 = cabin.Parts.GetPartOrNull(PartSpot.InternalProfileWall1) as IWithCutLength;
                if (innerWallProfile1 is not null) innerWallProfile1.CutLength = wallProfile.CutLength;
                var innerWallProfile1Alt = cabin.Parts.GetPartOrNull(PartSpot.InternalProfileWall1Alt) as IWithCutLength;
                if (innerWallProfile1Alt is not null) innerWallProfile1Alt.CutLength = wallProfile.CutLength;
            }
            else if (cabin.Parts.WallSideFixer is CabinSupport support)
            {
                support.Quantity = 2;
            }
            #endregion

            #region 2.Bottom Fixer
            //Floor Profile with or Without Step and With or Without Wall Profile
            if (floorProfile is not null && glasses.fixed1 is not null)
            {
                floorProfile.CutLength = glasses.fixed1.Length - glasses.fixed1.StepLength
                    - (wallProfile is not null ? wallProfile.GlassInProfileDepth : 0);

                if (cabin.HasStep)
                {
                    floorProfile.CutLengthStepPart = glasses.fixed1.StepLength;
                }
                else
                {
                    floorProfile.CutLengthStepPart = 0;
                }
            }
            #endregion

            cabin.Parts.HorizontalBar.CutLength = cabin.LengthMax - cabin.Constraints.BarCorrectionLength;

            if (cabin.Parts.SupportBar is not null)
            {
                cabin.Parts.SupportBar.ClampCenterDistanceFromGlass = cabin.Parts.SupportBar.ClampCenterDistanceFromGlassDefault;
            }
            if (cabin.Parts.SupportBar is not null)
            {
                cabin.Parts.SupportBar.ClampCenterDistanceFromGlass = cabin.Parts.SupportBar.ClampCenterDistanceFromGlassDefault;
            }

            //Set the Angle of the Secondary Structure to zero Quantity (The Structure needs only one Corner)
            if (cabin.IsPartOfDraw == CabinDrawNumber.DrawVA && cabin.SynthesisModel == CabinSynthesisModel.Secondary)
            {
                if (cabin.Parts.Angle is not null)
                {
                    cabin.Parts.Angle.Quantity = 0;
                }
            }
            //Find Length of Sealer
            var frontDoorSealer = cabin.Parts.GetPartOrNull(PartSpot.Door1FrontSealer) as IWithCutLength;
            if (frontDoorSealer is not null && glasses.door1 is not null)
            {
                frontDoorSealer.CutLength = glasses.door1.Length + cabin.Constraints.DoorSealerLengthCorrection;
            }

        }
        private static void CalculateV4PartsDimensions(CabinV4 cabin)
        {
            var glasses = SetGlassPolycarbonicsLengths(cabin);

            var step = cabin.GetStepCut();
            int stepHeight = step?.StepHeight ?? 0;
            int stepLength = step?.StepLength ?? 0;

            Profile wallProfile1 = cabin.Parts.WallSideFixer as Profile;
            Profile wallProfile2 = cabin.Parts.WallFixer2 as Profile;
            Profile floorProfile1 = cabin.Parts.BottomFixer1 as Profile;
            Profile floorProfile2 = cabin.Parts.BottomFixer2 as Profile;

            #region 1.Wall Fixer
            //Main Wall Profile , that can also have Step Cut
            if (wallProfile1 is not null)
            {
                wallProfile1.CutLength = cabin.Height - cabin.Constraints.FinalHeightCorrection - stepHeight;
                wallProfile1.CutLengthStepPart = stepHeight;
                
                //Pass Cut Length to any Siblings or to the Spots containing the extra profiles
                wallProfile1.PassCutLengthToSiblings();
                var innerWallProfile1 = cabin.Parts.GetPartOrNull(PartSpot.InternalProfileWall1) as IWithCutLength;
                if (innerWallProfile1 is not null) innerWallProfile1.CutLength = wallProfile1.CutLength;
                var innerWallProfile1Alt = cabin.Parts.GetPartOrNull(PartSpot.InternalProfileWall1Alt) as IWithCutLength;
                if (innerWallProfile1Alt is not null) innerWallProfile1Alt.CutLength = wallProfile1.CutLength;
            }
            else if (cabin.Parts.WallSideFixer is CabinSupport support)
            {
                support.Quantity = 2;
            }

            if (wallProfile2 is not null)
            {
                wallProfile2.CutLength = cabin.Height - cabin.Constraints.FinalHeightCorrection;
                wallProfile2.CutLengthStepPart = 0;

                //Pass Cut Length to any Siblings or to the Spots containing the extra profiles
                wallProfile2.PassCutLengthToSiblings();
                var innerWallProfile1 = cabin.Parts.GetPartOrNull(PartSpot.InternalProfileWall1) as IWithCutLength;
                if (innerWallProfile1 is not null) innerWallProfile1.CutLength = wallProfile2.CutLength;
                var innerWallProfile1Alt = cabin.Parts.GetPartOrNull(PartSpot.InternalProfileWall1Alt) as IWithCutLength;
                if (innerWallProfile1Alt is not null) innerWallProfile1Alt.CutLength = wallProfile2.CutLength;
            }
            else if (cabin.Parts.WallFixer2 is CabinSupport support2)
            {
                support2.Quantity = 2;
            }
            #endregion

            #region 2.Bottom Fixer
            //Floor Profile with or Without Step and With or Without Wall Profile
            if (floorProfile1 is not null && glasses.fixed1 is not null)
            {
                floorProfile1.CutLength = glasses.fixed1.Length - glasses.fixed1.StepLength
                    - (wallProfile1 is not null ? wallProfile1.GlassInProfileDepth : 0);

                if (cabin.HasStep)
                {
                    floorProfile1.CutLengthStepPart = glasses.fixed1.StepLength;
                }
                else
                {
                    floorProfile1.CutLengthStepPart = 0;
                }
            }

            if (floorProfile2 is not null && glasses.fixed2 is not null)
            {
                floorProfile2.CutLength = glasses.fixed2.Length
                    - (wallProfile2 is not null ? wallProfile2.GlassInProfileDepth : 0);
                floorProfile2.CutLengthStepPart = 0;
            }

            #endregion

            cabin.Parts.HorizontalBar.CutLength = cabin.LengthMax - cabin.Constraints.BarCorrectionLength;
            if (cabin.Parts.SupportBar is not null)
            {
                cabin.Parts.SupportBar.ClampCenterDistanceFromGlass = cabin.Parts.SupportBar.ClampCenterDistanceFromGlassDefault;
            }

            //Find Length of Sealer
            var frontDoorSealer = cabin.Parts.GetPartOrNull(PartSpot.Door1FrontSealer) as IWithCutLength;
            if (frontDoorSealer is not null && glasses.door1 is not null && glasses.door2 is not null)
            {
                frontDoorSealer.CutLength = glasses.door1.Length + glasses.door2.Length + cabin.Constraints.DoorSealerLengthCorrection;
            }
        }
        private static void CalculateVFPartsDimensions(CabinVF cabin)
        {
            var glasses = SetGlassPolycarbonicsLengths(cabin);

            var step = cabin.GetStepCut();
            int stepHeight = step?.StepHeight ?? 0;
            int stepLength = step?.StepLength ?? 0;

            Profile wallProfile = cabin.Parts.WallSideFixer as Profile;
            Profile floorProfile = cabin.Parts.BottomFixer as Profile;
            Profile sideProfile = cabin.Parts.SideFixer as Profile;

            #region 1.Wall Fixer
            //Main Wall Profile , that can also have Step Cut
            if (wallProfile is not null)
            {
                wallProfile.CutLength = cabin.Height - cabin.Constraints.FinalHeightCorrection - stepHeight;
                wallProfile.CutLengthStepPart = stepHeight;
                
                //Pass Cut Length to any Siblings or to the Spots containing the extra profiles
                wallProfile.PassCutLengthToSiblings();
                var innerWallProfile1 = cabin.Parts.GetPartOrNull(PartSpot.InternalProfileWall1) as IWithCutLength;
                if (innerWallProfile1 is not null) innerWallProfile1.CutLength = wallProfile.CutLength;
                var innerWallProfile1Alt = cabin.Parts.GetPartOrNull(PartSpot.InternalProfileWall1Alt) as IWithCutLength;
                if (innerWallProfile1Alt is not null) innerWallProfile1Alt.CutLength = wallProfile.CutLength;
            }
            else if (cabin.Parts.WallSideFixer is CabinSupport support)
            {
                support.Quantity = 2;
            }
            #endregion

            #region 2.Bottom Fixer
            //Floor Profile with or Without Step and With or Without Wall Profile
            if (floorProfile is not null && glasses.fixed1 is not null)
            {
                floorProfile.CutLength = glasses.fixed1.Length - glasses.fixed1.StepLength
                    - (wallProfile is not null ? wallProfile.GlassInProfileDepth : 0);

                if (cabin.HasStep)
                {
                    floorProfile.CutLengthStepPart = glasses.fixed1.StepLength;
                }
                else
                {
                    floorProfile.CutLengthStepPart = 0;
                }
            }
            #endregion

            #region 3.Side Fixer
            if (sideProfile is not null)
            {
                sideProfile.CutLength = cabin.Height - cabin.Constraints.FinalHeightCorrection;
                sideProfile.CutLengthStepPart = 0;
                sideProfile.PassCutLengthToSiblings();
            }
            else if (cabin.Parts.SideFixer is CabinSupport support)
            {
                support.Quantity = 2;
            }
            #endregion

            if (cabin.Parts.SupportBar is not null)
            {
                cabin.Parts.SupportBar.ClampCenterDistanceFromGlass = cabin.Parts.SupportBar.ClampCenterDistanceFromGlassDefault;
            }
        }
        private static void CalculateVSPartsDimensions(CabinVS cabin)
        {
            var glasses = SetGlassPolycarbonicsLengths(cabin);

            var step = cabin.GetStepCut();
            int stepHeight = step?.StepHeight ?? 0;
            int stepLength = step?.StepLength ?? 0;

            Profile wallProfile = cabin.Parts.WallSideFixer as Profile;
            Profile floorProfile = cabin.Parts.BottomFixer as Profile;

            #region 1.Wall Fixer
            //Main Wall Profile , that can also have Step Cut
            if (wallProfile is not null)
            {
                wallProfile.CutLength = cabin.Height - cabin.Constraints.FinalHeightCorrection - stepHeight;
                wallProfile.CutLengthStepPart = stepHeight;
                
                //Pass Cut Length to any Siblings or to the Spots containing the extra profiles
                wallProfile.PassCutLengthToSiblings();
                var innerWallProfile1 = cabin.Parts.GetPartOrNull(PartSpot.InternalProfileWall1) as IWithCutLength;
                if (innerWallProfile1 is not null) innerWallProfile1.CutLength = wallProfile.CutLength;
                var innerWallProfile1Alt = cabin.Parts.GetPartOrNull(PartSpot.InternalProfileWall1Alt) as IWithCutLength;
                if (innerWallProfile1Alt is not null) innerWallProfile1Alt.CutLength = wallProfile.CutLength;
            }
            else if (cabin.Parts.WallSideFixer is CabinSupport support)
            {
                support.Quantity = 2;
            }
            #endregion

            #region 2.Bottom Fixer
            //Floor Profile with or Without Step and With or Without Wall Profile
            if (floorProfile is not null && glasses.fixed1 is not null)
            {
                floorProfile.CutLength = glasses.fixed1.Length - glasses.fixed1.StepLength
                    - (wallProfile is not null ? wallProfile.GlassInProfileDepth : 0);

                if (cabin.HasStep)
                {
                    floorProfile.CutLengthStepPart = glasses.fixed1.StepLength;
                }
                else
                {
                    floorProfile.CutLengthStepPart = 0;
                }
            }
            #endregion


            if (cabin.Parts.CloseProfile is not null)
            {
                cabin.Parts.CloseProfile.CutLength = cabin.Height;
                cabin.Parts.CloseProfile.PassCutLengthToSiblings();
            }

            cabin.Parts.HorizontalBar.CutLength = cabin.LengthMax - cabin.Constraints.BarCorrectionLength;
            if (cabin.Parts.SupportBar is not null)
            {
                cabin.Parts.SupportBar.ClampCenterDistanceFromGlass = cabin.Parts.SupportBar.ClampCenterDistanceFromGlassDefault;
            }
            //Find Length of Sealer
            var frontDoorSealer = cabin.Parts.GetPartOrNull(PartSpot.Door1FrontSealer) as IWithCutLength;
            if (frontDoorSealer is not null && glasses.door1 is not null)
            {
                frontDoorSealer.CutLength = glasses.door1.Length + cabin.Constraints.DoorSealerLengthCorrection;
            }
        }
        private static void CalculateNPPartsDimensions(CabinNP cabin)
        {
            ProfileHinge middleProfile = cabin.Parts.MiddleHinge as ProfileHinge;
            var glasses = SetGlassPolycarbonicsLengths(cabin);

            cabin.Parts.WallHinge.CutLength = cabin.Height;
            double correctionLength = cabin.Height - (glasses.door1?.Height ?? 0);
            //Pass the difference as correction length otherwise the inner alluminiums will have the height of the Cabin instead of glass
            cabin.Parts.WallHinge.PassCutLengthToSiblings(-correctionLength);
            //Correct Length of Bottom Door Sealer
            var bottomSealerDoor1 = cabin.Parts.GetPartOrNull(PartSpot.DoorGlass1BottomSealer) as IWithCutLength;
            if (bottomSealerDoor1 is not null) bottomSealerDoor1.CutLength -= cabin.Parts.WallHinge?.GlassInProfileDepth ?? 0;

            if (middleProfile is not null && glasses.door1 is not null)
            {
                middleProfile.CutLength =
                    glasses.door1.Height
                    + middleProfile.TopHeightAboveGlass
                    + middleProfile.BottomHeightBelowGlass;
                middleProfile.PassCutLengthToSiblings();

                //Correct Length of Bottom Door Sealer
                var bottomSealerDoor2 = cabin.Parts.GetPartOrNull(PartSpot.DoorGlass2BottomSealer) as IWithCutLength;
                if (bottomSealerDoor2 is not null) bottomSealerDoor2.CutLength -= middleProfile.GlassInProfileDepth;
            }

            if (cabin.Parts.CloseProfile is not null)
            {
                //Cut as much as the glass plus any remaining on the Bottom (meaning deduct only the upper part of the profile)
                cabin.Parts.CloseProfile.CutLength = cabin.Height - cabin.Parts.WallHinge.TopHeightAboveGlass;
                cabin.Parts.CloseProfile.PassCutLengthToSiblings();
            }

            if (cabin.Parts.CloseStrip is not null)
            {
                cabin.Parts.CloseStrip.CutLength -= cabin.Constraints.CornerRadiusTopEdge;
            }
            if (cabin.Parts.MiddleHinge is not null)
            {
                cabin.Parts.MiddleHinge.Quantity = 2;
            }
            //Find Length of Sealer
            var frontDoorSealer = cabin.Parts.GetPartOrNull(PartSpot.Door1FrontSealer) as IWithCutLength;
            if (frontDoorSealer is not null && glasses.door1 is not null && glasses.door2 is not null)
            {
                frontDoorSealer.CutLength = glasses.door1.Length + glasses.door2.Length + cabin.Constraints.DoorSealerLengthCorrection;
            }
        }
        private static void CalculateHBPartsDimensions(CabinHB cabin)
        {
            var glasses = SetGlassPolycarbonicsLengths(cabin);
            var step = cabin.GetStepCut();
            int stepHeight = step?.StepHeight ?? 0;
            int stepLength = step?.StepLength ?? 0;

            Profile wallProfile = cabin.Parts.WallSideFixer as Profile;
            Profile floorProfile = cabin.Parts.BottomFixer as Profile;

            #region 1.Wall Fixer
            //Main Wall Profile , that can also have Step Cut
            var innerWallProfile1 = cabin.Parts.GetPartOrNull(PartSpot.InternalProfileWall1) as IWithCutLength;
            var innerWallProfile1Alt = cabin.Parts.GetPartOrNull(PartSpot.InternalProfileWall1Alt) as IWithCutLength;
            if (wallProfile is not null)
            {
                wallProfile.CutLength = cabin.Height - cabin.Constraints.FinalHeightCorrection - stepHeight;
                wallProfile.CutLengthStepPart = stepHeight;
                
                //Pass any Cut Length to Siblings or to placed Spots
                wallProfile.PassCutLengthToSiblings();
                
                if (innerWallProfile1 is not null) innerWallProfile1.CutLength = wallProfile.CutLength;
                if (innerWallProfile1Alt is not null) innerWallProfile1Alt.CutLength = wallProfile.CutLength;
            }
            else if (cabin.Parts.WallSideFixer is CabinSupport support)
            {
                support.Quantity = 2;
                
            }
            #endregion

            #region 2.Bottom Fixer
            //Floor Profile with or Without Step and With or Without Wall Profile
            if (floorProfile is not null && glasses.fixed1 is not null)
            {
                floorProfile.CutLength = glasses.fixed1.Length - glasses.fixed1.StepLength
                    - (wallProfile is not null ? wallProfile.GlassInProfileDepth : 0);

                if (cabin.HasStep)
                {
                    floorProfile.CutLengthStepPart = glasses.fixed1.StepLength;
                }
                else
                {
                    floorProfile.CutLengthStepPart = 0;
                }
            }
            else if(floorProfile is null && glasses.fixed1 is not null)
            {
                //Correct the bottom Sealer if there is no floor profile and if there is WallProfile
                var bottomSealer = cabin.Parts.GetPartOrNull(PartSpot.FixedGlass1BottomSealer) as IWithCutLength;
                if (bottomSealer != null && cabin.Parts.WallSideFixer is Profile p) bottomSealer.CutLength -= p.GlassInProfileDepth;
            }
            #endregion

            #region 3.CloseStrip
            if (cabin.Parts.CloseStrip is not null)
            {
                // The whole Height of the Door except if there is Rounding
                cabin.Parts.CloseStrip.CutLength -= cabin.Constraints.CornerRadiusTopEdge;

            }
            #endregion

            #region 4.Close Profile
            if (cabin.Parts.CloseProfile is not null)
            {
                cabin.Parts.CloseProfile.CutLength = cabin.Height
                        - cabin.Constraints.FinalHeightCorrection
                        - cabin.Constraints.CornerRadiusTopEdge;
                cabin.Parts.CloseProfile.PassCutLengthToSiblings();
            }
            #endregion

            #region 5.Support Bar
            if (cabin.Parts.SupportBar is not null)
            {
                cabin.Parts.SupportBar.ClampCenterDistanceFromGlass = cabin.Parts.SupportBar.ClampCenterDistanceFromGlassDefault;
            }
            #endregion

            //Find Length of Sealer
            var frontDoorSealer = cabin.Parts.GetPartOrNull(PartSpot.Door1FrontSealer) as IWithCutLength;
            if (frontDoorSealer is not null && glasses.door1 is not null)
            {
                frontDoorSealer.CutLength = glasses.door1.Length + cabin.Constraints.DoorSealerLengthCorrection;
            }
        }
        private static void CalculateWPartsDimensions(CabinW cabin)
        {
            var glasses = SetGlassPolycarbonicsLengths(cabin);
            
            var step = cabin.GetStepCut();
            int stepHeight = step?.StepHeight ?? 0;
            int stepLength = step?.StepLength ?? 0;

            Profile wallProfile = cabin.Parts.WallSideFixer as Profile;
            Profile floorProfile = cabin.Parts.BottomFixer as Profile;
            Profile topProfile = cabin.Parts.TopFixer as Profile;
            Profile sideProfile = cabin.Parts.SideFixer as Profile;

            #region 1. Wall Fixer
            //Main Wall Profile , that can also have Step Cut
            if (wallProfile is not null)
            {
                wallProfile.CutLength = cabin.Height - cabin.Constraints.FinalHeightCorrection - stepHeight;
                wallProfile.CutLengthStepPart = stepHeight;
                
                //Pass Cut Length to any Siblings or to the Spots containing the extra profiles
                wallProfile.PassCutLengthToSiblings();
                var innerWallProfile1 = cabin.Parts.GetPartOrNull(PartSpot.InternalProfileWall1) as IWithCutLength;
                if (innerWallProfile1 is not null) innerWallProfile1.CutLength = wallProfile.CutLength;
                var innerWallProfile1Alt = cabin.Parts.GetPartOrNull(PartSpot.InternalProfileWall1Alt) as IWithCutLength;
                if (innerWallProfile1Alt is not null) innerWallProfile1Alt.CutLength = wallProfile.CutLength;
            }
            else if (cabin.Parts.WallSideFixer is CabinSupport support)
            {
                support.Quantity = 2;
            }

            #endregion

            #region 2.BottomFixer
            //Floor Profile with or Without Step and With or Without Wall Profile and with or Without Side Profile
            if (floorProfile is not null && glasses.fixed1 is not null)
            {
                floorProfile.CutLength = glasses.fixed1.Length - glasses.fixed1.StepLength
                    - (wallProfile is not null ? wallProfile.GlassInProfileDepth : 0)
                    - (sideProfile is not null ? sideProfile.GlassInProfileDepth : 0);

                if (cabin.HasStep)
                {
                    floorProfile.CutLengthStepPart = glasses.fixed1.StepLength;
                }
                else
                {
                    floorProfile.CutLengthStepPart = 0;
                }
            }
            else if (floorProfile is null && glasses.fixed1 is not null)
            {
                //Correct the bottom Sealer if there is no floor profile and if there is WallProfile
                var bottomSealer = cabin.Parts.GetPartOrNull(PartSpot.FixedGlass1BottomSealer) as IWithCutLength;
                if (bottomSealer != null && cabin.Parts.WallSideFixer is Profile p) bottomSealer.CutLength -= p.GlassInProfileDepth;
            }
            #endregion

            #region 3.TopFixer
            //Top Profile with or without wall profile and Side Profile
            if (topProfile is not null && glasses.fixed1 is not null)
            {
                topProfile.CutLength = glasses.fixed1.Length
                    - (wallProfile is not null ? wallProfile.GlassInProfileDepth : 0)
                    - (sideProfile is not null ? sideProfile.GlassInProfileDepth : 0);
                topProfile.CutLengthStepPart = 0;
            }
            #endregion

            #region 4.SideFixer
            if (sideProfile is not null)
            {
                sideProfile.CutLength = cabin.Height - cabin.Constraints.FinalHeightCorrection;
                sideProfile.CutLengthStepPart = 0;
            }
            else if (cabin.Parts.SideFixer is CabinSupport support)
            {
                support.Quantity = 2;
            }
            #endregion

            #region 5.CloseStrip
            if (cabin.Parts.CloseStrip is not null)
            {
                cabin.Parts.CloseStrip.CutLength = glasses.fixed1?.Height ?? 0;
                //Deduct the Rounded Part
                //Deduct the Length of the Floor stopper if its There
                //Deduct the GlassInProfile if FloorProfile
                //Else Deduct nothing more
                cabin.Parts.CloseStrip.CutLength -= cabin.Constraints.CornerRadiusTopEdge;

                if (floorProfile is not null)
                {
                    cabin.Parts.CloseStrip.CutLength -= floorProfile.GlassInProfileDepth;
                }
                else if (cabin.Parts.BottomFixer is FloorStopperW stopper)
                {
                    cabin.Parts.CloseStrip.CutLength -= stopper.HeightView;
                }
                //else do not deduct something
            }
            #endregion

            #region 6.Support Bar
            //Alter the Suport Bar distance from the Side , if there is Corner Radius
            //otherwise use the Default Value
            if (cabin.Parts.SupportBar is not null)
            {
                //The Clamp get placed in the Default position or 10% back from the Corner Radius
                cabin.Parts.SupportBar.ClampCenterDistanceFromGlass =
                    cabin.Constraints.CornerRadiusTopEdge >= 0.9d * cabin.Parts.SupportBar.ClampCenterDistanceFromGlassDefault
                    ? (cabin.Constraints.CornerRadiusTopEdge * 1.1d)
                    : (cabin.Parts.SupportBar.ClampCenterDistanceFromGlassDefault);
            }
            #endregion
        }
        private static void Calculate9FPartsDimensions(Cabin9F cabin)
        {
            var glasses = SetGlassPolycarbonicsLengths(cabin);
            var step = cabin.GetStepCut();
            int stepHeight = step?.StepHeight ?? 0;
            int stepLength = step?.StepLength ?? 0;

            cabin.Parts.HorizontalProfileTop.CutLength = cabin.LengthMin
                - cabin.Parts.WallProfile1.ThicknessView
                - cabin.Parts.WallProfile2.ThicknessView;

            cabin.Parts.HorizontalProfileBottom.CutLength = cabin.Parts.HorizontalProfileTop.CutLength
                - stepLength;

            //If there is no Step then Simply the stepDimensions are Zero
            cabin.Parts.WallProfile1.CutLength = cabin.Height - stepHeight;
            cabin.Parts.WallProfile1.CutLengthStepPart = stepHeight;
            
            //Pass any Cut Length to Siblings or to placed Spots
            cabin.Parts.WallProfile1.PassCutLengthToSiblings();
            var innerWallProfile1 = cabin.Parts.GetPartOrNull(PartSpot.InternalProfileWall1) as IWithCutLength;
            if (innerWallProfile1 is not null) innerWallProfile1.CutLength = cabin.Parts.WallProfile1.CutLength;
            var innerWallProfile1Alt = cabin.Parts.GetPartOrNull(PartSpot.InternalProfileWall1Alt) as IWithCutLength;
            if (innerWallProfile1Alt is not null) innerWallProfile1Alt.CutLength = cabin.Parts.WallProfile1.CutLength;
            
            
            cabin.Parts.WallProfile2.CutLength = cabin.Height;
            cabin.Parts.WallProfile2.CutLengthStepPart = 0;

            //Pass any Cut Length to Siblings or to placed Spots
            cabin.Parts.WallProfile2.PassCutLengthToSiblings();
            var innerWallProfile2 = cabin.Parts.GetPartOrNull(PartSpot.InternalProfileWall2) as IWithCutLength;
            if (innerWallProfile2 is not null) innerWallProfile2.CutLength = cabin.Parts.WallProfile2.CutLength;
            var innerWallProfile2Alt = cabin.Parts.GetPartOrNull(PartSpot.InternalProfileWall2Alt) as IWithCutLength;
            if (innerWallProfile2Alt is not null) innerWallProfile2Alt.CutLength = cabin.Parts.WallProfile2.CutLength;

            //It has exactly the Length of the Measured Step to cover also the Second Wall Aluminium.
            cabin.Parts.StepBottomProfile.CutLength = stepLength;

            var sealerL0Top = cabin.Parts.GetPartOrNull(PartSpot.HorizontalGuideSealerTop1) as IWithCutLength;
            var sealerL0Bottom = cabin.Parts.GetPartOrNull(PartSpot.HorizontalGuideSealerBottom1) as IWithCutLength;

            if (glasses.fixed1 is not null)
            {
                if (sealerL0Top is not null) sealerL0Top.CutLength = glasses.fixed1.Length + cabin.Constraints.SealerL0LengthCorrection;
                if (sealerL0Bottom is not null) sealerL0Bottom.CutLength = glasses.fixed1.Length - glasses.fixed1.StepLength + cabin.Constraints.SealerL0LengthCorrection;
            }
        }
        private static void Calculate9BPartsDimensions(Cabin9B cabin)
        {
            var glasses = SetGlassPolycarbonicsLengths(cabin);
            var step = cabin.GetStepCut();
            int stepHeight = step?.StepHeight ?? 0;
            int stepLength = step?.StepLength ?? 0;
            
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

            //The Wall Profile 2 is Always Magnet profile the check is not needed now here but lets leave it for the future changes to be correct in case we put a connector here
            if (cabin.Parts.WallProfile2?.ProfileType == CabinProfileType.MagnetProfile || cabin.Parts.WallProfile2?.ProfileType == CabinProfileType.ConnectorProfile)
            {
                profile2Thickness = cabin.Parts.WallProfile2.InnerThicknessView;
            }
            else
            {
                profile2Thickness = cabin.Parts.WallProfile2?.ThicknessView ?? 0;
            }

            cabin.Parts.HorizontalProfileTop.CutLength = cabin.LengthMin
                - profile1Thickness
                - profile2Thickness
                + cabin.Constraints.CorrectionOfL0Length;

            cabin.Parts.HorizontalProfileBottom.CutLength = cabin.Parts.HorizontalProfileTop.CutLength
                - stepLength;

            //If there is no Step then Simply the stepDimensions are Zero
            cabin.Parts.WallProfile1.CutLength = cabin.Height - stepHeight;
            cabin.Parts.WallProfile1.CutLengthStepPart = stepHeight;
            
            //Pass any Cut Length to siblings or Innerprofiles in placed Spots
            cabin.Parts.WallProfile1.PassCutLengthToSiblings();
            var innerWallProfile1 = cabin.Parts.GetPartOrNull(PartSpot.InternalProfileWall1) as IWithCutLength;
            if (innerWallProfile1 is not null) innerWallProfile1.CutLength = cabin.Parts.WallProfile1.CutLength;
            var innerWallProfile1Alt = cabin.Parts.GetPartOrNull(PartSpot.InternalProfileWall1Alt) as IWithCutLength;
            if (innerWallProfile1Alt is not null) innerWallProfile1Alt.CutLength = cabin.Parts.WallProfile1.CutLength;

            cabin.Parts.WallProfile2.CutLength = cabin.Height;
            cabin.Parts.WallProfile2.CutLengthStepPart = 0;

            //Pass any Cut Length to siblings or Innerprofiles in placed Spots
            cabin.Parts.WallProfile2.PassCutLengthToSiblings();
            var innerWallProfile2 = cabin.Parts.GetPartOrNull(PartSpot.InternalProfileWall2) as IWithCutLength;
            if (innerWallProfile2 is not null) innerWallProfile2.CutLength = cabin.Parts.WallProfile2.CutLength;
            var innerWallProfile2Alt = cabin.Parts.GetPartOrNull(PartSpot.InternalProfileWall2Alt) as IWithCutLength;
            if (innerWallProfile2Alt is not null) innerWallProfile2Alt.CutLength = cabin.Parts.WallProfile2.CutLength;

            //It has exactly the Length of the Measured Step to cover also the Second Wall Aluminium.
            cabin.Parts.StepBottomProfile.CutLength = stepLength;

            var fixedGlassStopper1 = cabin.Parts.GetPartOrNull(PartSpot.HorizontalGuideStopper1);
            var fixedGlassStopper2 = cabin.Parts.GetPartOrNull(PartSpot.HorizontalGuideStopper2);
            var horizontalProfileSealerTop = cabin.Parts.GetPartOrNull(PartSpot.HorizontalGuideSealerTop1);
            var horizontalProfileSealerBottom = cabin.Parts.GetPartOrNull(PartSpot.HorizontalGuideSealerBottom1);
            var sideDoorSealer = cabin.Parts.GetPartOrNull(PartSpot.DoorGlass1SideSealer);
            var sideDoorSealerAlt = cabin.Parts.GetPartOrNull(PartSpot.DoorGlass1SideSealerAlt);
            var innerProfile = cabin.Parts.GetPartOrNull(PartSpot.InternalProfileWall1);
            var innerProfileAlt = cabin.Parts.GetPartOrNull(PartSpot.InternalProfileWall1Alt);

            if (glasses.fixed1 is null)
            {
                //Set the Stoppers of fixed to zero
                if (fixedGlassStopper1 is not null) fixedGlassStopper1.Quantity = 0;
                if (fixedGlassStopper2 is not null) fixedGlassStopper2.Quantity = 0;

                //Set the Profile Fixed Sealers to zero
                if (horizontalProfileSealerTop is not null) horizontalProfileSealerTop.Quantity = 0;
                if (horizontalProfileSealerBottom is not null) horizontalProfileSealerBottom.Quantity = 0;

                //Set the Side Sealers 
                if (sideDoorSealer is not null) sideDoorSealer.Quantity = 1;
                if (sideDoorSealerAlt is not null) sideDoorSealerAlt.Quantity = 0;
                
                //Set the Inner Profile
                if (innerProfile is not null) innerProfile.Quantity = 1;
                if (innerProfileAlt is not null) innerProfileAlt.Quantity = 0;
            }
            else
            {
                //Set the Stoppers of fixed to zero
                if (fixedGlassStopper1 is not null) fixedGlassStopper1.Quantity = 1;
                if (fixedGlassStopper2 is not null) fixedGlassStopper2.Quantity = 1;

                //Set the Profile Fixed Sealers to zero
                if (horizontalProfileSealerTop is not null) horizontalProfileSealerTop.Quantity = 1;
                if (horizontalProfileSealerBottom is not null) horizontalProfileSealerBottom.Quantity = 1;

                //Set the Side Sealers 
                if (sideDoorSealer is not null) sideDoorSealer.Quantity = 0;
                if (sideDoorSealerAlt is not null) sideDoorSealerAlt.Quantity = 1;

                //Set the Inner Profile
                if (innerProfile is not null) innerProfile.Quantity = 0;
                if (innerProfileAlt is not null) innerProfileAlt.Quantity = 1;
            }

            var sealerL0Top = cabin.Parts.GetPartOrNull(PartSpot.HorizontalGuideSealerTop1) as IWithCutLength;
            var sealerL0Bottom = cabin.Parts.GetPartOrNull(PartSpot.HorizontalGuideSealerBottom1) as IWithCutLength;

            if (glasses.fixed1 is not null)
            {
                if (sealerL0Top is not null) sealerL0Top.CutLength = glasses.fixed1.Length + cabin.Constraints.SealerL0LengthCorrection;
                if (sealerL0Bottom is not null) sealerL0Bottom.CutLength = glasses.fixed1.Length - glasses.fixed1.StepLength + cabin.Constraints.SealerL0LengthCorrection;
            }
            //Correct the Fixed Sealers Height
            if (cabin.Parts.GetPartOrNull(PartSpot.FixedGlass1SideSealer) is IWithCutLength sideSeal)
            {
                sideSeal.CutLength -= cabin.Parts.HorizontalProfileTop?.GlassInProfileDepth * 2 ?? 0;
            }
        }
        private static void Calculate9APartsDimensions(Cabin9A cabin)
        {
            var glasses = SetGlassPolycarbonicsLengths(cabin);
            var step = cabin.GetStepCut();
            int stepHeight = step?.StepHeight ?? 0;
            int stepLength = step?.StepLength ?? 0;

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

            cabin.Parts.HorizontalProfileTop.CutLength = cabin.LengthMin
                - profile1Thickness
                - cabin.Parts.Angle.AngleLengthL0;

            cabin.Parts.HorizontalProfileBottom.CutLength = cabin.Parts.HorizontalProfileTop.CutLength
                - stepLength;

            //If there is no Step then Simply the stepDimensions are Zero
            cabin.Parts.WallProfile1.CutLength = cabin.Height - stepHeight;
            cabin.Parts.WallProfile1.CutLengthStepPart = stepHeight;
            
            //Pass any Cut Length to Siblings or to placed Spots
            cabin.Parts.WallProfile1.PassCutLengthToSiblings();
            var innerWallProfile1 = cabin.Parts.GetPartOrNull(PartSpot.InternalProfileWall1) as IWithCutLength;
            if (innerWallProfile1 is not null) innerWallProfile1.CutLength = cabin.Parts.WallProfile1.CutLength;
            var innerWallProfile1Alt = cabin.Parts.GetPartOrNull(PartSpot.InternalProfileWall1Alt) as IWithCutLength;
            if (innerWallProfile1Alt is not null) innerWallProfile1Alt.CutLength = cabin.Parts.WallProfile1.CutLength;

            //It has exactly the Length of the Measured Step to cover also the Second Wall Aluminium.
            cabin.Parts.StepBottomProfile.CutLength = stepLength;

            var sealerL0Top = cabin.Parts.GetPartOrNull(PartSpot.HorizontalGuideSealerTop1) as IWithCutLength;
            var sealerL0Bottom = cabin.Parts.GetPartOrNull(PartSpot.HorizontalGuideSealerBottom1) as IWithCutLength;

            if (glasses.fixed1 is not null)
            {
                if (sealerL0Top is not null) sealerL0Top.CutLength = glasses.fixed1.Length + cabin.Constraints.SealerL0LengthCorrection;
                if (sealerL0Bottom is not null) sealerL0Bottom.CutLength = glasses.fixed1.Length - glasses.fixed1.StepLength + cabin.Constraints.SealerL0LengthCorrection;
            }
            //Correct the Fixed Sealers Height
            if (cabin.Parts.GetPartOrNull(PartSpot.FixedGlass1SideSealer) is IWithCutLength sideSeal)
            {
                sideSeal.CutLength -= cabin.Parts.HorizontalProfileTop?.GlassInProfileDepth * 2 ?? 0;
            }
        }
        private static void Calculate94PartsDimensions(Cabin94 cabin)
        {
            var glasses = SetGlassPolycarbonicsLengths(cabin);
            var step = cabin.GetStepCut();
            int stepHeight = step?.StepHeight ?? 0;
            int stepLength = step?.StepLength ?? 0;

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

            cabin.Parts.HorizontalProfileTop.CutLength = cabin.LengthMin
                - profile1Thickness
                - profile2Thickness;

            cabin.Parts.HorizontalProfileBottom.CutLength = cabin.Parts.HorizontalProfileTop.CutLength
                - stepLength;

            //If there is no Step then Simply the stepDimensions are Zero
            cabin.Parts.WallProfile1.CutLength = cabin.Height - stepHeight;
            cabin.Parts.WallProfile1.CutLengthStepPart = stepHeight;
            
            //Pass any Cut Length to Siblings or to placed Spots
            cabin.Parts.WallProfile1.PassCutLengthToSiblings();
            var innerWallProfile1 = cabin.Parts.GetPartOrNull(PartSpot.InternalProfileWall1) as IWithCutLength;
            if (innerWallProfile1 is not null) innerWallProfile1.CutLength = cabin.Parts.WallProfile1.CutLength;
            var innerWallProfile1Alt = cabin.Parts.GetPartOrNull(PartSpot.InternalProfileWall1Alt) as IWithCutLength;
            if (innerWallProfile1Alt is not null) innerWallProfile1Alt.CutLength = cabin.Parts.WallProfile1.CutLength;

            cabin.Parts.WallProfile2.CutLength = cabin.Height;
            cabin.Parts.WallProfile2.CutLengthStepPart = 0;

            //Pass any Cut Length to Siblings or to placed Spots
            cabin.Parts.WallProfile2.PassCutLengthToSiblings();
            var innerWallProfile2 = cabin.Parts.GetPartOrNull(PartSpot.InternalProfileWall2) as IWithCutLength;
            if (innerWallProfile2 is not null) innerWallProfile2.CutLength = cabin.Parts.WallProfile2.CutLength;
            var innerWallProfile2Alt = cabin.Parts.GetPartOrNull(PartSpot.InternalProfileWall2Alt) as IWithCutLength;
            if (innerWallProfile2Alt is not null) innerWallProfile2Alt.CutLength = cabin.Parts.WallProfile2.CutLength;

            //It has exactly the Length of the Measured Step to cover also the Second Wall Aluminium.
            cabin.Parts.StepBottomProfile.CutLength = stepLength;

            var sealerL0Top1 = cabin.Parts.GetPartOrNull(PartSpot.HorizontalGuideSealerTop1) as IWithCutLength;
            var sealerL0Bottom1 = cabin.Parts.GetPartOrNull(PartSpot.HorizontalGuideSealerBottom1) as IWithCutLength;
            var sealerL0Top2 = cabin.Parts.GetPartOrNull(PartSpot.HorizontalGuideSealerTop2) as IWithCutLength;
            var sealerL0Bottom2 = cabin.Parts.GetPartOrNull(PartSpot.HorizontalGuideSealerBottom2) as IWithCutLength;

            if (glasses.fixed1 is not null)
            {
                if (sealerL0Top1 is not null) sealerL0Top1.CutLength = glasses.fixed1.Length + cabin.Constraints.SealerL0LengthCorrection;
                if (sealerL0Bottom1 is not null) sealerL0Bottom1.CutLength = glasses.fixed1.Length - glasses.fixed1.StepLength + cabin.Constraints.SealerL0LengthCorrection;
            }

            if (glasses.fixed2 is not null)
            {
                if (sealerL0Top2 is not null) sealerL0Top2.CutLength = glasses.fixed2.Length + cabin.Constraints.SealerL0LengthCorrection;
                if (sealerL0Bottom2 is not null) sealerL0Bottom2.CutLength = glasses.fixed2.Length - glasses.fixed2.StepLength + cabin.Constraints.SealerL0LengthCorrection;
            }

            //Correct the Fixed Sealers Height
            if (cabin.Parts.GetPartOrNull(PartSpot.FixedGlass1SideSealer) is IWithCutLength sideSeal)
            {
                sideSeal.CutLength -= cabin.Parts.HorizontalProfileTop?.GlassInProfileDepth * 2 ?? 0;
            }
            //Correct the Fixed Sealers Height
            if (cabin.Parts.GetPartOrNull(PartSpot.FixedGlass2SideSealer) is IWithCutLength sideSeal2)
            {
                sideSeal2.CutLength -= cabin.Parts.HorizontalProfileTop?.GlassInProfileDepth * 2 ?? 0;
            }
        }
        private static void Calculate9SPartsDimensions(Cabin9S cabin)
        {
            var glasses = SetGlassPolycarbonicsLengths(cabin);
            var step = cabin.GetStepCut();
            int stepHeight = step?.StepHeight ?? 0;
            int stepLength = step?.StepLength ?? 0;

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

            //Profile 2 is always a Magnet Profile so the Check on Connector is not needed for now 
            if (cabin.Parts.WallProfile2?.ProfileType == CabinProfileType.MagnetProfile || cabin.Parts.WallProfile2?.ProfileType == CabinProfileType.ConnectorProfile)
            {
                profile2Thickness = cabin.Parts.WallProfile2.InnerThicknessView;
            }
            else
            {
                profile2Thickness = cabin.Parts.WallProfile2?.ThicknessView ?? 0;
            }

            cabin.Parts.HorizontalProfileTop.CutLength = cabin.LengthMin
                - profile2Thickness
                - profile1Thickness;

            cabin.Parts.HorizontalProfileBottom.CutLength = cabin.Parts.HorizontalProfileTop.CutLength
                - stepLength;

            //If there is no Step then Simply the stepDimensions are Zero
            cabin.Parts.WallProfile1.CutLength = cabin.Height - stepHeight;
            cabin.Parts.WallProfile1.CutLengthStepPart = stepHeight;

            //Pass Cut Length to any Siblings or to the Spots containing the extra profiles
            cabin.Parts.WallProfile1.PassCutLengthToSiblings();
            var innerWallProfile1 = cabin.Parts.GetPartOrNull(PartSpot.InternalProfileWall1) as IWithCutLength;
            if (innerWallProfile1 is not null) innerWallProfile1.CutLength = cabin.Parts.WallProfile1.CutLength;
            var innerWallProfile1Alt = cabin.Parts.GetPartOrNull(PartSpot.InternalProfileWall1Alt) as IWithCutLength;
            if (innerWallProfile1Alt is not null) innerWallProfile1Alt.CutLength = cabin.Parts.WallProfile1.CutLength;

            cabin.Parts.WallProfile2.CutLength = cabin.Height;
            cabin.Parts.WallProfile2.CutLengthStepPart = 0;

            //Pass Cut Length to any Siblings or to the Spots containing the extra profiles
            cabin.Parts.WallProfile2.PassCutLengthToSiblings();
            var innerWallProfile2 = cabin.Parts.GetPartOrNull(PartSpot.InternalProfileWall2) as IWithCutLength;
            if (innerWallProfile2 is not null) innerWallProfile2.CutLength = cabin.Parts.WallProfile2.CutLength;
            var innerWallProfile2Alt = cabin.Parts.GetPartOrNull(PartSpot.InternalProfileWall2Alt) as IWithCutLength;
            if (innerWallProfile2Alt is not null) innerWallProfile2Alt.CutLength = cabin.Parts.WallProfile2.CutLength;

            //It has exactly the Length of the Measured Step to cover also the Second Wall Aluminium.
            cabin.Parts.StepBottomProfile.CutLength = stepLength;

            var sealerL0Top = cabin.Parts.GetPartOrNull(PartSpot.HorizontalGuideSealerTop1) as IWithCutLength;
            var sealerL0Bottom = cabin.Parts.GetPartOrNull(PartSpot.HorizontalGuideSealerBottom1) as IWithCutLength;

            if (glasses.fixed1 is not null)
            {
                if (sealerL0Top is not null) sealerL0Top.CutLength = glasses.fixed1.Length + cabin.Constraints.SealerL0LengthCorrection;
                if (sealerL0Bottom is not null) sealerL0Bottom.CutLength = glasses.fixed1.Length - glasses.fixed1.StepLength + cabin.Constraints.SealerL0LengthCorrection;
            }

            //Correct the Fixed Sealers Height
            if (cabin.Parts.GetPartOrNull(PartSpot.FixedGlass1SideSealer) is IWithCutLength sideSeal)
            {
                sideSeal.CutLength -= cabin.Parts.HorizontalProfileTop?.GlassInProfileDepth * 2 ?? 0;
            }

        }

        /// <summary>
        /// Passes the Cut Length of a profile to its PartsPerStructureSiblings
        /// </summary>
        /// <param name="profile">The Profile</param>
        /// <param name="structureIdentifier">The Identifier of the Cabin that this Profile is Part Of</param>
        private static void PassCutLengthToSiblings(this Profile profile , double lengthCorrection = 0)
        {
            foreach (Profile additionalPart in profile.AdditionalParts.OfType<Profile>())
            {
                additionalPart.CutLength = profile.CutLength + lengthCorrection;
            }
        }

        private static (Glass fixed1 , Glass fixed2 , Glass door1 , Glass door2) SetGlassPolycarbonicsLengths(Cabin cabin)
        {
            Glass fixed1 = cabin.Glasses.FirstOrDefault(g => g.GlassType == GlassTypeEnum.FixedGlass);
            Glass fixed2 = cabin.Glasses.Skip(1).FirstOrDefault(g => g.GlassType == GlassTypeEnum.FixedGlass);
            Glass door1 = cabin.Glasses.FirstOrDefault(g => g.GlassType is GlassTypeEnum.DoorGlass or GlassTypeEnum.DoorGlassSemicircle);
            Glass door2 = cabin.Glasses.Skip(1).FirstOrDefault(g => g.GlassType is GlassTypeEnum.DoorGlass or GlassTypeEnum.DoorGlassSemicircle);
            
            //First Fixed
            if (fixed1 is not null)
            {
                if (cabin.Parts.GetPartOrNull(PartSpot.FixedGlass1SideSealer) is IWithCutLength sideSealer)
                    sideSealer.CutLength = fixed1.Height;

                if (cabin.Parts.GetPartOrNull(PartSpot.FixedGlass1BottomSealer) is IWithCutLength bottomSealer)
                    bottomSealer.CutLength = fixed1.Length;
            }
            //Second Fixed
            if (fixed2 is not null)
            {
                if (cabin.Parts.GetPartOrNull(PartSpot.FixedGlass2SideSealer) is IWithCutLength sideSealer)
                    sideSealer.CutLength = fixed2.Height;

                if (cabin.Parts.GetPartOrNull(PartSpot.FixedGlass2BottomSealer) is IWithCutLength bottomSealer)
                    bottomSealer.CutLength = fixed2.Length;
            }

            //First Door
            if (door1 is not null)
            {
                if (cabin.Parts.GetPartOrNull(PartSpot.DoorGlass1SideSealer) is IWithCutLength sideSealer)
                    sideSealer.CutLength = door1.Height;

                if (cabin.Parts.GetPartOrNull(PartSpot.DoorGlass1SideSealerAlt) is IWithCutLength sideSealerAlt)
                    sideSealerAlt.CutLength = door1.Height;

                if (cabin.Parts.GetPartOrNull(PartSpot.DoorGlass1BottomSealer) is IWithCutLength bottomSealer)
                    bottomSealer.CutLength = door1.Length;
                if (cabin.Parts.GetPartOrNull(PartSpot.CloseStrip) is IWithCutLength closeStrip)
                    closeStrip.CutLength = door1.Height;
            }
            //SecondDoor
            if (door2 is not null)
            {
                if (cabin.Parts.GetPartOrNull(PartSpot.DoorGlass2SideSealer) is IWithCutLength sideSealer2)
                    sideSealer2.CutLength = door2.Height;

                if (cabin.Parts.GetPartOrNull(PartSpot.DoorGlass2SideSealerAlt) is IWithCutLength sideSealerAlt2)
                    sideSealerAlt2.CutLength = door2.Height;

                if (cabin.Parts.GetPartOrNull(PartSpot.DoorGlass2BottomSealer) is IWithCutLength bottomSealer2)
                    bottomSealer2.CutLength = door2.Length;
            }

            return (fixed1, fixed2, door1, door2);
        }

    }
}
