using CommonInterfacesBronze;
using MongoDbCommonLibrary.CommonEntities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersRepoMongoDb
{
    public class UserInfoEntity : DbEntity, IDeepClonable<UserInfoEntity>
    {
        public bool IsEnabled { get; set; } = true;
        public string UserName { get; set; } = string.Empty;
        public string UserPassword { get; set; } = string.Empty;
        public string AccessoriesOptionsId { get; set; } = string.Empty;
        public string GraphUserObjectId { get; set; } = string.Empty;
        public string GraphUserDisplayName { get; set; } = string.Empty;
        public bool IsGraphUser { get; set; } = true;
        public string RegisteredMachine { get; set; } = string.Empty;

        public UserInfoEntity GetDeepClone()
        {
            return (UserInfoEntity)this.MemberwiseClone();
        }
    }

    public class UserEntityComparer : IEqualityComparer<UserInfoEntity>
    {
        public bool Equals(UserInfoEntity? x, UserInfoEntity? y)
        {
            if (x is null && y is null) return true;
            if (x is null || y is null) return false;
            var baseEntityComparer = new DbEntityEqualityComparer();


            return baseEntityComparer.Equals(x, y) &&
                x.IsEnabled == y.IsEnabled &&
                x.UserName == y.UserName &&
                x.UserPassword == y.UserPassword &&
                x.AccessoriesOptionsId == y.AccessoriesOptionsId &&
                x.GraphUserObjectId == y.GraphUserObjectId &&
                x.GraphUserDisplayName == y.GraphUserDisplayName &&
                x.IsGraphUser == y.IsGraphUser &&
                x.RegisteredMachine == y.RegisteredMachine;
        }

        public int GetHashCode([DisallowNull] UserInfoEntity obj)
        {
            throw new NotSupportedException($"{typeof(UserInfoEntity).Name} does not Support a Get HashCode Implementation");
        }
    }
}
