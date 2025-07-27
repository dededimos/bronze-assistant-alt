using BronzeArtWebApplication.Shared.ViewModels;
using BronzeRulesPricelistLibrary.Models;
using Microsoft.AspNetCore.Components;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HandlesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.StripsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;

namespace BronzeArtWebApplication.Pages.CabinsPage.Components.CabinPropertiesPanels
{
    public partial class CabinExtraOptionsPanel : ComponentBase, IDisposable
    {
        protected override void OnInitialized()
        {
            vm.PropertyChanged += Vm_PropertyChanged;
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(AssembleCabinViewModel.PrimaryCabin))
            {
                StateHasChanged();
            }
        }

        private bool? hasHandle { get => (vm.PrimaryCabin?.HasHandleOption is true) || (vm.SecondaryCabin?.HasHandleOption is true) || (vm.TertiaryCabin?.HasHandleOption is true); }
        private bool isHandlesOpen;

        private bool? hasBottomFixer { get => (vm.PrimaryCabin?.HasBottomFixerOption is true) || (vm.SecondaryCabin?.HasBottomFixerOption is true) || (vm.TertiaryCabin?.HasBottomFixerOption is true); }
        private bool isBottomFixersOpen;

        private bool hasRotationMechanism { get => isNBModel || isQBModel || isNPModel || isQPModel; }
        private bool isRotationMechanismOpen;

        private bool? hasMiddleHinge { get => (vm.PrimaryCabin?.HasMiddleHingeOption is true) || (vm.SecondaryCabin?.HasMiddleHingeOption is true) || (vm.TertiaryCabin?.HasMiddleHingeOption is true); }
        private bool isMiddleHingeOpen;

        private bool? hasCloseProfile { get=> vm.PrimaryCabin?.HasCloseProfileOption is true; }
        private bool isCloseProfileOpen;

        private bool? hasWallFixing { get => (vm.PrimaryCabin?.HasWallFixingOption is true) || (vm.SecondaryCabin?.HasWallFixingOption is true) || (vm.TertiaryCabin?.HasWallFixingOption is true); }
        private bool isWallFixingOpen;

        private bool isNBModel { get => vm.PrimaryCabin?.Model == CabinModelEnum.ModelNB; }
        private bool isQBModel { get => vm.PrimaryCabin?.Model == CabinModelEnum.ModelQB; }
        private bool isNPModel { get => vm.PrimaryCabin?.Model == CabinModelEnum.ModelNP; }
        private bool isQPModel { get => vm.PrimaryCabin?.Model == CabinModelEnum.ModelQP; }


        #region 1.Handle Option

