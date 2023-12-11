using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XR.Avatar;

namespace TPFive.AssetTools
{
    [TPFive.Game.Assist.Entry.RemoteController]
    public class AssetCollector : MonoBehaviour
    {
        private const int BatchSize = 5;
        [SerializeField]
        private AvatarAssetList avatarItemsFemale = null;
        [SerializeField]
        private AvatarAssetList avatarItemsMale = null;
        private List<GameObject> loadedAvatars = new List<GameObject>();

        public void CenterAvatars()
        {
            foreach (var go in loadedAvatars)
            {
                go.transform.position = Vector3.zero;
            }
        }

        public void RandomAllAvatarPosition()
        {
            foreach (var go in loadedAvatars)
            {
                go.transform.position = RandomPositionXZ(-10f, 10f);
            }
        }

        public void GenerateAvatars(int size)
        {
            var remainedSize = size;
            while (remainedSize > 0)
            {
                int batchSize = Mathf.Min(remainedSize, BatchSize);
                StartCoroutine(GenerateRandomAvatars(batchSize));
                remainedSize -= batchSize;
            }
        }

        public void ClearAllAvatar()
        {
            foreach (var go in loadedAvatars)
            {
                Destroy(go);
            }
        }

        protected void Start()
        {
            AsyncLoader.AutoInitAssetBundleSource();
        }

        private IEnumerator GenerateRandomAvatars(int size)
        {
            for (int i = 0; i < size; ++i)
            {
                yield return GenerateRandomAvatar();
            }
        }

        private Vector3 RandomPositionXZ(float min = -1f, float max = 1f)
        {
            return new Vector3(UnityEngine.Random.Range(min, max), 0f, UnityEngine.Random.Range(min, max));
        }

        private IEnumerator GenerateRandomAvatar()
        {
            // 50% female 50% male
            var isFemale = RandomChance(0.5f);
            var items = isFemale ? avatarItemsFemale : avatarItemsMale;

            var data = new CrossData();
            yield return AsyncLoader.LoadAsset<SkeletonData>(items.skeleton, data);
            var skeletonData = data.oValue as SkeletonData;

            var avatarForamt = RandomAvatarFormat(items, skeletonData);
            var go = new GameObject($"avatar_{loadedAvatars.Count}");
            go.transform.position = RandomPositionXZ(-10f, 10f);
            loadedAvatars.Add(go);
            var sm = go.AddComponent<SkinsManager>();
            yield return sm.LoadStylizedAvatar(new LoadAvatarOptions(avatarForamt, false, false, false));
            if (sm.TryGetComponent(out Animator ani))
            {
                var animationClipStr = RandomIndex(items.animations);
                yield return AsyncLoader.LoadAsset<AnimationClip>($"{animationClipStr}_anim", data);

                var controller = Instantiate(items.controller);
                var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                controller.GetOverrides(overrides);
                overrides[0] = new KeyValuePair<AnimationClip, AnimationClip>(
                    overrides[0].Key,
                    data.oValue as AnimationClip);
                controller.ApplyOverrides(overrides);
                ani.runtimeAnimatorController = controller;
            }

            var head = sm.GetSkinByPart(ESkinPart.S0);
            foreach (var mat in head.sharedMaterials)
            {
                if (mat.name.Contains("_head_"))
                {
                    // Clone main texture
                    mat.mainTexture = Instantiate(mat.mainTexture);
                }
            }

            var coat = sm.GetSkinByPart(ESkinPart.S1);
            foreach (var mat in coat.sharedMaterials)
            {
                if (mat.name.Contains("_body_"))
                {
                    // Clone main texture
                    mat.mainTexture = Instantiate(mat.mainTexture);
                }
            }
        }

        private bool RandomChance(float zeroToOneChance)
        {
            return UnityEngine.Random.value > Mathf.Clamp01(zeroToOneChance);
        }

