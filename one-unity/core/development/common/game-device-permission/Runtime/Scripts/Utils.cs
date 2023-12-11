namespace TPFive.Game.DevicePermission
{
    internal class Utils
    {
        public static string ToDescription(PermissionType permissionType) =>
            permissionType switch
            {
                PermissionType.Read => "read",
                _ => "write",
            };
    }
}
