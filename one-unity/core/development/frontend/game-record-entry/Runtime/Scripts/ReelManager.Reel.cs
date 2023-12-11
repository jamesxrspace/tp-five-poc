using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TPFive.Game.Reel;
using UnityEngine;

namespace TPFive.Game.Record.Entry
{
    public partial class ReelManager : MonoBehaviour
    {
        public async UniTask<bool> CreateAndPostReel(CreateReelData reelData, CancellationToken cancellationToken = default)
        {
            log.LogDebug("{Method}()", nameof(CreateAndPostReel));
            if (reelData == null)
            {
                string message = "Reel data is null, post failed.";
                log.LogWarning("{Method}(): {msg}", nameof(CreateAndPostReel), message);
                throw new ArgumentNullException(message);
            }

            var reel = await reelService.CreateReel(reelData, cancellationToken);
            if (string.IsNullOrEmpty(reel.Id))
            {
                log.LogWarning("{Method}(): Create reel failed.", nameof(CreateAndPostReel));
                return false;
            }

            return await reelService.PublishReel(reel.Id, cancellationToken);
        }

        public UniTask<bool> DeleteReel(string reelId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(reelId))
            {
                string message = "Reel id is invalid, delete reel failed.";
                log.LogWarning("{Method}(): {msg}", nameof(DeleteReel), message);
                throw new ArgumentException(message);
            }

            return reelService.DeleteReel(reelId, cancellationToken);
        }
    }
}
