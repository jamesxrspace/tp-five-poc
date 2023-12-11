using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TPFive.Creator.Editor
{
    using TPFive.SCG.Logging.Abstractions;

    using Bundle.Command.Editor;

    using CreatorCrossEditorBridge = TPFive.Creator.Cross.Editor.Bridge;

    [EditorUseLogging]
    public sealed partial class BundleEditorWindow : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset leftPaneVisualTreeAsset;

        [SerializeField]
        private VisualTreeAsset rightPaneVisualTreeAsset;

        [SerializeField]
        private int selectedIndex = -1;

        private VisualElement _rightPane;

        private string _addingType = "Level";

        [MenuItem("TPFive/Bundle/Bundle Editor Window")]
        public static void ShowDefaultWindow()
        {
            var wnd = GetWindow<BundleEditorWindow>();
            wnd.titleContent = new GUIContent("Bundle Editor Window");
        }

        public void CreateGUI()
        {
            if (leftPaneVisualTreeAsset == null)
            {
                return;
            }

            if (rightPaneVisualTreeAsset == null)
            {
                return;
            }

            RepaintLeftPanel();
        }

        private void RepaintLeftPanel()
        {
            // Clear previous content draw
            rootVisualElement.Clear();
            _rightPane?.Clear();
            _rightPane = new VisualElement();

            // Create a two-pane view with the left pane being fixed with
            var splitView = new TwoPaneSplitView(0, 150, TwoPaneSplitViewOrientation.Horizontal);

            // Add the panel to the visual tree by adding it as a child to the root element
            rootVisualElement.Add(splitView);

            var leftPane = CreateLeftPanelView();
            splitView.Add(leftPane);
            splitView.Add(_rightPane);
        }

        private VisualElement CreateLeftPanelView()
        {
            var leftPanel = new VisualElement();
            var container = leftPaneVisualTreeAsset.Instantiate();

            var setupProfileButton = container.Q<Button>("setupProfile");
            if (setupProfileButton != null)
            {
                setupProfileButton.clicked += () =>
                {
                    AddProfile.Handle();
                    RepaintLeftPanel();
                };
            }

            var dropdownField = container.Q<DropdownField>("addKind");
            if (dropdownField != null)
            {
                dropdownField.value = _addingType;
                dropdownField.RegisterValueChangedCallback(v =>
                {
                    _addingType = v.newValue;
                });
            }

            var addTypeButton = container.Q<Button>("addType");
            if (addTypeButton != null)
            {
                addTypeButton.clicked += () =>
                {
                    if (_addingType.Equals("Level"))
                    {
                        AddLevel.Handle();
                    }
                    else if (_addingType.Equals("SceneObject"))
                    {
                        AddSceneObject.Handle();
                    }
                    else if (_addingType.Equals("Particle"))
                    {
                        AddParticle.Handle();
                    }

                    RepaintLeftPanel();
                };
            }

            var refreshButton = container.Q<Button>("refresh");
            if (refreshButton != null)
            {
                refreshButton.clicked += RepaintLeftPanel;
            }

            leftPanel.Add(container);

            // Add BundleDetailData list to the left panel
            var dataListView = CreateBundleDetailDataListView(container);
            leftPanel.Add(dataListView);

            return leftPanel;
        }

        // Add list view to the container
        private ListView CreateBundleDetailDataListView(TemplateContainer container)
        {
            // Get a list of all bundle detail data in the project
            var bundleDetailDataList = GetBundleDetailDataList();

            var listView = new ListView();
            listView.makeItem = () => new Label();
            listView.bindItem = (item, index) => { (item as Label).text = bundleDetailDataList[index].title; };
            listView.itemsSource = bundleDetailDataList;
            listView.onSelectionChange += HandleLeftPaneOnSelectionChange;

            // Restore the selection index from before the hot reload
            listView.selectedIndex = selectedIndex;

            // Store the selection index when the selection changes
            listView.onSelectionChange += (items) => { selectedIndex = listView.selectedIndex; };

            return listView;
        }

        private List<BundleDetailData> GetBundleDetailDataList()
        {
            var guids = AssetDatabase.FindAssets("t:BundleDetailData");

            return guids.Select(guid => AssetDatabase.LoadAssetAtPath<BundleDetailData>(AssetDatabase.GUIDToAssetPath(guid)))
                .Where(x => x != null && System.Enum.GetName(typeof(BundleKind), x.bundleKind).Equals(_addingType))
                .ToList();
        }

        private void HandleLeftPaneOnSelectionChange(IEnumerable<object> selectedItems)
        {
            var bundleDetailData = selectedItems.FirstOrDefault() as BundleDetailData;
            if (bundleDetailData == null)
            {
                return;
            }

            // Select the asset
            Selection.activeObject = bundleDetailData;

            // Clear all previous content from the panel
            _rightPane.Clear();

            var rightPanelView = CreateRightPanelView(bundleDetailData);
            _rightPane.Add(rightPanelView);
        }

        private TemplateContainer CreateRightPanelView(BundleDetailData bundleDetailData)
        {
            var container = rightPaneVisualTreeAsset.Instantiate();

            var so = new SerializedObject(bundleDetailData);

            var titleProperty = so.FindProperty("title");
            var descriptionProperty = so.FindProperty("description");
            var bundleUsageProperty = so.FindProperty("bundleUsage");
            var idProperty = so.FindProperty("id");
            var bundleKindProperty = so.FindProperty("bundleKind");

            var titlePropertyField = container.Q<PropertyField>("title");
            titlePropertyField.BindProperty(titleProperty);

            var descriptionPropertyField = container.Q<PropertyField>("description");
            descriptionPropertyField.BindProperty(descriptionProperty);

            var bundleUsagePropertyField = container.Q<PropertyField>("bundleUsage");
            bundleUsagePropertyField.BindProperty(bundleUsageProperty);

            var idPropertyField = container.Q<PropertyField>("id");
            idPropertyField.BindProperty(idProperty);
            idPropertyField.SetEnabled(false);

            var bundleKindPropertyField = container.Q<PropertyField>("bundleKind");
            bundleKindPropertyField.BindProperty(bundleKindProperty);

            // Bind buttons from path
            var switchToLevelButton = container.Q<Button>("switchToLevel");
            if (bundleKindProperty.enumValueIndex == (int)BundleKind.Level)
            {
                switchToLevelButton.visible = true;
                if (switchToLevelButton != null)
                {
                    switchToLevelButton.clicked += LoadSpecificScene.Handle(bundleDetailData);
                }
            }
            else
            {
                switchToLevelButton.visible = false;
            }

            var setupAddressableButton = container.Q<Button>("setupAddressable");
            if (setupAddressableButton != null)
            {
                setupAddressableButton.clicked += SetupAddressableContent.Handle(bundleDetailData);
            }

            var buildAddressableButton = container.Q<Button>("buildAddressable");
            if (buildAddressableButton != null)
            {
                buildAddressableButton.clicked += BuildAddressable.Handle;
            }

            var uploadAddressableButton = container.Q<Button>("uploadAddressable");
            if (uploadAddressableButton != null)
            {
                var id = bundleDetailData.id;
                var path = AssetDatabase.GetAssetPath(bundleDetailData);
                var parentPath = Path.GetDirectoryName(path);

                uploadAddressableButton.clicked += UploadFolder.Handle(id, parentPath);
            }

            var uploadUnitypackageButton = container.Q<Button>("uploadUnitypackage");
            if (uploadUnitypackageButton != null)
            {
                var id = bundleDetailData.id;
                var path = AssetDatabase.GetAssetPath(bundleDetailData);
                var parentPath = Path.GetDirectoryName(path);

                uploadUnitypackageButton.clicked += UploadFile.Handle(id, parentPath);
            }

            var uploadUgcButton = container.Q<Button>("uploadUgc");
            if (uploadUgcButton != null)
            {
                uploadUgcButton.clicked += UploadToUgcService.Handle(bundleDetailData);
            }

            var uploadUnitypackageAndUgcButton = container.Q<Button>("uploadUnitypackageAndUgc");
            if (uploadUnitypackageAndUgcButton != null)
            {
                uploadUnitypackageAndUgcButton.clicked += () =>
                {
                    var id = bundleDetailData.id;
                    var path = AssetDatabase.GetAssetPath(bundleDetailData);
                    var parentPath = Path.GetDirectoryName(path);

                    UploadFile.Handle(id, parentPath)?.Invoke();
                    UploadToUgcService.Handle(bundleDetailData)?.Invoke();
                };
            }

            return container;
        }
    }
}
