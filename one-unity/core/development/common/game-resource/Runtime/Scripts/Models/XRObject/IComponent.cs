namespace TPFive.Game.Resource
{
    public interface IComponent
    {
        void Deserialize(string json);

        string Serialize();
    }
}