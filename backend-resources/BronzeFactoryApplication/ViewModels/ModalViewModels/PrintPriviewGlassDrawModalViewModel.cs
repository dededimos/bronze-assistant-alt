using BronzeFactoryApplication.ViewModels.ComponentsUCViewModels.DrawsViewModels;
using ShowerEnclosuresModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.ModalViewModels;

public partial class PrintPriviewGlassDrawModalViewModel : ModalViewModel
{
    [ObservableProperty]
    private GlassDrawViewModel? draw;
    [ObservableProperty]
    private string? specialDrawString;
    [ObservableProperty]
    private int? specialDrawNumber;

    public PrintPriviewGlassDrawModalViewModel()
    {
        Title = "lngPrintPreview".TryTranslateKey();
    }

    public void SetDraw(GlassDrawViewModel drawViewModel , string? specialDrawString = null ,int? specialDrawNumber = null)
    {
        Draw = drawViewModel;
        SpecialDrawString = specialDrawString;
        SpecialDrawNumber = specialDrawNumber;
    }
    public string FullDrawString { get => GetFullDrawString(); }
    private string GetFullDrawString()
    {
        if (string.IsNullOrWhiteSpace(SpecialDrawString))
        {
            return Draw?.GlassVm?.Draw.ToString().TryTranslateKey() ?? "";
        }
        else
        {
            return $"{Draw?.GlassVm?.Draw.ToString().TryTranslateKey()}{SpecialDrawString ?? "??"}-{SpecialDrawNumber?.ToString() ?? "??"}";
        }
    }
}
