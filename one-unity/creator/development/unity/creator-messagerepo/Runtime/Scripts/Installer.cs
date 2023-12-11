// TODO: Source code gen this part later
using MessagePipe;
using VContainer;
using VContainer.Unity;

namespace TPFive.Creator.MessageRepo
{
    public class Installer : IInstaller
    {
        private readonly MessagePipeOptions _messagePipeOptions;

        public Installer(MessagePipeOptions messagePipeOptions)
        {
            _messagePipeOptions = messagePipeOptions;
        }

        public void Install(IContainerBuilder builder)
        {
            builder.Register<Service>(Lifetime.Singleton).As<IService>();
        }
    }
}
