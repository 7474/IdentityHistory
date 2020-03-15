using IdentityHistoryFunctionApp.SlackAPI;
using System.Text.Json;
using System;
using System.Collections.Generic;

namespace IdentityHistoryFunctionApp.Entity
{
    // https://api.slack.com/methods/users.info
    public class SlackUserEntity
    {
        /// <summary>
        /// パーティションキー。
        /// TeamID（Workspace）。
        /// </summary>
        public string TeamId { get; set; }

        /// <summary>
        /// パーティション内でのID。
        /// </summary>
        public string id { get; set; }

        /// <remarks>
        /// 関心事ではないので履歴は持たない。
        /// </remarks>
        public UserInfo CurrentUser { get; set; }

        /// <remarks>
        /// https://docs.microsoft.com/ja-jp/azure/cosmos-db/modeling-data#when-not-to-embed
        /// Recentとしているが、当面はここにすべての履歴を入れる。
        /// </remarks>
        public ICollection<UserProfileInfo> RecentProfile { get => recentProfile; set => recentProfile = value ?? recentProfile; }
        private ICollection<UserProfileInfo> recentProfile;

        public SlackUserEntity()
        {
            recentProfile = new List<UserProfileInfo>();
        }
    }

    public class UserInfo : User
    {
    }

    public class UserProfileInfo : UserProfile
    {
        public DateTimeOffset Timestamp { get; set; }

        public bool hasChange(UserProfileInfo profile)
        {
            // XXX display_name もみたいけれどなんかフィールドがなかった。
            return image_512 != profile.image_512;
        }
    }
}
