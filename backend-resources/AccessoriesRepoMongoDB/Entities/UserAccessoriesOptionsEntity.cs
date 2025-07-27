using AccessoriesRepoMongoDB.Repositories;
using AccessoriesRepoMongoDB.Validators;
using BathAccessoriesModelsLibrary;
using CommonInterfacesBronze;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbCommonLibrary;
using MongoDbCommonLibrary.CommonEntities;
using MongoDbCommonLibrary.CommonValidators;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersRepoMongoDb;

namespace AccessoriesRepoMongoDB.Entities
{
    public class UserAccessoriesOptionsEntity : DescriptiveEntity, IDeepClonable<UserAccessoriesOptionsEntity>
    {
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// The Id of the Trait Group of Dimensions that should appear in the Dimensions of each Accessory Card
        /// </summary>
        public string AppearingDimensionsGroup { get; set; } = string.Empty;

        /// <summary>
        /// The Prices Trait Group Id from which the Customer gets its Prices
        /// </summary>
        public string PricesGroup { get; set; } = string.Empty;
        public UserAccessoriesDiscountsDTO Discounts { get; set; } = new();
        /// <summary>
        /// The Ids of the Custom Price Rules of this User
        /// </summary>
        public List<string> CustomPriceRules { get; set; } = new();

        public string OptionsSummaryString { get => GetOptionsSummaryString(); }

        private string GetOptionsSummaryString()
        {
            return $"{Discounts.DiscountsSummaryString}{Environment.NewLine}Number of Custom Rules:{CustomPriceRules.Count}";
        }

        public UserAccessoriesOptionsEntity GetDeepClone()
        {
            var clone = (UserAccessoriesOptionsEntity)this.MemberwiseClone();
            clone.Name = this.Name.GetDeepClone();
            clone.Description = this.Description.GetDeepClone();
            clone.ExtendedDescription = this.ExtendedDescription.GetDeepClone();
            clone.Discounts = this.Discounts.GetDeepClone();
            clone.CustomPriceRules = new(this.CustomPriceRules);
            return clone;
        }
    }
    public class UserAccessoriesDiscountsDTO : IDeepClonable<UserAccessoriesDiscountsDTO>
    {
        /// <summary>
        /// The General Discount in the Provided Catalogue Prices
        /// </summary>
        public decimal MainDiscount { get; set; }
        public decimal SecondaryDiscount { get; set; }
        public decimal TertiaryDiscount { get; set; }

        public decimal QuantityDiscPrimary { get; set; }
        public int QuantityDiscQuantityPrimary { get; set; }
        public decimal QuantityDiscSecondary { get; set; }
        public int QuantityDiscQuantitySecondary { get; set; }
        public decimal QuantityDiscTertiary { get; set; }
        public int QuantityDiscQuantityTertiary { get; set; }

        public string DiscountsSummaryString { get => GetDiscountsSummaryString(); }

        private string GetDiscountsSummaryString()
        {
            StringBuilder builder = new();
            builder.Append('-');
            builder.Append($"{MainDiscount * 100:0.00}");
            builder.Append('%');
            if (SecondaryDiscount != 0)
            {
                builder.Append('-');
                builder.Append($"{SecondaryDiscount * 100:0.00}");
                builder.Append('%');
            }
            if (TertiaryDiscount != 0)
            {
                builder.Append('-');
                builder.Append($"{TertiaryDiscount * 100:0.00}");
                builder.Append('%');
            }
            if (QuantityDiscPrimary != 0 || QuantityDiscSecondary != 0 || QuantityDiscTertiary != 0)
            {
                if (QuantityDiscPrimary != 0)
                {
                    builder.Append(Environment.NewLine);
                    builder.Append("QTY1 :");
                    builder.Append($"{QuantityDiscQuantityPrimary}");
                    builder.Append("=>");
                    builder.Append('-');
                    builder.Append($"{QuantityDiscPrimary * 100:0.00}");
                    builder.Append('%');
                }
                if (QuantityDiscSecondary != 0)
                {
                    builder.Append(Environment.NewLine);
                    builder.Append("QTY2 :");
                    builder.Append($"{QuantityDiscQuantitySecondary}");
                    builder.Append("=>");
                    builder.Append('-');
                    builder.Append($"{QuantityDiscSecondary * 100:0.00}");
                    builder.Append('%');
                }
                if (QuantityDiscTertiary != 0)
                {
                    builder.Append(Environment.NewLine);
                    builder.Append("QTY3 :");
                    builder.Append($"{QuantityDiscQuantityTertiary}");
                    builder.Append("=>");
                    builder.Append('-');
                    builder.Append($"{QuantityDiscTertiary * 100:0.00}");
                    builder.Append('%');
                }
            }
            return builder.ToString();
        }

