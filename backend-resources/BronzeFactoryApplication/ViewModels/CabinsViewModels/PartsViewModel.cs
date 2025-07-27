
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Tools.Extension;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HandlesModels;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels;

public partial class PartsViewModel : BaseViewModel
{
    /// <summary>
    /// Fires whenever a Part Changes
    /// </summary>
    public event EventHandler<PartChangedEventsArgs>? PartChanged;
    /// <summary>
    /// Fires whenever a Request to a Primary Part Happens
    /// </summary>
    public event EventHandler<RequestPrimaryPartArgs>? RequestPrimaryPart;

    /// <summary>
    /// The PartsList when set will always have parts set in its spots according to the Cabin that is being made
    /// If the Cabin happens to get retrieved from somewhere those values will not be at their default values
    /// If the parts in the Retrieved Cabins might have also been deprecated Changed code e.t.c.
    /// In order for these parts to appear as selectables for this particular models , they have to be added to the Selectable Lists
    /// So at Initilization we have to store those parts that are of different code for all the various selectables
    /// </summary>
    protected Dictionary<PartSpot, CabinPart?> partsNotPresentInValids = new();

    private CabinPartsList? partsListObject;
    public CabinPartsList? GetPartsObject()
    {
        return partsListObject;
    }
    private readonly ICabinMemoryRepository repo;
    protected readonly IMessenger messenger;

    protected virtual CabinPartsList? PartsListObject => partsListObject;

    public IEnumerable<CabinPart> GenericParts => PartsListObject?.GenericParts ?? new List<CabinPart>();

    public CabinIdentifier Identifier { get; private set; }

    private bool canConnectWithPrimary = true;
    public bool CanConnectWithPrimary
    {
        get => Identifier.SynthesisModel != CabinSynthesisModel.Primary && canConnectWithPrimary;
        set
        {
            if (canConnectWithPrimary != value)
            {
                canConnectWithPrimary = value;
                OnPropertyChanged(nameof(CanConnectWithPrimary));
            }
        }
    }

    public GlassStrip? DefaultCloseStrip => repo.GetDefaultPart<GlassStrip>(Identifier, PartSpot.CloseStrip);
    public IEnumerable<GlassStrip> SelectableCloseStrips
    {
        get
        {
            var selectables = repo.GetSpotValids(Identifier, PartSpot.CloseStrip).Select(c => repo.GetStrip(c,Identifier)).ToList();
            if (partsNotPresentInValids.TryGetValue(PartSpot.CloseStrip,out CabinPart? part) 
                && (part is not null && part is GlassStrip strip)
                && (selectables.Any(s=> s.Code == strip.Code) == false))
            {
                selectables.Add(strip);
            }
            return selectables;
        }
    }

    public CabinPart? DefaultBottomFixer => repo.GetDefaultPart(Identifier, PartSpot.BottomSide1);
    public IEnumerable<CabinPart> SelectableBottomFixers
    {
        get
        {
            var selectables = repo.GetSpotValids(Identifier, PartSpot.BottomSide1).Select(c => repo.GetPart(c,Identifier)).ToList();
            if (partsNotPresentInValids.TryGetValue(PartSpot.BottomSide1,out CabinPart? part) 
                && part is not null
                && (selectables.Any(s=> s.Code == part.Code) == false))
            {
                selectables.Add(part);
            }
            return selectables;
        }
    }

    public CabinPart? DefaultWallFixer => repo.GetDefaultPart(Identifier, PartSpot.WallSide1);
    public IEnumerable<CabinPart> SelectableWallFixers
    {
        get
        {
            var selectables = repo.GetSpotValids(Identifier, PartSpot.WallSide1).Select(c => repo.GetPart(c,Identifier)).ToList();
            if (partsNotPresentInValids.TryGetValue(PartSpot.WallSide1,out CabinPart? part) 
                && part is not null
                && (selectables.Any(s=> s.Code == part.Code) == false))
            {
                selectables.Add(part);
            }
            return selectables;
        }
    }

    public Profile? DefaultCloseProfile => repo.GetDefaultPart<Profile>(Identifier, PartSpot.CloseProfile);
    public IEnumerable<Profile> SelectableCloseProfiles
    {
        get
        {
            var selectables = repo.GetSpotValids(Identifier, PartSpot.CloseProfile).Select(c => repo.GetProfile(c,Identifier)).ToList();
            if (partsNotPresentInValids.TryGetValue(PartSpot.CloseProfile,out CabinPart? part) 
                && (part is not null && part is Profile profile)
                && (selectables.Any(s=> s.Code == profile.Code) == false))
            {
                selectables.Add(profile);
            }
            return selectables;
        }
    }

    public IEnumerable<Profile> SelectableHorizontalProfiles
    {
        get
        {
            var selectables = repo.GetSpotValids(Identifier, PartSpot.HorizontalGuideTop).Select(c => repo.GetProfile(c,Identifier)).ToList();
            if (partsNotPresentInValids.TryGetValue(PartSpot.HorizontalGuideTop,out CabinPart? part) 
                && (part is not null && part is Profile profile)
                && (selectables.Any(s=> s.Code == profile.Code) == false))
            {
                selectables.Add(profile);
            }
            return selectables;
        }
    }

