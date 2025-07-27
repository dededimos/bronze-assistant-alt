using SVGCabinDraws;
using SVGCabinDraws.ConcreteDraws.B6000Draws;
using SVGCabinDraws.ConcreteDraws.DBDraws;
using SVGCabinDraws.ConcreteDraws.NBDraws;
using SVGCabinDraws.ConcreteDraws.NPDraws;
using SVGDrawingLibrary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BronzeFactoryApplication.ViewModels.ComponentsUCViewModels.DrawsViewModels
{
    public partial class SynthesisDrawViewModel : BaseViewModel
    {
        private SynthesisDraw? draw;
        private readonly GlassesBuilderDirector glassesBuilder;
        private readonly CabinValidator validator;

        [ObservableProperty]
        private int selectedDrawIndex;

        public void SetIndexToGlasses()
        {
            //The 4th container (Index No3) is always the one with the Glasses
            SelectedDrawIndex = 3;
        }

        public IEnumerable<DrawShape> FrontShapes { get => draw?.FrontSideShapes ?? Enumerable.Empty<DrawShape>(); }
        public bool IsFrontDrawAvailable { get => draw?.IsFrontDrawAvailable ?? false; }
        public IEnumerable<DrawShape> RightShapes { get => draw?.RightSideShapes ?? Enumerable.Empty<DrawShape>(); }
        public bool IsRightDrawAvailable { get => draw?.IsRightDrawAvailable ?? false; }
        public IEnumerable<DrawShape> LeftShapes { get => draw?.LeftSideShapes ?? Enumerable.Empty<DrawShape>(); }
        public bool IsLeftDrawAvailable { get => draw?.IsLeftDrawAvailable ?? false; }
        public bool IsAnyDrawAvailable { get => IsFrontDrawAvailable || IsRightDrawAvailable || IsLeftDrawAvailable || IsGlassesDrawsAvailable; }
        public bool IsGlassesDrawsAvailable { get => GlassesDraws.Count > 0; }
        public ObservableCollection<GlassDrawViewModel> GlassesDraws { get; set; } = new();

        public SynthesisDrawViewModel(GlassesBuilderDirector glassesBuilder , CabinValidator validator)
        {
            this.glassesBuilder = glassesBuilder;
            this.validator = validator;
        }

        public void SetSynthesis(CabinSynthesis? synthesis)
        {
            if(synthesis != null)
            {
                draw = new(synthesis, glassesBuilder, validator);
                foreach (var item in draw.FrontSideShapes.Concat(draw.RightSideShapes).Concat(draw.LeftSideShapes))
                {
                    item.Fill = "white";
                    item.Stroke = "black";
                    if (item.Name.Contains("GlassDraw")) item.Opacity = "0.8";
                }
                draw.PaintMetalParts("gray", "black");
                draw.PaintGlasses("aliceblue", "black");
                draw.PaintHelperDraws("purple", "black");
                draw.PaintPolycarbonics("white", "black");
            }
            else
            {
                draw = null;
            }
            GenerateGlassDraws(synthesis);
            CheckSelectedIndexDrawExists();
            OnPropertyChanged("");
        }

        private void GenerateGlassDraws(CabinSynthesis? synthesis)
        {
            GlassesDraws.Clear();
            if (synthesis is null)
            {
                return;
            }

            foreach (Cabin cabin in synthesis.GetCabinList())
            {
                foreach (Glass glass in cabin.Glasses)
                {
                    var draw = new GlassDrawViewModel();
                    var glassVm = new GlassViewModel();
                    glassVm.SetGlass(glass,true);
                    draw.SetGlassDraw(glassVm);
                    GlassesDraws.Add(draw);
                }
            }
        }


        /// <summary>
        /// Checks if the Selected Draw Exists otherwise falls back to Primary otherwise falls back to -1 (Nothing)
        /// Does not Raise Property Change Events
        /// </summary>
        private void CheckSelectedIndexDrawExists()
        {
            if(SelectedDrawIndex is -1 && IsFrontDrawAvailable)
            {
                SelectedDrawIndex = 0;
            }
            else if (SelectedDrawIndex is 0 && !IsFrontDrawAvailable)
            {
                SelectedDrawIndex = -1;
            }
            else if (SelectedDrawIndex is 1 && !IsRightDrawAvailable)
            {
                if (IsFrontDrawAvailable) SelectedDrawIndex = 0;
                else
                    SelectedDrawIndex = -1;
            }
            else if (SelectedDrawIndex is 2 && !IsLeftDrawAvailable)
            {
                if (IsFrontDrawAvailable) SelectedDrawIndex = 0;
                else SelectedDrawIndex = -1;
            }
        }
        private void test()
        {
            
        }
    }
}
