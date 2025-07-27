using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.ModelsInterfaces
{
    public interface ISlidingConstraints
    {
        /// <summary>
        /// The Mininum Distance that the Sliding Door should have from the Wall when its fully opened
        /// </summary>
        public int MinDoorDistanceFromWallOpened { get; set; }
        /// <summary>
        /// The Cover Distance for a single Glass
        /// </summary>
        public int CoverDistance { get; set; }
        /// <summary>
        /// The Overlap for a single Glass
        /// </summary>
        public int Overlap { get; set; }
        
        /// <summary>
        /// Returns the Combined overlap for All the Glasses of the Structure
        /// If the Structure has one door , this matches the Overlap of the Structure
        /// </summary>
        /// <returns>The Total Overlap</returns>
        int GetTotalOverlap();
    }
}
