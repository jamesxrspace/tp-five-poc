using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TPFive.Extended.Addressable.Editor
{
    using TPFive.SCG.Logging.Abstractions;

    using TPFive.Extended.Addressable.Command.Editor;

    [CustomEditor(typeof(ContentOverviewData), true)]
    [EditorUseLogging]
    public sealed partial class ContentOverviewDataEditor : UnityEditor.Editor
    {
        private static readonly string UxmlPath = Path.Combine(
            Define.FetcherEditorPath,
            "Scripts",
            "Content",
            "UI",
            "Data Assets",
            "ContentOverviewDataEditor.uxml");

        [SerializeField]
        private VisualTreeAsset itemAsset;

        private ContentOverviewData Target => target as ContentOverviewData;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement container = new ();

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath);
            var visualElement = visualTree.Instantiate();
            container.Add(visualElement);

            // Use progress bar to show progress later, for now, showing text
            var progressLabel = container.Q<Label>("progressLabel");
            progressLabel.text = "Progress: 0%";
            progressLabel.SetEnabled(false);

            // Setup button event handlers
            {
                var button = container.Q<Button>("updateOverview");
                if (button != null)
                {
                    button.clicked += HandleUpdateOverviewButtonClicked();
                }
            }

            {
                var button = container.Q<Button>("clearSelected");
                if (button != null)
                {
                    button.clicked += HandleClearSelectedButtonClicked();
                }
            }

            {
                var button = container.Q<Button>("importSelected");
                if (button != null)
                {
                    button.clicked += HandleImportSelectedButtonClicked(progressLabel);
                }
            }

            {
                var button = container.Q<Button>("placeIntoAddressables");
                if (button != null)
                {
                    button.clicked += HandlePlaceIntoAddressablesButtonClicked();
                }
            }

            var listView = visualElement.Q<ListView>("listView");
            listView.makeItem = () => itemAsset.CloneTree();

            return container;
        }

        private System.Action HandleUpdateOverviewButtonClicked()
        {
            return async () =>
            {
                Logger.LogDebug("{Method}", nameof(HandleUpdateOverviewButtonClicked));
                await UpdateContentOverview.Handle(Target);
            };
        }

        private System.Action HandleClearSelectedButtonClicked()
        {
            return () =>
            {
                Logger.LogDebug("{Method}", nameof(HandleClearSelectedButtonClicked));
                foreach (var fileContent in Target.UnitypackageList)
                {
                    fileContent.ToBeImported = false;
                }
            };
        }

        private System.Action HandleImportSelectedButtonClicked(Label progressLabel)
        {
            return async () =>
            {
                Logger.LogDebug("{Method}", nameof(CreateInspectorGUI));

                if (!Target.UnitypackageList.Any())
                {
                    return;
                }

                var idList = Target.UnitypackageList.Where(x => x.ToBeImported).Select(x => x.Id).ToList();
                var parentPath = Path.GetFullPath(Define.FetcherDownloadPath);
                progressLabel.SetEnabled(true);
                progressLabel.text = "Progress: 0%";
                await DownloadFiles.Handle(idList, parentPath, HandleProgress);
                progressLabel.SetEnabled(false);
            };

            void HandleProgress(float progress)
            {
                Logger.LogDebug(
                    "{Method} - progress: {progress}",
                    nameof(HandleImportSelectedButtonClicked),
                    progress);

                // Note that delayCall will be called multiple times, so we need to remove the previous one.
                // Besides, delayCall will be called at main thread.
                EditorApplication.delayCall -= DelayCall(progressLabel, progress);
                EditorApplication.delayCall += DelayCall(progressLabel, progress);
            }
        }

        private EditorApplication.CallbackFunction DelayCall(Label progressLabel, float progress)
        {
            return () =>
            {
                progressLabel.text = $"Progress: {progress}%";
                Repaint();
            };
        }

        private System.Action HandlePlaceIntoAddressablesButtonClicked()
        {
            return () =>
            {
                var idList = Target.UnitypackageList.Where(x => x.ToBeImported).Select(x => x.Id).ToList();
                PlaceIntoAddressables.Handle(idList);
            };
        }
    }
}
