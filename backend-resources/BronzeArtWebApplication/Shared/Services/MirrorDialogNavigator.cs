using BronzeArtWebApplication.Shared.Enums;
using BronzeArtWebApplication.Shared.Helpers;
using BronzeArtWebApplication.Shared.Models;
using BronzeArtWebApplication.Shared.ViewModels;
using MirrorsModelsLibrary.Enums;
using System;
using System.Collections.Generic;

namespace BronzeArtWebApplication.Shared.Services
{
    public class MirrorDialogNavigator
    {
        private readonly AssembleMirrorViewModel vm;

        /// <summary>
        /// The Dialog to BeShown Next or The one Just Opening to be Shown
        /// </summary>
        public MirrorDialog CurrentDialog { get; private set; } = MirrorDialog.ChooseShape;
        /// <summary>
        /// Whether or Not Story Mode is On (The Consecutive Dialogs)
        /// </summary>
        public bool InStoryMode { get; set; } = true;
        public Dictionary<MirrorDialog, bool> IsDialogOpen { get; set; } = new();

        public MirrorDialogNavigator(AssembleMirrorViewModel vm)
        {
            this.vm = vm;

            //Initialize WhichDialog is Open Dictionary
            foreach (MirrorDialog dialog in Enum.GetValues(typeof(MirrorDialog)))
            {
                IsDialogOpen.Add(dialog, false);
            }
        }

        /// <summary>
        /// Turns Visibility of Dialogues On/Off Depending on the Clicked Button
        /// </summary>
        /// <param name="dialogToOpen"></param>
        public void GoToDialog(MirrorDialog dialogToOpen, bool isStoryDialog = true)
        {
            //Turn Off all the Dialogs except the one we need to Open
            foreach (MirrorDialog dialog in IsDialogOpen.Keys)
            {
                if (dialogToOpen == dialog)
                {
                    IsDialogOpen[dialog] = true;
                }
                else
                {
                    IsDialogOpen[dialog] = false;
                }
            }

            // Set the Current Dialog to the One that opened ,
            // If the Dialog to Open was None leave current Dialog as Is
            // If the Opened Dialog is Not a part of the StoryDialogs Do not Set Current Dialog
            if (dialogToOpen != MirrorDialog.None && isStoryDialog)
            {
                CurrentDialog = dialogToOpen;
            }

        }

        /// <summary>
        /// Weather a MirrorDialog is Valid to be Opened or Not
        /// <para>Ex. When there are no Options in a Dialog to Choose from , it is redundant for opening , or when the Options are only 1</para>
        /// </summary>
        /// <param name="dialog"></param>
        /// <returns></returns>
        private bool IsDialogValid(MirrorDialog dialog)
        {
            return dialog switch
            {
                //All mirrors have options for these kind of dialogs
                MirrorDialog.None or MirrorDialog.ChooseShape => true,

                MirrorDialog.ChooseLight => vm.Series is not MirrorSeries.ES and not MirrorSeries.EL,

                //Cannot choose dimensions on ready mirrors of certain series
                MirrorDialog.ChooseDimensions => true,
                //vm.Series is not MirrorSeries.R7
                //         and not MirrorSeries.R9
                //         and not MirrorSeries.P8
                //         and not MirrorSeries.P9
                //         and not MirrorSeries.ND
                //         and not MirrorSeries.N1
                //         and not MirrorSeries.N2
                //         and not MirrorSeries.ES
                //         and not MirrorSeries.EL,

                //Only if it can take more than 1 support is Valid
                MirrorDialog.ChooseSupport => StaticInfoMirror.GetSelectableSupports(vm.Series ?? MirrorSeries.Custom).Count > 1,
                //Only if it has Frame
                MirrorDialog.ChooseFrameFinish => vm.Support == MirrorSupport.Frame && vm.Series is not MirrorSeries.P8 and not MirrorSeries.P9,
                //Only if it can take more than 1 Sandblast is Valid
                MirrorDialog.ChooseSandblast => StaticInfoMirror.GetSelectableSandblasts(vm.Series ?? MirrorSeries.Custom).Count > 1,
                //Only if it can take at least one touch switch
                MirrorDialog.ChooseSwitch => StaticInfoMirror.GetSelectableTouchOptions(vm.Series ?? MirrorSeries.Custom).Count > 0,
                //Only if it can take at least one fog
                MirrorDialog.ChooseFog => StaticInfoMirror.GetSelectableFogOptions(vm.Series ?? MirrorSeries.Custom).Count > 0,
                //Only if it can take at least one magnifyer
                MirrorDialog.ChooseMagnifyer => StaticInfoMirror.GetSelectableMagnifierOptions(vm.Series ?? MirrorSeries.Custom).Count > 0,
                //Only if it can take at least one screen
                MirrorDialog.ChooseScreen => StaticInfoMirror.GetSelectableMediaOptions(vm.Series ?? MirrorSeries.Custom).Count > 0,
                //Only if it can take at least one extra
                MirrorDialog.ChooseVarious => StaticInfoMirror.GetSelectableExtraOptions(vm.Series ?? MirrorSeries.Custom).Count > 0,
                _ => false,
            };
        }