        private AvatarFormat RandomAvatarFormat(AvatarAssetList items, SkeletonData skeletonData)
        {
            var avatarFormat = new AvatarFormat(skeletonData);
            avatarFormat.skin_parts.SetPart(
                ESkinPart.S0,
                skeletonData.defaultBodyParts.GetPart(ESkinPart.S0));
            avatarFormat.skin_parts.SetPart(
                ESkinPart.S1,
                RandomIndex(items.coats));
            avatarFormat.skin_parts.SetPart(
                ESkinPart.S3,
                RandomIndex(items.pants));
            avatarFormat.skin_parts.SetPart(
                ESkinPart.S4,
                RandomIndex(items.foots));
            avatarFormat.skin_parts.SetPart(
                ESkinPart.S5,
                RandomIndex(items.tops));
            avatarFormat.skin_parts.SetPart(
                ESkinPart.S6,
                RandomIndex(items.nails));
            avatarFormat.skin_parts.SetPart(
                ESkinPart.S7,
                RandomIndex(items.backs));
            // 30% add attachment
            if (RandomChance(0.3f))
            {
                avatarFormat.attachments.SetPart(
                    EAttachmentPart.A0,
                    RandomIndex(items.facedecks));
            }

            avatarFormat.body.Shape = UnityEngine.Random.Range(0, 3);

            return avatarFormat;
        }

        private T RandomIndex<T>(List<T> list)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }

#if UNITY_EDITOR
        /// <summary>
        /// Collect all items from Assets.
        /// </summary>
        [ContextMenu("Collect Avatar Items")]
        private void CollectAvatarItems()
        {
            if (avatarItemsMale == null || avatarItemsFemale == null)
            {
                Debug.LogWarning("avatarItemsMale or avatarItemsFemale not setted.");
                return;
            }

            // Search From Project AvatarV2Asset's Unity root folder
            var inputPath = UnityEditor.EditorUtility.OpenFolderPanel("Select Avatar Project Root", "./", "Assets");
            if (string.IsNullOrEmpty(inputPath))
            {
                return;
            }

            // Simple check for root of prefab folder
            var prefabPath = System.IO.Path.Combine(inputPath, "AssetRoot/Prefabs/");
            if (!System.IO.Directory.Exists(prefabPath))
            {
                Debug.LogWarning("Path Not Avatar Unity Project! (Select avatar asset project at \"Assets\" folder)");
                return;
            }

            // Root of animation folder
            var animationPath = System.IO.Path.Combine(inputPath, "AssetRoot/Animations/");

            // Adding SkeletonData keys
            avatarItemsFemale.Clear();
            avatarItemsFemale.skeleton = "avatar2_skeleton_female";
            avatarItemsMale.Clear();
            avatarItemsMale.skeleton = "avatar2_skeleton_male";

            // Searching for all .prefab files
            foreach (var f in System.IO.Directory.EnumerateFiles(inputPath, "*.prefab", System.IO.SearchOption.AllDirectories))
            {
                var file = f.Replace("\\", "/");
                // Check path contains female keywords, and store the target AvatarAssetList
                var current = file.Contains("/female/") ? avatarItemsFemale : avatarItemsMale;
                var directoryName = System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(file));
                var fileName = System.IO.Path.GetFileNameWithoutExtension(file);
                // Checking file name role here, for all fileName contains "hum_{gender}{type:02d}_..."
                // where gender is "m" or "f"
                // type is 1, 2, 3, 4 (different body shape), 5 (lod)
                if (fileName.Length < 7)
                {
                    continue;
                }

                // type == 5 -> ignore lod model.
                if (fileName[6] == '5')
                {
                    continue;
                }

                switch (directoryName)
                {
                    case "coat":
                        current.coats.Add(fileName);
                        break;
                    case "pants":
                        current.pants.Add(fileName);
                        break;
                    case "foot":
                        current.foots.Add(fileName);
                        break;
                    case "back":
                        current.backs.Add(fileName);
                        break;
                    case "nail":
                        current.nails.Add(fileName);
                        break;
                    case "top":
                        current.tops.Add(fileName);
                        break;
                    case "facedeck":
                        current.facedecks.Add(fileName);
                        break;
                }
            }

            foreach (var f in System.IO.Directory.EnumerateFiles(animationPath, "*.anim", System.IO.SearchOption.AllDirectories))
            {
                var file = f.Replace("\\", "/");
                var current = file.Contains("/female/") ? avatarItemsFemale : avatarItemsMale;
                var fileName = System.IO.Path.GetFileNameWithoutExtension(file);
                current.animations.Add(fileName);
            }
        }
#endif
    }
}
