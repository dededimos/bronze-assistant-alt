using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ShowerEnclosuresModelsLibrary.Models.GlassProcesses.ConstantValues.GlassProcessesConstants;

namespace ShowerEnclosuresModelsLibrary.Models.GlassProcesses
{
    public class CutHotel8000 : GlassProcess
    {
        public int Height { get; set; }
        public override GlassProcessType ProcessType { get => GlassProcessType.Cut8000; }
        public int SemiCirclesCentersDistance { get; set; }
        public int SemiCircleDistanceFromEdge { get; set; }
        public int SemiCircleDiameter { get; set; }
        public double Length { get => GetLength(); }
        /// <summary>
        /// Weather this is placed at the left part of the Glass or Right
        /// </summary>
        public bool IsPlacedLeft { get; }

        public CutHotel8000(bool isPlacedLeft)
        {
            Height = ProcessesHotel8000.CutHeight;
            SemiCirclesCentersDistance = ProcessesHotel8000.CutSemiCirclesCenterDistance;
            SemiCircleDistanceFromEdge = ProcessesHotel8000.CutSemiCircleCenterEdgeDistance;
            SemiCircleDiameter = ProcessesHotel8000.CutSemiCircleDiameter;
            IsPlacedLeft = isPlacedLeft;
        }

        public CutHotel8000(int height , int semiCirclesCentersDistance , int semiCircleDistanceFromEdge , int semiCircleDiameter , bool isPlacedLeft)
        {
            Height = height;
            SemiCirclesCentersDistance = semiCirclesCentersDistance;
            SemiCircleDistanceFromEdge = semiCircleDistanceFromEdge;
            SemiCircleDiameter = semiCircleDiameter;
            IsPlacedLeft = isPlacedLeft;
        }


        /// <summary>
        /// Gets the StraightHorizontal Line Length of the Hinge Cut
        /// The Dimension is a doulbe as the Division in the Circle Diameter usually needs a decimal place
        /// </summary>
        /// <returns></returns>
        private double GetLength()
        {
            double length = SemiCircleDistanceFromEdge - (SemiCircleDiameter / 2d);
            return length;
        }
    }
}
