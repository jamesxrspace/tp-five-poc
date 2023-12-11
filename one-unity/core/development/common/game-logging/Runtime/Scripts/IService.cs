namespace TPFive.Game.Logging
{
    public interface IService
    {
        void Logging(System.Type t, int level, object message);
    }
}