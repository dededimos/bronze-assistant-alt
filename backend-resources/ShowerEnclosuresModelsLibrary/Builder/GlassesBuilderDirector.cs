using ShowerEnclosuresModelsLibrary.Builder.GlassesConcreteBuilders.B6000Glasses;
using ShowerEnclosuresModelsLibrary.Builder.GlassesConcreteBuilders.DBGlasses;
using ShowerEnclosuresModelsLibrary.Builder.GlassesConcreteBuilders.HBGlasses;
using ShowerEnclosuresModelsLibrary.Builder.GlassesConcreteBuilders.Inox304Glasses;
using ShowerEnclosuresModelsLibrary.Builder.GlassesConcreteBuilders.NBGlasses;
using ShowerEnclosuresModelsLibrary.Builder.GlassesConcreteBuilders.NPGlasses;
using ShowerEnclosuresModelsLibrary.Builder.GlassesConcreteBuilders.FreeGlasses;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Factory;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.DBModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.HBModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.Inox304Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.NBModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.NPModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShowerEnclosuresModelsLibrary.Builder.GlassesConcreteBuilders.WSGlasses;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.WSSmartModels;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces;
using ShowerEnclosuresModelsLibrary.Models.RepositoryModels.Interfaces;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.ConstraintsModels;
using ShowerEnclosuresModelsLibrary.Builder.BuilderExceptions;

namespace ShowerEnclosuresModelsLibrary.Builder
{
    public class GlassesBuilderDirector
    {
        #region NOTES : Glasses Equations
        /* Similar Model Builders can be refactored to the Below Equations when we have time
        Single Slider : 
        Deriving from Equations System : (FOR Corner models 'A' substitute Wall2 with AngleDistanceFromDoor) 
        (1) LengthMin = Wall1 + Wall2 + Fixed + Sliding - Overlap + MagnetStrip - ALST
        (2) Fixed - ALST - StepLength + CoverDistance = Sliding - HandleDistance

        Sliding Glass = (LengthMin -Wall1 - Wall2 - StepLength + CoverDistance + HandleDistance + Overlap - MagnetStrip)/2
        Fixed = (LengthMin -Wall1 - Wall2 + Step - CoverDistance - HandleDistance + Overlap - MagnetStrip ) /2 + ALST

        Double Slider :
        Deriving From Equations : 
        (1) Sliding 1 = Sliding 2
        (2) Fixed1 = Fixed1' + StepLength1
        (3) Fixed2 = Fixed2' + StepLength2
        (4) LengthMin - StepLength1 - StepLength2 = Wall1 + Wall2 + Fixed1' + Fixed2' + Sliding1 + Sliding2 + 2*MagnetStrip - ALST1 - ALST2 - 2*Overlap
        (5) Fixed1' + CoverDistance - ALST1 = Sliding1 - HandleDistance
        (6) Fixed2' + CoverDistance -ALST2 = Sliding2 - HandleDistance

        Slider = (LengthMin - StepLength1 - StepLength2 -Wall1 - Wall2 + 2*CoverDistance + 2*HandleDistance - 2*MagnetStrip + 2*Overlap) / 4
        Fixed 1 = (LengthMin + 3*StepLength1 - StepLength2 -Wall1 - Wall2 - 2*HandleDistance - 2*CoverDistance - 2*MagnetStrip + 2*Overlap) / 4 + ALST1
        Fixed 2 = (LengthMin - StepLength1 + 3*StepLength2 -Wall1 - Wall2 - 2*HandleDistance - 2*CoverDistance - 2*MagnetStrip + 2*Overlap) / 4 + ALST2
        */
        #endregion

        private IGlassBuilder builder;

        /// <summary>
        /// Creates a Glasses Builder Director Responsible , for all built Glasses
        /// </summary>
        /// <param name="r">The Repository Holding all Relevant Options</param>
        public GlassesBuilderDirector()
        {
            
        }

