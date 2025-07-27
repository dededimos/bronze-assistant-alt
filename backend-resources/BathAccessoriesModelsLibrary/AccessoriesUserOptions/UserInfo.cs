using CommonInterfacesBronze;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BathAccessoriesModelsLibrary.AccessoriesUserOptions
{
    public class UserInfo
    {
        public string GObjectId { get; set; } = string.Empty;
        public string GUserDisplayName { get; set; } = string.Empty;
        public bool IsGUser { get; set; }
        public string UserName { get; set; } = string.Empty;
        public UserAccessoriesOptions AccessoriesOptions { get; set; } = UserAccessoriesOptions.Undefined();

        public static UserInfo Undefined() => new();
    }

    public class UserInfoDTO : IDeepClonable<UserInfoDTO>
    {
        public string GObjectId { get; set; } = string.Empty;
        public string GUserDisplayName { get; set; } = string.Empty;
        public bool IsGUser { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string AccessoriesOptionsId { get; set; } = string.Empty;
        /// <summary>
        /// Registered Machine
        /// </summary>
        public string RM { get; set; } = string.Empty;

        public UserInfoDTO GetDeepClone()
        {
            return (UserInfoDTO)this.MemberwiseClone();
        }
        /// <summary>
        /// Returns the UserInfoDTO without exposing the Registered Machine string
        /// </summary>
        /// <returns></returns>
        public UserInfoDTO GetNonSensitiveUserInfo()
        {
            var nonSensitiveInfo = this.GetDeepClone();
            nonSensitiveInfo.RM = string.Empty;
            return nonSensitiveInfo;
        }

        public static UserInfoDTO Undefined() => new();

        public UserInfo ToUserInfo(UserAccessoriesOptions accOptions)
        {
            UserInfo userInfo = new()
            {
                GObjectId = this.GObjectId,
                GUserDisplayName = this.GUserDisplayName,
                IsGUser = this.IsGUser,
                UserName = this.UserName,
                AccessoriesOptions = accOptions
            };
            return userInfo;
        }
    }
}
