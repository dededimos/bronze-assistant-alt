using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.ModalViewModels
{
    public partial class ImageViewerModalViewModel : ModalViewModel
    {
        [ObservableProperty]
        private string imageUrl = string.Empty;

        public ImageViewerModalViewModel()
        {
            Title = "Image Viewer";
        }

        public void SetUrl(string imageUrl)
        {
            ImageUrl = imageUrl;
        }
    }
}
