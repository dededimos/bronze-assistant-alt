using BathAccessoriesModelsLibrary;
using BathAccessoriesModelsLibrary.Services;
using BronzeArtWebApplication.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CommonHelpers.CommonExtensions;

namespace BronzeArtWebApplication.Shared.ViewModels.AccessoriesPageViewModels
{
    public partial class AccessoriesPageViewModel : BaseViewModelCT
    {
        /// <summary>
        /// Weather the Series Menu is Expanded
        /// </summary>
        [ObservableProperty]
        private bool isSeriesExpanded;
        /// <summary>
        /// Weather the PrimaryType menu is Expanded
        /// </summary>
        [ObservableProperty]
        private bool isTypeExpanded;
        /// <summary>
        /// Weather the Finish Menu is Expanded
        /// </summary>
        [ObservableProperty]
        private bool isFinishExpanded;

        /// <summary>
        /// The Type of View selected in Accessories
        /// </summary>
        [ObservableProperty]
        private TypeOfView viewType = TypeOfView.GridView;

        public AccessoriesPageViewModel()
        {
            
        }
    }

    public enum TypeOfView
    {
        /// <summary>
        /// Shows accessories in a Grid Card Like Fashion
        /// </summary>
        GridView,
        /// <summary>
        /// Shows accessories in a table List Fashion
        /// </summary>
        ListView,
        /// <summary>
        /// Only Photos and Code
        /// </summary>
        CompactView,
        /// <summary>
        /// Groups the Accessories in Card Like Fashion
        /// </summary>
        SecondaryTypeGroupView
    }
}
