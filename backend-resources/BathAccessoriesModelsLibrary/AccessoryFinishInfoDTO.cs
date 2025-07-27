using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BathAccessoriesModelsLibrary
{
    public class AccessoryFinishInfoDTO
    {
        public string FinishId { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public string DimensionsPhotoUrl { get; set; } = string.Empty;
        public List<string> ExtraPhotosUrl { get; set; } = new();
        public string PdfUrl { get; set; } = string.Empty;

        public AccessoryFinish ToAccessoryFinish(Dictionary<string,AccessoryTrait> traits)
        {
            var finish = traits.TryGetValue(this.FinishId, out AccessoryTrait? foundFinish) ? foundFinish : AccessoryTrait.Empty(TypeOfTrait.FinishTrait);
            AccessoryFinish accessoryFinish = new()
            {
                Finish = finish,
                PhotoUrl = this.PhotoUrl,
                PdfUrl = this.PdfUrl,
                DimensionsPhotoUrl = this.DimensionsPhotoUrl,
                ExtraPhotosUrl = new(this.ExtraPhotosUrl),
            };
            return accessoryFinish;
        }
    }
}