        public UserAccessoriesDiscountsDTO GetDeepClone()
        {
            return (UserAccessoriesDiscountsDTO)this.MemberwiseClone();
        }
    }

    public class UserAccessoriesOptionsRepository : MongoEntitiesRepository<UserAccessoriesOptionsEntity>
    {
        private readonly IMongoCollection<UserInfoEntity> usersCollection;

        public UserAccessoriesOptionsRepository(
            UserAccessoriesOptionsEntityValidator validator,
            IMongoDbAccessoriesConnection connection,
            ILogger<UserAccessoriesOptionsRepository> logger)
            : base(validator, connection.UserOptionsCollection, logger)
        {
            usersCollection = connection.UsersCollection;
        }

        public override async Task DeleteEntityAsync(ObjectId id)
        {
            //Find if any User has this Id Assigned to its AccessoriesOptions
            var idToString = id.ToString();
            FilterDefinition<UserInfoEntity> usersFilter = Builders<UserInfoEntity>.Filter.Eq(o => o.AccessoriesOptionsId, idToString);

            //if it does throw with the UsersNames having this Options assigned and Prevent Deletion
            var resultUsers = await usersCollection.FindAsync(usersFilter);
            var foundUsers = await resultUsers.ToListAsync();

            if (foundUsers.Count != 0)
            {
                var usersStrings = foundUsers.Count <= 10 ? foundUsers.Select(u => $"{foundUsers.IndexOf(u) + 1} {u.UserName}-(Graph:{u.GraphUserDisplayName})-(GraphId:{u.GraphUserObjectId})") : foundUsers.Take(10).Select(u => $"{foundUsers.IndexOf(u) + 1} {u.UserName}-(Graph:{u.GraphUserDisplayName})-(GraphId:{u.GraphUserObjectId})");
                var usersJoinedStrings = string.Join(Environment.NewLine, usersStrings);
                logger.LogInformation("There where Users that have assigned this UserAccessoriesOptions with Id:{id}{newLine}{joinedStrings}", idToString, Environment.NewLine, usersJoinedStrings);
                throw new Exception($"Cannot Delete UserAccessoriesOptions while its Assigned to Users{Environment.NewLine}{usersJoinedStrings}{Environment.NewLine}...");
            }
            else
            {
                //If no Users where found that use this Option Delete it
                await base.DeleteEntityAsync(id);
            }
        }
    }

    public class UserAccessoriesOptionsEntityComparer : IEqualityComparer<UserAccessoriesOptionsEntity>
    {
        public bool Equals(UserAccessoriesOptionsEntity? x, UserAccessoriesOptionsEntity? y)
        {
            if (x is null && y is null) return true;
            if (x is null || y is null) return false;
            var descriptiveEntityComparer = new DescriptiveEntityEqualityComparer();


            return descriptiveEntityComparer.Equals(x, y) &&
                x.AppearingDimensionsGroup == y.AppearingDimensionsGroup &&
                x.PricesGroup == y.PricesGroup &&
                x.Discounts.MainDiscount == y.Discounts.MainDiscount &&
                x.Discounts.SecondaryDiscount == y.Discounts.SecondaryDiscount &&
                x.Discounts.TertiaryDiscount == y.Discounts.TertiaryDiscount &&
                x.Discounts.QuantityDiscPrimary == y.Discounts.QuantityDiscPrimary &&
                x.Discounts.QuantityDiscQuantityPrimary == y.Discounts.QuantityDiscQuantityPrimary &&
                x.Discounts.QuantityDiscSecondary == y.Discounts.QuantityDiscSecondary &&
                x.Discounts.QuantityDiscQuantitySecondary == y.Discounts.QuantityDiscQuantitySecondary &&
                x.Discounts.QuantityDiscTertiary == y.Discounts.QuantityDiscTertiary &&
                x.Discounts.QuantityDiscQuantityTertiary == y.Discounts.QuantityDiscQuantityTertiary &&
                x.CustomPriceRules.SequenceEqual(y.CustomPriceRules);
        }

        public int GetHashCode([DisallowNull] UserAccessoriesOptionsEntity obj)
        {
            throw new NotSupportedException($"{typeof(UserAccessoriesOptionsEntity).Name} does not Support a Get HashCode Implementation");
        }
    }
}
