namespace TPFive.Game.PlayerPrefs
{
    using TPFive.Model;

    public interface IService
    {
        Prefs GetPrefsByKey(string key);
    }
}