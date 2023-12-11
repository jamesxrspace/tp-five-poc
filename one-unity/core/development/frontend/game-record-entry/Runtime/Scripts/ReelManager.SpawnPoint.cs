using Microsoft.Extensions.Logging;
using TPFive.Game.Record.Scene.SpawnPoint;
using UnityEngine;

namespace TPFive.Game.Record.Entry
{
    public partial class ReelManager
    {
        private void SetupAvatarPlacement(ReelPlayer player, int spawnPointIndex)
        {
            if (spawnPointIndex < 0 || spawnPointService.PointCount == 0)
            {
                log.LogError(
                    "{Method}: spawn point index({SpawnPointIndex}) is out of range({PointCount})",
                    nameof(SetupAvatarPlacement),
                    spawnPointIndex,
                    spawnPointService.PointCount);

                return;
            }

            spawnPointIndex %= spawnPointService.PointCount;
            SetupAvatarPlacement(player, spawnPointService.GetPoint(spawnPointIndex));
        }

        private void SetupAvatarPlacement(ReelPlayer player, PointDesc pointDesc)
        {
            if (player == null || !player.Avatar.IsAlive())
            {
                log.LogError("{Method}: player or avatar is null", nameof(SetupAvatarPlacement));
                return;
            }

            if (pointDesc.Point == null)
            {
                log.LogWarning("{Method}: the point({PointDesc}) is null", nameof(SetupAvatarPlacement), pointDesc);
                return;
            }

            var avatar = player.Avatar;
            var point = new Pose(pointDesc.Point.position, pointDesc.Point.rotation);
            log.LogDebug("{Method}: place avatar to point({Point})", nameof(SetupAvatarPlacement), point.ToString("F3"));

            pointDesc.TryGetSpecificAnimationByGender(avatar.AvatarFormat.gender, out var specificAnimation);

            switch (pointDesc.LandingPose)
            {
                case LandingPoseType.Standing:
                    avatar.Controller.StandUp(point, specificAnimation, true, true);
                    break;
                case LandingPoseType.Sitting:
                    avatar.Controller.SitDown(point, specificAnimation, true, true);
                    break;
                case LandingPoseType.Lying:
                    // [TODO] lying pose not implemented yet
                    break;
                default:
                    break;
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Place host avatar to spawn point(0)")]
        private void Test_PlaceHostAvatarToSpawnPoint0()
        {
            if (userPlayer == null)
            {
                log.LogError("{Method}: host avatar not found", nameof(Test_PlaceHostAvatarToSpawnPoint0));
                return;
            }

            SetupAvatarPlacement(userPlayer, spawnPointService.GetPoint(0));
        }

        [ContextMenu("Place host avatar to spawn point(1)")]
        private void Test_PlaceHostAvatarToSpawnPoint1()
        {
            if (userPlayer == null)
            {
                log.LogError("{Method}: host avatar not found", nameof(Test_PlaceHostAvatarToSpawnPoint1));
                return;
            }

            SetupAvatarPlacement(userPlayer, spawnPointService.GetPoint(1));
        }

        [ContextMenu("Place host avatar to spawn point(2)")]
        private void Test_PlaceHostAvatarToSpawnPoint2()
        {
            if (userPlayer == null)
            {
                log.LogError("{Method}: host avatar not found", nameof(Test_PlaceHostAvatarToSpawnPoint2));
                return;
            }

            SetupAvatarPlacement(userPlayer, spawnPointService.GetPoint(2));
        }

        [ContextMenu("Place host avatar to spawn point(3)")]
        private void Test_PlaceHostAvatarToSpawnPoint3()
        {
            if (userPlayer == null)
            {
                log.LogError("{Method}: host avatar not found", nameof(Test_PlaceHostAvatarToSpawnPoint3));
                return;
            }

            SetupAvatarPlacement(userPlayer, spawnPointService.GetPoint(3));
        }
#endif
    }
}
