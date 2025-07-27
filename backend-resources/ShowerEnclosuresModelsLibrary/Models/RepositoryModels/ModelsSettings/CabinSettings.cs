using ShowerEnclosuresModelsLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.RepositoryModels.ModelsSettings
{
    /// <summary>
    /// Basic Initilization Settings of a Cabin
    /// </summary>
    public class CabinSettings
    {
        public CabinModelEnum Model { get; set; }
        public CabinFinishEnum MetalFinish { get; set; } = CabinFinishEnum.Polished;
        public CabinThicknessEnum Thicknesses { get; set; }
        public GlassFinishEnum GlassFinish { get; set; }
        public int Height { get; set; }
        public int NominalLength { get; set; }
        public bool IsReversible { get; set; }

        public CabinSettings()
        {

        }

        /// <summary>
        /// Creates a Settings Object According to the Provided Settings
        /// </summary>
        /// <param name="model"></param>
        /// <param name="finish"></param>
        /// <param name="thicknesses"></param>
        /// <param name="glassFinish"></param>
        /// <param name="height"></param>
        /// <param name="nominalLength"></param>
        /// <param name="isReversible"></param>
        /// <returns></returns>
        public static CabinSettings Create(
            CabinModelEnum model , 
            CabinFinishEnum finish , 
            CabinThicknessEnum thicknesses , 
            GlassFinishEnum glassFinish , 
            int height , 
            int nominalLength , 
            bool isReversible = false)
        {
            return new CabinSettings()
            {
                Model = model,
                MetalFinish = finish,
                Thicknesses = thicknesses,
                GlassFinish = glassFinish,
                Height = height ,
                NominalLength = nominalLength ,
                IsReversible = isReversible //Usually Ignored and gets only the default value when used in a Factory
            };
        }
    }
}