        /// <summary>
        /// The Sequence that the Dialogs Follow Backwards
        /// </summary>
        private readonly static List<MirrorDialog> previousDialogSequence =
        [
            MirrorDialog.ChooseVarious,
            MirrorDialog.ChooseScreen,
            MirrorDialog.ChooseMagnifyer,
            MirrorDialog.ChooseFog,
            MirrorDialog.ChooseSwitch,
            MirrorDialog.ChooseSandblast,
            MirrorDialog.ChooseFrameFinish,
            MirrorDialog.ChooseSupport,
            MirrorDialog.ChooseDimensions,
            MirrorDialog.ChooseLight,
            MirrorDialog.ChooseShape,
            MirrorDialog.None,
        ];
        /// <summary>
        /// The Sequence that the Dialogs Follow Forwards
        /// </summary>
        private readonly static List<MirrorDialog> nextDialogSequence =
        [
            MirrorDialog.ChooseShape,
            MirrorDialog.ChooseLight,
            MirrorDialog.ChooseDimensions,
            MirrorDialog.ChooseSupport,
            MirrorDialog.ChooseFrameFinish,
            MirrorDialog.ChooseSandblast,
            MirrorDialog.ChooseSwitch,
            MirrorDialog.ChooseFog,
            MirrorDialog.ChooseMagnifyer,
            MirrorDialog.ChooseScreen,
            MirrorDialog.ChooseVarious,
            MirrorDialog.None,
        ];


        /// <summary>
        /// Opens the Previous Dialog
        /// </summary>
        public void GoToPreviousDialog()
        {
            if (CurrentDialog == MirrorDialog.None) return; //othwerwise the indexer will throw an out of bound exception

            // Find the index in the previousSequence of the current Dialog
            int currentIndex = previousDialogSequence.IndexOf(CurrentDialog);

            // Check weather the next dialog in the Sequence is valid and open it , otherwise check the next one until one is found
            for (int i = currentIndex + 1; i < previousDialogSequence.Count; i++)
            {
                if (IsDialogValid(previousDialogSequence[i]))
                {
                    GoToDialog(previousDialogSequence[i]);
                    return;
                }
            }
        }
        /// <summary>
        /// Opens the Next Dialog
        /// </summary>
        public void GoToNextDialog()
        {
            var nextDialog = GetNextDialog();
            GoToDialog(nextDialog);
        }
        /// <summary>
        /// Returns the Next Dialog in the Sequence
        /// </summary>
        /// <returns></returns>
        public MirrorDialog GetNextDialog()
        {
            // Find the index in the nextDialogSequence of the current Dialog
            int currentIndex = nextDialogSequence.IndexOf(CurrentDialog);

            // Check weather the next dialog in the Sequence is valid and open it , otherwise check the next one until one is found
            for (int i = currentIndex + 1; i < nextDialogSequence.Count; i++)
            {
                if (IsDialogValid(nextDialogSequence[i]))
                {
                    return nextDialogSequence[i];
                }
            }
            return MirrorDialog.None;
        }


        /// <summary>
        /// Changes the Current Story Dialog // When A Mirror From the Catalogue is Selected
        /// </summary>
        public void ChangeCurrentDialog(MirrorDialog newCurrentDialog)
        {
            CurrentDialog = newCurrentDialog;
        }

        #region 1.Button Click Navigation Methods

