using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TPFive.OpenApi.GameServer;
using TPFive.OpenApi.GameServer.Model;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Game.RealtimeChat
{
    public class AgoraRtcTokenProvider : IDisposable
    {
        private readonly ILogger _logger;
        private readonly IAgoraApi _agoraApi;
        private readonly string _appId;
        private readonly string _channelId;
        private readonly AgoraStreamingTokenPayload.RoleEnum _userRole;
        private readonly int _expiresIn;

        private AgoraTokenInfo _tokenInfo;
        private CancellationTokenSource _generateTokenCancellationSource;
        private bool _isTokenGenerating = false;
        private bool _disposed = false;

        public AgoraRtcTokenProvider(ILogger logger, IAgoraApi agoraApi, string appId, string channelId)
            : this(logger, agoraApi, appId, channelId, AgoraStreamingTokenPayload.RoleEnum.Publisher, 3600)
        {
        }

        public AgoraRtcTokenProvider(
            ILogger logger,
            IAgoraApi agoraApi,
            string appId,
            string channelId,
            AgoraStreamingTokenPayload.RoleEnum role,
            int expiresIn)
        {
            _logger = logger;
            _agoraApi = agoraApi;
            _appId = appId;
            _channelId = channelId;
            _userRole = role;
            _expiresIn = expiresIn;
        }

        public event Action<string> OnReceivedNewToken;

        public string Token => _tokenInfo?.Token;

        public void GenerateNewToken()
        {
            if (_isTokenGenerating)
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("Token is generating...");
                }

                return;
            }

            GetAgoraToken().Forget();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                CancelTokenGeneration();
            }

            _disposed = true;
        }

        private async UniTaskVoid GetAgoraToken()
        {
            _isTokenGenerating = true;

            AgoraTokenInfo newToken = null;
            CancellationToken cancellationToken = CreateGenerationCancellationToken();
            try
            {
                newToken = await FetchTokenFromServer(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("Cancel Agora rtc token generation");
                }

                _isTokenGenerating = false;
                return;
            }
            catch (Exception e)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError($"{nameof(GetAgoraToken)} failed.", e);
                }
            }

            // Token generate failed, start retry mechanism
            if (newToken == null)
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("Failed to get new Agora rtc token. Start retry...");
                }

                RetryGetAgoraToken(cancellationToken).Forget();
                return;
            }

            _tokenInfo = newToken;
            OnReceivedNewToken?.Invoke(newToken.Token);

            _isTokenGenerating = false;
        }

        private async UniTask<AgoraTokenInfo> FetchTokenFromServer(CancellationToken token)
        {
            var payload = new AgoraStreamingTokenPayload
            {
                ExpiresIn = _expiresIn,
                ChannelId = _channelId,
                Role = _userRole,
            };

            var response = await _agoraApi.GetAgoraStreamingTokenAsync(payload, cancellationToken: token).AsUniTask();

            if (response.IsSuccess)
            {
                return new AgoraTokenInfo(response.Data);
            }
            else
            {
                if (_logger.IsEnabled(LogLevel.Warning))
                {
                    _logger.LogWarning($"Failed to get rtm token, code={response.HttpStatusCode}, error_code={response.ErrorCode}");
                }
            }

            return null;
        }

        private async UniTaskVoid RetryGetAgoraToken(CancellationToken token, int delayInMilliseconds = 5000)
        {
            try
            {
                await UniTask.Delay(delayInMilliseconds, cancellationToken: token);
            }
            catch (OperationCanceledException)
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("Cancel Agora rtc token generation");
                }

                _isTokenGenerating = false;
                return;
            }

            // Try again
            GetAgoraToken().Forget();
        }

        private CancellationToken CreateGenerationCancellationToken()
        {
            CancelTokenGeneration();
            _generateTokenCancellationSource = new CancellationTokenSource();
            return _generateTokenCancellationSource.Token;
        }

        private void CancelTokenGeneration()
        {
            if (_generateTokenCancellationSource != null)
            {
                _generateTokenCancellationSource.Cancel();
                _generateTokenCancellationSource.Dispose();
                _generateTokenCancellationSource = null;
            }
        }
    }
}
