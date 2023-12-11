using System.Runtime.Serialization;

namespace TPFive.Home.Entry.SocialLobby.Enum
{
    /// <summary>
    /// Define valid feed tags for game server feed api
    /// at POC stage, game server feed api only accept tags: enum: [friends, music, dance, talk_show, culture]
    /// ref1: openapi-doc/game-server/documents/app/feed.yaml
    /// ref2: https://www.figma.com/file/uRYUznWNIuMj5dpqFgzu9o/New-App-Design%EF%BC%BFUS?type=design&amp;node-id=0-1&amp;mode=design&amp;t=mvX6kgsR9CRFEOVY-0
    /// </summary>
    public enum FeedTag
    {
        /// <summary>
        /// Friends
        /// </summary>
        [EnumMember(Value = "friends")]
        Friends = 0,

        /// <summary>
        /// Music
        /// </summary>
        [EnumMember(Value = "music")]
        Music = 1,

        /// <summary>
        /// Dance
        /// </summary>
        [EnumMember(Value = "dance")]
        Dance = 2,

        /// <summary>
        /// TalkShow
        /// </summary>
        [EnumMember(Value = "talk_show")]
        TalkShow = 3,

        /// <summary>
        /// Culture
        /// </summary>
        [EnumMember(Value = "culture")]
        Culture = 4,
    }
}