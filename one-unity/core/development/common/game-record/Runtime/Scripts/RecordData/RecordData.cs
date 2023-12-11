using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TPFive.Game.Record
{
    public abstract class RecordData : IEquatable<RecordData>
    {
        private string id;
        private JObject profile;

        private List<string> tags = new ();

        public RecordData(string id = default)
        {
            this.id = string.IsNullOrEmpty(id) ? Guid.NewGuid().ToString() : id;
            profile = new ();
        }

        public List<string> Tags => tags;

        public string Id => id;

        protected JObject Profile => profile;

        public abstract new RecordDataType GetType();

        // Serialize is supposed to be run on worker thread, for things that
        // need to be uses in Serialize function, and can be generated in
        // the main thread, we create them in BeforeSerialize
        // Since this function will be run on the main thread, do not put heavy
        // task into it, which may cause serious delay in the main thread
        public virtual void BeforeSerialize()
        {
        }

        public virtual byte[] Serialize()
        {
            // TBD: [TF3R-155] [Unity][Optimize] add hash table for serialize list
            return Segment.SerializeList(new List<byte[]>()
            {
                Encoding.UTF8.GetBytes(id),
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(tags)),
                GetByteFromProfile(Profile),
            });
        }

        public virtual void Deserialize(byte[] buffer)
        {
            var byteList = Segment.DeserializeList(buffer);
            id = Encoding.UTF8.GetString(byteList[0]);
            tags = JsonConvert.DeserializeObject<List<string>>(Encoding.UTF8.GetString(byteList[1]));
            SetProfile(GetProfileFromByte(byteList[2]));
        }

        // Deserialize is supposed to be run on worker thread, for things that
        // need to be uses in Deserialize function, and can be generated in
        // the main thread, we create them in PostDeserialize
        // Since this function will be run on the main thread, do not put heavy
        // task into it, which may cause serious delay in the main thread
        public virtual void PostDeserialize()
        {
        }

        public bool Equals(RecordData other)
        {
            return other != null && id == other.id && GetType() == other.GetType();
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(id, GetType());
        }

        protected static byte[] GetByteFromProfile(object profile)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(profile).Replace("\n", " "));
        }

        protected static object GetProfileFromByte(byte[] bytes)
        {
            return JsonConvert.DeserializeObject<JObject>(Encoding.UTF8.GetString(bytes));
        }

        protected void SetProfile(object profile)
        {
            this.profile = JObject.FromObject(profile);
        }
    }
}
