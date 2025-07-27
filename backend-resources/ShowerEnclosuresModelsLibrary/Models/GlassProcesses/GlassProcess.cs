using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.GlassProcesses
{
    public class GlassProcess
    {
        /// <summary>
        /// Defines from where we measure the Vertical Distance of the Start(Relative to the Glass)
        /// </summary>
        public VertDistancing VerticalDistancing { get; set; }
        /// <summary>
        /// Defines from where we Measure the Horizontal Distance of the Start(Relative to the Glass)
        /// </summary>
        public HorizDistancing HorizontalDistancing { get; set; }
        /// <summary>
        /// Distance of the Hole Center from the Top or Bottom (Depending on Positioning)
        /// When Positioning does not define Top or Bottom the distance is equal from either side (Middle)
        /// </summary>
        public double VerticalDistance { get; set; }
        /// <summary>
        /// Horizontal Distance of the hole from the Glass (Side Defined in HorizontalDistancing
        /// </summary>
        public double HorizontalDistance { get; set; }
        public virtual GlassProcessType ProcessType { get; }
    }

    public enum GlassProcessType
    {
        Undefined,
        Hole,
        ConicalHole,
        Cut8000,
        Cut9B,
        StepCut,
        RoundingCut
    }
}