        /// <summary>
        /// The Choose ButtonClick for the Support of the Mirror
        /// </summary>
        public void ChooseSupportClick()
        {
            if (InStoryMode)
            {
                if (vm.Shape is MirrorShape.Rectangular)
                {
                    switch (vm.Support)
                    {
                        case MirrorSupport.Without:
                        case MirrorSupport.FrontSupports:
                            GoToDialog(MirrorDialog.ChooseVarious);
                            break;
                        case MirrorSupport.Double:
                            GoToDialog(MirrorDialog.ChooseFog);
                            break;
                        case MirrorSupport.Perimetrical:
                            if (vm.Light != MirrorLight.WithoutLight) //If with Light
                            {
                                GoToDialog(MirrorDialog.ChooseSandblast);
                            }
                            else //Without Light
                            {
                                GoToDialog(MirrorDialog.ChooseFog);
                            }
                            break;
                        case MirrorSupport.Frame:
                            GoToDialog(MirrorDialog.ChooseFrameFinish);
                            break;
                        default:
                            GoToDialog(MirrorDialog.None);
                            break;
                    }
                }
                else if (vm.Shape == MirrorShape.Circular)
                {
                    switch (vm.Support)
                    {
                        case MirrorSupport.Double:
                            GoToDialog(MirrorDialog.ChooseFog);
                            break;
                        case MirrorSupport.Perimetrical:
                            if (vm.Light != MirrorLight.WithoutLight) //If with Light
                            {
                                GoToDialog(MirrorDialog.ChooseSandblast);
                            }
                            else //Without Light
                            {
                                GoToDialog(MirrorDialog.ChooseFog);
                            }
                            break;
                        case MirrorSupport.Frame:
                            if (vm.Light is not null and not MirrorLight.WithoutLight)
                            {
                                vm.Sandblast = MirrorSandblast.N6;
                            }
                            GoToDialog(MirrorDialog.ChooseFrameFinish);
                            break;
                        case MirrorSupport.Without:
                        case MirrorSupport.FrontSupports:
                        default:
                            GoToDialog(MirrorDialog.None);
                            break;
                    }
                }
                else if (vm.Shape is MirrorShape.Capsule or MirrorShape.Ellipse)
                {
                    switch (vm.Support)
                    {
                        case MirrorSupport.Double:
                            GoToDialog(MirrorDialog.ChooseFog);
                            break;
                        case MirrorSupport.Perimetrical:
                            if (vm.Light != MirrorLight.WithoutLight) //If with Light
                            {
                                GoToDialog(MirrorDialog.ChooseSwitch);
                            }
                            else //Without Light
                            {
                                GoToDialog(MirrorDialog.ChooseFog);
                            }
                            break;
                        case MirrorSupport.Frame:
                        case MirrorSupport.Without:
                        case MirrorSupport.FrontSupports:
                        default:
                            GoToDialog(MirrorDialog.None);
                            break;
                    }
                }
                else
                {
                    GoToDialog(MirrorDialog.None);
                }
            }
            else // If not in Story Mode just close the Dialog
            {
                IsDialogOpen[MirrorDialog.ChooseSupport] = false;
            }
        }

        /// <summary>
        /// The Choose ButtonClick for the FrameFinish of the Mirror
        /// </summary>
        public void ChooseFrameFinishClick()
        {
            if (InStoryMode)
            {
                if (vm.Shape == MirrorShape.Rectangular)
                {
                    if (vm.Light != MirrorLight.WithoutLight)
                    {
                        GoToDialog(MirrorDialog.ChooseSandblast);
                    }
                    else
                    {
                        GoToDialog(MirrorDialog.ChooseFog);
                    }
                }
                else if (vm.Shape == MirrorShape.Circular)
                {
                    if (vm.Light != MirrorLight.WithoutLight)
                    {
                        if (vm.Support == MirrorSupport.Frame)
                        {
                            //Sandblast is already set
                            GoToDialog(MirrorDialog.ChooseSwitch);
                        }
                        else
                            GoToDialog(MirrorDialog.ChooseSandblast);
                    }
                    else
                    {
                        GoToDialog(MirrorDialog.ChooseFog);
                    }
                }
                else
                {
                    GoToDialog(MirrorDialog.None);
                }
            }
            else
            {
                GoToDialog(MirrorDialog.None);
            }
        }

        #endregion
    }
}
