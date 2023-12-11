using System.Collections.Generic;
using System.Collections.Specialized;
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
    public class RecordWindow : WindowBase
    {
        [SerializeField]
        private Button setCameraButton;

        [SerializeField]
        private Toggle recordToggle;

        [SerializeField]
        private Toggle faceTrackingToggle;

        [SerializeField]
        private Toggle bodyTrackingToggle;

        [SerializeField]
        private Toggle musicToggle;

        [SerializeField]
        private Toggle micToggle;

        [SerializeField]
        private Button motionButton;

        [SerializeField]
        private GameObject musicDataContainer;

        [SerializeField]
        private MusicDataView musicDataTemplate;

        [SerializeField]
        private ToggleGroup musicToggleGroup;

        [SerializeField]
        private ToggleGroup playMusicleGroup;

        [SerializeField]
        private TMP_Dropdown cameraDropdown;

        [Tooltip("To shot editting")]
        [SerializeField]
        private Button nextButton;

        [Tooltip("Back to record")]
        [SerializeField]
        private Button backToRecordButton;

        [Tooltip("Back to feed record")]
        [SerializeField]
        private Button backToFeedButton;

        [Tooltip("Upload to feed")]
        [SerializeField]
        private Button endButton;

        [SerializeField]
        private Toggle playAIGCButton;

        [SerializeField]
        private Button clearAIGCButton;

        [Tooltip("Upload Reel")]
        [SerializeField]
        private Button cancelUploadButton;

        [SerializeField]
        private Image previewImage;

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
        private List<WindowUIState> windowUIStateList;
        private RecordStateTypeEnum recordState;

        private bool showMusicList;
        private int cameraTrackCount;
        private ObservableList<MusicDataViewModel> musicDataViewModelList = new ();
        private List<MusicDataView> musicDataViewList = new ();

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

        public bool ShowMusicList
        {
            get => showMusicList;
            set
            {
                if (showMusicList == value)
                {
                    return;
                }

                showMusicList = value;
                musicDataContainer.SetActive(value);
                musicToggle.SetIsOnWithoutNotify(value);
            }
        }

        public ObservableList<MusicDataViewModel> MusicDataViewModelList
        {
            get => musicDataViewModelList;
            set
            {
                if (musicDataViewModelList == value)
                {
                    return;
                }

                if (musicDataViewModelList != null)
                {
                    musicDataViewModelList.CollectionChanged -= OnCollectionChanged;
                }

                musicDataViewModelList = value;

                OnDataChanged();

                if (musicDataViewModelList != null)
                {
                    musicDataViewModelList.CollectionChanged += OnCollectionChanged;
                }
            }
        }

        protected override void OnCreate(IBundle bundle)
        {
            var bindingSet = this.CreateBindingSet<RecordWindow, RecordWindowViewModel>();

            bindingSet.Bind(setCameraButton).For(v => v.onClick).To(vm => vm.SetCameraCommand);
            bindingSet.Bind(recordToggle).For(v => v.onValueChanged).To(vm => vm.RecordCommand);
            bindingSet.Bind(faceTrackingToggle).For(v => v.onValueChanged).To(vm => vm.FaceTrackingCommand);
            bindingSet.Bind(bodyTrackingToggle).For(v => v.onValueChanged).To(vm => vm.BodyTrackingCommand);
            bindingSet.Bind(musicToggle).For(v => v.onValueChanged).To(vm => vm.MusicCommand);
            bindingSet.Bind(micToggle).For(v => v.onValueChanged).To(vm => vm.MicCommand);
            bindingSet.Bind(cameraDropdown).For(v => v.onValueChanged).To(vm => vm.CameraTrackCommand);
            bindingSet.Bind(nextButton).For(v => v.onClick).To(vm => vm.NextCommand);
            bindingSet.Bind(backToRecordButton).For(v => v.onClick).To(vm => vm.BackToRecordCommand);
            bindingSet.Bind(backToFeedButton).For(v => v.onClick).To(vm => vm.BackToFeedCommand);
            bindingSet.Bind(endButton).For(v => v.onClick).To(vm => vm.EndCommand);
            bindingSet.Bind().For(v => v.RecordState).To(vm => vm.RecordState);
            bindingSet.Bind(playAIGCButton).For(v => v.onValueChanged).To(vm => vm.AIGCCommand);
            bindingSet.Bind(clearAIGCButton).For(v => v.onClick).To(vm => vm.ClearAIGCCommand);
            bindingSet.Bind(motionButton).For(v => v.onClick).To(vm => vm.MotionCommand);
            bindingSet.Bind(cancelUploadButton).For(v => v.onClick).To(vm => vm.CancelUploadCommand);
            bindingSet.Bind(descriptionInputField).For(v => v.onEndEdit).To(vm => vm.DescriptionText).TwoWay();
            bindingSet.Bind(joinModeDropdown).For(v => v.onValueChanged).To(vm => vm.JoinModeCommand);
            bindingSet.Bind(discardButton).For(v => v.onClick).To(vm => vm.DiscardCommand);
            bindingSet.Bind(draftButton).For(v => v.onClick).To(vm => vm.DraftCommand);
            bindingSet.Bind(uploadButton).For(v => v.onClick).To(vm => vm.UploadCommand);
            bindingSet.Bind().For(v => v.CameraTrackCount).To(vm => vm.CameraTrackCount);
            bindingSet.Bind().For(v => v.MusicDataViewModelList).To(vm => vm.MusicDataViewModelList);
            bindingSet.Bind().For(v => v.ShowMusicList).To(vm => vm.ShowMusicList);
            bindingSet.Build();
        }

        protected void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            switch (eventArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in eventArgs.NewItems)
                    {
                        this.AddItem(eventArgs.NewStartingIndex, item);
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in eventArgs.OldItems)
                    {
                        this.RemoveItem(eventArgs.OldStartingIndex, item);
                    }

                    break;
                case NotifyCollectionChangedAction.Replace:
                    for (int i = 0; i < eventArgs.NewItems.Count; i++)
                    {
                        this.ReplaceItem(eventArgs.OldStartingIndex + i, eventArgs.OldItems[i], eventArgs.NewItems[i]);
                    }

                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.ResetItem();
                    break;
                case NotifyCollectionChangedAction.Move:
                    this.MoveItem(eventArgs.OldStartingIndex, eventArgs.NewStartingIndex, eventArgs.NewItems[0]);
                    break;
            }
        }

        private void OnDataChanged()
        {
            if (MusicDataViewModelList == null)
            {
                return;
            }

            for (int i = 0; i < MusicDataViewModelList.Count; i++)
            {
                AddItem(i, MusicDataViewModelList[i]);
            }
        }

        private void UpdateUIDisplay(RecordStateTypeEnum state)
        {
            windowUIStateList.Find(x => x.State == state)?.Apply();
        }

        private void AddItem(int index, object item)
        {
            if (item is MusicDataViewModel musicDataViewModel)
            {
                var musicDataView = Instantiate(musicDataTemplate, musicDataContainer.transform);
                musicDataView.SetDataContext(musicDataViewModel);
                musicDataViewList.Add(musicDataView);
                musicDataView.TryGetComponent(out Toggle toggle);
                toggle.group = musicToggleGroup;
                musicDataView.PlayToggle.group = playMusicleGroup;
            }
        }

        private void RemoveItem(int index, object item)
        {
            if (item is MusicDataViewModel musicDataViewModel)
            {
                musicDataViewModel.Dispose();
            }

            Destroy(musicDataViewList[index].gameObject);
            musicDataViewList.RemoveAt(index);
        }

        private void ReplaceItem(int index, object oldItem, object item)
        {
            if (oldItem is MusicDataViewModel oldMusicDataViewModel)
            {
                oldMusicDataViewModel.Dispose();
            }

            musicDataViewList[index].SetDataContext(item);
        }

        private void ResetItem()
        {
            foreach (var musicDataViewModel in musicDataViewModelList)
            {
                musicDataViewModel.Dispose();
            }

            musicDataViewList.ForEach(view => Destroy(view.gameObject));

            musicDataViewList.Clear();
        }

        private void MoveItem(int oldIndex, int index, object item)
        {
            var oldView = musicDataViewList[oldIndex];
            musicDataViewList.RemoveAt(oldIndex);

            oldView.transform.SetSiblingIndex(index);
            musicDataViewList.Insert(index, oldView);
        }
    }
}