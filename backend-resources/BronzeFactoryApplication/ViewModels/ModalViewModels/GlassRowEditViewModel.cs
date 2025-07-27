using BronzeFactoryApplication.ApplicationServices.MessangerService;
using BronzeFactoryApplication.ViewModels.ComponentsUCViewModels.DrawsViewModels;
using DocumentFormat.OpenXml.Drawing;
using EnumsNET;
using GlassesOrdersModels.Models;
using SVGGlassDrawsLibrary;

namespace BronzeFactoryApplication.ViewModels.ModalViewModels;

public partial class GlassRowEditViewModel : ModalViewModel
{
    private readonly IMessenger messenger;
    private readonly CloseModalService closeModalService;
    private readonly OpenPrintPreviewGlassModalService printPreviewModalService;
    private GlassOrderRow _undoStore = GlassOrderRow.Empty();

    public bool HasUnsavedEdits { get => hasSaved == false && !IsEqualWithUndoStore(); }
    public bool hasSaved = false;
    /// <summary>
    /// Weather the Current state is equal to the undo store state
    /// </summary>
    /// <returns></returns>
    private bool IsEqualWithUndoStore()
    {
        return
            _undoStore.ReferencePA0 == this.ReferencePA0
            && _undoStore.Notes == this.Notes
            && _undoStore.Quantity == this.Quantity
            && _undoStore.CabinRowKey == this.CabinKey
            && _undoStore.FilledQuantity == this.FilledQuantity
            && _undoStore.CancelledQuantity == this.CancelledQuantity
            && _undoStore.OrderedGlass.IsEqualGlass(Glass.GetGlass());
    }

    [ObservableProperty]
    private string referencePA0 = string.Empty;
    [ObservableProperty]
    private string notes = string.Empty;
    [ObservableProperty]
    private int quantity;
    [ObservableProperty]
    private Guid cabinKey;
    [ObservableProperty]
    private int filledQuantity;
    [ObservableProperty]
    private int cancelledQuantity;
    [ObservableProperty]
    private bool isFromStock;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FullDrawString))]
    private int? specialDrawNumber;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FullDrawString))]
    private string? specialDrawString;
    [ObservableProperty]
    private GlassViewModel glass = new();
    [ObservableProperty]
    private GlassDrawViewModel glassDraw = new();

    public string FullDrawString { get => GetFullDrawString(); }
    private string GetFullDrawString()
    {
        if (string.IsNullOrWhiteSpace(SpecialDrawString))
        {
            return Glass.Draw.ToString().TryTranslateKey();
        }
        else
        {
            return $"{Glass.Draw.ToString().TryTranslateKey()}{SpecialDrawString ?? "??"}-{SpecialDrawNumber?.ToString() ?? "??"}";
        }
    }


    public GlassRowEditViewModel(IMessenger messenger,CloseModalService closeModalService,OpenPrintPreviewGlassModalService printPreviewModalService)
    {
        this.messenger = messenger;
        this.closeModalService = closeModalService;
        this.printPreviewModalService = printPreviewModalService;
        this.closeModalService.ModalClosing += OnClosingModal;
    }

    private void OnClosingModal(object? sender, ModalClosingEventArgs e)
    {
        if (HasUnsavedEdits)
        {
            if (MessageService.Questions.UnsavedChangesContinue() == MessageBoxResult.Cancel)
            {
                e.ShouldCancelClose = true;
            }
        }
    }

    [RelayCommand]
    private void EditRow()
    {
        messenger.Send(new GlassRowEditMessage(_undoStore, GetGlassRow()));
        hasSaved = true; //ByPass checking for unsaved Edits
        closeModalService.CloseModal(this);
    }

    [RelayCommand]
    private void UndoEdits()
    {
        this.SetGlassRow(_undoStore);
    }

    [RelayCommand]
    private void OpenPrintPreview()
    {
        printPreviewModalService.OpenModal(GlassDraw, SpecialDrawString, SpecialDrawNumber);
    }

    /// <summary>
    /// Gets the Glass Row represented by this ViewModels state
    /// </summary>
    /// <returns></returns>
    public GlassOrderRow GetGlassRow()
    {
        var row = GlassOrderRow.CreateNew(
                                this.ReferencePA0,
                                this.Notes,
                                this.Quantity,
                                this.Glass.GetGlass(),
                                CabinKey,
                                _undoStore.RowId,
                                IsFromStock,
                                FilledQuantity,
                                CancelledQuantity);
        row.SpecialDrawNumber = this.SpecialDrawNumber;
        row.SpecialDrawString = this.SpecialDrawString;
        row.ParentCabinRow = _undoStore.ParentCabinRow;
        row.Created = _undoStore.Created;
        row.LastModified = _undoStore.LastModified;
        return row;
    }
    /// <summary>
    /// Sets the State of the ViewModel to that of the specified GlassRow to be Edited
    /// </summary>
    /// <param name="row">The Glass Row that is being edited</param>
    public void SetGlassRow(GlassOrderRow row)
    {
        Title = row.OrderedGlass.GlassType.ToString().TryTranslateKey();
        ReferencePA0 = row.ReferencePA0;
        Notes = row.Notes;
        Quantity = row.Quantity;
        CabinKey = row.CabinRowKey;
        IsFromStock = row.IsFromStock;
        FilledQuantity = row.FilledQuantity;
        CancelledQuantity = row.CancelledQuantity;
        Glass.SetGlass(row.OrderedGlass,false);
        SpecialDrawNumber = row.SpecialDrawNumber;
        SpecialDrawString = row.SpecialDrawString;

        //****TEST****
        GlassDraw.SetGlassDraw(Glass);
        //****TEST****


        //Store the Row
        _undoStore = row;
        OnPropertyChanged("");
    }

    //IF MADE TRANSIENT WE HAVE TO DISPOSE ON CLOSING WINDOW . CURRENTLY ITS SINGLETON DOES NOT NEED DISPOSING
    private bool _disposed;
    public override void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)//Managed Resources
        {
            this.closeModalService.ModalClosing -= OnClosingModal;
            GlassDraw.Dispose();
        }

        //object has been disposed
        _disposed = true;
        base.Dispose(disposing);

        //If this comes from inheritance Where the parent implement IDisposable the must call base Dispose and the Dispose() method is only in the Parent
        //The subclasses only implement the virtual method and a field '_disposed'
        //Call the base Dispose(bool)
        //base.Dispose(disposing);
    }

}
