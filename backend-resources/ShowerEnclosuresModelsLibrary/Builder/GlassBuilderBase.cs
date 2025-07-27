using ShowerEnclosuresModelsLibrary.Builder.GlassBuilderHelperMethods;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Builder;

/// <summary>
/// The Base GlassBuilder Class , all Glass Builders Inherit From this Base
/// Takes Care of any Overriding Options
/// </summary>
/// <typeparam name="T">The Type of Cabin for Which the Builder will build a Glass</typeparam>
public abstract class GlassBuilderBase<T> : IGlassBuilder
    where T : Cabin
{
    protected readonly T cabin;
    protected readonly GlassBuilderOptions options;
    protected Glass glass;

    public GlassBuilderBase(T cabin , GlassBuilderOptions options = null)
    {
        this.cabin = cabin;
        glass = new();
        this.options = options ?? new GlassBuilderOptions();
    }

    public Glass GetGlass()
    {
        Glass builtGlass = glass;
        glass = new();
        return builtGlass;
    }
    public IGlassBuilder SetGlassDraw()
    {
        if (options.DrawShouldOverride)
        {
            glass.Draw = options.DrawOverride;
        }
        else
        {
            SetDefaultGlassDraw();
        }
        return this;
    }
    public abstract void SetDefaultGlassDraw();

    public IGlassBuilder SetGlassFinish()
    {
        if (options.FinishShouldOverride)
        {
            glass.Finish = options.FinishOverride;
        }
        else
        {
            SetDefaultGlassFinish();
        }
        return this;
    }
    public abstract void SetDefaultGlassFinish();

    public IGlassBuilder SetGlassHeight()
    {
        if (options.HeightShouldOverride)
        {
            glass.Height = options.HeightOverride;
        }
        else
        {
            SetDefaultGlassHeight();
        }
        return this;
    }
    public abstract void SetDefaultGlassHeight();

    public IGlassBuilder SetGlassLength()
    {
        if (options.LengthShouldOverride)
        {
            glass.Length = options.LengthOverride;
        }
        else
        {
            SetDefaultGlassLength();
        }

        return this;
    }
    public abstract void SetDefaultGlassLength();

    public IGlassBuilder SetGlassStepHeight()
    {
        if (options.StepHeightShouldOverride)
        {
            glass.StepHeight = options.StepHeightOverride;
        }
        else
        {
            SetDefaultGlassStepHeight();
        }
        return this;
    }
    public abstract void SetDefaultGlassStepHeight();

    public IGlassBuilder SetGlassStepLength()
    {
        if (options.StepLengthShouldOverride)
        {
            glass.StepLength = options.StepLengthOverride;
        }
        else
        {
            SetDefaultGlassStepLength();
        }
        return this;
    }
    public abstract void SetDefaultGlassStepLength();

    public IGlassBuilder SetGlassThickness()
    {
        if (options.ThicknessShouldOverride)
        {
            glass.Thickness = options.ThicknessOverride;
        }
        else
        {
            SetDefaultGlassThickness();
        }
        return this;
    }
    public abstract void SetDefaultGlassThickness();

    public IGlassBuilder SetGlassType()
    {
        if (options.TypeShouldOverride)
        {
            glass.GlassType = options.TypeOverride;
        }
        else
        {
            SetDefaultGlassType();
        }
        return this;
    }
    public abstract void SetDefaultGlassType();

    public IGlassBuilder SetCornerRadius()
    {
        //First
        SetDefaultCornerRadius();

        //Then change if anything is overriden
        if (options.CornerRadiusTopLeftShouldOverride)
        {
            glass.CornerRadiusTopLeft = options.CornerRadiusTopLeftOverride;
        }

        if (options.CornerRadiusTopRightShouldOverride)
        {
            glass.CornerRadiusTopRight = options.CornerRadiusTopRightOverride;
        }
        return this;
    }
    public abstract void SetDefaultCornerRadius();
}