        /// <summary>
        /// Passes Selected Handle to All Models of the Synthesis
        /// </summary>
        /// <param name="handle"></param>
        private void OnHandleSelection(CabinHandle handle)
        {
            if (vm.PrimaryCabin?.HasHandleOption == true)
            {
                if (vm.PrimaryCabin.ShouldHaveHandle && handle is null)
                {
                    vm.PrimaryCabin.HandleOption = repo.GetHandle(vm.PrimaryCabin.DefaultHandle).GetDeepClone() as CabinHandle;
                }
                else
                {
                    vm.PrimaryCabin.HandleOption = handle?.GetDeepClone() as CabinHandle;
                }
            }
            if (vm.SecondaryCabin?.HasHandleOption == true)
            {
                if (vm.SecondaryCabin.ShouldHaveHandle && handle is null)
                {
                    vm.SecondaryCabin.HandleOption = repo.GetHandle(vm.SecondaryCabin.DefaultHandle).GetDeepClone() as CabinHandle;
                }
                else
                {
                    vm.SecondaryCabin.HandleOption = handle?.GetDeepClone() as CabinHandle;
                }
            }
            if (vm.TertiaryCabin?.HasHandleOption == true)
            {
                if (vm.TertiaryCabin.ShouldHaveHandle && handle is null)
                {
                    vm.TertiaryCabin.HandleOption = repo.GetHandle(vm.TertiaryCabin.DefaultHandle).GetDeepClone() as CabinHandle;
                }
                else
                {
                    vm.TertiaryCabin.HandleOption = handle?.GetDeepClone() as CabinHandle;
                }
            }
        }
        /// <summary>
        /// Sets teh Selected Handle to the First it Finds in one of the Models starting from the Primary and Iterating through to Tertiary
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private CabinHandle InitialSelectedHandle()
        {
            if (vm.PrimaryCabin?.HasHandleOption == true)
            {
                return vm.PrimaryCabin.HandleOption;
            }
            else if (vm.SecondaryCabin?.HasHandleOption == true)
            {
                return vm.SecondaryCabin.HandleOption;
            }
            else if (vm.TertiaryCabin?.HasHandleOption == true)
            {
                return vm.TertiaryCabin.HandleOption;
            }
            else
            {
                throw new InvalidOperationException("There is no HandleOption for this Synthesis");
            }
        }
        /// <summary>
        /// Gets the Handles Selection List available in the First Model that Has Handle starting from The Primary
        /// </summary>
        /// <returns></returns>
        private IEnumerable<CabinHandle> GetHandlesSelectionList()
        {
            if (vm.PrimaryCabin?.HasHandleOption is true)
            {
                return repo.GetAllParts().Where(h => vm.PrimaryCabin.SelectableHandles.Contains(h.Code)).Select(h=>h as CabinHandle);
            }
            else if (vm.SecondaryCabin?.HasHandleOption is true)
            {
                return repo.GetAllParts().Where(h => vm.SecondaryCabin.SelectableHandles.Contains(h.Code)).Select(h => h as CabinHandle);
            }
            else if (vm.TertiaryCabin?.HasHandleOption is true)
            {
                return repo.GetAllParts().Where(h => vm.TertiaryCabin.SelectableHandles.Contains(h.Code)).Select(h => h as CabinHandle);
            }
            return new List<CabinHandle>();
        }

        #endregion

        #region 2.BottomFixer Option
        /// <summary>
        /// Sets the Selected Bottom Fixer to All the Models that Have one
        /// </summary>
        /// <param name="part"></param>
        private void OnBottomFixerSelection(CabinPart part)
        {
            if (vm.PrimaryCabin?.HasBottomFixerOption is true)
            {
                if (part is null)
                {
                    vm.PrimaryCabin.BottomFixerOption = repo.GetPart(vm.PrimaryCabin.DefaultBottomFixer).GetDeepClone();
                }
                else
                {
                    vm.PrimaryCabin.BottomFixerOption = part.GetDeepClone();
                }
            }
            
            if (vm.SecondaryCabin?.HasBottomFixerOption is true)
            {
                if (part is null)
                {
                    vm.SecondaryCabin.BottomFixerOption = repo.GetPart(vm.SecondaryCabin.DefaultBottomFixer).GetDeepClone();
                }
                else
                {
                    vm.SecondaryCabin.BottomFixerOption = part.GetDeepClone();
                }
            }
            
            if (vm.TertiaryCabin?.HasBottomFixerOption is true)
            {
                if (part is null)
                {
                    vm.TertiaryCabin.BottomFixerOption = repo.GetPart(vm.TertiaryCabin.DefaultBottomFixer).GetDeepClone();
                }
                else
                {
                    vm.TertiaryCabin.BottomFixerOption = part.GetDeepClone();
                }
            }
        }
        /// <summary>
        /// Sets the initially Selected Botttom Fixer to the First It Finds starting From Primary Model
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private CabinPart InitialSelectedBottomFixer()
        {
            if (vm.PrimaryCabin?.HasBottomFixerOption == true)
            {
                return vm.PrimaryCabin.BottomFixerOption;
            }
            else if (vm.SecondaryCabin?.HasBottomFixerOption == true)
            {
                return vm.SecondaryCabin.BottomFixerOption;
            }
            else if (vm.TertiaryCabin?.HasBottomFixerOption == true)
            {
                return vm.TertiaryCabin.BottomFixerOption;
            }
            else
            {
                throw new InvalidOperationException("There is no BottomFixer Option for this Synthesis");
            }
        }
        /// <summary>
        /// Gets the Available Bottom Fixers Selection for the Current Synthesis (Gets the Selection from the First Model it has A Fixer starting from Primary)
        /// </summary>
        /// <returns></returns>
        private IEnumerable<CabinPart> GetBottomFixerSelectionList()
        {
            // HACK Do not Include 8WAL-10-WALL NEED TO FIX THIS 8WAL is only selectable for Perimetrical Frame!!!!
            if (vm.PrimaryCabin?.HasBottomFixerOption is true)
            {
                return repo.GetAllParts().Where(p => vm.PrimaryCabin.SelectableBottomFixers.Contains(p.Code) && p.Code != "8WAL-10-WALL");
            }
            else if (vm.SecondaryCabin?.HasBottomFixerOption is true)
            {
                return repo.GetAllParts().Where(p => vm.SecondaryCabin.SelectableBottomFixers.Contains(p.Code) && p.Code != "8WAL-10-WALL");
            }
            else if (vm.TertiaryCabin?.HasBottomFixerOption is true)
            {
                return repo.GetAllParts().Where(p => vm.TertiaryCabin.SelectableBottomFixers.Contains(p.Code) && p.Code != "8WAL-10-WALL");
            }
            return new List<CabinPart>();
        }
        #endregion

