namespace TPFive.Game
{
    public interface ISpawnPoint
    {
#if UNITY_EDITOR
        void SetRadiusForGizmo(float radius);
#endif
    }
}
