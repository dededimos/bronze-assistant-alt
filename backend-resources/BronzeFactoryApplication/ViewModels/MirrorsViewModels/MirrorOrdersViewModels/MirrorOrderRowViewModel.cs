using CommonInterfacesBronze;
using MirrorsLib;
using MirrorsLib.MirrorsOrderModels;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.MirrorOrdersViewModels
{
    public partial class MirrorOrderRowViewModel : BaseViewModel , IEditorViewModel<MirrorOrderRow>
    {
        [ObservableProperty]
        private string notes = string.Empty;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PendingQuantity))]
        [NotifyPropertyChangedFor(nameof(Status))]
        private double quantity;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PendingQuantity))]
        [NotifyPropertyChangedFor(nameof(Status))]
        private double filledQuantity;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PendingQuantity))]
        [NotifyPropertyChangedFor(nameof(Status))]
        private double cancelledQuantity;
        [ObservableProperty]
        private MirrorSynthesis? mirror;
        public DateTime Created { get; private set; }
        [ObservableProperty]
        private DateTime lastModified;
        [ObservableProperty]
        private int lineNumber;
        [ObservableProperty]
        string refPAOPAM = string.Empty;

        [ObservableProperty]
        private string parentOrderNo = string.Empty;

        private string rowId = string.Empty;
            

        public CommonOrderModels.OrderStatus Status { get => MirrorOrderRow.GetStatus(PendingQuantity, FilledQuantity, CancelledQuantity); }
        public double PendingQuantity { get => Quantity - FilledQuantity - CancelledQuantity; }

        public void SetModel(MirrorOrderRow model)
        {
            SuppressPropertyNotifications();
            this.Notes = model.Notes;
            this.Quantity = model.Quantity;
            this.FilledQuantity = model.FilledQuantity;
            this.CancelledQuantity = model.CancelledQuantity;
            this.Mirror = model.RowItem?.GetDeepClone();
            this.Created = model.Created;
            this.LastModified = model.LastModified;
            this.LineNumber = model.LineNumber;
            this.ParentOrderNo = model.ParentOrderNo;
            this.rowId = model.RowId;
            this.RefPAOPAM = model.GetMetadata(MirrorOrderRow.PaoPamMetadataKey)?.ToString() ?? string.Empty;
            ResumePropertyNotifications();
            OnPropertyChanged(string.Empty);
        }

        public MirrorOrderRow CopyPropertiesToModel(MirrorOrderRow model)
        {
            model.RowId = this.rowId;
            model.ParentOrderNo = ParentOrderNo;
            model.Notes = Notes;
            model.Quantity = Quantity;
            model.FilledQuantity = FilledQuantity;
            model.CancelledQuantity = CancelledQuantity;
            model.RowItem = Mirror?.GetDeepClone();
            model.Created = this.Created;
            model.LastModified = this.LastModified;
            model.LineNumber = this.LineNumber;
            model.Metadata.Clear();
            model.AddMetadata(MirrorOrderRow.PaoPamMetadataKey, RefPAOPAM);
            return model;
        }

        public MirrorOrderRow GetModel()
        {
            return CopyPropertiesToModel(new());
        }
    }
}
