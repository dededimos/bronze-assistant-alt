using System.Threading.Tasks;
using System;
using BathAccessoriesModelsLibrary.Services;
using BathAccessoriesModelsLibrary;
using BronzeArtWebApplication.Shared.Helpers;
using BronzeArtWebApplication.Pages.NewAccessoriesPage;
using MudBlazor;
using BronzeArtWebApplication.Shared.ViewModels.AccessoriesPageViewModels;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace BronzeArtWebApplication.Shared.Layouts
{
    public partial class AccessoriesLayout : IDisposable
    {
        public const string bodyElementId = "accessoriesBodyContainer";
        private bool isRetrieving = false;
        private string repoBuildingPhase = string.Empty;
        private IndexedItem selectedSearchTerm;
        protected override async Task OnInitializedAsync()
        {
            if (!repo.IsBuilt)
            {
                // Check if Cache should be ignored
                var ignoreCache = ShouldIgnoreCache();
                if (ignoreCache)
                {
                    Console.WriteLine("Cache has been Ignored");
                    // Redirect by removing the last query
                    NavManager.NavigateTo(NavManager.Uri.Replace("ignoreCache", "", StringComparison.OrdinalIgnoreCase));
                    return;
                }
                await BuildRepoAsync(ignoreCache);
            }
            //Subscribe to receive notifications of ItemsCount Changes
            Basket.PropertyChanged += Basket_PropertyChanged;
        }

        private void Basket_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //Subscribe to receive notifications of ItemsCount Changes
            if (e.PropertyName == nameof(BasketViewModel.ItemsCount))
            {
                StateHasChanged();
            }
        }


        /// <summary>
        /// Builds the Repo By Calling the API and providing the stash to the repo for Building
        /// </summary>
        /// <param name="ignoreCache"></param>
        /// <returns></returns>
        private async Task BuildRepoAsync(bool ignoreCache)
        {
            try
            {
                isRetrieving = true;
                await InvokeAsync(StateHasChanged);
                int totalWaited = 0;

                //Have to wait for the User to Authorize because its fire and Forget 
                //Upon Refresh the Initilization here is not waiting for it to finish and thinks user is not authed when he actually is
                while (user.IsCurrentlyAuthorizing && totalWaited <= 2000)
                {
                    await Task.Delay(100);
                    totalWaited += 100;
                }
                var stash = await api.GetAccessoriesStash(ignoreCache);
                if (stash is null) return;
                repo.OnRepositoryBuilding += Repo_OnRepositoryBuilding;
                await repo.BuildRepositoryAsync(stash,user.IsPowerUser);

                await cacheService.InitializeAllAsync();
                await cacheService.Get<BathroomAccessory>("accessories").SaveAsync(repo.Accessories);
                await cacheService.SetVersionAsync("accessories","test");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            finally
            {
                repo.OnRepositoryBuilding -= Repo_OnRepositoryBuilding;
                isRetrieving = false;
                repoBuildingPhase = string.Empty;
                await InvokeAsync(StateHasChanged);
            }
        }

        /// <summary>
        /// Fires whenever A building phase completes to inform the loader
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Repo_OnRepositoryBuilding(object sender, string e)
        {
            repoBuildingPhase = e;
            StateHasChanged();
        }

        /// <summary>
        /// Weather to Ignore Cache based on the URI string of the Address Bar
        /// </summary>
        /// <returns></returns>
        private bool ShouldIgnoreCache()
        {
            return NavManager.Uri.EndsWith("ignoreCache", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns the Image html Element for an image Link , as a clipped circle
        /// </summary>
        /// <param name="imgLink"></param>
        /// <returns></returns>
        public static string GetImageSvgString(string imgLink, string imgElementClass)
        {
            return @$"<image x=""0"" y=""0"" class=""{imgElementClass}"" xlink:href=""{imgLink}""/>";
            // @$"<clipPath id=""circle-clip""><circle cx=""12"" cy=""12"" r=""12"" /></clipPath><image x=""0"" y=""0"" class=""{imgElementClass}"" xlink:href=""{imgLink}"" clip-path=""url(#circle-clip)""/>";
        }

        private void NavigateAfterSearch()
        {
            if (selectedSearchTerm?.AssociatedTrait != null || selectedSearchTerm?.AssociatedAccessory != null)
            {
                var route = GetIndexedItemRoute(selectedSearchTerm);
                
                //DEPRACATED SEARCH
                //if (selectedSearchTerm.AssociatedAccessory != null) 
                //{
                //    // if the selected search term is an accessory put its code as a route param so that the rendering element finds it and scrolls to it
                //    route = $"{route}?{RoutesStash.SearchTermCodeParamName}={selectedSearchTerm.AssociatedAccessory.Code}";
                //}

                NavManager.NavigateTo(route);
            }
        }

        /// <summary>
        /// Returns the Navigation route matching a certain Indexed Item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private static string GetIndexedItemRoute(IndexedItem item)
        {
            if (item.AssociatedAccessory is not null)
            {
                return $"{RoutesStash.DetailedAccessoryCard}?{RoutesStash.CodeParamName}={item.AssociatedAccessory.Code}";
            }
            else if (item.AssociatedTrait is not null)
            {
                return $"{RoutesStash.AccessoriesMain}/{item.AssociatedTrait.TraitType}/{item.AssociatedTrait.Code}";
            }
            else
            {
                return string.Empty;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            //item already disposed above code , supress finalize
            GC.SuppressFinalize(this);
        }

        //IF MADE TRANSIENT WE HAVE TO DISPOSE ON CLOSING WINDOW . CURRENTLY ITS SINGLETON DOES NOT NEED DISPOSING
        private bool _disposed;
        public virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)//Managed Resources
            {
                Basket.PropertyChanged -= Basket_PropertyChanged;
            }

            //object has been disposed
            _disposed = true;

            //If this comes from inheritance Where the parent implement IDisposable the must call base Dispose and the Dispose() method is only in the Parent
            //The subclasses only implement the virtual method and a field '_disposed'
            //Call the base Dispose(bool) for derived classes 
            //base.Dispose(disposing);
        }
    }
}
