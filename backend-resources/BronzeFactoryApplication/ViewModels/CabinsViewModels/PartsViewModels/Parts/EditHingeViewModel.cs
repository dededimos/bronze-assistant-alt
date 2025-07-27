using DataAccessLib.NoSQLModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.PartsViewModels.Parts
{
    public partial class EditHingeViewModel : EditPartViewModel
    {
        //Main Hinge Props
        [ObservableProperty]
        private int lengthView;
        [ObservableProperty]
        private int heightView;
        [ObservableProperty]
        private int glassGapAER;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsGlassToGlassHinge))]
        [NotifyPropertyChangedFor(nameof(IsHinge9B))]
        [NotifyPropertyChangedFor(nameof(IsHingeDB))]
        private CabinHingeType hingeType;

        //GlassToGlass Hinge Specific
        [ObservableProperty]
        private int inDoorLength;
        public bool IsGlassToGlassHinge { get => HingeType is CabinHingeType.GlassToGlassHinge; }

        //Hinge9B Specific
        [ObservableProperty]
        private int hingeOverlappingHeight;
        [ObservableProperty]
        private int supportTubeLength;
        [ObservableProperty]
        private int supportTubeHeight;
        [ObservableProperty]
        private int cornerRadiusInGlass;
        public bool IsHinge9B { get => HingeType is CabinHingeType.Hinge9B; }

        //HingeDB Specific
        [ObservableProperty]
        private int innerHeight;
        [ObservableProperty]
        private int wallPlateThicknessView;
        public bool IsHingeDB { get => HingeType is CabinHingeType.HingeDB; }

        public EditHingeViewModel():base(CabinPartType.Hinge)
        {

        }
        
        public EditHingeViewModel(CabinPartEntity entity, bool isEdit = true) : base(entity,isEdit)
        {
            CabinHinge hinge = entity.Part as CabinHinge ?? throw new ArgumentException($"{nameof(EditHingeViewModel)} accepts Only CabinPartEntities of a type {nameof(CabinHinge)}");
            InitilizeHingeViewModel(hinge);
        }
        private void InitilizeHingeViewModel(CabinHinge part)
        {
            this.LengthView = part.LengthView;
            this.HeightView = part.HeightView;
            this.GlassGapAER = part.GlassGapAER;
            this.HingeType = part.HingeType;
            switch (part)
            {

                case GlassToGlassHinge gtg:
                    this.InDoorLength = gtg.InDoorLength;
                    break;
                case Hinge9B hinge9B:
                    this.HingeOverlappingHeight = hinge9B.HingeOverlappingHeight;
                    this.SupportTubeLength = hinge9B.SupportTubeLength;
                    this.SupportTubeHeight = hinge9B.SupportTubeHeight;
                    this.CornerRadiusInGlass = hinge9B.CornerRadiusInGlass;
                    break;
                case HingeDB hingeDB:
                    this.InnerHeight = hingeDB.InnerHeight;
                    this.WallPlateThicknessView = hingeDB.WallPlateThicknessView;
                    break;
                default:
                    break;
            }
        }

        public override CabinHinge GetPart()
        {
            CabinHinge hinge = HingeType switch
            {
                CabinHingeType.GlassToGlassHinge => new GlassToGlassHinge()
                {
                    InDoorLength = this.InDoorLength,
                },
                CabinHingeType.Hinge9B => new Hinge9B()
                {
                    HingeOverlappingHeight = this.HingeOverlappingHeight,
                    SupportTubeLength = this.SupportTubeLength,
                    SupportTubeHeight = this.SupportTubeHeight,
                    CornerRadiusInGlass = this.CornerRadiusInGlass,
                },
                CabinHingeType.HingeDB => new HingeDB()
                {
                    InnerHeight = this.InnerHeight,
                    WallPlateThicknessView = this.WallPlateThicknessView,
                },
                _ => new CabinHinge(),
            };
            hinge.LengthView = this.LengthView;
            hinge.HeightView = this.HeightView;
            hinge.GlassGapAER = this.GlassGapAER;
            
            ExtractPropertiesForBasePart(hinge);
            
            return hinge;
        }

    }
}
