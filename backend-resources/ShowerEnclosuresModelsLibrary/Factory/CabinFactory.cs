using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Helpers;
using ShowerEnclosuresModelsLibrary.Helpers.Custom_Exceptions;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HandlesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.StripsModels;
using ShowerEnclosuresModelsLibrary.Models.RepositoryModels.Interfaces;
using ShowerEnclosuresModelsLibrary.Models.RepositoryModels.ModelsSettings;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.ConstraintsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.DBModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels.ConstraintsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.HBModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.Inox304Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.Inox304Models.ConstraintsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.Inox304Models.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.NBModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.NPModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.WSSmartModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Factory;

public class CabinFactory
{
#nullable enable
    private readonly ICabinMemoryRepository r;

    /// <summary>
    /// Instantiates a Cabin Factory for the Provided Repository
    /// </summary>
    /// <param name="r">The Repository Containing All Options And Parts for the Cabins</param>
    public CabinFactory(ICabinMemoryRepository r)
    {
        this.r = r;
    }

    /// <summary>
    /// Creates a Cabin Synthesis with the Default Cabins for the Selected Draw Number
    /// </summary>
    /// <param name="drawNumber">The Draw Number of the Synthesis</param>
    /// <returns>The Cabin Synthesis</returns>
    public CabinSynthesis CreateSynthesis(CabinDrawNumber drawNumber)
    {
        CabinSynthesis synthesis = new()
        {
            DrawNo = drawNumber,
            Primary = CreateCabin(drawNumber, CabinSynthesisModel.Primary),
            Secondary = CreateCabin(drawNumber, CabinSynthesisModel.Secondary),
            Tertiary = CreateCabin(drawNumber, CabinSynthesisModel.Tertiary)
        };

        //Pass any Side Tollerance to the Primary (negative for 9F without Tol)
        //Get All the side tollerances sum and adjust the Primary accordingly
        var sideTolleranceSecondary = synthesis.Secondary?
            .Parts
            .GetCabinPartsNotNested()
            .OfType<Profile>()
            .Sum(p => p.SideTollerance) ?? 0;
        var sideTolleranceTertiary = synthesis.Tertiary?
            .Parts
            .GetCabinPartsNotNested()
            .OfType<Profile>()
            .Sum(p => p.SideTollerance) ?? 0;

        //Set the Adjusted Tollerance
        synthesis.Primary?.SetTotalTolleranceAdjustment(sideTolleranceSecondary + sideTolleranceTertiary);

        return synthesis;
    }

    /// <summary>
    /// Creates a Cabin Synthesis from up to three Cabins
    /// </summary>
    /// <param name="primary">The Primary Model</param>
    /// <param name="secondary">The Secondary Model</param>
    /// <param name="tertiary">The Tertiary Model</param>
    /// <returns>The Cabin Synthesis</returns>
    /// <exception cref="InvalidOperationException">When models have different CabinDrawNumber or when Primary is Null and the rest are Defined</exception>
    public static CabinSynthesis CreateSynthesis(Cabin? primary = null , Cabin? secondary = null , Cabin? tertiary = null)
    {
        bool primaryExists = primary is not null;
        bool secondaryExists = secondary is not null;
        bool tertiaryExists = tertiary is not null;

        if ((primaryExists && secondaryExists && primary!.IsPartOfDraw != secondary!.IsPartOfDraw) ||
            (primaryExists && tertiaryExists && primary!.IsPartOfDraw != tertiary!.IsPartOfDraw) )
        {
            throw new InvalidSynthesisDrawsException(primary?.IsPartOfDraw, secondary?.IsPartOfDraw, tertiary?.IsPartOfDraw);
        }
        if(primaryExists is false && (secondaryExists || tertiaryExists))
        {
            throw new InvalidOperationException($"Synthesis Cannot be Made only with Secondary or Tertiary Models , a Primary must be always present");
        }
        
        CabinSynthesis synthesis = new()
        {
            DrawNo = primary?.IsPartOfDraw ?? CabinDrawNumber.None,
            Primary = primary,
            Secondary = secondary,
            Tertiary = tertiary
        };

        //Pass any Side Tollerance to the Primary (negative for 9F without Tol)
        //Get All the side tollerances sum and adjust the Primary accordingly
        var sideTolleranceSecondary = synthesis.Secondary?
            .Parts
            .GetCabinPartsNotNested()
            .OfType<Profile>()
            .Sum(p => p.SideTollerance) ?? 0;
        var sideTolleranceTertiary = synthesis.Tertiary?
            .Parts
            .GetCabinPartsNotNested()
            .OfType<Profile>()
            .Sum(p => p.SideTollerance) ?? 0;

        //Set the Adjusted Tollerance
        synthesis.Primary?.SetTotalTolleranceAdjustment(sideTolleranceSecondary + sideTolleranceTertiary);

        return synthesis;

    }

