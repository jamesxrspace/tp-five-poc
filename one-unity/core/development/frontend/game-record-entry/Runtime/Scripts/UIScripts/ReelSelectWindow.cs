using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Observables;
using Loxodon.Framework.Views;
using TMPro;
using TPFive.Game.UI;
using TPFive.Model;
using UnityEngine;
using UnityEngine.UI;

namespace TPFive.Game.Record.Entry
{
    public sealed class ReelSelectWindow : WindowBase
    {
        [SerializeField]
        private TMP_Dropdown fileDropdown;

        [SerializeField]
        private Button joinButton;

        [SerializeField]
        private Button endButton;

        [SerializeField]
        private Toggle faceTrackingToggle;

        [SerializeField]
        private Toggle bodyTrackingToggle;

        [SerializeField]
        private Toggle micToggle;

        [SerializeField]
        private Button setCameraButton;

        [SerializeField]
        private Button previewDoneButton;

        [SerializeField]
        private Button backToRecordButton;

        [SerializeField]
        private Button backToFeedButton;

        [SerializeField]
        private Toggle recordToggle;

        [SerializeField]
        private TMP_Dropdown cameraDropdown;

        [Tooltip("Upload Reel")]
        [SerializeField]
        private Button cancelUploadButton;

        [SerializeField]
        private TMP_InputField descriptionInputField;

        [SerializeField]
        private TMP_Dropdown joinModeDropdown;

        [SerializeField]
        private Button discardButton;

        [SerializeField]
        private Button draftButton;

        [SerializeField]
        private Button uploadButton;

        [SerializeField]
        private TMP_Text countdownText;
        [SerializeField]
        private Button generateMotionButton;

        [SerializeField]
        private Toggle playMusicMotionToggle;

        [SerializeField]
        private List<WindowUIState> windowUIStateList;

        private bool isJoined;
        private bool showAIGCHint;
        private int cameraTrackCount;
        private int recordingCountdown;
        private float recordDuration;
        private RecordStateTypeEnum recordState;
        private ObservableList<string> files = new ObservableList<string>();

        public RecordStateTypeEnum RecordState
        {
            get => recordState;
            set
            {
                if (recordState == value)
                {
                    return;
                }

                recordState = value;
                UpdateUIDisplay(value);
            }
        }

        public ObservableList<string> Files
        {
            get => files;
            set
            {
                if (files == value)
                {
                    return;
                }

                if (files != null)
                {
                    files.CollectionChanged -= OnCollectionChanged;
                }

                files = value;

                OnFilesChanged();

                if (files != null)
                {
                    files.CollectionChanged += OnCollectionChanged;
                }
            }
        }

        public bool IsJoined
        {
            get => isJoined;
            set
            {
                if (isJoined == value)
                {
                    return;
                }

                isJoined = value;
                joinButton.interactable = !isJoined;
                recordToggle.gameObject.SetActive(isJoined);
            }
        }

        public int CameraTrackCount
        {
            get => cameraTrackCount;
            set
            {
                if (cameraTrackCount == value)
                {
                    return;
                }

                if (cameraTrackCount > value)
                {
                    cameraDropdown.options.RemoveRange(value, cameraTrackCount - value);
                }
                else
                {
                    var options = Enumerable.Range(cameraTrackCount, value - cameraTrackCount)
                        .Select(i => new TMP_Dropdown.OptionData($"Shot {i + 1}"));
                    cameraDropdown.options.AddRange(options);
                }

                cameraTrackCount = value;
                cameraDropdown.RefreshShownValue();
            }
        }

        public int RecordingCountdown
        {
            get => recordingCountdown;
            set
            {
                if (recordingCountdown == value)
                {
                    return;
                }

                recordingCountdown = value;
                countdownText.text = value.ToString();
            }
        }

        public float RecordDuration
        {
            get => recordDuration;
            set
            {
                recordDuration = value;
            }
        }

