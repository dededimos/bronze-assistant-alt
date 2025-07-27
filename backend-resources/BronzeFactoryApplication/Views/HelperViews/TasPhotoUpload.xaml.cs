using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BronzeFactoryApplication.Views.HelperViews
{
    /// <summary>
    /// Interaction logic for TasPhotoUpload.xaml
    /// </summary>
    public partial class TasPhotoUpload : UserControl
    {
        private OpenImageViewerModalService? openImageViewerService;


        public TasPhotoUpload()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                // Initialize services only at runtime
                openImageViewerService = App.AppHost!.Services.GetService<OpenImageViewerModalService>();
            }
        }

        [RelayCommand]
        private void OpenImageViewer(string imgUrl)
        {
            openImageViewerService?.OpenModal(imgUrl);
        }
        [RelayCommand]
        private void ChangePhoto()
        {
            var filePath = GeneralHelpers.SelectImageFile();
            if (string.IsNullOrWhiteSpace(filePath)) return;
            else
            {
                ImageUrl = filePath;
            }
        }

        [RelayCommand]
        private void RemovePhoto()
        {
            if (string.IsNullOrWhiteSpace(ImageUrl)) return;
            if (MessageService.Question($"Do you want to Remove Photo : {Environment.NewLine}{Environment.NewLine}{ImageUrl}", "Remove Photo", "Remove", "Cancel")
            == MessageBoxResult.OK)
            {
                ImageUrl = string.Empty;
            }
        }

        public string ControlTitle
        {
            get { return (string)GetValue(ControlTitleProperty); }
            set { SetValue(ControlTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ControlTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ControlTitleProperty =
            DependencyProperty.Register("ControlTitle", typeof(string), typeof(TasPhotoUpload), new PropertyMetadata(defaultValue: null));

        public string ImageUrl
        {
            get { return (string)GetValue(ImageUrlProperty); }
            set { SetValue(ImageUrlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageUrl.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageUrlProperty =
            DependencyProperty.Register("ImageUrl", typeof(string), typeof(TasPhotoUpload),
                new FrameworkPropertyMetadata(defaultValue: string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));



        public IValueConverter BlobUrlConverter
        {
            get { return (IValueConverter)GetValue(BlobUrlConverterProperty); }
            set { SetValue(BlobUrlConverterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BlobUrlConverter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BlobUrlConverterProperty =
            DependencyProperty.Register("BlobUrlConverter", typeof(IValueConverter), typeof(TasPhotoUpload), new PropertyMetadata(defaultValue:null));


    }
}
