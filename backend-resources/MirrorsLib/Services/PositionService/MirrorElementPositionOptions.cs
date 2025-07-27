using CommonHelpers;
using MirrorsLib.Enums;
using MirrorsLib.MirrorElements;
using ShapesLibrary;
using ShapesLibrary.Services;
using System.Collections.Generic;

namespace MirrorsLib.Services.PositionService
{
    public class MirrorElementPositionOptions
    {
        /// <summary>
        /// The Id of the Element that these Position Options are for
        /// </summary>
        public string ConcerningElementId { get; set; } = string.Empty;
        /// <summary>
        /// The Defaults
        /// </summary>
        public DefaultPositionInstructions DefaultPositions { get; set; } = DefaultPositionInstructions.Empty();
        /// <summary>
        /// Additional Positions that this element can have
        /// </summary>
        public Dictionary<MirrorOrientedShape, List<MirrorElementPosition>> AdditionalPositions { get; set; } = new();

        /// <summary>
        /// Returns the Default Position Point for a certain Shape , if there is no Default Position then 0,0 is returned
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        public PointXY GetDefaultPosition(ShapeInfo shape)
        {
            return DefaultPositions.GetPosition(shape);
        }
        /// <summary>
        /// Returns the Default Position Element for a certain Shape or Null if there is no Default Position
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        public MirrorElementPosition GetDefaultPositionElement(MirrorOrientedShape shape)
        {
            return DefaultPositions.GetPositionElement(shape);
        }
        /// <summary>
        /// Returns the Default PositionInstruction Object for the Selected Shape , or Null if None Are Assigned
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        public PositionInstructionsBase GetDefaultPositionInstructions(MirrorOrientedShape shape)
        {
            return GetDefaultPositionElement(shape).Instructions;
        }

        /// <summary>
        /// Returns any Additional Positions apart from the default for a certain Shape
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        public List<MirrorElementPosition> GetAdditionalPositionElements(MirrorOrientedShape shape)
        {
            AdditionalPositions.TryGetValue(shape, out var positions);
            return positions ?? [];
        }
        /// <summary>
        /// Returns any Additional PositionInstructions apart from the default for a certain Shape
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        public List<PositionInstructionsBase> GetAdditionalPositionInstructions(MirrorOrientedShape shape) 
        { 
            return GetAdditionalPositionElements(shape).Select(mep=> mep.Instructions).ToList();
        }
        
        /// <summary>
        /// Returns the Default as well as any Additional Position Elements
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        public List<MirrorElementPosition> GetAllAvailablePositionElements(MirrorOrientedShape shape)
        {
            List<MirrorElementPosition> list = [];
            var defaultPos = GetDefaultPositionElement(shape);
            list.AddNotNull(defaultPos);
            var additionalPos = GetAdditionalPositionElements(shape);
            list.AddRange(additionalPos);
            return list;
        }

        /// <summary>
        /// Returns the Default as well as any Additional PositionInstruction Objects assigned to the provided Oriented Shape
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        public List<PositionInstructionsBase> GetAllAvailablePositionInstructions(MirrorOrientedShape shape)
        {
            List<PositionInstructionsBase> list = [];
            var defaultPos = GetDefaultPositionInstructions(shape);
            list.AddNotNull(defaultPos);
            var additionalPos = GetAdditionalPositionInstructions(shape);
            list.AddRange(additionalPos);
            return list;
        }

        public static MirrorElementPositionOptions Empty() => new();
    }
}