    public IEnumerable<CabinAngle> SelectableAngles
    {
        get
        {
            var selectables = repo.GetSpotValids(Identifier, PartSpot.Angle).Select(c => repo.GetAngle(c,Identifier)).ToList();
            if (partsNotPresentInValids.TryGetValue(PartSpot.Angle, out CabinPart? part) 
                && (part is not null && part is CabinAngle angle)
                && (selectables.Any(s=> s.Code == angle.Code) == false))
            {
                selectables.Add(angle);
            }
            return selectables;
        }
    }

    public CabinHandle? DefaultHandle => repo.GetDefaultPart<CabinHandle>(Identifier, PartSpot.Handle1);
    public IEnumerable<CabinHandle> SelectableHandles
    {
        get
        {
            var selectables = repo.GetSpotValids(Identifier, PartSpot.Handle1).Select(c => repo.GetHandle(c,Identifier)).ToList();
            if (partsNotPresentInValids.TryGetValue(PartSpot.Handle1, out CabinPart? part) 
                && (part is not null && part is CabinHandle handle)
                && (selectables.Any(s=> s.Code == handle.Code) == false))
            {
                selectables.Add(handle);
            }
            return selectables;
        }
    }

    public bool HasTwoHandles { get => partsListObject?.GetPartOrNull<CabinHandle>(PartSpot.Handle2) != null; }

    #region CONSTRUCTOR
    public PartsViewModel(ICabinMemoryRepository repo)
    {
        this.repo = repo;

        //#TODO Have to do it with ctor injection , but now needs a ton of changes maybe in the Future
        this.messenger = App.AppHost!.Services.GetRequiredService<IMessenger>();
    } 
    #endregion

    public virtual void SetNewPartsList(CabinPartsList partsList, CabinIdentifier identifier)
    {
        partsListObject = partsList;
        Identifier = identifier;

        //The PartsList when set will always have parts set in its spots according to the Cabin that is being made
        //If the Cabin happens to get retrieved from somewhere those values will not be at their default values
        //If the parts in the Retrieved Cabins might have also been deprecated Changed code e.t.c.
        //In order for these parts to appear as selectables for this particular models , they have to be added to the Selectable Lists
        //So at Initilization we have to store those parts that are of different code for all the various selectables
        partsNotPresentInValids = new();
        //keep a copy of the initial parts (add them in the selectable getters if different)
        foreach (var spotPart in partsListObject.GetPositionedParts())
        {
            partsNotPresentInValids.Add(spotPart.Key, spotPart.Value);
        }
    }

    public virtual DefaultPartsList? GetDefaults()
    {
        return repo.DefaultLists.Where(dl => dl.Key == (Identifier.Model,Identifier.DrawNumber,Identifier.SynthesisModel)).FirstOrDefault().Value;
    }

    /// <summary>
    /// Fires when a Part Changes
    /// </summary>
    /// <param name="newPart"></param>
    /// <param name="oldPart"></param>
    protected virtual void RaisePartChanged(PartSpot spot, CabinPart? newPart, CabinPart? oldPart)
    {
        PartChangedEventsArgs args = new(this.Identifier, spot, newPart, oldPart);
        PartChanged?.Invoke(this, args);
    }

    /// <summary>
    /// Fires when this ViewModel needs a Part as per the Primary Model
    /// </summary>
    /// <param name="spot"></param>
    protected virtual void RaiseRequestPrimaryPart(PartSpot spot)
    {
        RequestPrimaryPartArgs args = new(spot, Identifier.SynthesisModel);
        RequestPrimaryPart?.Invoke(this, args);
    }

    public virtual void InformSpotPartChanged(PartSpot spot)
    {
        OnPropertyChanged("");
    }
}

public partial class PartsViewModel<T> : PartsViewModel
    where T : CabinPartsList
{
    protected T? partsListObject;

    public PartsViewModel(ICabinMemoryRepository repo) : base(repo)
    {
    }

    protected override T? PartsListObject => partsListObject;

}

public class PartChangedEventsArgs : EventArgs
{
    public CabinPart? NewPart { get; set; }
    public CabinPart? OldPart { get; set; }
    public PartSpot Spot { get; set; }
    public CabinIdentifier Identifier { get; set; }

    public PartChangedEventsArgs(CabinIdentifier identifier, PartSpot spot, CabinPart? newPart, CabinPart? oldPart)
    {
        Identifier = identifier;
        Spot = spot;
        NewPart = newPart;
        OldPart = oldPart;
    }
}

public class RequestPrimaryPartArgs : EventArgs
{
    /// <summary>
    /// The Spot for which the Part of the Primary Model is Requested
    /// </summary>
    public PartSpot RequestSpot { get; set; }
    /// <summary>
    /// The Synthesis Model that Requests the Primary Part
    /// </summary>
    public CabinSynthesisModel SynthesisModel { get; set; }

    public RequestPrimaryPartArgs(PartSpot requestSpot, CabinSynthesisModel synthesisModel)
    {
        RequestSpot = requestSpot;
        SynthesisModel = synthesisModel;
    }
}


