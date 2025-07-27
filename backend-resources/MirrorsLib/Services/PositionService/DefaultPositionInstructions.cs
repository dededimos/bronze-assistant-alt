using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using MirrorsLib.Helpers;
using MirrorsLib.MirrorElements;
using MirrorsLib.Services.PositionService.PositionInstructionsModels;
using ShapesLibrary;
using ShapesLibrary.Enums;
using ShapesLibrary.Services;
using ShapesLibrary.ShapeInfoModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsLib.Services.PositionService
{
    public class DefaultPositionInstructions : IDeepClonable<DefaultPositionInstructions>
    {
        private Dictionary<MirrorOrientedShape, MirrorElementPosition> defaultPositions = [];
        public IReadOnlyDictionary<MirrorOrientedShape,MirrorElementPosition> DefaultPositions { get => defaultPositions; }

        public DefaultPositionInstructions GetDeepClone()
        {
            var clone = (DefaultPositionInstructions)this.MemberwiseClone();
            clone.defaultPositions = this.DefaultPositions.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.GetDeepClone());
            return clone;
        }

        public DefaultPositionInstructions(Dictionary<MirrorOrientedShape,MirrorElementPosition> defaultPositions)
        {
            this.defaultPositions = defaultPositions;
        }

        public static DefaultPositionInstructions Empty() => new([]);

        /// <summary>
        /// Returns the Position that a certain object must have inside the provided shape , according to the Saved Default instructions 
        /// </summary>
        /// <param name="shape">The Shape where the Object is being inserted into</param>
        /// <returns></returns>
        public PointXY GetPosition(ShapeInfo shape)
        {
            var orientedShape = shape.ToMirrorOrientedShape();
            DefaultPositions.TryGetValue(orientedShape, out var positionElement);
            if (positionElement is not null)
            {
                return positionElement.Instructions.GetPosition(shape);
            }
            else
            {
                return new PointXY(0, 0);
            }
        }
        /// <summary>
        /// Returns the Position Element for the specified shape 
        /// <para>Returns a default Position if there is no Position Element Found</para>
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        public MirrorElementPosition GetPositionElement(MirrorOrientedShape shape)
        {
            if (DefaultPositions.TryGetValue(shape, out var positionElement))
            {
                return positionElement;
            } 
            else return MirrorElementPosition.DefaultPositionElement();
        }
    }
}