        #region 3. MiddleHinge Option

        private void OnMiddleHingeSelection(CabinPart midHinge)
        {
            if (vm.PrimaryCabin?.HasMiddleHingeOption is true)
            {
                if (midHinge is null)
                {
                    vm.PrimaryCabin.MiddleHingeOption = repo.GetPart(vm.PrimaryCabin.DefaultMiddleHinge).GetDeepClone();
                }
                else
                {
                    vm.PrimaryCabin.MiddleHingeOption = midHinge.GetDeepClone();
                }
            }

            if (vm.SecondaryCabin?.HasMiddleHingeOption is true)
            {
                if (midHinge is null)
                {
                    vm.SecondaryCabin.MiddleHingeOption = repo.GetPart(vm.SecondaryCabin.DefaultMiddleHinge).GetDeepClone();
                }
                else
                {
                    vm.SecondaryCabin.MiddleHingeOption = midHinge.GetDeepClone();
                }
                
            }

            if (vm.TertiaryCabin?.HasMiddleHingeOption is true)
            {
                if (midHinge is null)
                {
                    vm.TertiaryCabin.MiddleHingeOption = repo.GetPart(vm.TertiaryCabin.DefaultMiddleHinge).GetDeepClone();
                }
                else
                {
                    vm.TertiaryCabin.MiddleHingeOption = midHinge.GetDeepClone();
                }
            }
        }
        private CabinPart InitialSelectedMiddleHinge()
        {
            if (vm.PrimaryCabin?.HasMiddleHingeOption == true)
            {
                return vm.PrimaryCabin.MiddleHingeOption;
            }
            else if (vm.SecondaryCabin?.HasMiddleHingeOption == true)
            {
                return vm.SecondaryCabin.MiddleHingeOption;
            }
            else if (vm.TertiaryCabin?.HasMiddleHingeOption == true)
            {
                return vm.TertiaryCabin.MiddleHingeOption;
            }
            else
            {
                throw new InvalidOperationException("There is no MiddleHinge Option for this Synthesis");
            }
        }
        private IEnumerable<CabinPart> GetMiddleHingesSelectionList()
        {
            if (vm.PrimaryCabin?.HasMiddleHingeOption is true)
            {
                return repo.GetAllParts().Where(p => vm.PrimaryCabin.SelectableMiddleHinges.Contains(p.Code));
            }
            else if (vm.SecondaryCabin?.HasMiddleHingeOption is true)
            {
                return repo.GetAllParts().Where(p => vm.SecondaryCabin.SelectableMiddleHinges.Contains(p.Code));
            }
            else if (vm.TertiaryCabin?.HasMiddleHingeOption is true)
            {
                return repo.GetAllParts().Where(p => vm.TertiaryCabin.SelectableMiddleHinges.Contains(p.Code));
            }
            return new List<CabinPart>();
        }

        #endregion

        #region 4. CloseProfile Option

