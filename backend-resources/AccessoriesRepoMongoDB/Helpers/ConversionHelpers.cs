using AccessoriesRepoMongoDB.Entities;
using BathAccessoriesModelsLibrary;
using static MongoDbCommonLibrary.ExtensionMethods.MongoDbExtensions;
using CommonInterfacesBronze;
using MongoDbCommonLibrary.CommonEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesRepoMongoDB.Helpers
{
    public static class ConversionHelpers
    {
        /// <summary>
        /// Converts a Trait Group Entity into an Accessory Trait Group for the Selected Language
        /// </summary>
        /// <param name="entity">The Entity</param>
        /// <param name="lngIdentifier">The Language identifier</param>
        /// <returns></returns>
        public static AccessoryTraitGroup ToAccessoryTraitGroup(TraitGroupEntity? entity, string lngIdentifier)
        {
            if (entity == null) return AccessoryTraitGroup.Empty();
            return new AccessoryTraitGroup()
            {
                Code = entity.Code,
                SortNo = entity.SortNo,
                Id = entity.IdAsString,
                DescriptionInfo = entity.GetDescriptionInfo(lngIdentifier)
            };
        }
    }
}
