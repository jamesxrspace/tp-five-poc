using TPFive.OpenApi.GameServer.Model;

namespace TPFive.Game.RealtimeChat
{
    public class AgoraTokenInfo
    {
        private readonly string _token;

        public AgoraTokenInfo(string token)
        {
            _token = token;
        }

        public AgoraTokenInfo(AgoraStreamingTokenData data)
        {
            _token = data.Token;
        }

        public string Token => _token;

        public override string ToString()
        {
            return $"token={_token}";
        }
    }
}