        private void OnCloseProfileSelection(CabinPart selected)
        {
            if (vm.PrimaryCabin?.HasCloseProfileOption is true)
            {
                if (selected is Profile closeProfile)
                {
                    //Hack... mpourdes
                    vm.PrimaryCabin.CloseProfileOption = closeProfile.GetDeepClone();
                    vm.PrimaryCabin.CloseStripOption = repo.GetStrip("8APL-MA-G18");
                }
                else if (selected is GlassStrip strip)
                {
                    vm.PrimaryCabin.CloseProfileOption = null;
                    vm.PrimaryCabin.CloseStripOption = strip.GetDeepClone();
                }
                else
                {
                    vm.PrimaryCabin.CloseProfileOption = null;
                    vm.PrimaryCabin.CloseStripOption = null;
                }
            }
        }
        private CabinPart InitialSelectedCloseProfile()
        {
            if (vm.PrimaryCabin?.HasCloseProfileOption == true)
            {
                if (vm.PrimaryCabin.CloseProfileOption != null)
                {
                    return vm.PrimaryCabin.CloseProfileOption;
                }
                else if (vm.PrimaryCabin.CloseStripOption != null)
                {
                    return vm.PrimaryCabin.CloseStripOption;
                }
                else return null;
            }
            else
            {
                throw new InvalidOperationException("There is no CloseProfile Option for this Synthesis");
            }
        }
        private IEnumerable<CabinPart> GetCloseProfileSelectionList()
        {
            if (vm.PrimaryCabin?.HasCloseProfileOption is true)
            {
                //HACK...
                return repo.GetAllParts()
                    .Where(p => 
                    vm.PrimaryCabin.SelectableCloseProfiles.Contains(p.Code) 
                    || vm.PrimaryCabin.SelectableCloseStrips.Contains(p.Code)).Where(p=>p.Code != "8APL-MA-G18");
            }
            return new List<CabinPart>();
        }

        #endregion

        #region 5.Wall Fixer
        private void OnWallFixerSelection(CabinPart selected)
        {
            if (vm.PrimaryCabin?.HasWallFixingOption is true)
            {
                if (selected is null)
                {
                    vm.PrimaryCabin.WallSideFixerOption = repo.GetPart(vm.PrimaryCabin.DefaultWallFixer).GetDeepClone();
                }
                else
                {
                    vm.PrimaryCabin.WallSideFixerOption = selected.GetDeepClone();
                }
            }

            if (vm.SecondaryCabin?.HasWallFixingOption is true)
            {
                if (selected is null)
                {
                    vm.SecondaryCabin.WallSideFixerOption = repo.GetPart(vm.SecondaryCabin.DefaultWallFixer).GetDeepClone();
                }
                else
                {
                    vm.SecondaryCabin.WallSideFixerOption = selected.GetDeepClone();
                }

            }

            if (vm.TertiaryCabin?.HasWallFixingOption is true)
            {
                if (selected is null)
                {
                    vm.TertiaryCabin.WallSideFixerOption = repo.GetPart(vm.TertiaryCabin.DefaultWallFixer).GetDeepClone();
                }
                else
                {
                    vm.TertiaryCabin.WallSideFixerOption = selected.GetDeepClone();
                }
            }
        }
        private CabinPart InitialSelectedWallFixer()
        {
            if (vm.PrimaryCabin?.HasWallFixingOption == true)
            {
                return vm.PrimaryCabin.WallSideFixerOption;
            }
            else if (vm.SecondaryCabin?.HasWallFixingOption == true)
            {
                return vm.SecondaryCabin.WallSideFixerOption;
            }
            else if (vm.TertiaryCabin?.HasWallFixingOption == true)
            {
                return vm.TertiaryCabin.WallSideFixerOption;
            }
            else
            {
                throw new InvalidOperationException("There is no Wall Fixer Option for this Synthesis");
            }
        }
        private IEnumerable<CabinPart> GetWallFixerSelectionList()
        {
            if (vm.PrimaryCabin?.HasWallFixingOption is true)
            {
                return repo.GetAllParts().Where(p => vm.PrimaryCabin.SelectableWallFixers.Contains(p.Code));
            }
            else if (vm.SecondaryCabin?.HasWallFixingOption is true)
            {
                return repo.GetAllParts().Where(p => vm.SecondaryCabin.SelectableWallFixers.Contains(p.Code));
            }
            else if (vm.TertiaryCabin?.HasWallFixingOption is true)
            {
                return repo.GetAllParts().Where(p => vm.TertiaryCabin.SelectableWallFixers.Contains(p.Code));
            }
            return new List<CabinPart>();
        }
        #endregion

