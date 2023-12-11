namespace TPFive.Creator
{
    public interface IService
    {
        T GetInstance<T>();
    }

    public class ServiceLocator : IService
    {
        // Need to use the actual scheme to get reference.
        public T GetInstance<T>()
        {
            return default;
        }
    }
}
