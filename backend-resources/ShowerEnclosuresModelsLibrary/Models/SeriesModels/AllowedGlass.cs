using CommonInterfacesBronze;
using ShowerEnclosuresModelsLibrary.Enums;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels
{
    public class AllowedGlass : IDeepClonable<AllowedGlass>
    {
        public GlassDrawEnum Draw { get; set; }
        public GlassTypeEnum GlassType { get; set; }
        public int Quantity { get; set; }

        public AllowedGlass(GlassDrawEnum draw, GlassTypeEnum glassType, int quantity)
        {
            Draw = draw;
            GlassType = glassType;
            Quantity = quantity;
        }

        public AllowedGlass GetDeepClone()
        {
            return (AllowedGlass)this.MemberwiseClone();
        }

        /// <summary>
        /// Weather the Provided glass matches the Allowed Glass Options
        /// </summary>
        /// <param name="glass"></param>
        /// <returns></returns>
        public bool IsAllowed(Glass glass)
        {
            return (glass.GlassType == GlassType && glass.Draw == Draw);
        }

    }

}
