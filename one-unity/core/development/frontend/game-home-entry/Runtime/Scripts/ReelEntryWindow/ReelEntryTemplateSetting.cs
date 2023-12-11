using System;
using System.Linq;
using TPFive.Game.Record;
using UnityEngine;

namespace TPFive.Game.Home.Entry
{
    [Serializable]
    public class ReelEntryTemplate
    {
        [SerializeField]
        private ReelSceneDesc reelSceneDesc;

        public ReelSceneDesc ReelSceneDesc => reelSceneDesc;
    }

    [CreateAssetMenu(fileName = "ReelEntryTemplateSetting", menuName = "TPFive/Game/Home/ReelEntryTemplateSetting")]
    public class ReelEntryTemplateSetting : ScriptableObject
    {
        [SerializeField]
        private ReelEntryTemplate[] reelEntryTemplates;

        public ReelEntryTemplate[] ReelEntryTemplates => reelEntryTemplates;

        public ReelEntryTemplate GetReelEntryTemplate(string name)
        {
            try
            {
                return Array.Find(reelEntryTemplates, reelEntryTemplate => reelEntryTemplate.ReelSceneDesc.Name == name);
            }
            catch (ArgumentNullException e)
            {
                Debug.LogWarning(e);
                return reelEntryTemplates.FirstOrDefault();
            }
        }
    }
}