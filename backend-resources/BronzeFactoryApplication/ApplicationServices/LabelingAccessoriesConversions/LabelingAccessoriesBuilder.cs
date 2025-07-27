using AccessoriesRepoMongoDB.Entities;
using AccessoriesRepoMongoDB.Repositories;
using AzureBlobStorageLibrary;
using CommonInterfacesBronze;
using HandyControl.Tools.Extension;
using Microsoft.Extensions.Logging;
using SqliteApplicationSettings;
using SqliteApplicationSettings.DTOs;
using SqliteLabelingDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.LabelingAccessoriesConversions
{
    public class LabelingAccessoriesBuilder
    {
        private readonly IAccessoryEntitiesRepository repo;
        private readonly ILogger<LabelingAccessoriesBuilder> logger;
        private readonly BlobUrlHelper urlHelper;
        private readonly AccessoriesLabelsDbContextFactory dbContextFactory;
        private HttpClient client = new();

        private Dictionary<string, TraitEntity> Traits { get; } = [];

        public LabelingAccessoriesBuilder(IAccessoryEntitiesRepository repo,
                                          ILogger<LabelingAccessoriesBuilder> logger,
                                          BlobUrlHelper urlHelper,
                                          AccessoriesLabelsDbContextFactory dbContextFactory)
        {
            this.repo = repo;
            this.logger = logger;
            this.urlHelper = urlHelper;
            this.dbContextFactory = dbContextFactory;
        }

        public async Task AddEntitiesToLabelsDatabase(IProgress<int>? progress = null)
        {
            //Assign the Traits Dictionary
            Traits.Clear();
            foreach (var t in repo.Traits.Cache)
            {
                Traits.Add(t.IdAsString, t);
            }
            if (Traits.Count == 0) return;


            //Remove all previous
            await RemoveAllDatabaseEntries();

            int totalSteps = repo.Cache.Sum(e => e.AvailableFinishes.Count);
            int completedSteps = 0;

            foreach (var e in repo.Cache)
            {
                foreach (var finish in e.AvailableFinishes)
                {
                    try
                    {
                        var dto = await ConvertToAccessoryLabelDTO(e, finish.FinishId);
                        await AddNewAccessoryLabel(dto);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Skipping item {code} with Finish {itemFinish}", e.Code, GetGreekTrait(finish.FinishId));
                        continue;
                    }
                    finally
                    {
                        completedSteps++;
                        progress?.Report((int)((double)completedSteps / totalSteps * 100)); // Report percentage
                    }
                }
            }
        }
        
        public async Task AddNewAccessoryLabel(AccessoryLabelDTO dto)
        {
            try
            {
                using var context = dbContextFactory.CreateDbContext();
                dto.Created = DateTime.Now;
                context.AccessoriesTable.Add(dto);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task RemoveAllDatabaseEntries()
        {
            try
            {
                using var context = dbContextFactory.CreateDbContext();
                var all = context.AccessoriesTable.ToList();
                context.AccessoriesTable.RemoveRange(all);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        private async Task<AccessoryLabelDTO> ConvertToAccessoryLabelDTO(BathAccessoryEntity e,string finishId)
        {
            AccessoryLabelDTO a = new();

            if (Traits.TryGetValue(finishId, out TraitEntity? finish) == false) throw new Exception("InvalidFinish");

            a.Code = BathAccessoryEntity.GenerateSpecificCode(e.MainCode,finish.Code,e.ExtraCode,e.UsesOnlyMainCode);
            a.FinishGreek = GetGreekTrait(e.Finish);
            a.FinishEnglish = GetEnglishTrait(finishId);
            a.PrimaryTypeGreek = GetGreekTrait(e.PrimaryTypes.FirstOrDefault() ?? "UNDEFINED-PRIMARY-TYPE");
            a.PrimaryTypeEnglish = GetEnglishTrait(e.PrimaryTypes.FirstOrDefault() ?? "UNDEFINED-PRIMARY-TYPE");
            a.SecondaryTypeGreek = GetGreekTrait(e.SecondaryTypes.FirstOrDefault() ?? "UNDEFINED-SECONDARY-TYPE");
            a.SecondaryTypeEnglish = GetEnglishTrait(e.SecondaryTypes.FirstOrDefault() ?? "UNDEFINED-SECONDARY-TYPE");
            a.SeriesGreek = GetGreekTrait(e.Series.FirstOrDefault() ?? "UNDEFINED-SERIES");
            a.SeriesEnglish = GetEnglishTrait(e.Series.FirstOrDefault() ?? "UNDEFINED-SERIES");
            a.MountingTypeGreek = GetGreekTrait(e.MountingTypes.FirstOrDefault() ?? "UNDEFINED-MOUNTINGTYPE");
            a.MountingTypeEnglish = GetEnglishTrait(e.MountingTypes.FirstOrDefault() ?? "UNDEFINED-MOUNTINGTYPE");
            a.DescriptionGreek = $"{a.SecondaryTypeGreek} {a.SeriesGreek} {a.FinishGreek} {a.MountingTypeGreek}";
            a.DescriptionEnglish = $"{a.SecondaryTypeEnglish} {a.SeriesEnglish} {a.FinishEnglish} {a.MountingTypeEnglish}";
            
            //Find the selected Finish
            var f = e.AvailableFinishes.FirstOrDefault(f=> f.FinishId == finishId) ?? throw new Exception($"{e.Code} => There was not finish with the selected ID : {finishId}");
            var imgUrlRelative = string.IsNullOrEmpty(f.PhotoUrl) ? e.MainPhotoURL : f.PhotoUrl;
            
            a.Photo = await DownloadImageAsByteArrayAsync(imgUrlRelative);
            return a;
        }
        private string GetGreekDescriptionFromEntity(BathAccessoryEntity entity)
        {
            return $"{entity.Name.GetLocalizedValue(LocalizedString.GREEKIDENTIFIER)} {entity.Finish}";
        }
        private string GetGreekTrait(string traitId,bool skipErrors = false)
        {
            if (Traits.TryGetValue(traitId, out var t))
            {
                return GetGreeKDecription(t);
            }
            else if (skipErrors == false)
            {
                throw new Exception($"Trait with Id {traitId} was not Found");
            }
            else return $"Undefined-Trait-Greek-Value-With-Id-{traitId}";
        }
        private string GetEnglishTrait(string traitId)
        {
            if (Traits.TryGetValue(traitId, out var t))
            {
                return GetEnglishDescription(t);
            }
            else throw new Exception($"Trait with Id {traitId} was not Found");
        }
        private static string GetGreeKDecription(TraitEntity trait)
        {
            return trait.Trait.GetLocalizedValue(LocalizedString.GREEKIDENTIFIER);
        }
        private static string GetEnglishDescription(TraitEntity trait)
        {
            return trait.Trait.GetLocalizedValue(LocalizedString.ENGLISHIDENTIFIER);
        }
        public async Task<byte[]> DownloadImageAsByteArrayAsync(string imgRelativeUrl)
        {
            var url = urlHelper.ConvertBlobPartialToFullUrl(imgRelativeUrl, BlobContainerOption.AccessoriesBlobs, BlobPhotoSize.LargeSizePhoto);

            try
            {
                byte[] bytes = await client.GetByteArrayAsync(url);
                return bytes;
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"COULD NOT DOWNLOAD IMAGE"); ;
                throw new Exception(ex.Message);
            }
        }

    }
}
