namespace TPFive.Game.ReferenceLocator
{
    public interface IService
    {
        TService GetInstance<TService>();
    }
}
