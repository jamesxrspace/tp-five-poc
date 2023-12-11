using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace TPFive.Game.SceneFlow
{
    public class LoadContext
    {
        public string Category { get; set; }

        public int CategoryOrder { get; set; }

        public int SubOrder { get; set; }

        public object Title { get; set; }

        public LoadSceneMode LoadSceneMode { get; set; }

        public LifetimeScope LifetimeScope { get; set; }
    }

    public class UnloadContext
    {
        public string Category { get; set; }

        public int CategoryOrder { get; set; }

        public int SubOrder { get; set; }

        public object Title { get; set; }
    }

    public class SceneContext
    {
        public object Title { get; set; }

        // public object ObjectTitle { get; set; }
        public Key Key { get; set; }

        public VContainer.Unity.LifetimeScope ParentLifetimeScope { get; set; }

        public VContainer.Unity.LifetimeScope CurrentLifetimeScope { get; set; }

        public Scene Scene { get; set; }

        public int CompareTo(Key other) => Key.CompareTo(other);

        // Should be moved to extension for extensibility.
        public int CompareTo(SceneContext other) => Key.CompareTo(other.Key);

        public int CompareTo(int other) => Key.CompareTo(new Key(other, 1));

        public override string ToString()
        {
            var desc = $"Title: {Title} Key: {Key}";

            return desc;
        }
    }

#pragma warning disable SA1313
    public record Key(int CategoryOrder, int SubOrder)
        : System.IComparable<Key>
    {
        public int CompareTo(Key other) =>
            other is null ? 1 : (CategoryOrder, SubOrder).CompareTo((other.CategoryOrder, other.SubOrder));

        public override string ToString()
        {
            return $"{CategoryOrder} - {SubOrder}";
        }
    }
#pragma warning restore SA1313
}
