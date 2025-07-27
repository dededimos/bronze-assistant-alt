using BronzeFactoryApplication.ApplicationServices.MessangerService;
using BronzeFactoryApplication.ApplicationServices.NavigationService;
using GlassesOrdersModels.Models;
using ShowerEnclosuresModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.ModalViewModels;

public partial class AddSynthesisToOrderModalViewModel : ModalViewModel
{
    private CabinSynthesis? synthesis;
    private GlassSwap? swap;
    private readonly CloseModalService closeModalService;
    private readonly IMessenger messenger;

    [ObservableProperty]
    private string referencePA0 = string.Empty;
    [ObservableProperty]
    private int? quantity = 1;
    [ObservableProperty]
    private string notes = string.Empty;


    public AddSynthesisToOrderModalViewModel(CloseModalService closeModalService, IMessenger messenger)
    {
        Title = "lngAddNewOrderRow".TryTranslateKey();
        this.closeModalService = closeModalService;
        this.messenger = messenger;
    }

    public void Initilize(CabinSynthesis synthesis, string refPA0Number, GlassSwap? swap)
    {
        this.synthesis = synthesis;
        ReferencePA0 = refPA0Number;
        this.swap = swap;
    }

    /// <summary>
    /// Adds the Structure pieces to the Glass Order and Closes the Modal
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private void TryAddAndClose()
    {
        ArgumentNullException.ThrowIfNull(synthesis, nameof(synthesis));


        if (string.IsNullOrEmpty(ReferencePA0))
        {
            MessageService.Warning("lngPleaseAddReferencePao".TryTranslateKey(), "lngMissingField".TryTranslateKey());
            return;
        }
        if (Quantity is null || Quantity <= 0)
        {
            MessageService.Warning("lngPleaseAddValidQuantity".TryTranslateKey(), "lngInvalidField".TryTranslateKey());
            return;
        }

        //1.Generate a New Key for the Synthesis that is being Added to the Order (so that it can be properly Grouped)
        //2.Foreach Cabin Generate a key and Make A Cabin Row for each Row pass That Key and Glasses to Create the Glasses Rows

        var synthesisKey = Guid.NewGuid();
        bool isSwapFound = false;
        IEnumerable<CabinOrderRow> cabinRows = synthesis.GetCabinList().Select(cabin =>
        {
            var cabinKey = Guid.NewGuid();

            IEnumerable<GlassOrderRow> glassRows = cabin.Glasses.Select(g =>
            {
                // Start always as not From Stock
                bool isFromStock = false;
                // If there is a Swap and has not yet been found Set this glass if is the Swapped one as From Stock
                if (swap is not null && !isSwapFound && g.IsEqualGlass(swap.NewGlass))
                {
                    isSwapFound = true;
                    isFromStock = true;
                }
                return GlassOrderRow.CreateNew(ReferencePA0, Notes, (int)Quantity, g, cabinKey, "", isFromStock, 0, 0);
            });

            return CabinOrderRow.CreateNew(ReferencePA0, Notes, (int)Quantity, cabin, glassRows, synthesisKey, cabinKey, "");
        }).ToList(); //Must call to List otherwise the above does not work

        //Find the Glass that has been Swapped
        if (swap is not null && cabinRows.SelectMany(r => r.GlassesRows).Any(gr => gr.IsFromStock) is false)
        {
            throw new Exception("The Swapped Glass Could not be Found in the Synthesis , you try to Add");
        }

        //SEND MESSAGE HERE
        messenger.Send(new AddCabinRowsMessage(cabinRows));

        //Log.Information("Added {code} to Order , Qunatity:{quantity} and PAO/PAM:{refPAO}", cabin.Code, quantity, referencePA0);
        closeModalService.CloseModal(this);
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

        }

        //object has been disposed
        _disposed = true;

        //If this comes from inheritance Where the parent implement IDisposable the must call base Dispose and the Dispose() method is only in the Parent
        //The subclasses only implement the virtual method and a field '_disposed'
        //Call the base Dispose(bool)
        base.Dispose(disposing);
    }
}
