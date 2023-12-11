using System.Collections.Generic;

namespace TPFive.Extended.Decoration
{
    /// <summary>
    /// Calculate how many entities this object has and store it.
    /// </summary>
    /// <typeparam name="T">Any type you want.</typeparam>
    public class EntityReference<T>
        where T : UnityEngine.Object
    {
        public EntityReference(string id)
        {
            ID = id;
        }

        public string ID { get; private set; }

        public int Count => UpdateReferenceCount();

        public List<T> EntityList { get; private set; } = new ();

        public void Add(T go)
        {
            EntityList.Add(go);
        }

        public bool Remove(T go)
        {
            return EntityList.Remove(go);
        }

        public void Clear()
        {
            EntityList.Clear();
        }

        private int UpdateReferenceCount()
        {
            for (var i = 0; i < EntityList.Count; i++)
            {
                if (EntityList[i] == null)
                {
                    EntityList.RemoveAt(i);
                    i--;
                }
            }

            return EntityList.Count;
        }
    }
}