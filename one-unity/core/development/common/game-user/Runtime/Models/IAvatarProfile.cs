using System;
using Newtonsoft.Json.Linq;

namespace TPFive.Game.User
{
    public interface IAvatarProfile
    {
        string BinfileUrl { get; }

        DateTime CreatedAt { get; }

        AvatarFormat Format { get; }

        string FullBodyPhotoUrl { get; }

        string HalfBodyPhotoUrl { get; }

        string HeadshotPhotoUrl { get; }

        string Id { get; }

        string OwnerId { get; }
    }
}