    /// <summary>
    /// Returns A Default Cabin for the Selected Draw and Synthesis Model
    /// </summary>
    /// <param name="drawNumber">The Draw Number that this Cabin is Part Of</param>
    /// <param name="synthesisModel">The Synthesis Model of this Cabin</param>
    /// <returns></returns>
    public Cabin? CreateCabin(CabinDrawNumber drawNumber, CabinSynthesisModel synthesisModel)
    {
        switch (drawNumber)
        {
            case CabinDrawNumber.None:
                return null;
            case CabinDrawNumber.Draw9S:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefault9S(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.Draw9S9F:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefault9S(drawNumber, synthesisModel);
                else if (synthesisModel is CabinSynthesisModel.Secondary) return CreateDefault9F(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.Draw9S9F9F:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefault9S(drawNumber, synthesisModel);
                else if (synthesisModel is CabinSynthesisModel.Secondary) return CreateDefault9F(drawNumber, synthesisModel);
                else if (synthesisModel is CabinSynthesisModel.Tertiary) return CreateDefault9F(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.Draw94:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefault94(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.Draw949F:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefault94(drawNumber, synthesisModel);
                else if (synthesisModel is CabinSynthesisModel.Secondary) return CreateDefault9F(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.Draw949F9F:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefault94(drawNumber, synthesisModel);
                else if (synthesisModel is CabinSynthesisModel.Secondary) return CreateDefault9F(drawNumber, synthesisModel);
                else if (synthesisModel is CabinSynthesisModel.Tertiary) return CreateDefault9F(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.Draw9A:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefault9A(drawNumber, synthesisModel);
                else if (synthesisModel is CabinSynthesisModel.Secondary) return CreateDefault9A(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.Draw9A9F:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefault9A(drawNumber, synthesisModel);
                else if (synthesisModel is CabinSynthesisModel.Secondary) return CreateDefault9A(drawNumber, synthesisModel);
                else if (synthesisModel is CabinSynthesisModel.Tertiary) return CreateDefault9F(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.Draw9C:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefault9C(drawNumber, synthesisModel);
                else if (synthesisModel is CabinSynthesisModel.Secondary) return CreateDefault9C(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.Draw9C9F:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefault9C(drawNumber, synthesisModel);
                else if (synthesisModel is CabinSynthesisModel.Secondary) return CreateDefault9C(drawNumber, synthesisModel);
                else if (synthesisModel is CabinSynthesisModel.Tertiary) return CreateDefault9F(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.Draw9B:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefault9B(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.Draw9B9F:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefault9B(drawNumber, synthesisModel);
                else if (synthesisModel is CabinSynthesisModel.Secondary) return CreateDefault9F(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.Draw9B9F9F:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefault9B(drawNumber, synthesisModel);
                else if (synthesisModel is CabinSynthesisModel.Secondary) return CreateDefault9F(drawNumber, synthesisModel);
                else if (synthesisModel is CabinSynthesisModel.Tertiary) return CreateDefault9F(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.DrawVS:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefaultVS(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.DrawVSVF:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefaultVS(drawNumber, synthesisModel);
                else if (synthesisModel is CabinSynthesisModel.Secondary) return CreateDefaultVF(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.DrawV4:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefaultV4(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.DrawV4VF:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefaultV4(drawNumber, synthesisModel);
                else if (synthesisModel is CabinSynthesisModel.Secondary) return CreateDefaultVF(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.DrawVA:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefaultVA(drawNumber, synthesisModel);
                else if (synthesisModel is CabinSynthesisModel.Secondary) return CreateDefaultVA(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.DrawWS:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefaultWS(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.DrawNP44:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefaultNP(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.Draw2CornerNP46:
            case CabinDrawNumber.Draw2StraightNP48:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefaultNP(drawNumber, synthesisModel);
                else if (synthesisModel is CabinSynthesisModel.Secondary) return CreateDefaultNP(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.DrawCornerNP6W45:
            case CabinDrawNumber.DrawStraightNP6W47:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefaultNP(drawNumber, synthesisModel);
                else if (synthesisModel is CabinSynthesisModel.Secondary) return CreateDefaultW(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.DrawNB31:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefaultNB(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.DrawCornerNB6W32:
            case CabinDrawNumber.DrawStraightNB6W38:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefaultNB(drawNumber, synthesisModel);
                else if (synthesisModel is CabinSynthesisModel.Secondary) return CreateDefaultW(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.Draw2CornerNB33:
            case CabinDrawNumber.Draw2StraightNB41:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefaultNB(drawNumber, synthesisModel);
                else if (synthesisModel is CabinSynthesisModel.Secondary) return CreateDefaultNB(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.DrawDB51:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefaultDB(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.DrawCornerDB8W52:
            case CabinDrawNumber.DrawStraightDB8W59:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefaultDB(drawNumber, synthesisModel);
                else if (synthesisModel is CabinSynthesisModel.Secondary) return CreateDefaultW(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.Draw2CornerDB53:
            case CabinDrawNumber.Draw2StraightDB61:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefaultDB(drawNumber, synthesisModel);
                else if (synthesisModel is CabinSynthesisModel.Secondary) return CreateDefaultDB(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.DrawHB34:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefaultHB(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.DrawCornerHB8W35:
            case CabinDrawNumber.DrawStraightHB8W40:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefaultHB(drawNumber, synthesisModel);
                else if (synthesisModel is CabinSynthesisModel.Secondary) return CreateDefaultW(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.Draw2CornerHB37:
            case CabinDrawNumber.Draw2StraightHB43:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefaultHB(drawNumber, synthesisModel);
                else if (synthesisModel is CabinSynthesisModel.Secondary) return CreateDefaultHB(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.Draw8W:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefaultW(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.DrawE:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefaultE(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.Draw8WFlipper81:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefaultW(drawNumber, synthesisModel);
                else if (synthesisModel is CabinSynthesisModel.Secondary) return CreateDefaultWFlipper(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.Draw2Corner8W82:
            case CabinDrawNumber.Draw1Corner8W84:
            case CabinDrawNumber.Draw2Straight8W85:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefaultW(drawNumber, synthesisModel);
                else if (synthesisModel is CabinSynthesisModel.Secondary) return CreateDefaultW(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.Draw2CornerStraight8W88:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefaultW(drawNumber, synthesisModel);
                else if (synthesisModel is CabinSynthesisModel.Secondary) return CreateDefaultW(drawNumber, synthesisModel);
                else if (synthesisModel is CabinSynthesisModel.Tertiary) return CreateDefaultW(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.Draw8W40:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefaultW(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.DrawNV:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefaultNB(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.DrawNV2:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefaultNP(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.DrawMV2:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefaultNP(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.Draw9F:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefault9F(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.DrawVF:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefaultVF(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.DrawQP44:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefaultQP(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.Draw2CornerQP46:
            case CabinDrawNumber.Draw2StraightQP48:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefaultQP(drawNumber, synthesisModel);
                else if (synthesisModel is CabinSynthesisModel.Secondary) return CreateDefaultQP(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.DrawCornerQP6W45:
            case CabinDrawNumber.DrawStraightQP6W47:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefaultQP(drawNumber, synthesisModel);
                else if (synthesisModel is CabinSynthesisModel.Secondary) return CreateDefaultW(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.DrawQB31:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefaultQB(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.DrawCornerQB6W32:
            case CabinDrawNumber.DrawStraightQB6W38:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefaultQB(drawNumber, synthesisModel);
                else if (synthesisModel is CabinSynthesisModel.Secondary) return CreateDefaultW(drawNumber, synthesisModel);
                else return null;
            case CabinDrawNumber.Draw2CornerQB33:
            case CabinDrawNumber.Draw2StraightQB41:
                if (synthesisModel is CabinSynthesisModel.Primary) return CreateDefaultQB(drawNumber, synthesisModel);
                else if (synthesisModel is CabinSynthesisModel.Secondary) return CreateDefaultQB(drawNumber, synthesisModel);
                else return null;
            default:
                return null;
        }
    }

    /// <summary>
    /// Creates a Default Cabin by the Provided Model
    /// </summary>
    /// <param name="model">The Model of the Cabin</param>
    /// <returns>The Default Cabin for this Model or Null if does not exist</returns>
    public Cabin? CreateCabin(CabinModelEnum? model)
    {
        return model switch
        {
            CabinModelEnum.Model9A => CreateDefault9A(CabinDrawNumber.Draw9A, CabinSynthesisModel.Primary),
            CabinModelEnum.Model9S => CreateDefault9S(CabinDrawNumber.Draw9S, CabinSynthesisModel.Primary),
            CabinModelEnum.Model94 => CreateDefault94(CabinDrawNumber.Draw94, CabinSynthesisModel.Primary),
            CabinModelEnum.Model9F => CreateDefault9F(CabinDrawNumber.Draw9F, CabinSynthesisModel.Primary),
            CabinModelEnum.Model9B => CreateDefault9B(CabinDrawNumber.Draw9B, CabinSynthesisModel.Primary),
            CabinModelEnum.ModelW => CreateDefaultW(CabinDrawNumber.Draw8W, CabinSynthesisModel.Primary),
            CabinModelEnum.ModelHB => CreateDefaultHB(CabinDrawNumber.DrawHB34, CabinSynthesisModel.Primary),
            CabinModelEnum.ModelNP => CreateDefaultNP(CabinDrawNumber.DrawNP44, CabinSynthesisModel.Primary),
            CabinModelEnum.ModelQP => CreateDefaultQP(CabinDrawNumber.DrawQP44, CabinSynthesisModel.Primary),
            CabinModelEnum.ModelVS => CreateDefaultVS(CabinDrawNumber.DrawVS, CabinSynthesisModel.Primary),
            CabinModelEnum.ModelVF => CreateDefaultVF(CabinDrawNumber.DrawVF, CabinSynthesisModel.Primary),
            CabinModelEnum.ModelV4 => CreateDefaultV4(CabinDrawNumber.DrawV4, CabinSynthesisModel.Primary),
            CabinModelEnum.ModelVA => CreateDefaultVA(CabinDrawNumber.DrawVA, CabinSynthesisModel.Primary),
            CabinModelEnum.ModelWS => CreateDefaultWS(CabinDrawNumber.DrawWS, CabinSynthesisModel.Primary),
            CabinModelEnum.ModelE => CreateDefaultE(CabinDrawNumber.DrawE, CabinSynthesisModel.Primary),
            CabinModelEnum.ModelWFlipper => CreateDefaultWFlipper(CabinDrawNumber.Draw8WFlipper81, CabinSynthesisModel.Secondary),
            CabinModelEnum.ModelDB => CreateDefaultDB(CabinDrawNumber.DrawDB51, CabinSynthesisModel.Primary),
            CabinModelEnum.ModelNB => CreateDefaultNB(CabinDrawNumber.DrawNB31, CabinSynthesisModel.Primary),
            CabinModelEnum.ModelQB => CreateDefaultQB(CabinDrawNumber.DrawQB31, CabinSynthesisModel.Primary),
            CabinModelEnum.ModelNV => CreateDefaultNB(CabinDrawNumber.DrawNV, CabinSynthesisModel.Primary),
            CabinModelEnum.ModelMV2 => CreateDefaultNP(CabinDrawNumber.DrawMV2, CabinSynthesisModel.Primary),
            CabinModelEnum.ModelNV2 => CreateDefaultNP(CabinDrawNumber.DrawNV2, CabinSynthesisModel.Primary),
            CabinModelEnum.Model9C => CreateDefault9C(CabinDrawNumber.Draw9C, CabinSynthesisModel.Primary),
            CabinModelEnum.Model8W40 => CreateDefaultW(CabinDrawNumber.Draw8W40, CabinSynthesisModel.Primary),
            CabinModelEnum.Model6WA => null,
            null => null,
            _ => null,
        };
    }
    /// <summary>
    /// Creates a Default Cabin from the Provided Settings
    /// </summary>
    /// <param name="settings">The Settings for which to create a Cabin</param>
    /// <param name="defaultReversible">Weather to Keep the Default IsReversible Setting of the Model or Change it according to the Provided Settings</param>
    /// <returns>The Cabin</returns>
    /// <exception cref="InvalidOperationException">When the Provided Model Setting is invalid</exception>
    public Cabin CreateCabin(CabinSettings settings , bool defaultReversible = true)
    {
        var cabin = CreateCabin(settings.Model) 
            ?? throw new InvalidOperationException($"Invalid Setting : {nameof(CabinModelEnum)}");
        
        cabin.MetalFinish = settings.MetalFinish;
        cabin.Thicknesses = settings.Thicknesses;
        cabin.GlassFinish = settings.GlassFinish;
        cabin.Height = settings.Height;
        cabin.NominalLength = settings.NominalLength;
        if (!defaultReversible)
        {
            cabin.IsReversible = settings.IsReversible;
        }

        return cabin;
    }

    #region 1.B6000 Default Initializations
    private Cabin9S CreateDefault9S(CabinDrawNumber drawNumber, CabinSynthesisModel synthesisModel)
    {
        r.AllSettings.TryGetValue((CabinModelEnum.Model9S, drawNumber, synthesisModel), out CabinSettings? s);
        ArgumentNullException.ThrowIfNull(s, "Settings9S");

        r.DefaultLists.TryGetValue((CabinModelEnum.Model9S, drawNumber, synthesisModel), out DefaultPartsList? defaultList);
        ArgumentNullException.ThrowIfNull(defaultList, "DefaultParts9S");

        r.AllConstraints.TryGetValue((CabinModelEnum.Model9S, drawNumber, synthesisModel), out CabinConstraints? constraints);
        ArgumentNullException.ThrowIfNull(constraints, "Constraints9S");
        if (constraints is not Constraints9S) throw new InvalidOperationException($"Expected Constraints where not of the Appropriate Type :{nameof(Constraints9S)}");
        
        Cabin9S cabin = new(new(constraints as Constraints9S),defaultList.ToPartsList<Cabin9SParts>(new(CabinModelEnum.Model9S,drawNumber,synthesisModel),r))
        {
            Model = s.Model,
            IsPartOfDraw = drawNumber,
            MetalFinish = s.MetalFinish,
            SynthesisModel = synthesisModel,
            Thicknesses = s.Thicknesses,
            GlassFinish = s.GlassFinish,
            Height = s.Height,
            NominalLength = s.NominalLength,
            IsReversible = s.IsReversible,
        };
        return cabin;

    }
    private Cabin94 CreateDefault94(CabinDrawNumber drawNumber, CabinSynthesisModel synthesisModel)
    {
        r.AllSettings.TryGetValue((CabinModelEnum.Model94, drawNumber, synthesisModel), out CabinSettings? s);
        ArgumentNullException.ThrowIfNull(s, "Settings94");

        r.DefaultLists.TryGetValue((CabinModelEnum.Model94, drawNumber, synthesisModel), out DefaultPartsList? defaultList);
        ArgumentNullException.ThrowIfNull(defaultList, "DefaultParts94");

        r.AllConstraints.TryGetValue((CabinModelEnum.Model94, drawNumber, synthesisModel), out CabinConstraints? constraints);
        ArgumentNullException.ThrowIfNull(constraints, "Constraints94");
        if (constraints is not Constraints94) throw new InvalidOperationException($"Expected Constraints where not of the Appropriate Type :{nameof(Constraints94)}");
        
        Cabin94 cabin = new(new(constraints as Constraints94), defaultList.ToPartsList<Cabin94Parts>(new(CabinModelEnum.Model94, drawNumber, synthesisModel), r))
        {
            Model = s.Model,
            MetalFinish = s.MetalFinish,
            IsPartOfDraw = drawNumber,
            SynthesisModel = synthesisModel,
            Thicknesses = s.Thicknesses,
            GlassFinish = s.GlassFinish,
            Height = s.Height,
            NominalLength = s.NominalLength,
            IsReversible = s.IsReversible,
        };

        return cabin;
    }
    private Cabin9A CreateDefault9A(CabinDrawNumber drawNumber, CabinSynthesisModel synthesisModel)
    {
        r.AllSettings.TryGetValue((CabinModelEnum.Model9A, drawNumber, synthesisModel), out CabinSettings? s);
        ArgumentNullException.ThrowIfNull(s, "Settings9A");

        r.DefaultLists.TryGetValue((CabinModelEnum.Model9A, drawNumber, synthesisModel), out DefaultPartsList? defaultList);
        ArgumentNullException.ThrowIfNull(defaultList, "DefaultParts9A");

        r.AllConstraints.TryGetValue((CabinModelEnum.Model9A, drawNumber, synthesisModel), out CabinConstraints? constraints);
        ArgumentNullException.ThrowIfNull(constraints, "Constraints9A");
        if (constraints is not Constraints9A) throw new InvalidOperationException($"Expected Constraints where not of the Appropriate Type :{nameof(Constraints9A)}");
        
        Cabin9A cabin = new(new(constraints as Constraints9A), defaultList.ToPartsList<Cabin9AParts>(new(CabinModelEnum.Model9A, drawNumber, synthesisModel), r))
        {
            Model = s.Model,
            MetalFinish = s.MetalFinish,
            IsPartOfDraw = drawNumber,
            SynthesisModel = synthesisModel,
            Thicknesses = s.Thicknesses,
            GlassFinish = s.GlassFinish,
            Height = s.Height,
            NominalLength = s.NominalLength,
            IsReversible = s.IsReversible,
        };
        return cabin;
    }
    private Cabin9B CreateDefault9B(CabinDrawNumber drawNumber, CabinSynthesisModel synthesisModel)
    {
        r.AllSettings.TryGetValue((CabinModelEnum.Model9B, drawNumber, synthesisModel), out CabinSettings? s);
        ArgumentNullException.ThrowIfNull(s, "Settings9B");

        r.DefaultLists.TryGetValue((CabinModelEnum.Model9B, drawNumber, synthesisModel), out DefaultPartsList? defaultList);
        ArgumentNullException.ThrowIfNull(defaultList, "DefaultParts9B");

        r.AllConstraints.TryGetValue((CabinModelEnum.Model9B, drawNumber, synthesisModel), out CabinConstraints? constraints);
        ArgumentNullException.ThrowIfNull(constraints, "Constraints9B");
        if (constraints is not Constraints9B) throw new InvalidOperationException($"Expected Constraints where not of the Appropriate Type :{nameof(Constraints9B)}");

        Cabin9B cabin = new(new(constraints as Constraints9B), defaultList.ToPartsList<Cabin9BParts>(new(CabinModelEnum.Model9B, drawNumber, synthesisModel), r))
        {
            Model = s.Model,
            MetalFinish = s.MetalFinish,
            IsPartOfDraw = drawNumber,
            SynthesisModel = synthesisModel,
            Thicknesses = s.Thicknesses,
            GlassFinish = s.GlassFinish,
            Height = s.Height,
            NominalLength = s.NominalLength,
            IsReversible = s.IsReversible,
        };
        return cabin;
    }
    private Cabin9F CreateDefault9F(CabinDrawNumber drawNumber, CabinSynthesisModel synthesisModel)
    {
        r.AllSettings.TryGetValue((CabinModelEnum.Model9F, drawNumber, synthesisModel), out CabinSettings? s);
        ArgumentNullException.ThrowIfNull(s, "Settings9F");

        r.DefaultLists.TryGetValue((CabinModelEnum.Model9F, drawNumber, synthesisModel), out DefaultPartsList? defaultList);
        ArgumentNullException.ThrowIfNull(defaultList, "DefaultParts9F");

        r.AllConstraints.TryGetValue((CabinModelEnum.Model9F, drawNumber, synthesisModel), out CabinConstraints? constraints);
        ArgumentNullException.ThrowIfNull(constraints, "Constraints9F");
        if (constraints is not Constraints9F) throw new InvalidOperationException($"Expected Constraints where not of the Appropriate Type :{nameof(Constraints9F)}");
       
        Cabin9F cabin = new(new(constraints as Constraints9F), defaultList.ToPartsList<Cabin9FParts>(new(CabinModelEnum.Model9F, drawNumber, synthesisModel), r))
        {
            Model = s.Model,
            MetalFinish = s.MetalFinish,
            IsPartOfDraw = drawNumber,
            SynthesisModel = synthesisModel,
            Thicknesses = s.Thicknesses,
            GlassFinish = s.GlassFinish,
            Height = s.Height,
            NominalLength = s.NominalLength,
            IsReversible = s.IsReversible,
        };
        return cabin;
    }
    private Cabin9C CreateDefault9C(CabinDrawNumber drawNumber, CabinSynthesisModel synthesisModel)
    {
        r.AllSettings.TryGetValue((CabinModelEnum.Model9C, drawNumber, synthesisModel), out CabinSettings? s);
        ArgumentNullException.ThrowIfNull(s, "Settings9C");

        r.DefaultLists.TryGetValue((CabinModelEnum.Model9C, drawNumber, synthesisModel), out DefaultPartsList? defaultList);
        ArgumentNullException.ThrowIfNull(defaultList, "DefaultParts9C");

        r.AllConstraints.TryGetValue((CabinModelEnum.Model9C, drawNumber, synthesisModel), out CabinConstraints? constraints);
        ArgumentNullException.ThrowIfNull(constraints, "Constraints9C");
        if (constraints is not Constraints9C) throw new InvalidOperationException($"Expected Constraints where not of the Appropriate Type :{nameof(Constraints9C)}");
       
        Cabin9C cabin = new(new(constraints as Constraints9C), defaultList.ToPartsList<Cabin9CParts>(new(CabinModelEnum.Model9C, drawNumber, synthesisModel), r))
        {
            Model = s.Model,
            MetalFinish = s.MetalFinish,
            IsPartOfDraw = drawNumber,
            SynthesisModel = synthesisModel,
            Thicknesses = s.Thicknesses,
            GlassFinish = s.GlassFinish,
            Height = s.Height,
            NominalLength = s.NominalLength,
            IsReversible = s.IsReversible,
        };

        return cabin;
    }

    #endregion

    #region 2.DB Default Initilizations

    private CabinDB CreateDefaultDB(CabinDrawNumber drawNumber, CabinSynthesisModel synthesisModel)
    {
        r.AllSettings.TryGetValue((CabinModelEnum.ModelDB, drawNumber, synthesisModel), out CabinSettings? s);
        ArgumentNullException.ThrowIfNull(s, "SettingsDB");

        r.DefaultLists.TryGetValue((CabinModelEnum.ModelDB, drawNumber, synthesisModel), out DefaultPartsList? defaultList);
        ArgumentNullException.ThrowIfNull(defaultList, "DefaultPartsDB");

        r.AllConstraints.TryGetValue((CabinModelEnum.ModelDB, drawNumber, synthesisModel), out CabinConstraints? constraints);
        ArgumentNullException.ThrowIfNull(constraints, "ConstraintsDB");
        if (constraints is not ConstraintsDB) throw new InvalidOperationException($"Expected Constraints where not of the Appropriate Type :{nameof(ConstraintsDB)}");
        
        CabinDB cabin = new(new(constraints as ConstraintsDB), defaultList.ToPartsList<CabinDBParts>(new(CabinModelEnum.ModelDB, drawNumber, synthesisModel), r))
        {
            Model = s.Model,
            MetalFinish = s.MetalFinish,
            IsPartOfDraw = drawNumber,
            SynthesisModel = synthesisModel,
            Thicknesses = s.Thicknesses,
            GlassFinish = s.GlassFinish,
            Height = s.Height,
            NominalLength = s.NominalLength,
            IsReversible = s.IsReversible,
        };
        return cabin;
    }

    #endregion

    #region 3.Free Default Initilizations

    private CabinE CreateDefaultE(CabinDrawNumber drawNumber, CabinSynthesisModel synthesisModel)
    {
        r.AllSettings.TryGetValue((CabinModelEnum.ModelE, drawNumber, synthesisModel), out CabinSettings? s);
        ArgumentNullException.ThrowIfNull(s, "SettingsE");

        r.DefaultLists.TryGetValue((CabinModelEnum.ModelE, drawNumber, synthesisModel), out DefaultPartsList? defaultList);
        ArgumentNullException.ThrowIfNull(defaultList, "DefaultPartsE");

        r.AllConstraints.TryGetValue((CabinModelEnum.ModelE, drawNumber, synthesisModel), out CabinConstraints? constraints);
        ArgumentNullException.ThrowIfNull(constraints, "ConstraintsE");
        if (constraints is not ConstraintsE) throw new InvalidOperationException($"Expected Constraints where not of the Appropriate Type :{nameof(ConstraintsE)}");

        CabinE cabin = new(new(constraints as ConstraintsE), defaultList.ToPartsList<CabinEParts>(new(CabinModelEnum.ModelE, drawNumber, synthesisModel), r))
        {
            Model = s.Model,
            MetalFinish = s.MetalFinish,
            IsPartOfDraw = drawNumber,
            SynthesisModel = synthesisModel,
            Thicknesses = s.Thicknesses,
            GlassFinish = s.GlassFinish,
            Height = s.Height,
            NominalLength = s.NominalLength,
            IsReversible = s.IsReversible,
        };
        return cabin;
    }
    private CabinW CreateDefaultW(CabinDrawNumber drawNumber, CabinSynthesisModel synthesisModel)
    {
        var model = drawNumber is CabinDrawNumber.Draw8W40
            ? CabinModelEnum.Model8W40
            : CabinModelEnum.ModelW;

        r.AllSettings.TryGetValue((model, drawNumber, synthesisModel), out CabinSettings? s);
        ArgumentNullException.ThrowIfNull(s, "SettingsW");

        r.DefaultLists.TryGetValue((model, drawNumber, synthesisModel), out DefaultPartsList? defaultList);
        ArgumentNullException.ThrowIfNull(defaultList, "DefaultPartsW");

        r.AllConstraints.TryGetValue((model, drawNumber, synthesisModel), out CabinConstraints? constraints);
        ArgumentNullException.ThrowIfNull(constraints, "ConstraintsW");
        if (constraints is not ConstraintsW) throw new InvalidOperationException($"Expected Constraints where not of the Appropriate Type :{nameof(ConstraintsW)}");

        CabinW cabin = new(new(constraints as ConstraintsW), defaultList.ToPartsList<CabinWParts>(new(model, drawNumber, synthesisModel), r))
        {
            Model = s.Model,
            IsPartOfDraw = drawNumber,
            SynthesisModel = synthesisModel,
            MetalFinish = s.MetalFinish,
            Thicknesses = s.Thicknesses,
            GlassFinish= s.GlassFinish,
            Height= s.Height,
            NominalLength = s.NominalLength,
            IsReversible = s.IsReversible,
        };
        return cabin;
    }
    private CabinWFlipper CreateDefaultWFlipper(CabinDrawNumber drawNumber, CabinSynthesisModel synthesisModel)
    {
        r.AllSettings.TryGetValue((CabinModelEnum.ModelWFlipper, drawNumber, synthesisModel), out CabinSettings? s);
        ArgumentNullException.ThrowIfNull(s, "SettingsWFlipper");

        r.DefaultLists.TryGetValue((CabinModelEnum.ModelWFlipper, drawNumber, synthesisModel), out DefaultPartsList? defaultList);
        ArgumentNullException.ThrowIfNull(defaultList, "DefaultPartsWFlipper");

        r.AllConstraints.TryGetValue((CabinModelEnum.ModelWFlipper, drawNumber, synthesisModel), out CabinConstraints? constraints);
        ArgumentNullException.ThrowIfNull(constraints, "ConstraintsWFlipper");
        if (constraints is not ConstraintsWFlipper) throw new InvalidOperationException($"Expected Constraints where not of the Appropriate Type :{nameof(ConstraintsWFlipper)}");
        
        CabinWFlipper cabin = new(new(constraints as ConstraintsWFlipper), defaultList.ToPartsList<CabinWFlipperParts>(new(CabinModelEnum.ModelWFlipper, drawNumber, synthesisModel), r))
        {
            Model = s.Model,
            IsPartOfDraw = drawNumber,
            SynthesisModel = synthesisModel,
            MetalFinish = s.MetalFinish,
            Thicknesses = s.Thicknesses,
            GlassFinish = s.GlassFinish,
            Height = s.Height,
            NominalLength = s.NominalLength,
            IsReversible = s.IsReversible,
        };
        return cabin;
    }
    #endregion

    #region 4.HB Default Initilizations

    private CabinHB CreateDefaultHB(CabinDrawNumber drawNumber, CabinSynthesisModel synthesisModel)
    {
        r.AllSettings.TryGetValue((CabinModelEnum.ModelHB, drawNumber, synthesisModel), out CabinSettings? s);
        ArgumentNullException.ThrowIfNull(s, "SettingsHB");

        r.DefaultLists.TryGetValue((CabinModelEnum.ModelHB, drawNumber, synthesisModel), out DefaultPartsList? defaultList);
        ArgumentNullException.ThrowIfNull(defaultList, "DefaultPartsHB");

        r.AllConstraints.TryGetValue((CabinModelEnum.ModelHB, drawNumber, synthesisModel), out CabinConstraints? constraints);
        ArgumentNullException.ThrowIfNull(constraints, "ConstraintsHB");
        if (constraints is not ConstraintsHB) throw new InvalidOperationException($"Expected Constraints where not of the Appropriate Type :{nameof(ConstraintsHB)}");

        CabinHB cabin = new(new(constraints as ConstraintsHB), defaultList.ToPartsList<CabinHBParts>(new(CabinModelEnum.ModelHB, drawNumber, synthesisModel), r))
        {
            Model = s.Model,
            IsPartOfDraw = drawNumber,
            SynthesisModel = synthesisModel,
            MetalFinish = s.MetalFinish,
            Thicknesses = s.Thicknesses,
            GlassFinish = s.GlassFinish,
            Height = s.Height,
            NominalLength = s.NominalLength,
            IsReversible = s.IsReversible,
        };
        return cabin;
    }

    #endregion

    #region 5.Inox304 Default Initilizations

    private CabinVS CreateDefaultVS(CabinDrawNumber drawNumber, CabinSynthesisModel synthesisModel)
    {
        r.AllSettings.TryGetValue((CabinModelEnum.ModelVS, drawNumber, synthesisModel), out CabinSettings? s);
        ArgumentNullException.ThrowIfNull(s, "SettingsVS");

        r.DefaultLists.TryGetValue((CabinModelEnum.ModelVS, drawNumber, synthesisModel), out DefaultPartsList? defaultList);
        ArgumentNullException.ThrowIfNull(defaultList, "DefaultPartsVS");

        r.AllConstraints.TryGetValue((CabinModelEnum.ModelVS, drawNumber, synthesisModel), out CabinConstraints? constraints);
        ArgumentNullException.ThrowIfNull(constraints, "ConstraintsVS");
        if (constraints is not ConstraintsVS) throw new InvalidOperationException($"Expected Constraints where not of the Appropriate Type :{nameof(ConstraintsVS)}");

        CabinVS cabin = new(new(constraints as ConstraintsVS), defaultList.ToPartsList<CabinVSParts>(new(CabinModelEnum.ModelVS, drawNumber, synthesisModel), r))
        {
            Model = s.Model,
            IsPartOfDraw = drawNumber,
            SynthesisModel = synthesisModel,
            MetalFinish = s.MetalFinish,
            Thicknesses = s.Thicknesses,
            GlassFinish = s.GlassFinish,
            Height = s.Height,
            NominalLength = s.NominalLength,
            IsReversible = s.IsReversible,
        };
        return cabin;
    }
    private CabinV4 CreateDefaultV4(CabinDrawNumber drawNumber, CabinSynthesisModel synthesisModel)
    {
        r.AllSettings.TryGetValue((CabinModelEnum.ModelV4, drawNumber, synthesisModel), out CabinSettings? s);
        ArgumentNullException.ThrowIfNull(s, "SettingsV4");

        r.DefaultLists.TryGetValue((CabinModelEnum.ModelV4, drawNumber, synthesisModel), out DefaultPartsList? defaultList);
        ArgumentNullException.ThrowIfNull(defaultList, "DefaultPartsV4");

        r.AllConstraints.TryGetValue((CabinModelEnum.ModelV4, drawNumber, synthesisModel), out CabinConstraints? constraints);
        ArgumentNullException.ThrowIfNull(constraints, "ConstraintsV4");
        if (constraints is not ConstraintsV4) throw new InvalidOperationException($"Expected Constraints where not of the Appropriate Type :{nameof(ConstraintsV4)}");

        CabinV4 cabin = new(new(constraints as ConstraintsV4), defaultList.ToPartsList<CabinV4Parts>(new(CabinModelEnum.ModelV4, drawNumber, synthesisModel), r))
        {
            Model = s.Model,
            IsPartOfDraw = drawNumber,
            SynthesisModel = synthesisModel,
            MetalFinish = s.MetalFinish,
            Thicknesses = s.Thicknesses,
            GlassFinish = s.GlassFinish,
            Height = s.Height,
            NominalLength = s.NominalLength,
            IsReversible = s.IsReversible,
        };
        return cabin;
    }
    private CabinVA CreateDefaultVA(CabinDrawNumber drawNumber, CabinSynthesisModel synthesisModel)
    {
        r.AllSettings.TryGetValue((CabinModelEnum.ModelVA, drawNumber, synthesisModel), out CabinSettings? s);
        ArgumentNullException.ThrowIfNull(s, "SettingsVA");

        r.DefaultLists.TryGetValue((CabinModelEnum.ModelVA, drawNumber, synthesisModel), out DefaultPartsList? defaultList);
        ArgumentNullException.ThrowIfNull(defaultList, "DefaultPartsVA");

        r.AllConstraints.TryGetValue((CabinModelEnum.ModelVA, drawNumber, synthesisModel), out CabinConstraints? constraints);
        ArgumentNullException.ThrowIfNull(constraints, "ConstraintsVA");
        if (constraints is not ConstraintsVA) throw new InvalidOperationException($"Expected Constraints where not of the Appropriate Type :{nameof(ConstraintsVA)}");

        CabinVA cabin = new(new(constraints as ConstraintsVA), defaultList.ToPartsList<CabinVAParts>(new(CabinModelEnum.ModelVA, drawNumber, synthesisModel), r))
        {
            Model = s.Model,
            IsPartOfDraw = drawNumber,
            SynthesisModel = synthesisModel,
            MetalFinish = s.MetalFinish,
            Thicknesses = s.Thicknesses,
            GlassFinish = s.GlassFinish,
            Height = s.Height,
            NominalLength = s.NominalLength,
            IsReversible = s.IsReversible,
        };
        return cabin;
    }
    private CabinVF CreateDefaultVF(CabinDrawNumber drawNumber, CabinSynthesisModel synthesisModel)
    {
        r.AllSettings.TryGetValue((CabinModelEnum.ModelVF, drawNumber, synthesisModel), out CabinSettings? s);
        ArgumentNullException.ThrowIfNull(s, "SettingsVF");

        r.DefaultLists.TryGetValue((CabinModelEnum.ModelVF, drawNumber, synthesisModel), out DefaultPartsList? defaultList);
        ArgumentNullException.ThrowIfNull(defaultList, "DefaultPartsVF");

        r.AllConstraints.TryGetValue((CabinModelEnum.ModelVF, drawNumber, synthesisModel), out CabinConstraints? constraints);
        ArgumentNullException.ThrowIfNull(constraints, "ConstraintsVF");
        if (constraints is not ConstraintsVF) throw new InvalidOperationException($"Expected Constraints where not of the Appropriate Type :{nameof(ConstraintsVF)}");

        CabinVF cabin = new(new(constraints as ConstraintsVF), defaultList.ToPartsList<CabinVFParts>(new(CabinModelEnum.ModelVF, drawNumber, synthesisModel), r))
        {
            Model = s.Model,
            IsPartOfDraw = drawNumber,
            SynthesisModel = synthesisModel,
            MetalFinish = s.MetalFinish,
            Thicknesses = s.Thicknesses,
            GlassFinish = s.GlassFinish,
            Height = s.Height,
            NominalLength = s.NominalLength,
            IsReversible = s.IsReversible,
        };
        return cabin;
    }

    #endregion

    #region 6.NB Default Initilizations

    private CabinNB CreateDefaultNB(CabinDrawNumber drawNumber, CabinSynthesisModel synthesisModel)
    {
        var model = drawNumber is CabinDrawNumber.DrawNV
            ? CabinModelEnum.ModelNV
            : CabinModelEnum.ModelNB;

        r.AllSettings.TryGetValue((model, drawNumber, synthesisModel), out CabinSettings? s);
        ArgumentNullException.ThrowIfNull(s, "SettingsNB");

        r.DefaultLists.TryGetValue((model, drawNumber, synthesisModel), out DefaultPartsList? defaultList);
        ArgumentNullException.ThrowIfNull(defaultList, "DefaultPartsNB");

        r.AllConstraints.TryGetValue((model, drawNumber, synthesisModel), out CabinConstraints? constraints);
        ArgumentNullException.ThrowIfNull(constraints, "ConstraintsNB");
        if (constraints is not ConstraintsNB) throw new InvalidOperationException($"Expected Constraints where not of the Appropriate Type :{nameof(ConstraintsNB)}");

        CabinNB cabin = new(new(constraints as ConstraintsNB), defaultList.ToPartsList<CabinNBParts>(new(model, drawNumber, synthesisModel), r))
        {
            Model = s.Model,
            IsPartOfDraw = drawNumber,
            SynthesisModel = synthesisModel,
            MetalFinish = s.MetalFinish,
            Thicknesses = s.Thicknesses,
            GlassFinish = s.GlassFinish,
            Height = s.Height,
            NominalLength = s.NominalLength,
            IsReversible = s.IsReversible,
        };
        return cabin;
    }
    private CabinNB CreateDefaultQB(CabinDrawNumber drawNumber, CabinSynthesisModel synthesisModel)
    {
        var model = CabinModelEnum.ModelQB;

        r.AllSettings.TryGetValue((model, drawNumber, synthesisModel), out CabinSettings? s);
        ArgumentNullException.ThrowIfNull(s, "SettingsQB");

        r.DefaultLists.TryGetValue((model, drawNumber, synthesisModel), out DefaultPartsList? defaultList);
        ArgumentNullException.ThrowIfNull(defaultList, "DefaultPartsQB");

        r.AllConstraints.TryGetValue((model, drawNumber, synthesisModel), out CabinConstraints? constraints);
        ArgumentNullException.ThrowIfNull(constraints, "ConstraintsQB");
        if (constraints is not ConstraintsNB) throw new InvalidOperationException($"Expected Constraints where not of the Appropriate Type :{nameof(ConstraintsNB)}");

        CabinNB cabin = new(new(constraints as ConstraintsNB), defaultList.ToPartsList<CabinNBParts>(new(model, drawNumber, synthesisModel), r))
        {
            Model = s.Model,
            IsPartOfDraw = drawNumber,
            SynthesisModel = synthesisModel,
            MetalFinish = s.MetalFinish,
            Thicknesses = s.Thicknesses,
            GlassFinish = s.GlassFinish,
            Height = s.Height,
            NominalLength = s.NominalLength,
            IsReversible = s.IsReversible,
        };
        return cabin;
    }

    #endregion

    #region 7.NP-QP Default Initilizations

    private CabinNP CreateDefaultNP(CabinDrawNumber drawNumber, CabinSynthesisModel synthesisModel)
    {
        var model = drawNumber switch
        {
            CabinDrawNumber.DrawMV2 => CabinModelEnum.ModelMV2,
            CabinDrawNumber.DrawNV2 => CabinModelEnum.ModelNV2,
            _ => CabinModelEnum.ModelNP,
        };
        r.AllSettings.TryGetValue((model, drawNumber, synthesisModel), out CabinSettings? s);
        ArgumentNullException.ThrowIfNull(s, "SettingsNP");

        r.DefaultLists.TryGetValue((model, drawNumber, synthesisModel), out DefaultPartsList? defaultList);
        ArgumentNullException.ThrowIfNull(defaultList, "DefaultPartsNP");

        r.AllConstraints.TryGetValue((model, drawNumber, synthesisModel), out CabinConstraints? constraints);
        ArgumentNullException.ThrowIfNull(constraints, "ConstraintsNP");
        if (constraints is not ConstraintsNP) throw new InvalidOperationException($"Expected Constraints where not of the Appropriate Type :{nameof(ConstraintsNP)}");

        CabinNP cabin = new(new(constraints as ConstraintsNP), defaultList.ToPartsList<CabinNPParts>(new(model, drawNumber, synthesisModel), r))
        {
            Model = s.Model,
            IsPartOfDraw = drawNumber,
            SynthesisModel = synthesisModel,
            MetalFinish = s.MetalFinish,
            Thicknesses = s.Thicknesses,
            GlassFinish = s.GlassFinish,
            Height = s.Height,
            NominalLength = s.NominalLength,
            IsReversible = s.IsReversible,
        };
        return cabin;
    }
    private CabinNP CreateDefaultQP(CabinDrawNumber drawNumber, CabinSynthesisModel synthesisModel)
    {
        var model = CabinModelEnum.ModelQP;

        r.AllSettings.TryGetValue((model, drawNumber, synthesisModel), out CabinSettings? s);
        ArgumentNullException.ThrowIfNull(s, "SettingsQP");

        r.DefaultLists.TryGetValue((model, drawNumber, synthesisModel), out DefaultPartsList? defaultList);
        ArgumentNullException.ThrowIfNull(defaultList, "DefaultPartsQP");

        r.AllConstraints.TryGetValue((model, drawNumber, synthesisModel), out CabinConstraints? constraints);
        ArgumentNullException.ThrowIfNull(constraints, "ConstraintsQP");
        if (constraints is not ConstraintsNP) throw new InvalidOperationException($"Expected Constraints where not of the Appropriate Type :{nameof(ConstraintsNP)}");

        CabinNP cabin = new(new(constraints as ConstraintsNP), defaultList.ToPartsList<CabinNPParts>(new(model, drawNumber, synthesisModel), r))
        {
            Model = s.Model,
            IsPartOfDraw = drawNumber,
            SynthesisModel = synthesisModel,
            MetalFinish = s.MetalFinish,
            Thicknesses = s.Thicknesses,
            GlassFinish = s.GlassFinish,
            Height = s.Height,
            NominalLength = s.NominalLength,
            IsReversible = s.IsReversible,
        };
        return cabin;
    }

    #endregion

    #region 8.WS Default Initilizations

    private CabinWS CreateDefaultWS(CabinDrawNumber drawNumber, CabinSynthesisModel synthesisModel)
    {
        r.AllSettings.TryGetValue((CabinModelEnum.ModelWS, drawNumber, synthesisModel), out CabinSettings? s);
        ArgumentNullException.ThrowIfNull(s, "SettingsWS");

        r.DefaultLists.TryGetValue((CabinModelEnum.ModelWS, drawNumber, synthesisModel), out DefaultPartsList? defaultList);
        ArgumentNullException.ThrowIfNull(defaultList, "DefaultPartsWS");

        r.AllConstraints.TryGetValue((CabinModelEnum.ModelWS, drawNumber, synthesisModel), out CabinConstraints? constraints);
        ArgumentNullException.ThrowIfNull(constraints, "ConstraintsWS");
        if (constraints is not ConstraintsWS) throw new InvalidOperationException($"Expected Constraints where not of the Appropriate Type :{nameof(ConstraintsWS)}");

        CabinWS cabin = new(new(constraints as ConstraintsWS), defaultList.ToPartsList<CabinWSParts>(new(CabinModelEnum.ModelWS, drawNumber, synthesisModel), r))
        {
            Model = s.Model,
            IsPartOfDraw = drawNumber,
            SynthesisModel = synthesisModel,
            MetalFinish = s.MetalFinish,
            Thicknesses = s.Thicknesses,
            GlassFinish = s.GlassFinish,
            Height = s.Height,
            NominalLength = s.NominalLength,
            IsReversible = s.IsReversible,
        };
        return cabin;
    }

    #endregion

}
