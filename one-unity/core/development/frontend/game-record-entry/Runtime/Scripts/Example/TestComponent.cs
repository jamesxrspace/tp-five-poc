using Newtonsoft.Json.Linq;
using TPFive.Game.Resource;
using UnityEngine;

namespace TPFive.Game.Record.Example
{
    internal class TestComponent : MonoBehaviour, IComponent
    {
        private const string Key = "speed";

        [SerializeField]
        private float speed = 10f;

        public float Speed
        {
            get => speed;
            set => speed = value;
        }

        public void Deserialize(string json)
        {
            var data = JObject.Parse(json);
            speed = data[Key].ToObject<float>();
        }

        public string Serialize()
        {
            var data = new JObject() { { Key, JToken.FromObject(speed) }, };
            return data.ToString();
        }
    }
}