namespace TPFive.Game.RealtimeChat
{
    public class Participant : IParticipant
    {
        private readonly uint _uid;
        private readonly string _xrid;
        private readonly IChannel _channel;

        public Participant(IChannel channel, uint uid, string xrSocialId)
        {
            _channel = channel;
            _uid = uid;
            _xrid = xrSocialId;
        }

        public uint Uid => _uid;

        public string XRSocialId => _xrid;

        public IChannel Channel => _channel;
    }
}
