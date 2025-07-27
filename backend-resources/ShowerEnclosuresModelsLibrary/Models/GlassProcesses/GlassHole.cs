using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.GlassProcesses
{
    public class GlassHole : GlassProcess
    {
        /// <summary>
        /// The Diameter of the Hole
        /// </summary>
        public int Diameter { get; set; }
        public double Radius { get => Diameter / 2d; }
        public override GlassProcessType ProcessType { get => GlassProcessType.Hole; }

        public GlassHole(int diameter)
        {
            Diameter = diameter;
        }
        public GlassHole()
        {

        }

    }
    public class ConicalHole : GlassHole
    {
        public int BigDiameter { get; set; }
        public override GlassProcessType ProcessType { get => GlassProcessType.ConicalHole; }

        public ConicalHole(int diameter,int bigDiameter) : base(diameter)
        {
            BigDiameter = bigDiameter;
        }
        public ConicalHole()
        {

        }
    }
}
