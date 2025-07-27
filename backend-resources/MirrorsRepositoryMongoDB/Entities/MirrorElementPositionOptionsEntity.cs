using CommonHelpers;
using CommonHelpers.Comparers;
using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using MirrorsLib.MirrorElements;
using MirrorsLib.Services.PositionService;
using MirrorsLib.Services.PositionService.PositionInstructionsModels;
using MongoDB.Bson;
using MongoDbCommonLibrary.CommonEntities;
using MongoDbCommonLibrary.CommonInterfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsRepositoryMongoDB.Entities
{
    public class MirrorElementPositionOptionsEntity : MongoDatabaseEntity, IDeepClonable<MirrorElementPositionOptionsEntity>, IEqualityComparerCreator<MirrorElementPositionOptionsEntity>
    {
        /// <summary>
        /// The Id of the Element that these Position Options are for
        /// </summary>
        public string ConcerningElementId { get; set; } = string.Empty;
        /// <summary>
        /// A Dictionary matching shapes with the Ids of MirrorPositionElements 
        /// </summary>
        public Dictionary<MirrorOrientedShape, string> DefaultPositions { get; set; } = new();
        /// <summary>
        /// A Dictionary matching shapes with Lists of Ids of MirrorPositionElements , representing additional Positions that the concerning Element can have
        /// </summary>
        public Dictionary<MirrorOrientedShape, List<string>> AdditionalPositions { get; set; } = new();

        static IEqualityComparer<MirrorElementPositionOptionsEntity> IEqualityComparerCreator<MirrorElementPositionOptionsEntity>.GetComparer()
        {
            return new MirrorElementPositionOptionsEntityEqualityComparer();
        }

        public override MirrorElementPositionOptionsEntity GetDeepClone()
        {
            var clone = (MirrorElementPositionOptionsEntity)this.MemberwiseClone();
            clone.DefaultPositions = new(DefaultPositions);
            clone.AdditionalPositions = AdditionalPositions.ToDictionary(kvp => kvp.Key, kvp => new List<string>(kvp.Value));
            return clone;
        }

        public MirrorElementPositionOptions ToPositionOptions(IEnumerable<MirrorElementPositionEntity> positionEntities)
        {
            var options = new MirrorElementPositionOptions()
            {
                ConcerningElementId = this.ConcerningElementId,
                DefaultPositions = new(this.DefaultPositions.ToDictionary(
                    kvp=> kvp.Key,
                    kvp=> positionEntities.FirstOrDefault(e=> e.Id == kvp.Value)?.ToPosition() ?? MirrorElementPosition.DefaultPositionElement())),
                AdditionalPositions = this.AdditionalPositions.ToDictionary(
                    kvp=> kvp.Key,
                    kvp => new List<MirrorElementPosition>(
                        kvp.Value.Select(id=> positionEntities.FirstOrDefault(e=> e.Id == id)?.ToPosition() ?? MirrorElementPosition.DefaultPositionElement())
                        .ToList())),
            };
            return options;
        }
    }

    public class MirrorElementPositionOptionsEntityEqualityComparer : IEqualityComparer<MirrorElementPositionOptionsEntity>
    {
        public bool Equals(MirrorElementPositionOptionsEntity? x, MirrorElementPositionOptionsEntity? y)
        {
            var baseComparer = new DatabaseEntityEqualityComparer();

            return baseComparer.Equals(x, y) &&
                x!.DefaultPositions.IsEqualToOtherDictionary(y!.DefaultPositions) &&
                x.AdditionalPositions.IsEqualToOtherDictionary(y.AdditionalPositions, null, new ListEqualityComparer<string>());
        }

        public int GetHashCode([DisallowNull] MirrorElementPositionOptionsEntity obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }
}
