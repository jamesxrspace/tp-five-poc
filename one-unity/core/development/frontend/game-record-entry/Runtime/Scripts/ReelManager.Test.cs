#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TPFive.Game.Record.Entry.Settings;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TPFive.Game.Record.Entry
{
    using TPFive.Game.Avatar.Motion;
    using TPFive.Game.Decoration;
    using TPFive.Game.Resource;
    using TPFive.Game.Resource.Serialization;
    using AvatarAnchorPointType = TPFive.Game.Avatar.Attachment.AnchorPointType;
    using DecorationAnchorPointDefinition = TPFive.Game.Decoration.Attachment.AnchorPointDefinition;
    using DecorationAnchorPointType = TPFive.Game.Decoration.Attachment.AnchorPointType;

    /// <summary>
    /// This partial class is used to test the record scene and decoration function.
    /// </summary>
    public partial class ReelManager
    {
        [SerializeField]
        [Tooltip("Display current store file path")]
        private string filePath;

        [SerializeField]
        [Tooltip("The bundle id of the decoration to attach to the avatar.")]
        private string attachmentBundleId = "a0b8cc4e-6284-4b21-9bca-d0142214ec6b";

        [SerializeField]
        private MotionItem testMotionItem;

        private DecorationAttachment curAttachment;

        private ReelAvatarMotionController _reelAvatarMotionController;

        [ContextMenu("Test/StartRecordScene")]
        public void StartRecordScene()
        {
            log.LogDebug("StartRecord - Scene");
            var sceneData = new XRObject()
            {
                Uid = "123456",
                Owner = "Tingwei",
                ObjectName = "Bar",
                ObjectType = "Scene",
                FileFormat = ".bundle",
                BundleId = "XXXXXXXXXXXXXXXXXX-scene",
            };

            var tree = new XRObject()
            {
                Uid = "test_tree",

                // Instantiator = "Tingwei",        // Test Ignore serialize value
                ObjectName = "Tree",
                ObjectType = "Decoration",
                FileFormat = ".bundle",
                BundleId = "XXXXXXXXXXXXXXXXXX-tree",
            };

            var table = new XRObject()
            {
                Uid = "test_table",
                Owner = "Tingwei",
                ObjectName = "Table",
                ObjectType = "Decoration",
                FileFormat = ".bundle",
                BundleId = "XXXXXXXXXXXXXXXXXX-table",
            };

            var decorations = new Dictionary<string, XRObject>()
            {
                { tree.ObjectName, tree }, { table.ObjectName, table },
            };

            var sceneRecordData = new SceneRecordData(sceneData, decorations);
            var recordData = new RecordData[] { sceneRecordData };
            recordService.StartRecord(recordData);
        }

        [ContextMenu("Test/StartRecordDecoration")]
        public void StartRecordDecoration()
        {
            log.LogDebug("StartRecord - Decoration");
            var tree = new XRObject()
            {
                Uid = "test_tree",

                // Instantiator = "Tingwei",        // Test Ignore serialize value
                ObjectName = "Tree",
                ObjectType = "Decoration",
                FileFormat = ".bundle",
                BundleId = "XXXXXXXXXXXXXXXXXX-tree",
            };

            var table = new XRObject()
            {
                Uid = "test_table",
                Owner = "Tingwei",
                ObjectName = "Table",
                ObjectType = "Decoration",
                FileFormat = ".bundle",
                BundleId = "XXXXXXXXXXXXXXXXXX-table",
            };

            var decorationRecordData = new RecordData[]
            {
                new DecorationRecordData(tree),
                new DecorationRecordData(table),
            };

            recordService.StartRecord(decorationRecordData);
        }

        [ContextMenu("Test/StopRecordAndStoreData")]
        public UniTask<string> StopRecordAndStoreData()
        {
            log.LogDebug("StopRecord");

            recordService.StopRecord();

            var recordData = recordService.GetRecordData();

            log.LogDebug("Start ExportToFile");
            return recordService.ExportToFile(recordData)
                .ContinueWith(result =>
                {
                    filePath = result.Item1;
                    log.LogDebug($"ExportToFile Success: FilePath={result.Item1}, BgmPath={result.Item2}");
                }).ContinueWith(() => filePath);
        }

        [ContextMenu("Test/RunRecordSceneAndLoad")]
        public void RunRecordSceneAndLoad()
        {
            StartRecordScene();
            StopRecordAndStoreData()
                .ContinueWith(filePath => LoadRecordAssets(filePath))
                .ContinueWith(recordData => DebugPrintRecordData(recordData));
        }

        [ContextMenu("Test/RunRecordDecorationAndLoad")]
        public void RunRecordDecorationAndLoad()
        {
            StartRecordDecoration();
            StopRecordAndStoreData()
                .ContinueWith(filePath => LoadRecordAssets(filePath))
                .ContinueWith(recordData => DebugPrintRecordData(recordData));
        }

        [ContextMenu("Test/Serialize Component To Another")]
        public void SerializeComponentToAnother()
        {
            // Create a mock Decoration(XRSceneObject) in Scene.
            var xrSceneObject = CreateDecoration();
            var data = CreateDecorationData("My Decoration Table", xrSceneObject.transform);

            // Simulate XRSceneObject can read XRObject data.
            xrSceneObject.FromXRObject(data);

            // Get this XRSceneObject's XRObject data.
            var decorationData = xrSceneObject.ToXRObject();

            // Create another XRSceneObject and write data in Scene.
            CreateDecoration().FromXRObject(decorationData);
        }

        [ContextMenu("Test/Record Scene And Decoration Components To File")]
        public async UniTask RecordSceneAndDecorationComponentsToFile()
        {
            var xrSceneObject = CreateDecoration();
            var data = CreateDecorationData("My Decoration Table", xrSceneObject.transform);
            xrSceneObject.FromXRObject(data);

            var decorationData = xrSceneObject.ToXRObject();
            var sceneRecordData = new SceneRecordData(CreateMockSceneData("MyHome"));

            // Put decoration into SceneRecordData
            sceneRecordData.AddDecoration(decorationData);

            // Save XRObject to file.
            recordService.StartRecord(new RecordData[] { sceneRecordData });

            recordService.StopRecord();

            // Read XRObject from file.
            var location = await recordService.ExportToFile(recordService.GetRecordData());
            filePath = location.Item1;
            log.LogDebug($"ExportToFile Success : {filePath}");
        }

        [ContextMenu("Test/Read Scene And Decoration Components From File")]
        public void ReadSceneAndDecorationComponentsFromFile()
        {
            LoadRecordAssets(filePath)
                .ContinueWith(recordData =>
                {
                    DebugPrintRecordData(recordData);

                    // Read XRObject to instantiate Decoration on Scene.
                    var decorationData = recordData.OfType<SceneRecordData>()
                        .Select(x => x.Decorations);

                    foreach (var map in decorationData)
                    {
                        InstantiateGameObjectFromDecorationMap(map);
                    }
                });
        }

        private void InstantiateGameObjectFromDecorationMap(Dictionary<string, XRObject> map)
        {
            foreach (var item in map)
            {
                CreateDecoration().FromXRObject(item.Value);
            }
        }

        private void DebugPrintRecordData(RecordData[] recordData)
        {
            log.LogDebug("DebugRecordData - Print Below");
            foreach (var data in recordData)
            {
                log.LogDebug($"RecordData: {data}");
            }
        }

        private async UniTask<RecordData[]> LoadRecordAssets(string path)
        {
            try
            {
                var recordData = await recordService.LoadRecordAssets(path);
                log.LogDebug("LoadRecordAssets - Success");
                return recordData.ToArray();
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
            }

            return null;
        }

        private XRObject CreateMockSceneData(string name = "TestScene")
        {
            var sceneData = new XRObject()
            {
                Uid = $"{DateTime.Now}-{name}",
                Owner = "Tingwei",
                ObjectName = name,
                ObjectType = "Scene",
                FileFormat = ".bundle",
                BundleId = $"bundle-id-{name}",
            };

            return sceneData;
        }

        private XRObject CreateDecorationData(string name, Transform t)
        {
            var data = new XRObject()
            {
                Uid = $"{DateTime.Now}_{name}",
                Owner = "Tingwei",
                ObjectName = name,
                ObjectType = "Decoration",
                FileFormat = ".bundle",
                BundleId = $"bundle-id-{name}",
            };

            // Apply Transform
            data.WriteComponent(t.transform.GetType().Name, t.transform.ToJson());

            // Apply other components
            foreach (var comp in t.GetComponentsInChildren<IComponent>())
            {
                data.WriteComponent(comp.GetType().Name, comp);
            }

            return data;
        }

        private XRSceneObject CreateDecoration()
        {
            // Instantiate a Decoration on Scene, and serialize his component data.
            GameObject go = new GameObject();
            var randomPosition = Random.Range(0, 10f);
            var randomRotation = Random.Range(0, 360f);
            var randomLocalScale = Random.Range(0, 5f);
            var randomSpeed = Random.Range(0, 10f);

            // Change some variable for testing.
            go.transform.position = new Vector3(randomPosition, randomPosition, randomPosition);
            go.transform.Rotate(randomRotation, randomRotation, randomRotation);
            go.transform.localScale = new Vector3(randomLocalScale, randomLocalScale, randomLocalScale);

            var xrSceneObject = go.AddComponent<XRSceneObject>();

            // Add some component for testing that implement IComponent interface.
            go.AddComponent<Example.TestComponent>().Speed = randomSpeed;

            return xrSceneObject;
        }

        [ContextMenu("Test/Decoration/" + nameof(LoadGameObjectToAttachAvatar))]
        private void LoadGameObjectToAttachAvatar()
        {
            if (userPlayer.Avatar.AnchorPointProvider.TryGetAnchorPoint(Avatar.Attachment.AnchorPointType.RightWrist, out var anchorTransform))
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.transform.SetParent(anchorTransform, false);
                go.transform.SetLocalPositionAndRotation(Vector3.one, Quaternion.identity);
                go.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            }
        }

        [ContextMenu("Test/Decoration/" + nameof(LoadDecorationToAttachPredefineAvatar))]
        private void LoadDecorationToAttachPredefineAvatar()
        {
            loader.CreateDecorationAttachment(
                attachmentBundleId)
                .ContinueWith(attachment =>
                {
                    curAttachment = attachment;
                    ApplyAttachmentToMotionMode(curAttachment, ReelAvatarMotionMode.Predefine);
                });
        }

        [ContextMenu("Test/Decoration/" + nameof(LoadDecorationToAttachUpperBodyTrackingAvatar))]
        private void LoadDecorationToAttachUpperBodyTrackingAvatar()
        {
            loader.CreateDecorationAttachment(
                    attachmentBundleId)
                .ContinueWith(attachment =>
                {
                    curAttachment = attachment;
                    ApplyAttachmentToMotionMode(curAttachment, ReelAvatarMotionMode.UpperBodyTracking);
                });
        }

        [ContextMenu("Test/Decoration/" + nameof(DestroyDecoration))]
        private void DestroyDecoration()
        {
            loader.DestroyDecorationAttachment(attachmentBundleId, curAttachment).Forget();
        }

        [ContextMenu("Test/Decoration/" + nameof(ApplyPredefineAttachmentToAvatar))]
        private void ApplyPredefineAttachmentToAvatar()
        {
            ApplyAttachmentToMotionMode(curAttachment, ReelAvatarMotionMode.Predefine);
        }

        [ContextMenu("Test/Decoration/" + nameof(ApplyUpperBodyTrackingAttachmentToAvatar))]
        private void ApplyUpperBodyTrackingAttachmentToAvatar()
        {
            ApplyAttachmentToMotionMode(curAttachment, ReelAvatarMotionMode.UpperBodyTracking);
        }

        private void ApplyAttachmentToMotionMode(DecorationAttachment attachment, ReelAvatarMotionMode mode)
        {
            if (!reelAttachmentSettings.TryGetCategory(mode, out var category))
            {
                log.LogWarning("Cannot find category for motion {Mode}.", mode);
                return;
            }

            loader.TryApplyAttachmentToAvatar(
                attachment,
                category,
                userPlayer.Avatar.AnchorPointProvider);
        }

        [ContextMenu("Test/Motion/" + nameof(PlayAvatarMotionByGuid))]
        private void PlayAvatarMotionByGuid()
        {
            _reelAvatarMotionController ??= new ReelAvatarMotionController(loggerFactory, userPlayer.Avatar);
            _reelAvatarMotionController.PlayMotion(testMotionItem.Uid);
        }

        [ContextMenu("Test/Motion/" + nameof(PlayAvatarMotionByTimeline))]
        private void PlayAvatarMotionByTimeline()
        {
            _reelAvatarMotionController ??= new ReelAvatarMotionController(loggerFactory, userPlayer.Avatar);
            _reelAvatarMotionController.PlayMotion(testMotionItem.Asset);
        }
    }
}
#endif
