using BathAccessoriesModelsLibrary;
using Blazored.LocalStorage;
using FluentValidation.Internal;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace BronzeArtWebApplication.Shared.Services.CacheService
{
    /// <summary>
    /// A Service that manages Caching of Lists of Items in the Local Storage IndexedDB
    /// 1.Saves a key Versions Key to Local Storage
    /// 2.Can check if a specific collection key is equal to the expected version
    /// 3.Can set the version of a specific collection key
    /// 4.Can clear all collections
    /// 5.Can clear a specific collection
    /// 6.Can get a specific collection
    /// 7.Can Save a specific collection
    /// 8.Needs the js Script to be added to the index.html file , so to be able to interoperate with the IndexedDB
    /// </summary>
    public class BronzeCacheService
    {
        private readonly IJSRuntime _js;
        private readonly ILocalStorageService localStorage;
        private readonly Dictionary<string, object> _collections = [];
        private const string VersionKey = "collectionVersions";

        public BronzeCacheService(
            IJSRuntime js,
            ILocalStorageService localStorage,
            List<ICacheCollection> cacheCollections
        )
        {
            _js = js;
            this.localStorage = localStorage;

            foreach (var collection in cacheCollections)
            {
                _collections.Add(collection.StoreName, collection);
            }
        }
        
        public async Task InitializeAllAsync()
        {
            var storeNames = _collections.Values
                .OfType<ICacheCollection>()
                .Select(c => c.StoreName)
                .Distinct()
                .ToArray();

            await _js.InvokeVoidAsync("blazorCache.openDb", new object[] { storeNames });

            foreach (var collection in _collections.Values)
            {
                if (collection is ICacheCollection cache)
                    await cache.InitializeAsync();
            }
        }
        public ICacheCollection<T> Get<T>(string name)
        {
            if (_collections.TryGetValue(name, out var service) && service is ICacheCollection<T> typed)
            {
                return typed;
            }

            throw new InvalidOperationException($"No cache registered with name '{name}' for type '{typeof(T).Name}'.");
        }


        // 🟢 Get version dictionary from localStorage
        private async Task<Dictionary<string, string>> GetVersionMapAsync()
        {
            try
            {
                return await localStorage.GetItemAsync<Dictionary<string, string>>(VersionKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return [];
            }
            
        }

        // 🔵 Check if specific collection is up-to-date
        public async Task<bool> IsUpToDateAsync(string storeName, string expectedVersion)
        {
            var versions = await GetVersionMapAsync();
            return versions.TryGetValue(storeName, out var currentVersion) && currentVersion == expectedVersion;
        }

        // 🟡 Update the version for a specific collection
        public async Task SetVersionAsync(string storeName, string version)
        {
            var versions = await GetVersionMapAsync();
            versions[storeName] = version;

            await localStorage.SetItemAsync(VersionKey, versions);
        }

        public async Task ClearAllAsync()
        {
            foreach (var collection in _collections.Values)
            {
                if (collection is ICacheCollection cache)
                    await cache.ClearAsync();
            }
            await localStorage.RemoveItemAsync(VersionKey);
        }

        public async Task ClearStoreVersionAsync(string storeName)
        {
            var versions = await GetVersionMapAsync();
            if (versions.Remove(storeName))
            {
                //replace with the new versions map that has the removed the storedList 
                await localStorage.SetItemAsync(VersionKey, versions);
            }
        }
    }
    public interface ICacheCollection
    {
        string StoreName { get; }
        Task InitializeAsync();
        Task ClearAsync();
    }
    public interface ICacheCollection<T> : ICacheCollection
    {
        Task<List<T>> LoadAsync();
        Task SaveAsync(List<T> items);
        Task<bool> IsEmptyAsync();
    }

    /// <summary>
    /// A Base Class for the Cache Collections (as the collections of documents in MongoDB)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CacheCollectionBase<T> : ICacheCollection<T>
    {
        protected readonly IJSRuntime _js;

        public abstract string StoreName { get; }

        protected CacheCollectionBase(IJSRuntime js) => _js = js;

        public virtual async Task InitializeAsync()
        {
            //Opens the database and creates the store/collection if it does not exist
            await _js.InvokeVoidAsync("blazorCache.openDb", new object[] { new[] { StoreName } });
        }

        public async Task<List<T>> LoadAsync()
        {
            //Load all items from the store
            var result = await _js.InvokeAsync<T[]>("blazorCache.getAllItems", StoreName);
            return [.. result];
        }

        public async Task SaveAsync(List<T> items)
        {
            //Save all items to the store
            await _js.InvokeVoidAsync("blazorCache.saveItems", StoreName, items);
        }

        public async Task ClearAsync()
        {
            //Wipe the store
            await _js.InvokeVoidAsync("blazorCache.clearStore", StoreName);
        }

        public async Task<bool> IsEmptyAsync()
        {
            //Check if the store is empty
            var items = await LoadAsync();
            return items.Count == 0;
        }
    }

    public class AccessoriesCacheCollection : CacheCollectionBase<BathroomAccessory>
    {
        public override string StoreName => "accessories";
        public AccessoriesCacheCollection(IJSRuntime js) : base(js) { }
    }
}
