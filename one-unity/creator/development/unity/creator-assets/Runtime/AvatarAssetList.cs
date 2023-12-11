using System.Collections.Generic;
using UnityEngine;

namespace TPFive.AssetTools
{
    [CreateAssetMenu(menuName = "Benchmark Tools/Create Asset List")]
    public class AvatarAssetList : ScriptableObject
    {
        public string skeleton;
        // body parts
        public List<string> coats = new List<string>();
        public List<string> pants = new List<string>();
        public List<string> foots = new List<string>();
        public List<string> backs = new List<string>();
        public List<string> nails = new List<string>();
        public List<string> tops = new List<string>();
        // attachments
        public List<string> facedecks = new List<string>();

        public AnimatorOverrideController controller;
        // animation clips
        public List<string> animations = new List<string>();

        public void Clear()
        {
            coats.Clear();
            pants.Clear();
            foots.Clear();
            backs.Clear();
            nails.Clear();
            tops.Clear();
            facedecks.Clear();
            animations.Clear();
        }
    }
}
