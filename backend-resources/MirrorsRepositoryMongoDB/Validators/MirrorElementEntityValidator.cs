using CommonInterfacesBronze;
using FluentValidation;
using MirrorsLib.Interfaces;
using MirrorsLib.MirrorElements.MirrorExtras;
using MirrorsLib.MirrorElements.MirrorModules;
using MirrorsLib.MirrorElements.Supports;
using MirrorsLib.Services.PositionService;
using MirrorsLib.Validators;
using MirrorsLib.Validators.Modules;
using MirrorsLib.Validators.Positions;
using MirrorsLib.Validators.Sandblasts;
using MirrorsLib.Validators.Supports;
using MirrorsRepositoryMongoDB.Entities;
using MongoDbCommonLibrary.CommonEntities;
using MongoDbCommonLibrary.CommonValidators;

namespace MirrorsRepositoryMongoDB.Validators
{
    public class MirrorElementEntityValidator: AbstractValidator<MirrorElementEntity>
    {
        public MirrorElementEntityValidator(bool includeIdValidation)
        {
            RuleFor(e => e).SetInheritanceValidator(v =>
            {
                v.Add<MirrorSandblastEntity>(new MirrorSandblastEntityValidator(includeIdValidation));
                v.Add<MirrorSupportEntity>(new MirrorSupportEntityValidator(includeIdValidation));
                v.Add<MirrorSeriesElementEntity>(new MirrorSeriesElementEntityValidator(includeIdValidation));
                v.Add<MirrorElementPositionEntity>(new MirrorElementPositionEntityValidator(includeIdValidation));
                v.Add<MirrorLightElementEntity>(new MirrorLightElementEntityValidator(includeIdValidation));
                v.Add<MirrorModuleEntity>(new MirrorModuleEntityValidator(includeIdValidation));
                v.Add<CustomMirrorElementEntity>(new CustomMirrorElementEntityValidator(includeIdValidation));
                v.Add<MirrorFinishElementEntity>(new MirrorFinishElementEntityValidator(includeIdValidation));
                v.Add<MirrorElementEntity>(new MirrorElementEntityBaseValidator<MirrorElementEntity>(includeIdValidation, true));
            });
        }
    }
    public class MirrorFinishElementEntityValidator : MirrorElementEntityBaseValidator<MirrorFinishElementEntity>
    {
        public MirrorFinishElementEntityValidator(bool includeIdValidation) : base(includeIdValidation, true)
        {
            //no rules yet specific to Finish
        }
    }
    public class CustomMirrorElementEntityValidator : MirrorElementEntityBaseValidator<CustomMirrorElementEntity>
    {
        public CustomMirrorElementEntityValidator(bool includeIdValidation) : base(includeIdValidation,true)
        {
            RuleFor(e => e.CustomElementType).SetValidator(new LocalizedStringValidator(true));
        }
    }

    public class MirrorSandblastEntityValidator : MirrorElementEntityBaseValidator<MirrorSandblastEntity>
    {
        public MirrorSandblastEntityValidator(bool includeIdValidation) : base(includeIdValidation, true)
        {
            RuleFor(e => e.Sandblast).SetValidator(new MirrorSandblastInfoValidator());
        }
    }
    public class MirrorSupportEntityValidator : MirrorElementEntityBaseValidator<MirrorSupportEntity>
    {
        public MirrorSupportEntityValidator(bool includeIdValidation) : base(includeIdValidation, true)
        {
            RuleFor(e => e.Support).SetInheritanceValidator(v =>
            {
                v.Add<MirrorMultiSupports>(new MirrorMultiSupportsValidator());
                v.Add<MirrorVisibleFrameSupport>(new MirrorVisibleFrameSupportValidator());
                v.Add<MirrorBackFrameSupport>(new MirrorBackFrameSupportValidator());
            });
        }
    }
    public class MirrorModuleEntityValidator : MirrorElementEntityBaseValidator<MirrorModuleEntity>
    {
        public MirrorModuleEntityValidator(bool includeIdValidation) : base(includeIdValidation, true)
        {
            RuleFor(e => e.ModuleInfo).SetInheritanceValidator(v =>
            {
                v.Add<BluetoothModuleInfo>(new BluetoothModuleValidator());
                v.Add<MagnifierSandblastedModuleInfo>(new MagnifyerSandblastModuleValidator());
                v.Add<MagnifierModuleInfo>(new MagnifyerModuleValidator());
                v.Add<ScreenModuleInfo>(new ScreenModuleValidator());
                v.Add<ResistancePadModuleInfo>(new ResistancePadModuleValidator());
                v.Add<RoundedCornersModuleInfo>(new RoundedCornersModuleValidator());
                v.Add<TouchButtonModuleInfo>(new TouchButtonModuleValidator());
                v.Add<MirrorLampModuleInfo>(new MirrorLampModuleValidator());
                v.Add<TransformerModuleInfo>(new TransformerModuleValidator());
            });
        }
    }
    public class MirrorLightElementEntityValidator : MirrorElementEntityBaseValidator<MirrorLightElementEntity>
    {
        public MirrorLightElementEntityValidator(bool includeIdValidation) : base(includeIdValidation,true)
        {
            RuleFor(e => e.LightInfo).SetValidator(new MirrorLightInfoValidator());
        }
    }

    public class MirrorElementPositionEntityValidator : MirrorElementEntityBaseValidator<MirrorElementPositionEntity>
    {
        public MirrorElementPositionEntityValidator(bool includeIdValidation):base(includeIdValidation, true)
        {
            RuleFor(e => e.Instructions).SetValidator(new PositionInstructionsValidator());
        }
    }
}