        #region 6.Hinge Profile Selection

        /// <summary>
        /// Gets the Available Hinge Profiles Selection for the Current Synthesis (Hardcoded Codes of the Hinge Profiles)
        /// </summary>
        /// <returns></returns>
        private IEnumerable<CabinPart> GetHingeProfilesSelectionList()
        {
            if (hasRotationMechanism)
            {
                return repo.GetAllParts().Where(p => p.Code == "QBAL-10-HINGE" || p.Code == "NBAL-10-HINGE");
            }
            return new List<CabinPart>();
        }

        private void OnHingeProfileSelection(CabinPart part)
        {
            var drawToAlternate = ReturnAlternateDrawNumberOfCurrentModel();
            vm.TransformSynthesisToNewDraw(drawToAlternate);
        }
        /// <summary>
        /// Sets the initially Selected Botttom Fixer to the First It Finds starting From Primary Model
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private CabinPart InitialSelectedHingeProfile()
        {
            if (isNBModel || isNPModel)
            {
                return repo.GetPart("NBAL-10-HINGE");
            }
            else if (isQBModel || isQPModel)
            {
                return repo.GetPart("QBAL-10-HINGE");
            }
            else return repo.GetPart("NBAL-10-HINGE");
        }


        #endregion

        /// <summary>
        /// Returns the QP-QB draw or the NB-NP draw depending on the Input
        /// </summary>
        /// <returns></returns>
        private CabinDrawNumber ReturnAlternateDrawNumberOfCurrentModel()
        {
            return vm.PrimaryCabin.IsPartOfDraw switch
            {
                CabinDrawNumber.DrawNP44 => CabinDrawNumber.DrawQP44,
                CabinDrawNumber.Draw2CornerNP46 => CabinDrawNumber.Draw2CornerQP46,
                CabinDrawNumber.Draw2StraightNP48 => CabinDrawNumber.Draw2StraightQP48,
                CabinDrawNumber.DrawCornerNP6W45 => CabinDrawNumber.DrawCornerQP6W45,
                CabinDrawNumber.DrawStraightNP6W47 => CabinDrawNumber.DrawStraightQP6W47,
                CabinDrawNumber.DrawNB31 => CabinDrawNumber.DrawQB31,
                CabinDrawNumber.DrawCornerNB6W32 => CabinDrawNumber.DrawCornerQB6W32,
                CabinDrawNumber.Draw2CornerNB33 => CabinDrawNumber.Draw2CornerQB33,
                CabinDrawNumber.DrawStraightNB6W38 => CabinDrawNumber.DrawStraightQB6W38,
                CabinDrawNumber.Draw2StraightNB41 => CabinDrawNumber.Draw2StraightQB41,
                CabinDrawNumber.DrawQP44 => CabinDrawNumber.DrawNP44,
                CabinDrawNumber.Draw2CornerQP46 => CabinDrawNumber.Draw2CornerNP46,
                CabinDrawNumber.Draw2StraightQP48 => CabinDrawNumber.Draw2StraightNP48,
                CabinDrawNumber.DrawCornerQP6W45 => CabinDrawNumber.DrawCornerNP6W45,
                CabinDrawNumber.DrawStraightQP6W47 => CabinDrawNumber.DrawStraightNP6W47,
                CabinDrawNumber.DrawQB31 => CabinDrawNumber.DrawNB31,
                CabinDrawNumber.DrawCornerQB6W32 => CabinDrawNumber.DrawCornerNB6W32,
                CabinDrawNumber.Draw2CornerQB33 => CabinDrawNumber.Draw2CornerNB33,
                CabinDrawNumber.DrawStraightQB6W38 => CabinDrawNumber.DrawStraightNB6W38,
                CabinDrawNumber.Draw2StraightQB41 => CabinDrawNumber.Draw2StraightNB41,
                null => CabinDrawNumber.None,
                _ => throw new InvalidOperationException("Only QP and NB Draw Numbers can be interchanged"),
            };
        }


        public void Dispose()
        {
            vm.PropertyChanged -= Vm_PropertyChanged;
        }
    }
}
