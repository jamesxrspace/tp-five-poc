namespace TPFive.Creator.MessageRepo
{
    public interface IService
    {
        IServiceProvider NullServiceProvider { get; }

        // This is actually used mainly in visual scripting.
        // For now only adding one api for taking one string param, will adjust latter.
        void PublishMessage(string name, string stringParam);
    }

    public interface IServiceProvider : Game.IServiceProvider
    {
        void PublishMessage(string name, string stringParam);
    }
}