        /// <summary>
        /// Builds ALL Glasses of the inserted cabin (Instantates the Correct Builder for the Passed Cabin Argument)
        /// </summary>
        /// <param name="cabin"></param>
        public void BuildAllGlasses(Cabin cabin)
        {
            switch (cabin.Model)
            {
                case CabinModelEnum.Model9A:
                    cabin.Glasses.Add(Build9AFixedGlass(cabin));
                    cabin.Glasses.Add(Build9ADoorGlass(cabin));
                    break;

                case CabinModelEnum.Model9S:
                    cabin.Glasses.Add(Build9SFixedGlass(cabin));
                    cabin.Glasses.Add(Build9SDoorGlass(cabin));
                    break;

                case CabinModelEnum.Model94:
                    cabin.Glasses.Add(Build94FixedGlass(cabin, true)); //Builds the normal Glass (Cannot take Step)
                    cabin.Glasses.Add(Build94FixedGlass(cabin, false));  //Either build the glass with Step or the Same as above
                    cabin.Glasses.Add(Build94DoorGlass(cabin));
                    cabin.Glasses.Add(Build94DoorGlass(cabin));
                    break;

                case CabinModelEnum.Model9F:
                    cabin.Glasses.Add(Build9FFixedGlass(cabin));
                    break;

                case CabinModelEnum.Model9B:
                    if (cabin.LengthMin > ((Constraints9B)cabin.Constraints).AddedFixedGlassLengthBreakpoint)
                    {//Build Fixed ONLY when the breakpoint is Broken (After a certain length)
                        cabin.Glasses.Add(Build9BFixedGlass(cabin));
                    }
                    cabin.Glasses.Add(Build9BDoorGlass(cabin));
                    break;

                case CabinModelEnum.ModelW:
                case CabinModelEnum.Model8W40:
                    cabin.Glasses.Add(BuildWFixedGlass(cabin));
                    break;

                case CabinModelEnum.ModelHB:
                    cabin.Glasses.Add(BuildHBFixedGlass(cabin));
                    cabin.Glasses.Add(BuildHBDoorGlass(cabin));
                    break;

                case CabinModelEnum.ModelNP:
                case CabinModelEnum.ModelQP:
                case CabinModelEnum.ModelMV2:
                case CabinModelEnum.ModelNV2:
                    cabin.Glasses.Add(BuildNPDoorGlass(cabin, true));  //DP3
                    cabin.Glasses.Add(BuildNPDoorGlass(cabin, false)); //DP1
                    break;

                case CabinModelEnum.ModelVS:
                    cabin.Glasses.Add(BuildVSFixedGlass(cabin));
                    cabin.Glasses.Add(BuildVSDoorGlass(cabin));
                    break;

                case CabinModelEnum.ModelVF:
                    cabin.Glasses.Add(BuildVFFixedGlass(cabin));
                    break;

                case CabinModelEnum.ModelV4:
                    cabin.Glasses.Add(BuildV4FixedGlass(cabin, true));
                    cabin.Glasses.Add(BuildV4FixedGlass(cabin, false));
                    cabin.Glasses.Add(BuildV4DoorGlass(cabin));
                    cabin.Glasses.Add(BuildV4DoorGlass(cabin));
                    break;

                case CabinModelEnum.ModelVA:
                    cabin.Glasses.Add(BuildVAFixedGlass(cabin));
                    cabin.Glasses.Add(BuildVADoorGlass(cabin));
                    break;

                case CabinModelEnum.ModelE:
                    cabin.Glasses.Add(BuildEFixedGlass(cabin));
                    break;

                case CabinModelEnum.ModelWFlipper:
                    cabin.Glasses.Add(BuildWFlipperGlass(cabin));
                    break;

                case CabinModelEnum.ModelDB:
                    cabin.Glasses.Add(BuildDBDoorGlass(cabin));
                    break;

                case CabinModelEnum.ModelNB:
                case CabinModelEnum.ModelQB:
                case CabinModelEnum.ModelNV:
                    cabin.Glasses.Add(BuildNBDoorGlass(cabin));
                    break;
                case CabinModelEnum.Model6WA:
                    break;
                case CabinModelEnum.Model9C:
                    cabin.Glasses.Add(Build9CFixedGlass(cabin));
                    cabin.Glasses.Add(Build9CFixedGlass(cabin));
                    cabin.Glasses.Add(Build9CDoorGlass(cabin));
                    cabin.Glasses.Add(Build9CDoorGlass(cabin));
                    break;
                case CabinModelEnum.ModelWS:
                    cabin.Glasses.Add(BuildWSFixedGlass(cabin));
                    cabin.Glasses.Add(BuildWSDoorGlass(cabin));
                    break;
                default:
                    break;
            }
        }

        //All Glasses Methods

        #region 1. 9S Glasses
        private Glass Build9SFixedGlass(Cabin cabin)
        {
            builder = new FixedGlass9SBuilder((Cabin9S)cabin);
            return builder.BuildGlass();
        }
        private Glass Build9SDoorGlass(Cabin cabin)
        {
            builder = new DoorGlass9SBuilder((Cabin9S)cabin);
            return builder.BuildGlass();
        }

        #endregion

        #region 2. 9A Glasses
        private Glass Build9ADoorGlass(Cabin cabin)
        {
            builder = new DoorGlass9ABuilder((Cabin9A)cabin);
            return builder.BuildGlass();
        }
        private Glass Build9AFixedGlass(Cabin cabin)
        {
            builder = new FixedGlass9ABuilder((Cabin9A)cabin);
            return builder.BuildGlass();
        }
        #endregion

        #region 3. 94 Glasses
        private Glass Build94DoorGlass(Cabin cabin)
        {
            builder = new DoorGlass94Builder((Cabin94)cabin);
            return builder.BuildGlass();
        }

        /// <summary>
        /// Build 94 Fixed Glass. 
        /// </summary>
        /// <param name="cabin"></param>
        /// <param name="canHaveStep">True means The Glass that can be Cut with Step, False is the other one</param>
        /// <returns></returns>
        private Glass Build94FixedGlass(Cabin cabin,bool canHaveStep = false)
        {
            builder = new FixedGlass94Builder((Cabin94)cabin,canHaveStep);
            return builder.BuildGlass();
        }
        #endregion

