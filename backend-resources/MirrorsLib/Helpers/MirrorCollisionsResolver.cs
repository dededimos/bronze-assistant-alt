using Microsoft.Extensions.Logging;
using MirrorsLib.MirrorElements;
using MirrorsLib.Repositories;
using MirrorsLib.Services.PositionService;
using ShapesLibrary;
using ShapesLibrary.Interfaces;
using ShapesLibrary.Services;

namespace MirrorsLib.Helpers
{
    public class MirrorCollisionsResolver
    {
        public MirrorCollisionsResolver(IMirrorsDataProvider dataProvider, ILogger<MirrorCollisionsResolver> logger)
        {
            this.dataProvider = dataProvider;
            this.logger = logger;
        }

        private MirrorSynthesis? _mirror;
        private MirrorSynthesis Mirror { get => _mirror ?? throw new Exception("MirrorGlass Has not Been Set..."); }

        private ShapeInfo MirrorContainer => Mirror.DimensionsInformation;
        private readonly IMirrorsDataProvider dataProvider;
        private readonly ILogger<MirrorCollisionsResolver> logger;

        private List<(IMirrorElement element, ShapeInfo shape)> PlacedElements { get; } = [];

        public void SetMirror(MirrorSynthesis mirror)
        {
            PlacedElements.Clear();
            _mirror = mirror;
            PlaceSandblast();
            PlaceMagnifierWithSandblast();
        }

        public void PlaceSandblast()
        {
            //if (_mirror?.Sandblast != null)
            //{
            //    var sandblastShape = _mirror.Sandblast.SandblastShape;
            //    if (sandblastShape is ITwoShapeInfo two)
            //    {
            //        PlacedElements.Add((_mirror.Sandblast, two.FirstShape));
            //        PlacedElements.Add((_mirror.Sandblast, two.SecondShape));
            //    }
            //    else PlacedElements.Add((_mirror.Sandblast, sandblastShape));
            //}
        }
        public void PlaceMagnifierWithSandblast()
        {
            //if (Mirror.MagnifiersWithSandblasts.Count == 0) return;

            //foreach (var magn in Mirror.MagnifiersWithSandblasts)
            //{
            //    var magnShape = magn.ModuleInfo.SandblastDimensions;
            //    var positionOptions = dataProvider.GetPositionOptionsOfElement(magn.ElementInfo.ElementId);
            //    if (positionOptions is null)
            //    {
            //        logger.LogInformation("Position Options not Found for Item with Code : {code} and Name {name}",magn.ElementInfo.Code,magn.ElementInfo.LocalizedDescriptionInfo.Name.GetDefaultValue());
            //        positionOptions = MirrorElementPositionOptions.Empty();
            //    }

            //    var position = positionOptions.GetDefaultPosition(Mirror.DimensionsInformation);
            //    magnShape.LocationX = position.X;
            //    magnShape.LocationY = position.Y;

            //    //bool resolved = CollisionsResolver.ResolveCollisionWithBoundaries(magnShape, PlacedElements.Select(pe => pe.shape).ToList(), MirrorContainer.GetCentroid(), MirrorContainer, logger);
            //    //if (!resolved)
            //    //{
            //    //    //do something;
            //    //}
            //    //else
            //    //{
            //    //    PlacedElements.Add((magn, magnShape));
            //    //}

            //}

        }


        //public void CheckSupportsIntersection()
        //{
        //    if (Mirror.Support is null) return;
        //    var supportShapes = Mirror.Support.SupportInfo.GetSupportRearShapes(Container);

        //    //Check weather any support collides with anything (only sandblasts are in at this point)
        //    foreach (var supportShape in supportShapes)
        //    {
        //        foreach (var (element, shape) in PlacedElements)
        //        {
        //            if (shape.IntersectsWithShape(supportShape))
        //            {
                        
        //            }
        //        }
        //    }

        //}

    }

}
