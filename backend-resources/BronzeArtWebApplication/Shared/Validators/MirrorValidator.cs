using AKSoftware.Localization.MultiLanguages;
using BronzeArtWebApplication.Shared.ViewModels;
using FluentValidation;
using MirrorsModelsLibrary.Enums;
using MirrorsModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BronzeArtWebApplication.Shared.Validators
{
    public class MirrorValidator : AbstractValidator<AssembleMirrorViewModel>
    {
        /// <summary>
        /// An AssembleMirrorViewModel Validator
        /// </summary>
        /// <param name="lc">The Language Container</param>
        /// <param name="withDrawBoundsValidation">Wheather to Validate also if the Mirror Extras Are out Of Bounds</param>
        public MirrorValidator(ILanguageContainerService lc, bool withDrawBoundsValidation = true)
        {
            #region 1.General Selection Rules
             
            if (withDrawBoundsValidation)
            {
                //Draw Error (Out OfBounds Shapes Must be zero
                RuleFor(viewmodel => viewmodel.Draw.OutOfBoundsShapesNames.Count)
                       .LessThan(1)
                       .When(m =>   m.Series is not MirrorSeries.P8
                                    and not MirrorSeries.P9
                                    and not MirrorSeries.NS
                                    and not MirrorSeries.ND
                                    and not MirrorSeries.N1
                                    and not MirrorSeries.N2
                                    and not MirrorSeries.ES
                                    and not MirrorSeries.EL,
                                    ApplyConditionTo.CurrentValidator)
                       .WithMessage($"{lc.Keys["SelectedExtrasAreOutOfBounds"]}");
            }

            //When Mirror Shape is Not Null Perform all Checks
            When(m => m.Shape != null, () =>
            {
                //When Shape is Rectangular
                When(m => m.Shape is MirrorShape.Rectangular or MirrorShape.Capsule or MirrorShape.Ellipse, () =>
                {
                    //Mirror Length
                    RuleFor(m => m.Length)
                        .NotNull()
                        .WithMessage(lc.Keys["LengthNotSelected"])

                        .InclusiveBetween(Mirror.MinLength, Mirror.MaxLength)
                        .WithMessage($"{lc.Keys["MirrorLengthWithin"]}: {Mirror.MinLength}-{Mirror.MaxLength}{lc.Keys["cm"]}");

                    //Mirror Height
                    RuleFor(m => m.Height)
                        .NotNull()
                        .WithMessage(lc.Keys["HeightNotSelected"])

                        .InclusiveBetween(Mirror.MinHeight, Mirror.MaxHeight)
                        .WithMessage($"{lc.Keys["MirrorHeightWithin"]}: {Mirror.MinHeight}-{Mirror.MaxHeight}{lc.Keys["cm"]}");

                    //Mirror Height/Length when Frame is Selected
                    RuleFor(m => m)
                        .Must(m => Math.Min((int)m.Height, (int)m.Length) <= Mirror.MaxMinFrameDimension).When(m => m.Height != null && m.Length != null && m.Support is MirrorSupport.Frame, ApplyConditionTo.CurrentValidator)
                        .WithMessage($"{lc.Keys["MaxDimensionsWithVisibleFrame"]}-{Mirror.MaxLength}x{Mirror.MaxMinFrameDimension}{lc.Keys["cm"]}");

                    RuleFor(m => m)
                        .Must(m => m.Shape is not MirrorShape.Capsule).When(m => m.Support is MirrorSupport.Frame, ApplyConditionTo.CurrentValidator)
                        .WithMessage("CannotCreateFramedCapsule");
                    RuleFor(m => m)
                        .Must(m => m.Shape is not MirrorShape.Ellipse).When(m => m.Support is MirrorSupport.Frame, ApplyConditionTo.CurrentValidator)
                        .WithMessage("CannotCreateFramedEllipse");
                });

                // When Shape is Circular
                When(m => m.Shape == MirrorShape.Circular, () =>
                {
                    //Mirror Diameter
                    RuleFor(m => m.Diameter)
                        .NotNull()
                        .WithMessage(lc.Keys["DiameterNotSelected"])

                        .InclusiveBetween(Mirror.MinDiameter, Mirror.MaxDiameter)
                        .WithMessage($"{lc.Keys["MirrorDiameterWithin"]}: {Mirror.MinDiameter}-{Mirror.MaxDiameter}{lc.Keys["cm"]}");

                    //Visible Frame
                    //RuleFor(m => m)
                    //    .Must(m => m.Support != MirrorSupport.Frame)
                    //    .When(m =>
                    //    (m.Diameter is not 60 and not 70 and not 80) ||
                    //    (m.Light == MirrorLight.WithoutLight || m.Light == null) ||
                    //    m.Sandblast is not MirrorSandblast.N6 ||
                    //    (m.Extras.Count < 1 || !m.Extras.Any(e => e.Option is MirrorOption.TouchSwitch or MirrorOption.DimmerSwitch or MirrorOption.SensorSwitch)) //When Does not have touch cannot be with Frame
                    //    , ApplyConditionTo.CurrentValidator)
                    //    .WithMessage(lc.Keys["CircularWithFrame"]);
                    When(m => m.Support is MirrorSupport.Frame && m.Light is MirrorLight.WithoutLight, () =>
                    {
                        RuleFor(m => m.Diameter)
                        .Must(d => d is 40 or 50 or 60 or 70 or 80 or 90 or 100 or 110 or 120)
                        .WithMessage(lc.Keys["CircularWithFrame"]);

                        RuleFor(m => m.Sandblast)
                        .Equal(MirrorSandblast.N9).WithMessage(lc.Keys["CircularWithFrameNoLightSandblastN9"]);
                    });

                    When(m => m.Support is MirrorSupport.Frame && m.Light is not null and not MirrorLight.WithoutLight, () =>
                    {
                        RuleFor(m => m.Diameter)
                        .Must(d => d is 40 or 50 or 60 or 70 or 80 or 90 or 100 or 110 or 120)
                        .WithMessage(lc.Keys["CircularWithFrame"]);

                        RuleFor(m => m.Sandblast)
                        .Equal(MirrorSandblast.N6).When(m=> m.Series is not MirrorSeries.P9).WithMessage(lc.Keys["CircularWithFrameWithLightSandblastN6"]);
                    });

                });

                //Rules for Sandblast
                RuleFor(m => m).Cascade(CascadeMode.Stop)
                    .Must(m => m.Sandblast != null)
                    .WithMessage(lc.Keys["SandblastNotSelected"])

                    .Must(m => m.Sandblast is MirrorSandblast.H7).When(m => m.Shape is MirrorShape.Capsule, ApplyConditionTo.CurrentValidator)
                    .WithMessage(lc.Keys["CapsuleCannotHaveSandblast"])

                    .Must(m => m.Sandblast is MirrorSandblast.H7).When(m => m.Shape is MirrorShape.Ellipse, ApplyConditionTo.CurrentValidator)
                    .WithMessage(lc.Keys["EllipseCannotHaveSandblast"]);



                //DEPRECATED RULE
                //.Must(m => m.Sandblast != MirrorSandblast.H7).When(m => m.Support == MirrorSupport.Frame && m.Light != MirrorLight.WithoutLight, ApplyConditionTo.CurrentValidator)
                //.WithMessage(lc.Keys["SandblastObligatoryWithLightFrame"]);


                //Rules for Supports
                RuleFor(m => m).Cascade(CascadeMode.Stop)
                    .Must(m => m.Support != null)
                    .WithMessage(lc.Keys["SupportNotSelected"])

                    .Must(m => m.FinishType != null).When(m => m.Support == MirrorSupport.Frame, ApplyConditionTo.CurrentValidator)
                    .WithMessage(lc.Keys["FrameFinishTypeNotSelected"])

                    .Must(m => m.PaintFinish != null).When(m => m.FinishType == SupportFinishType.Painted, ApplyConditionTo.CurrentValidator)
                    .WithMessage(lc.Keys["PaintFinishNotSelected"])

                    .Must(m => m.ElectroplatedFinish != null).When(m => m.FinishType == SupportFinishType.Electroplated, ApplyConditionTo.CurrentValidator)
                    .WithMessage(lc.Keys["ElectroplatedFinishNotSelected"]);

                //Light Rules
                RuleFor(m => m.Light)
                    .NotNull().WithMessage(lc.Keys["LightNotSelected"]);

                //Special Rules per Series
                When(m => m.Series == MirrorSeries.R7 || m.Series == MirrorSeries.R9, () =>
                {
                    RuleFor(m=> m).Must(m=> m.Light is MirrorLight.Warm_16W or MirrorLight.Day_16W or MirrorLight.Cold_16W or MirrorLight.Warm_Cold_Day_16W)
                    .WithMessage(lc.Keys["PremiumMirrorsCannotHaveLightsLessThan16W"]);
                });

                When(m => m.Series is not MirrorSeries.P8 and not MirrorSeries.P9, () =>
                {
                    RuleFor(m => m).Must(m => m.Extras.Select(e => e.Option).Any(o => o is MirrorOption.FrontLightSealedChannel) == false).WithMessage("OnlyGenesisIsavellaTakeFrontChannel");
                });
                When(m => m.Series is not MirrorSeries.ES and not MirrorSeries.EL, () =>
                {
                    RuleFor(m => m).Must(m => m.Extras.Select(e => e.Option).Any(o => o is MirrorOption.SingleTopLightedPlexiglass or MirrorOption.DoubleTopBottomLightedPlexiglass) == false).WithMessage("OnlyRivieraTakePlexiglass");
                });
                When(m => m.Series is not MirrorSeries.R7 and not MirrorSeries.R9, () =>
                {
                    RuleFor(m => m).Must(m => m.Extras.Select(e => e.Option).Any(o => o is MirrorOption.BackLightSealedChannel) == false).WithMessage("OnlyPremiumTakeBackChannel");
                });


                // Otherwise Shape is Null
            }).Otherwise(() =>
                {
                    RuleFor(m => m.Shape).NotNull().WithMessage(lc.Keys["ShapeNotSelected"]);
                });

            #endregion

            #region 2.Rules with Error Codes

            //LIGHTED MIRRORS

            //When Lighted Mirror Cannot Select Double-Front-Without Support
            When(m => m.Light != null && m.Light != MirrorLight.WithoutLight, () =>
            {
                //Incompatible Supports
                RuleFor(m => m.Support)
                    .NotEqual(MirrorSupport.Double).WithErrorCode("LightWithDoubleSupport").WithMessage(lc.Keys["LightWithDoubleSupport"])
                    .NotEqual(MirrorSupport.FrontSupports).WithErrorCode("LightWithFrontSupport").WithMessage(lc.Keys["LightWithFrontSupport"])
                    .NotEqual(MirrorSupport.Without).WithErrorCode("LightWithoutSupport").WithMessage(lc.Keys["LightWithoutSupport"]);

                RuleFor(m => m)
                    .Must(m => m.HasRounding == false)
                    .When(m => m.Support == MirrorSupport.Frame && m.Series is not MirrorSeries.P8)
                    .WithErrorCode("LightWithRounding").WithMessage(lc.Keys["LightWithRounding"]);

            });

            //SIMPLE MIRRORS

            //When Mirror Without Light
            When(m => m.Light == MirrorLight.WithoutLight, () =>
            {

                // Incompatible Sandblasts
                RuleFor(m => m.Sandblast).Cascade(CascadeMode.Stop)
                    // When Rectangular without Light cannot Have Sandblasts
                    .Must(sandblast => sandblast == null || sandblast == MirrorSandblast.H7)
                    .When(m => m.Shape == MirrorShape.Rectangular, ApplyConditionTo.CurrentValidator)
                    .WithErrorCode("SimpleRectangularWithSandblast").WithMessage(lc.Keys["SimpleRectangularWithSandblast"])

                    // When Circular without Light cannot Have Sandblasts
                    .Must(sandblast => sandblast == null || sandblast == MirrorSandblast.N9)
                    .When(m => m.Shape == MirrorShape.Circular, ApplyConditionTo.CurrentValidator)
                    .WithErrorCode("SimpleCircularWithSandblast").WithMessage(lc.Keys["SimpleCircularWithSandblast"]);

                // Incompatible Switches
                RuleFor(m => m).Cascade(CascadeMode.Stop)
                    .Must(m => m.HasSwitch == false).WithErrorCode("SimpleWithTouch").WithMessage(lc.Keys["SimpleWithTouch"])
                    .Must(m => m.HasDimmer == false).WithErrorCode("SimpleWithDimmer").WithMessage(lc.Keys["SimpleWithDimmer"])
                    .Must(m => m.HasSensor == false).WithErrorCode("SimpleWithSensor").WithMessage(lc.Keys["SimpleWithSensor"]);

                // Incompatible Magnifyers
                RuleFor(m => m).Cascade(CascadeMode.Stop)

                    //Cannot Have Magnifyer when the Back Supports are not Perimetrical or Frame
                    .Must(m => m.HasMagnifyer == false)
                    .When(m => m.Support != MirrorSupport.Perimetrical &&
                               m.Support != MirrorSupport.Frame &&
                               m.Support != MirrorSupport.Double,
                               ApplyConditionTo.CurrentValidator)
                    .WithErrorCode("SimpleWithMagnifyer").WithMessage(lc.Keys["SimpleWithMagnifyer"])

                    //Cannot Have Magnifyer with Led on Simple Mirrors
                    .Must(m => m.HasMagnifyerLed == false)
                    .WithErrorCode("SimpleWithMagnifyerLed").WithMessage(lc.Keys["SimpleWithMagnifyerLed"])

                    //Cannot Have Magnifyer with LedTouch on Simple Mirrors
                    .Must(m => m.HasMagnifyerLedTouch == false)
                    .WithErrorCode("SimpleWithMagnifyerLedTouch").WithMessage(lc.Keys["SimpleWithMagnifyerLedTouch"]);

                // Incompatible Fog-Switch
                RuleFor(m => m.HasFogSwitch)
                    .Equal(false).WithErrorCode("SimpleWithFogSwitch").WithMessage(lc.Keys["SimpleWithFogSwitch"]);
                RuleFor(m => m.HasEcoFogSwitch)
                    .Equal(false).WithErrorCode("SimpleWithFogSwitch").WithMessage(lc.Keys["SimpleWithFogSwitch"]);

                // Incompatible Anti-Fog
                When(m => m.Support != MirrorSupport.Perimetrical &&
                          m.Support != MirrorSupport.Frame &&
                          m.Support != MirrorSupport.Double, () =>
                          {
                              RuleFor(m => m)
                                .Must(m => m.HasFog16 == false)
                                .WithErrorCode("SimpleWithFog16").WithMessage(lc.Keys["SimpleWithFog16"])

                                .Must(m => m.HasFog24 == false)
                                .WithErrorCode("SimpleWithFog24").WithMessage(lc.Keys["SimpleWithFog24"])

                                .Must(m => m.HasFog55 == false)
                                .WithErrorCode("SimpleWithFog55").WithMessage(lc.Keys["SimpleWithFog55"]);
                          });

                // Incompatible Lid or Rounding (Without Cascade Here Multiple Selections Can Co-Exist)
                RuleFor(m => m)
                    .Must(m => m.HasLid == false)
                    .When(m => m.Support is not MirrorSupport.Frame
                                      and not MirrorSupport.Perimetrical,
                                      ApplyConditionTo.CurrentValidator)
                    .WithErrorCode("SimpleWithLid").WithMessage(lc.Keys["SimpleWithLid"])

                    .Must(m => m.HasRounding == false)
                    .When(m => m.Support is MirrorSupport.Frame, ApplyConditionTo.CurrentValidator)
                    .WithErrorCode("SimpleWithRounding").WithMessage(lc.Keys["SimpleWithRounding"]);

                // Incompatible Screens
                RuleFor(m => m)
                    .Must(m => m.HasClock == false)
                    .WithErrorCode("SimpleWithClock").WithMessage(lc.Keys["SimpleWithClock"])

                    .Must(m => m.HasBluetooth == false)
                    .WithErrorCode("SimpleWithBluetooth").WithMessage(lc.Keys["SimpleWithBluetooth"])

                    .Must(m => m.HasDisplay11 == false)
                    .WithErrorCode("SimpleWithDisplay11").WithMessage(lc.Keys["SimpleWithDisplay11"])

                    .Must(m => m.HasDisplay19 == false)
                    .WithErrorCode("SimpleWithDisplay19").WithMessage(lc.Keys["SimpleWithDisplay19"])

                    .Must(m => m.HasDisplay20 == false)
                    .WithErrorCode("SimpleWithDisplay20").WithMessage(lc.Keys["SimpleWithDisplay20"])
                    
                    .Must(m => m.HasDisplay11Black == false)
                    .WithErrorCode("SimpleWithDisplay11").WithMessage(lc.Keys["SimpleWithDisplay11"]);

                //Incompatible Support
                RuleFor(m => m)
                    .Must(m => m.Support != MirrorSupport.Perimetrical).When(m => m.Shape is MirrorShape.Circular or MirrorShape.Capsule or MirrorShape.Ellipse)
                    .WithErrorCode("SimpleCircularCapsulePerimetrical").WithMessage(lc.Keys["SimpleCircularCapsulePerimetrical"]);

            });

            //SUPPORT CHANGE

            When(m => m.Support != MirrorSupport.Frame, () =>
            {
                // Incompatible Finish Type without Frame
                RuleFor(m => m).Cascade(CascadeMode.Stop)
                    .Must(m => m.FinishType == null)
                    .WithErrorCode("FinishTypeWithoutFrame").WithMessage(lc.Keys["FinishTypeWithoutFrame"]);
            });

            #endregion
        }
    }
}
