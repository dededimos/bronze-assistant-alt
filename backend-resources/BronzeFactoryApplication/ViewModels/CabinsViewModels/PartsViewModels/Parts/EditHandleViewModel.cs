using DataAccessLib.NoSQLModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.PartsViewModels.Parts
{
    public partial class EditHandleViewModel : EditPartViewModel
    {
        [ObservableProperty]
        private CabinHandleType handleType;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsCircularHandle))]
        private int handleLengthToGlass;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsCircularHandle))]
        private int handleHeightToGlass;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsCircularHandle))]
        private double handleEdgesCornerRadius;

        [ObservableProperty]
        private double handleOuterThickness;

        [ObservableProperty]
        private int handleComfortDistance;

        [ObservableProperty]
        private int minimumDistanceFromEdge;

        public bool IsCircularHandle { get => (HandleLengthToGlass == HandleHeightToGlass && HandleLengthToGlass == HandleEdgesCornerRadius * 2); }

        public EditHandleViewModel():base(CabinPartType.Handle)
        {

        }

        public EditHandleViewModel(CabinPartEntity entity , bool isEdit = true) : base(entity , isEdit)
        {
            CabinHandle handle = entity.Part as CabinHandle ?? throw new ArgumentException($"{nameof(EditHandleViewModel)} accepts Only CabinPartEntities of a type {nameof(CabinHandle)}");
            InitilizeHandleViewModel(handle);
        }

        private void InitilizeHandleViewModel(CabinHandle part)
        {
            this.HandleType = part.HandleType;
            this.HandleLengthToGlass = part.HandleLengthToGlass;
            this.HandleHeightToGlass = part.HandleHeightToGlass;
            this.HandleEdgesCornerRadius = part.HandleEdgesCornerRadius;
            this.HandleOuterThickness = part.HandleOuterThickness;
            this.HandleComfortDistance = part.HandleComfortDistance;
            this.MinimumDistanceFromEdge = part.MinimumDistanceFromEdge;
        }

        public override CabinHandle GetPart()
        {
            CabinHandle handle = new(HandleType);
            ExtractPropertiesForBasePart(handle);

            handle.HandleLengthToGlass = HandleLengthToGlass;
            handle.HandleHeightToGlass = HandleHeightToGlass;
            handle.HandleEdgesCornerRadius = HandleEdgesCornerRadius;
            handle.HandleOuterThickness = HandleOuterThickness;
            handle.HandleComfortDistance = HandleComfortDistance;
            handle.MinimumDistanceFromEdge = MinimumDistanceFromEdge;
            
            return handle;
        }

    }
}
