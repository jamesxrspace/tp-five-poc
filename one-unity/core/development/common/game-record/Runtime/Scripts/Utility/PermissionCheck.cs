using UnityEngine;

namespace TPFive.Game.Record
{
    public static class PermissionCheck
    {
        public static bool GetPermissionCheck()
        {
            return Application.HasUserAuthorization(UserAuthorization.Microphone);
        }

        public static void RequestPermission()
        {
            Application.RequestUserAuthorization(UserAuthorization.Microphone);
        }
    }
}