        #region 4. 9B Glasses

        private Glass Build9BDoorGlass(Cabin cabin)
        {
            builder = new DoorGlass9BBuilder((Cabin9B)cabin);
            return builder.BuildGlass();
        }

        private Glass Build9BFixedGlass(Cabin cabin)
        {
            builder = new FixedGlass9BBuilder((Cabin9B)cabin);
            return builder.BuildGlass();
        }


        #endregion

        #region 5. 9F Glasses

        private Glass Build9FFixedGlass(Cabin cabin) 
        {
            builder = new FixedGlass9FBuilder((Cabin9F)cabin);
            return builder.BuildGlass();
        }

        #endregion

        #region 6. DB Glasses
        private Glass BuildDBDoorGlass(Cabin cabin)
        {
            builder = new DoorGlassDBBuilder((CabinDB)cabin);
            return builder.BuildGlass();
        }

        #endregion

        #region 7. Free Glasses
        private Glass BuildWFixedGlass(Cabin cabin)
        {
            builder = new FixedGlassWBuilder((CabinW)cabin);
            return builder.BuildGlass();
        }
        private Glass BuildEFixedGlass(Cabin cabin)
        {
            builder = new FixedGlassEBuilder((CabinE)cabin);
            return builder.BuildGlass();
        }
        private Glass BuildWFlipperGlass(Cabin cabin)
        {
            builder = new DoorGlassWFlipperBuilder((CabinWFlipper)cabin);
            return builder.BuildGlass();
        }
        #endregion

        #region 8. HB Glasses
        private Glass BuildHBDoorGlass(Cabin cabin)
        {
            builder = new DoorGlassHBBuilder((CabinHB)cabin);
            return builder.BuildGlass();
        }
        private Glass BuildHBFixedGlass(Cabin cabin)
        {
            builder = new FixedGlassHBBuilder((CabinHB)cabin);
            return builder.BuildGlass();
        }
        #endregion

        #region 9. VS Glasses
        private Glass BuildVSDoorGlass(Cabin cabin)
        {
            builder = new DoorGlassVSBuilder((CabinVS)cabin);
            return builder.BuildGlass();
        }
        private Glass BuildVSFixedGlass(Cabin cabin)
        {
            builder = new FixedGlassVSBuilder((CabinVS)cabin);
            return builder.BuildGlass();
        }
        #endregion

        #region 10. V4 Glasses
        private Glass BuildV4FixedGlass(Cabin cabin , bool canHaveStep)
        {
            builder = new FixedGlassV4Builder((CabinV4)cabin,canHaveStep);
            return builder.BuildGlass();
        }
        private Glass BuildV4DoorGlass(Cabin cabin)
        {
            builder = new DoorGlassV4Builder((CabinV4)cabin);
            return builder.BuildGlass();
        }
        #endregion

        #region 11. VF Glasses
        private Glass BuildVFFixedGlass(Cabin cabin)
        {
            builder = new FixedGlassVFBuilder((CabinVF)cabin);
            return builder.BuildGlass();
        }
        #endregion

        #region 12.VA Glasses
        private Glass BuildVADoorGlass(Cabin cabin)
        {
            builder = new DoorGlassVABuilder((CabinVA)cabin);
            return builder.BuildGlass();
        }
        private Glass BuildVAFixedGlass(Cabin cabin)
        {
            builder = new FixedGlassVABuilder((CabinVA)cabin);
            return builder.BuildGlass();
        }
        #endregion

        #region 13. NB Glasses
        private Glass BuildNBDoorGlass(Cabin cabin)
        {
            builder = new DoorGlassNBBuilder((CabinNB)cabin);
            return builder.BuildGlass();
        }
        #endregion

        #region 14. NP Glasses
        private Glass BuildNPDoorGlass(Cabin cabin,bool isWallGlass)
        {
            builder = new DoorGlassNPBuilder((CabinNP)cabin,isWallGlass);
            return builder.BuildGlass();
        }
        #endregion

        #region 15.WS Glasses
        private Glass BuildWSDoorGlass(Cabin cabin)
        {
            builder = new DoorGlassWSBuilder((CabinWS)cabin);
            return builder.BuildGlass();
        }
        private Glass BuildWSFixedGlass(Cabin cabin)
        {
            builder = new FixedGlassWSBuilder((CabinWS)cabin);
            return builder.BuildGlass();
        }
        #endregion

        #region 16. 9C Glasses
        private Glass Build9CFixedGlass(Cabin cabin)
        {
            builder = new FixedGlass9CBuilder((Cabin9C)cabin);
            return builder.BuildGlass();
        }
        private Glass Build9CDoorGlass(Cabin cabin)
        {
            builder = new DoorGlass9CBuilder((Cabin9C)cabin);
            return builder.BuildGlass();
        }
        #endregion


    }
}