        protected override void OnCreate(IBundle bundle)
        {
            var bindingSet = this.CreateBindingSet<ReelSelectWindow, ReelSelectWindowViewModel>();
            bindingSet.Bind(fileDropdown).For(v => v.onValueChanged).To(vm => vm.SelectFileCommand);
            bindingSet.Bind(recordToggle).For(v => v.onValueChanged).To(vm => vm.RecordCommand);
            bindingSet.Bind(micToggle).For(v => v.onValueChanged).To(vm => vm.MicCommand);
            bindingSet.Bind(joinButton).For(v => v.onClick).To(vm => vm.JoinCommand);
            bindingSet.Bind(endButton).For(v => v.onClick).To(vm => vm.EndCommand);
            bindingSet.Bind(setCameraButton).For(v => v.onClick).To(vm => vm.SetCameraCommand);
            bindingSet.Bind(previewDoneButton).For(v => v.onClick).To(vm => vm.NextCommand);
            bindingSet.Bind(backToFeedButton).For(v => v.onClick).To(vm => vm.BackToFeedCommand);
            bindingSet.Bind(backToRecordButton).For(v => v.onClick).To(vm => vm.BackToRecordCommand);
            bindingSet.Bind(faceTrackingToggle).For(v => v.onValueChanged).To(vm => vm.FaceTrackingCommand);
            bindingSet.Bind(bodyTrackingToggle).For(v => v.onValueChanged).To(vm => vm.BodyTrackingCommand);
            bindingSet.Bind(cameraDropdown).For(v => v.onValueChanged).To(vm => vm.CameraTrackCommand);
            bindingSet.Bind(cancelUploadButton).For(v => v.onClick).To(vm => vm.CancelUploadCommand);
            bindingSet.Bind(descriptionInputField).For(v => v.onEndEdit).To(vm => vm.DescriptionText).TwoWay();
            bindingSet.Bind(joinModeDropdown).For(v => v.onValueChanged).To(vm => vm.JoinModeCommand);
            bindingSet.Bind(discardButton).For(v => v.onClick).To(vm => vm.DiscardCommand);
            bindingSet.Bind(draftButton).For(v => v.onClick).To(vm => vm.DraftCommand);
            bindingSet.Bind(uploadButton).For(v => v.onClick).To(vm => vm.UploadCommand);
            bindingSet.Bind(generateMotionButton).For(v => v.onClick).To(vm => vm.MotionCommand);
            bindingSet.Bind(playMusicMotionToggle).For(v => v.onValueChanged).To(vm => vm.AIGCCommand);
            bindingSet.Bind().For(v => v.CameraTrackCount).To(vm => vm.CameraTrackCount);
            bindingSet.Bind().For(v => v.IsJoined).To(vm => vm.IsJoined);
            bindingSet.Bind().For(v => v.Files).To(vm => vm.Files).OneWay();
            bindingSet.Bind().For(v => v.RecordState).To(vm => vm.RecordState);
            bindingSet.Bind().For(v => v.RecordDuration).To(vm => vm.RecordDuration);
            bindingSet.Build();

            UpdateUIDisplay(recordState);
        }

        private void Update()
        {
            var duration = RecordDuration - Time.deltaTime;
            if (duration > 0f)
            {
                recordDuration = duration;
                RecordingCountdown = Mathf.CeilToInt(recordDuration);
            }
        }

        private void OnFilesChanged()
        {
            if (files == null)
            {
                fileDropdown.options.Clear();
                return;
            }

            fileDropdown.options = files.Select(CreateOptionData).ToList();
            fileDropdown.RefreshShownValue();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        for (int i = 0; i < e.NewItems.Count; ++i)
                        {
                            var option = CreateOptionData(e.NewItems[i] as string);
                            fileDropdown.options.Insert(e.NewStartingIndex + i, option);
                        }

                        fileDropdown.RefreshShownValue();
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        fileDropdown.options.RemoveRange(e.OldStartingIndex, e.OldItems.Count);
                        fileDropdown.RefreshShownValue();
                    }

                    break;
                case NotifyCollectionChangedAction.Replace:
                    {
                        var option = CreateOptionData(e.NewItems[0] as string);
                        fileDropdown.options[e.OldStartingIndex] = option;
                        fileDropdown.RefreshShownValue();
                    }

                    break;
                case NotifyCollectionChangedAction.Reset:
                    fileDropdown.ClearOptions();
                    break;
                case NotifyCollectionChangedAction.Move:
                    {
                        var option = CreateOptionData(e.NewItems[0] as string);
                        fileDropdown.options.RemoveAt(e.OldStartingIndex);
                        fileDropdown.options.Insert(e.NewStartingIndex, option);
                        fileDropdown.RefreshShownValue();
                    }

                    break;
            }
        }

        private TMP_Dropdown.OptionData CreateOptionData(string file) => new (PathToOptionName(file));

        private string PathToOptionName(string name)
        {
            return name.Replace(Path.Combine(Application.persistentDataPath, "Recordings\\"), string.Empty);
        }

        private void UpdateUIDisplay(RecordStateTypeEnum state)
        {
            windowUIStateList.Find(x => x.State == state)?.Apply();

            switch (state)
            {
                case RecordStateTypeEnum.Recording:
                    recordToggle.SetIsOnWithoutNotify(false);
                    break;
                default:
                    break;
            }
        }
    }
}
