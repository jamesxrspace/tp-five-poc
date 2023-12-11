using Cysharp.Threading.Tasks;

namespace TPFive.Game.RealtimeChat
{
    public delegate void AudioInputMutedEventHandler(bool isMute);

    public delegate void LocalUserSpeakingEventHandler(bool isSpeaking);

    public interface IService
    {
        event AudioInputMutedEventHandler OnAudioInputMuted;

        event LocalUserSpeakingEventHandler OnLocalUserSpeaking;

        IServiceProvider NullServiceProvider { get; }

        void MuteAudioInput(bool isMute);

        bool GetLocalUserSpeaking();

        bool GetAudioInputMuted();

        UniTask<IChannel> CreateChannel(ChannelId channelId);

        void ReleaseChannel(ChannelId channelId);
    }

    public interface IServiceProvider : Game.IServiceProvider
    {
        event AudioInputMutedEventHandler OnAudioInputMuted;

        event LocalUserSpeakingEventHandler OnLocalUserSpeaking;

        void MuteAudioInput(bool isMute);

        bool GetLocalUserSpeaking();

        bool GetAudioInputMuted();

        IChannel CreateChannel(ChannelId channelId);

        void ReleaseChannel(ChannelId channelId);
    }
}
