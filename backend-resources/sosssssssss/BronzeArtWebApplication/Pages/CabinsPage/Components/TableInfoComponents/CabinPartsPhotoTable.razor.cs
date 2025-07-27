using BronzeArtWebApplication.Shared.Helpers;
using CommonHelpers;
using Microsoft.AspNetCore.Components;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HandlesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.StripsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.SupportsModels;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.DBModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.HBModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.Inox304Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.NBModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.NPModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.WSSmartModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BronzeArtWebApplication.Pages.CabinsPage.Components.TableInfoComponents
{
    public partial class CabinPartsPhotoTable : ComponentBase
    {
        /// <summary>
        /// The Synthesis for which the Parts must be Shown
        /// </summary>
        [Parameter] public CabinSynthesis Synthesis { get; set; }

        /// <summary>
        /// The Style of the CabinParts Table
        /// </summary>
        [Parameter] public string Style { get; set; }

        private List<TableColumn> columns = new();

        protected override void OnParametersSet()
        {
            columns.Clear();
            if (Synthesis is null)
            {
                CreateEmptyColumns();
            }
            else
            {
                switch (Synthesis.DrawNo)
                {
                    case CabinDrawNumber.Draw9S:
                    case CabinDrawNumber.Draw9S9F:
                    case CabinDrawNumber.Draw9S9F9F:
                    case CabinDrawNumber.Draw94:
                    case CabinDrawNumber.Draw949F:
                    case CabinDrawNumber.Draw949F9F:
                    case CabinDrawNumber.Draw9A:
                    case CabinDrawNumber.Draw9A9F:
                    case CabinDrawNumber.Draw9C:
                    case CabinDrawNumber.Draw9C9F:
                    case CabinDrawNumber.Draw9B:
                    case CabinDrawNumber.Draw9B9F:
                    case CabinDrawNumber.Draw9B9F9F:
                        CreateB6000Columns();
                        break;
                    case CabinDrawNumber.DrawVS:
                    case CabinDrawNumber.DrawVSVF:
                    case CabinDrawNumber.DrawV4:
                    case CabinDrawNumber.DrawV4VF:
                    case CabinDrawNumber.DrawVA:
                        CreateInox304Columns();
                        break;
                    case CabinDrawNumber.DrawWS:
                        CreateWSColumns();
                        break;
                    case CabinDrawNumber.DrawNP44:
                    case CabinDrawNumber.DrawQP44:
                    case CabinDrawNumber.Draw2CornerNP46:
                    case CabinDrawNumber.Draw2CornerQP46:
                    case CabinDrawNumber.Draw2StraightNP48:
                    case CabinDrawNumber.Draw2StraightQP48:
                    case CabinDrawNumber.DrawCornerNP6W45:
                    case CabinDrawNumber.DrawCornerQP6W45:
                    case CabinDrawNumber.DrawStraightNP6W47:
                    case CabinDrawNumber.DrawStraightQP6W47:
                    case CabinDrawNumber.DrawNV2:
                    case CabinDrawNumber.DrawMV2:
                        CreateNPColumns();
                        break;
                    case CabinDrawNumber.DrawNB31:
                    case CabinDrawNumber.DrawQB31:
                    case CabinDrawNumber.DrawCornerNB6W32:
                    case CabinDrawNumber.DrawCornerQB6W32:
                    case CabinDrawNumber.Draw2CornerNB33:
                    case CabinDrawNumber.Draw2CornerQB33:
                    case CabinDrawNumber.DrawStraightNB6W38:
                    case CabinDrawNumber.DrawStraightQB6W38:
                    case CabinDrawNumber.Draw2StraightNB41:
                    case CabinDrawNumber.Draw2StraightQB41:
                    case CabinDrawNumber.DrawNV:
                        CreateNBColumns();
                        break;
                    case CabinDrawNumber.DrawDB51:
                    case CabinDrawNumber.DrawCornerDB8W52:
                    case CabinDrawNumber.Draw2CornerDB53:
                    case CabinDrawNumber.DrawStraightDB8W59:
                    case CabinDrawNumber.Draw2StraightDB61:
                        CreateDBPanelColumns();
                        break;
                    case CabinDrawNumber.DrawHB34:
                    case CabinDrawNumber.DrawCornerHB8W35:
                    case CabinDrawNumber.Draw2CornerHB37:
                    case CabinDrawNumber.DrawStraightHB8W40:
                    case CabinDrawNumber.Draw2StraightHB43:
                        CreateHBColumns();
                        break;
                    case CabinDrawNumber.Draw2Corner8W82:
                    case CabinDrawNumber.Draw8W:
                    case CabinDrawNumber.Draw1Corner8W84:
                    case CabinDrawNumber.Draw2Straight8W85:
                    case CabinDrawNumber.Draw2CornerStraight8W88:
                    case CabinDrawNumber.Draw8WFlipper81:
                    case CabinDrawNumber.Draw8W40:
                        CreateWPanelColumns();
                        break;
                    case CabinDrawNumber.DrawE:
                        CreateEPanelColumns();
                        break;
                    case CabinDrawNumber.None:
                    case CabinDrawNumber.Draw9F:
                    case CabinDrawNumber.DrawVF:
                    default:
                        CreateEmptyColumns();
                        break;
                }
            }

            base.OnParametersSet();
        }

        /// <summary>
        /// Empty Table 2 Columns
        /// </summary>
        private void CreateEmptyColumns()
        {
            columns.Clear();
            TableColumn column1 = new()
            {
                HeaderKey = "NotAvailable",
                ImgSrc = "n/a",
                ImgStyle = "max-height:50px"
            };
            columns.Add(column1);
            columns.Add(column1);
        }

        /// <summary>
        /// Creates the Columns for B6000 Models
        /// </summary>
        private void CreateB6000Columns()
        {
            #region 1.CommonColumns
            TableColumn column1 = new()
            {
                HeaderKey = "PerimetricalAluminium",
                ImgSrc = "../Images/CabinImages/Series/ImgBronze6000/Charachteristics/B6000Aluminium.png",
                ImgStyle = "max-height:160px"
            };
            var handle = Synthesis.Primary.Parts.GetPartOrNull<CabinHandle>(PartSpot.Handle1) ?? throw new InvalidOperationException("Cabin Has No Handle");
            TableColumn column2 = new()
            {
                HeaderKey = StaticInfoCabins.CabinHandleTypeDescKey[handle.HandleType],
                ImgSrc = handle.PhotoPath,
                ImgStyle = "max-height:160px"
            };
            TableColumn column3 = null;
            TableColumn column4 = null;
            #endregion

            #region 2.Wheels or Hinges Column
            if (Synthesis.Primary.Model is not CabinModelEnum.Model9B)
            {
                column3 = new()
                {
                    HeaderKey = "Wheels",
                    ImgSrc = "../Images/CabinImages/Series/ImgBronze6000/Charachteristics/B6000Wheel.png",
                    ImgStyle = "max-height:160px"
                };

            }
            else
            {
                column3 = new()
                {
                    HeaderKey = "Hinges",
                    ImgSrc = "../Images/CabinImages/Series/ImgBronze6000/Charachteristics/Hinge9B.png",
                    ImgStyle = "max-height:160px"
                };
            }
            #endregion

            #region 3.Closure Column
            if (Synthesis.Primary.Model is CabinModelEnum.Model9S or CabinModelEnum.Model9B)
            {
                column4 = new()
                {
                    HeaderKey = "HiddenMagnetClosure",
                    ImgSrc = "../Images/CabinImages/Series/ImgBronze6000/Charachteristics/9SMagnetClosure.jpg",
                    ImgStyle = "max-height:80px;"
                };
            }
            else if (Synthesis.Primary.Model is CabinModelEnum.Model94)
            {
                column4 = new()
                {
                    HeaderKey = "MagnetClosure",
                    ImgSrc = "../Images/CabinImages/Polycarbonics/StraightMagnet.jpg",
                    ImgStyle = "max-height:120px"
                };
            }
            else if (Synthesis.Primary.Model is CabinModelEnum.Model9A)
            {
                column4 = new()
                {
                    HeaderKey = "MagnetClosure",
                    ImgSrc = "../Images/CabinImages/Polycarbonics/CornerStrip.jpg",
                    ImgStyle = "max-height:100px"
                };
            }
            else if (Synthesis.Primary.Model is CabinModelEnum.Model9C)
            {
                column4 = new()
                {
                    HeaderKey = "MagnetClosure",
                    ImgSrc = "../Images/CabinImages/Series/ImgBronze6000/Charachteristics/9CMagnetClosure.jpg",
                    ImgStyle = "max-height:120px"
                };
            }
            #endregion

            columns.AddNotNull(column1);
            columns.AddNotNull(column2);
            columns.AddNotNull(column3);
            columns.AddNotNull(column4);
        }

        /// <summary>
        /// Creates the Columns for Inox304 Models
        /// </summary>
        private void CreateInox304Columns()
        {
            #region 1.CommonColumns
            TableColumn column1 = new()
            {
                HeaderKey = "HeavyDutyMechanism",
                ImgSrc = "../Images/CabinImages/Series/ImgInox304/Charachteristics/Inox304Mechanism.jpg",
                ImgStyle = "max-height:160px"
            };

            var handle = Synthesis.Primary.Parts.GetPartOrNull<CabinHandle>(PartSpot.Handle1) ?? throw new InvalidOperationException("Cabin Has No Handle");
            TableColumn column2 = new()
            {
                HeaderKey = StaticInfoCabins.CabinHandleTypeDescKey[handle.HandleType],
                ImgSrc = handle.PhotoPath,
                ImgStyle = "width:100px"
            };
            string headerKey3;
            string imgSrc3;
            string imgStyle3;
            if (Synthesis.Primary.Parts is IWallSideFixer fixer)
            {
                if (fixer.WallSideFixer is not null and Profile)
                {
                    headerKey3 = "AdjustmentProfile";
                    imgSrc3 = "../Images/CabinImages/Series/ImgFree/Charachteristics/WallProfile8W.png";
                    imgStyle3 = "max-height:160px;";
                }
                else if (fixer.WallSideFixer is not null and CabinSupport)
                {
                    headerKey3 = "Clamps";
                    imgSrc3 = "../Images/CabinImages/Series/ImgFree/Charachteristics/WallClamp.jpg";
                    imgStyle3 = "max-height:100px;";
                }
                else
                {
                    headerKey3 = "N/A";
                    imgSrc3 = "N/A";
                    imgStyle3 = "N/A";
                }
            }
            else
            {
                headerKey3 = "N/A";
                imgSrc3 = "N/A";
                imgStyle3 = "N/A";
            }
            TableColumn column3 = new()
            {
                HeaderKey = headerKey3,
                ImgSrc = imgSrc3,
                ImgStyle = imgStyle3
            };
            TableColumn column4 = null;
            #endregion

            #region 2.Closure Column
            if (Synthesis.Primary is CabinVS vs)
            {
                if (vs.Parts.CloseProfile is not null)
                {
                    column4 = new()
                    {
                        HeaderKey = StaticInfoCabins.GetCabinPartTypeDescKey(vs.Parts.CloseProfile.Part),
                        ImgSrc = vs.Parts.CloseProfile.PhotoPath,
                        ImgStyle = "max-height:160px;"
                    };
                }
                else if (vs.Parts.CloseStrip is not null)
                {
                    column4 = new()
                    {
                        HeaderKey = StaticInfoCabins.GetCabinPartTypeDescKey(vs.Parts.CloseStrip.Part),
                        ImgSrc = vs.Parts.CloseStrip.PhotoPath,
                        ImgStyle = "max-height:80px;"
                    };
                }
            }
            else if (Synthesis.Primary is CabinV4)
            {
                column4 = new()
                {
                    HeaderKey = "MagnetClosure",
                    ImgSrc = "../Images/CabinImages/Polycarbonics/StraightMagnet.jpg",
                    ImgStyle = "max-height:120px"
                };
            }
            else if (Synthesis.Primary is CabinVA)
            {
                column4 = new()
                {
                    HeaderKey = "MagnetClosure",
                    ImgSrc = "../Images/CabinImages/Polycarbonics/CornerStrip.jpg",
                    ImgStyle = "max-height:100px"
                };
            }

            #endregion

            columns.AddNotNull(column1);
            columns.AddNotNull(column2);
            columns.AddNotNull(column3);
            columns.AddNotNull(column4);
        }

        /// <summary>
        /// Creates the Columns for WPanel Models
        /// </summary>
        private void CreateWPanelColumns()
        {
            TableColumn column1 = null;
            TableColumn column2 = null;
            TableColumn column3 = null;
            TableColumn column4 = null;

            #region 1.Fixing Options
            if (Synthesis.Primary is CabinW || Synthesis.Secondary is CabinW)
            {
                //If its the Primary check only for Primary else check only for Secondary (Extra Panel for DB-HB-NP-NB e.t.c.)
                CabinW w = Synthesis.Primary is CabinW w1 ? w1 : (CabinW)Synthesis.Secondary;
                if (w.Parts.WallSideFixer is not null and Profile)
                {
                    column1 = new()
                    {
                        HeaderKey = "AdjustmentProfile",
                        ImgSrc = "../Images/CabinImages/Series/ImgFree/Charachteristics/WallProfile8W.png",
                        ImgStyle = "max-height:160px;"
                    };
                }
                else if (w.Parts.WallSideFixer is not null and CabinSupport)
                {
                    column1 = new()
                    {
                        HeaderKey = StaticInfoCabins.GetCabinPartTypeDescKey(w.Parts.WallSideFixer.Part),
                        ImgSrc = w.Parts.WallSideFixer.PhotoPath,
                        ImgStyle = "max-height:100px;"
                    };
                }
                if (w.Parts is IPerimetricalFixer fixer && fixer.HasPerimetricalFrame)
                {
                    column2 = new()
                    {
                        HeaderKey = "PerimetricalFrame",
                        ImgSrc = "https://storagebronze.blob.core.windows.net/cabins-images/Parts/Profiles/WFrame.jpg",
                        ImgStyle = "max-height:120px;"
                    };
                }
                else if (w.Parts.BottomFixer is not null and FloorStopperW)
                {
                    column2 = new()
                    {
                        HeaderKey = "FloorStopper",
                        ImgSrc = w.Parts.BottomFixer.PhotoPath,
                        ImgStyle = "max-height:120px;"
                    };
                }
                else if (w.Parts.BottomFixer is not null and Profile)
                {
                    column2 = new()
                    {
                        HeaderKey = "FloorAluminium",
                        ImgSrc = w.Parts.BottomFixer.PhotoPath,
                        ImgStyle = "max-height:160px;"
                    };
                }
            }
            #endregion

            #region 2.SupportBar
            column3 = new()
            {
                HeaderKey = "SupportBar",
                ImgSrc = "../Images/CabinImages/Series/ImgFree/Charachteristics/SupportBar.png",
                ImgStyle = "max-height:160px;"
            };
            #endregion

            #region 3.CombinationDraws 81-84-88
            if (Synthesis.Primary is CabinW wCombo && wCombo.IsPartOfDraw is CabinDrawNumber.Draw1Corner8W84 or CabinDrawNumber.Draw2CornerStraight8W88)
            {
                //When there are already supports do not show them again
                if ((wCombo.Parts.BottomFixer is FloorStopperW || wCombo.Parts.BottomFixer is Profile) && wCombo.Parts.WallSideFixer is not CabinSupport)
                {
                    column4 = new()
                    {
                        HeaderKey = "Clamps",
                        ImgSrc = "../Images/CabinImages/Series/ImgFree/Charachteristics/WallClamp.jpg",
                        ImgStyle = "max-height:100px;"
                    };
                }
            }
            if (Synthesis.Primary.IsPartOfDraw is CabinDrawNumber.Draw8WFlipper81)
            {
                column4 = new()
                {
                    HeaderKey = "Hinges",
                    ImgSrc = "../Images/CabinImages/Series/ImgFree/Charachteristics/FlipperHinge.jpg",
                    ImgStyle = "max-height:120px;"
                };
            }
            #endregion

            columns.AddNotNull(column1);
            columns.AddNotNull(column2);
            columns.AddNotNull(column3);
            columns.AddNotNull(column4);
        }

        /// <summary>
        /// Creates the Columns for EPanel Model
        /// </summary>
        private void CreateEPanelColumns()
        {
            TableColumn column1 = null;

            if (Synthesis.Primary is CabinE e)
            {
                if (e.Parts.BottomFixer is FloorStopperW)
                {
                    column1 = new()
                    {
                        HeaderKey = "FloorStopper",
                        ImgSrc = e.Parts.BottomFixer.PhotoPath,
                        ImgStyle = "max-height:120px;"
                    };
                }
                else if (e.Parts.BottomFixer is Profile)
                {
                    column1 = new()
                    {
                        HeaderKey = "FloorAluminium",
                        ImgSrc = e.Parts.BottomFixer.PhotoPath,
                        ImgStyle = "max-height:160px;"
                    };
                }

            }

            TableColumn column2 = new()
            {
                HeaderKey = "TwoSupportBars",
                ImgSrc = "../Images/CabinImages/Series/ImgFree/Charachteristics/SupportBar.png",
                ImgStyle = "max-height:160px;"
            };
            columns.AddNotNull(column1);
            columns.AddNotNull(column2);
        }

        /// <summary>
        /// Creates the Columns for DB Models
        /// </summary>
        private void CreateDBPanelColumns()
        {
            #region 1.Common Columns
            TableColumn column1 = new()
            {
                HeaderKey = "HeavyDutyHinges",
                ImgSrc = "../Images/CabinImages/Series/ImgDB/Charachteristics/Hinge.png",
                ImgStyle = "max-height:160px;"
            };

            var handle = Synthesis.Primary.Parts.GetPartOrNull<CabinHandle>(PartSpot.Handle1);
            TableColumn column2;
            if (handle != null)
            {
                column2 = new()
                {
                    HeaderKey = StaticInfoCabins.CabinHandleTypeDescKey[handle.HandleType],
                    ImgSrc = handle.PhotoPath,
                    ImgStyle = "max-height:160px;"
                };
            }
            else
            {
                //Empty handle
                column2 = new()
                {
                    HeaderKey = "Handle",
                    ImgSrc = "../Images/Various/UnavailableIcon.png",
                    ImgStyle = "max-height:100px;"
                };
            }

            TableColumn column3 = null;
            #endregion

            #region 2.Closure Options
            if (Synthesis.Primary is CabinDB db)
            {


                if (db.Parts.CloseProfile is not null)
                {
                    column3 = new()
                    {
                        HeaderKey = "MagnetClosure",
                        ImgSrc = db.Parts.CloseProfile.PhotoPath,
                        ImgStyle = "max-height:160px;"
                    };
                }
                else if (db.IsPartOfDraw is CabinDrawNumber.DrawCornerDB8W52 or CabinDrawNumber.Draw2CornerDB53)
                {
                    column3 = new()
                    {
                        HeaderKey = "MagnetClosure",
                        ImgSrc = "../Images/CabinImages/Polycarbonics/CornerStrip.jpg",
                        ImgStyle = "max-height:160px;"
                    };
                }
                else if (db.IsPartOfDraw is CabinDrawNumber.Draw2StraightDB61 or CabinDrawNumber.DrawStraightDB8W59)
                {
                    column3 = new()
                    {
                        HeaderKey = "MagnetClosure",
                        ImgSrc = "../Images/CabinImages/Polycarbonics/StraightMagnet.jpg",
                        ImgStyle = "max-height:160px;"
                    };
                }
                else if (db.Parts.CloseStrip is not null && db.Parts.CloseStrip.StripType is CabinStripType.PolycarbonicBumper)
                {
                    column3 = new()
                    {
                        HeaderKey = "BumperClosure",
                        ImgSrc = db.Parts.CloseStrip.PhotoPath,
                        ImgStyle = "max-height:80px;"
                    };
                }
            }
            #endregion

            columns.AddNotNull(column1);
            columns.AddNotNull(column2);
            columns.AddNotNull(column3);

            //Add any Columns also for Extra Panels
            if (Synthesis.GetCabinList().Any(c => c is CabinW))
            {
                CreateWPanelColumns();
            }
        }

        /// <summary>
        /// Create the Columns for NB Models
        /// </summary>
        private void CreateNBColumns()
        {
            var model = Synthesis.Primary.Model;
            #region 1.Common Columns
            TableColumn column1 = new()
            {
                HeaderKey = model is CabinModelEnum.ModelNB or CabinModelEnum.ModelNV
                ? "AluminiumHinge"
                : model is CabinModelEnum.ModelQB 
                    ? "AluminiumHingeMinimal"
                    : "Undefined",
                ImgSrc = model is CabinModelEnum.ModelNB or CabinModelEnum.ModelNV
                ? "../Images/CabinImages/Series/ImgNB/Charachteristics/NiagaraProfile.jpg"
                : model is CabinModelEnum.ModelQB
                    ? "https://storagebronze.blob.core.windows.net/bronzewebapp-images/Cabins/Models/QBQP/QProfile.jpg"
                    : string.Empty,
                ImgStyle = "max-height:140px;"
            };

            var handle = Synthesis.Primary.Parts.GetPartOrNull<CabinHandle>(PartSpot.Handle1);
            TableColumn column2;
            if (handle != null)
            {
                column2 = new()
                {
                    HeaderKey = StaticInfoCabins.CabinHandleTypeDescKey[handle.HandleType],
                    ImgSrc = handle.PhotoPath,
                    ImgStyle = "max-height:160px;"
                };
            }
            else
            {
                //Empty handle
                column2 = new()
                {
                    HeaderKey = "Handle",
                    ImgSrc = "../Images/Various/UnavailableIcon.png",
                    ImgStyle = "max-height:100px;"
                };
            }

            TableColumn column3 = null;
            #endregion

            #region 2.Closure Options
            if (Synthesis.Primary is CabinNB nb)
            {

                if (nb.Parts.CloseProfile is not null)
                {
                    column3 = new()
                    {
                        HeaderKey = "MagnetClosure",
                        ImgSrc = nb.Parts.CloseProfile.PhotoPath,
                        ImgStyle = "max-height:160px;"
                    };
                }
                else if (nb.IsPartOfDraw is CabinDrawNumber.DrawCornerNB6W32 
                                         or CabinDrawNumber.DrawCornerQB6W32
                                         or CabinDrawNumber.Draw2CornerNB33
                                         or CabinDrawNumber.Draw2CornerQB33)
                {
                    column3 = new()
                    {
                        HeaderKey = "MagnetClosure",
                        ImgSrc = "../Images/CabinImages/Polycarbonics/CornerStrip.jpg",
                        ImgStyle = "max-height:160px;"
                    };
                }
                else if (nb.IsPartOfDraw is CabinDrawNumber.Draw2StraightNB41
                                         or CabinDrawNumber.Draw2StraightQB41
                                         or CabinDrawNumber.DrawStraightNB6W38
                                         or CabinDrawNumber.DrawStraightQB6W38)
                {
                    column3 = new()
                    {
                        HeaderKey = "MagnetClosure",
                        ImgSrc = "../Images/CabinImages/Polycarbonics/StraightMagnet.jpg",
                        ImgStyle = "max-height:160px;"
                    };
                }
                else if (nb.Parts.CloseStrip is not null && nb.Parts.CloseStrip.StripType is CabinStripType.PolycarbonicBumper)
                {
                    column3 = new()
                    {
                        HeaderKey = "BumperClosure",
                        ImgSrc = nb.Parts.CloseStrip.PhotoPath,
                        ImgStyle = "max-height:80px;"
                    };
                }
            }
            #endregion

            columns.AddNotNull(column1);
            columns.AddNotNull(column2);
            columns.AddNotNull(column3);

            //Add any Columns also for Extra Panels
            if (Synthesis.GetCabinList().Any(c => c is CabinW))
            {
                CreateWPanelColumns();
            }
        }

        /// <summary>
        /// Create the Columns for NB Models
        /// </summary>
        private void CreateNPColumns()
        {
            var model = Synthesis.Primary.Model;

            #region 1.Common Columns
            TableColumn column1 = new()
            {
                HeaderKey = model is CabinModelEnum.ModelNP or CabinModelEnum.ModelMV2 or CabinModelEnum.ModelNV or CabinModelEnum.ModelNV2
                ? "AluminiumHinge"
                : model is CabinModelEnum.ModelQP
                    ? "AluminiumHingeMinimal"
                    : "Undefined",
                ImgSrc = model is CabinModelEnum.ModelNP or CabinModelEnum.ModelMV2 or CabinModelEnum.ModelNV or CabinModelEnum.ModelNV2
                ? "../Images/CabinImages/Series/ImgNB/Charachteristics/NiagaraProfile.jpg" 
                : model is CabinModelEnum.ModelQP 
                    ? "https://storagebronze.blob.core.windows.net/bronzewebapp-images/Cabins/Models/QBQP/QProfile.jpg"
                    : string.Empty,
                ImgStyle = "max-height:140px;"
            };

            var handle = Synthesis.Primary.Parts.GetPartOrNull<CabinHandle>(PartSpot.Handle1);
            TableColumn column2;
            if (handle != null)
            {
                column2 = new()
                {
                    HeaderKey = StaticInfoCabins.CabinHandleTypeDescKey[handle.HandleType],
                    ImgSrc = handle.PhotoPath,
                    ImgStyle = "max-height:160px;"
                };
            }
            else
            {
                //Empty handle
                column2 = new()
                {
                    HeaderKey = "Handle",
                    ImgSrc = "../Images/Various/UnavailableIcon.png",
                    ImgStyle = "max-height:100px;"
                };
            }

            TableColumn column3 = null;
            TableColumn column4 = null;
            //With Hinges Normal
            if (Synthesis.Primary?.IsPartOfDraw is CabinDrawNumber.DrawMV2)
            {
                column3 = new()
                {
                    HeaderKey = "InΒetweenHinges",
                    ImgSrc = "../Images/CabinImages/Series/ImgNVMV/Charachteristics/MVHinge1.jpg",
                    ImgStyle = "max-height:160px;"
                };
                column4 = new()
                {
                    HeaderKey = "InΒetweenHinges",
                    ImgSrc = "../Images/CabinImages/Series/ImgNVMV/Charachteristics/MVHinge2.jpg",
                    ImgStyle = "max-height:160px;"
                };
            }
            //With Aluminium Hinge
            else if (Synthesis.Primary?.IsPartOfDraw is CabinDrawNumber.DrawNV2)
            {
                column3 = new()
                {
                    HeaderKey = "InΒetweenHingeAluminium",
                    ImgSrc = "../Images/CabinImages/Series/ImgNVMV/Charachteristics/AluminiumBetweenHinge.jpg",
                    ImgStyle = "max-height:160px;"
                };
            }
            else //When not Bathtub Glass
            {
                column3 = new()
                {
                    HeaderKey = "InΒetweenHinges",
                    ImgSrc = "../Images/CabinImages/Series/ImgNP/Charachteristics/NPHinge.jpg",
                    ImgStyle = "max-height:160px;"
                };
            }
            TableColumn column5 = null;
            #endregion

            #region 2.Closure Options
            if (Synthesis.Primary is CabinNP np)
            {
                if (np.Parts.CloseProfile is not null)
                {
                    column5 = new()
                    {
                        HeaderKey = "MagnetClosure",
                        ImgSrc = np.Parts.CloseProfile.PhotoPath,
                        ImgStyle = "max-height:160px;"
                    };
                }
                else if (np.IsPartOfDraw is CabinDrawNumber.Draw2CornerNP46
                                         or CabinDrawNumber.Draw2CornerQP46
                                         or CabinDrawNumber.DrawCornerNP6W45
                                         or CabinDrawNumber.DrawCornerQP6W45)
                {
                    column5 = new()
                    {
                        HeaderKey = "MagnetClosure",
                        ImgSrc = "../Images/CabinImages/Polycarbonics/CornerStrip.jpg",
                        ImgStyle = "max-height:160px;"
                    };
                }
                else if (np.IsPartOfDraw is CabinDrawNumber.Draw2StraightNP48
                                         or CabinDrawNumber.Draw2StraightQP48
                                         or CabinDrawNumber.DrawStraightNP6W47
                                         or CabinDrawNumber.DrawStraightQP6W47)
                {
                    column5 = new()
                    {
                        HeaderKey = "MagnetClosure",
                        ImgSrc = "../Images/CabinImages/Polycarbonics/StraightMagnet.jpg",
                        ImgStyle = "max-height:160px;"
                    };
                }
                else if (np.Parts.CloseStrip is not null && np.Parts.CloseStrip.StripType is CabinStripType.PolycarbonicBumper)
                {
                    column5 = new()
                    {
                        HeaderKey = "BumperClosure",
                        ImgSrc = np.Parts.CloseStrip.PhotoPath,
                        ImgStyle = "max-height:80px;"
                    };
                }
            }
            #endregion

            columns.AddNotNull(column1);
            columns.AddNotNull(column2);
            columns.AddNotNull(column3);
            columns.AddNotNull(column4);
            columns.AddNotNull(column5);

            //Add any Columns also for Extra Panels
            if (Synthesis.GetCabinList().Any(c => c is CabinW))
            {
                CreateWPanelColumns();
            }
        }

        /// <summary>
        /// Create the Columns for HB Models
        /// </summary>
        private void CreateHBColumns()
        {
            var hb = Synthesis.Primary as CabinHB ?? throw new InvalidOperationException($"Cabin is not of type {nameof(CabinHB)}");

            #region 1.Common Columns
            TableColumn column1 = new()
            {
                HeaderKey = "HeavyDutyHinges",
                ImgSrc = "../Images/CabinImages/Series/ImgHB/Charachteristics/GlassToGlassHinge.jpg",
                ImgStyle = "max-height:140px;"
            };
            TableColumn column2;

            if (hb.Parts.WallSideFixer is Profile)
            {
                column2 = new()
                {
                    HeaderKey = "AdjustmentProfile",
                    ImgSrc = hb.Parts.WallSideFixer.PhotoPath,
                    ImgStyle = "max-height:160px;"
                };
            }
            else if (hb.Parts.WallSideFixer is CabinSupport)
            {
                column2 = new()
                {
                    HeaderKey = StaticInfoCabins.GetCabinPartTypeDescKey(hb.Parts.WallSideFixer.Part),
                    ImgSrc = hb.Parts.WallSideFixer.PhotoPath,
                    ImgStyle = "max-height:160px;"
                };
            }
            else { throw new InvalidOperationException("Cabin Wall Side Fixer is Not of the Correct Type"); }

            var handle = Synthesis.Primary.Parts.GetPartOrNull<CabinHandle>(PartSpot.Handle1);
            TableColumn column3;
            if (handle != null)
            {
                column3 = new()
                {
                    HeaderKey = StaticInfoCabins.CabinHandleTypeDescKey[handle.HandleType],
                    ImgSrc = handle.PhotoPath,
                    ImgStyle = "max-height:160px;"
                };
            }
            else
            {
                //Empty handle
                column3 = new()
                {
                    HeaderKey = "Handle",
                    ImgSrc = "../Images/Various/UnavailableIcon.png",
                    ImgStyle = "max-height:100px;"
                };
            }

            TableColumn column4;

            if (hb.Parts.BottomFixer is FloorStopperW)
            {
                column4 = new()
                {
                    HeaderKey = "FloorStopper",
                    ImgSrc = "../Images/CabinImages/Series/ImgFree/Charachteristics/StopperSketch.png",
                    ImgStyle = "max-height:120px;"
                };
            }
            else if (hb.Parts.BottomFixer is Profile)
            {
                column4 = new()
                {
                    HeaderKey = StaticInfoCabins.GetCabinPartTypeDescKey(hb.Parts.BottomFixer.Part),
                    ImgSrc = hb.Parts.BottomFixer.PhotoPath,
                    ImgStyle = "max-height:120px;"
                };
            }
            else { throw new InvalidOperationException("Cabin has BottomFixer of Invalid Type"); }


            TableColumn column5 = new()
            {
                HeaderKey = "SupportBar",
                ImgSrc = "../Images/CabinImages/Series/ImgFree/Charachteristics/SupportBar.png",
                ImgStyle = "max-height:160px;"
            };

            TableColumn column6 = null;
            #endregion

            #region 2.Closure Options
            if (hb.Parts.CloseProfile is not null)
            {
                column6 = new()
                {
                    HeaderKey = "MagnetClosure",
                    ImgSrc = hb.Parts.CloseProfile.PhotoPath,
                    ImgStyle = "max-height:160px;"
                };
            }
            else if (hb.IsPartOfDraw is CabinDrawNumber.DrawCornerHB8W35 or CabinDrawNumber.Draw2CornerHB37)
            {
                column6 = new()
                {
                    HeaderKey = "MagnetClosure",
                    ImgSrc = "../Images/CabinImages/Polycarbonics/CornerStrip.jpg",
                    ImgStyle = "max-height:160px;"
                };
            }
            else if (hb.IsPartOfDraw is CabinDrawNumber.Draw2StraightHB43 or CabinDrawNumber.DrawStraightHB8W40)
            {
                column6 = new()
                {
                    HeaderKey = "MagnetClosure",
                    ImgSrc = "../Images/CabinImages/Polycarbonics/StraightMagnet.jpg",
                    ImgStyle = "max-height:160px;"
                };
            }
            else if (hb.Parts.CloseStrip is not null && hb.Parts.CloseStrip.StripType is CabinStripType.PolycarbonicBumper)
            {
                column6 = new()
                {
                    HeaderKey = "BumperClosure",
                    ImgSrc = hb.Parts.CloseStrip.PhotoPath,
                    ImgStyle = "max-height:80px;"
                };
            }
            #endregion

            columns.AddNotNull(column1);
            columns.AddNotNull(column2);
            columns.AddNotNull(column3);
            columns.AddNotNull(column4);
            columns.AddNotNull(column5);
            columns.AddNotNull(column6);
        }

        /// <summary>
        /// Create the Columns for WS Models
        /// </summary>
        private void CreateWSColumns()
        {
            #region 1.Common Columns
            TableColumn column1 = new()
            {
                HeaderKey = "HeavyDutyMechanism",
                ImgSrc = "../Images/CabinImages/Series/ImgSmart/Charachteristics/WSMechanism.jpg",
                ImgStyle = "max-height:160px;"
            };
            TableColumn column2 = new()
            {
                HeaderKey = "HeavyDutyProfile",
                ImgSrc = "../Images/CabinImages/Series/ImgSmart/Charachteristics/WSWallProfile.jpg",
                ImgStyle = "max-height:160px;"
            };

            var handle = Synthesis.Primary.Parts.GetPartOrNull<CabinHandle>(PartSpot.Handle1) ?? throw new InvalidOperationException($"{nameof(CabinWS)} has no Handle");
            TableColumn column3 = new()
            {
                HeaderKey = StaticInfoCabins.CabinHandleTypeDescKey[handle.HandleType],
                ImgSrc = handle.PhotoPath,
                ImgStyle = "width:100px"
            };
            TableColumn column4 = new()
            {
                HeaderKey = "SupportBarHeavyDuty",
                ImgSrc = "../Images/CabinImages/Series/ImgSmart/Charachteristics/SupportWS.jpg",
                ImgStyle = "max-height:160px;"
            };
            TableColumn column5 = null;
            #endregion

            if (Synthesis.Primary is CabinWS ws)
            {
                if (ws.Parts.CloseStrip is not null && ws.Parts.CloseStrip.StripType is CabinStripType.PolycarbonicBumper)
                {
                    column5 = new()
                    {
                        HeaderKey = "BumperClosure",
                        ImgSrc = ws.Parts.CloseStrip.PhotoPath,
                        ImgStyle = "max-height:80px;"
                    };
                }
                else if (ws.Parts.CloseProfile is not null)
                {
                    column5 = new()
                    {
                        HeaderKey = "MagnetClosure",
                        ImgSrc = ws.Parts.CloseProfile.PhotoPath,
                        ImgStyle = "max-height:160px;"
                    };
                }
            }
            columns.AddNotNull(column1);
            columns.AddNotNull(column2);
            columns.AddNotNull(column3);
            columns.AddNotNull(column4);
            columns.AddNotNull(column5);
        }

    }
    class TableColumn
    {
        public string HeaderKey { get; set; }
        public string ImgSrc { get; set; }
        public string ImgStyle { get; set; }
    }
}
