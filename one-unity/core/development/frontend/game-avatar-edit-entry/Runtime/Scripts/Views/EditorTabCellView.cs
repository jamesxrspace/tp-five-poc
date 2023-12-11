using System;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Views;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TPFive.Game.AvatarEdit.Entry
{
    internal class EditorTabCellView : UIView
    {
        [FormerlySerializedAs("m_Toggle")]
        [SerializeField]
        private Toggle _toggle;
        [FormerlySerializedAs("m_TMPName")]
        [SerializeField]
        private TextMeshProUGUI _tmpName;
        [FormerlySerializedAs("m_TMPSelectedName")]
        [SerializeField]
        private TextMeshProUGUI _tmpSelectedName;

        public ToggleGroup Group
        {
            get => _toggle != null ? _toggle.group : null;
            set => _toggle.group = value;
        }

        public Action<string, bool> OnSelected { get; set; }

        protected override void Start()
        {
            var bindingSet = this.CreateBindingSet<EditorTabCellView, EditorTabCellViewModel>();
            bindingSet.Bind(_tmpName).For(v => v.text).To(vm => vm.Name);
            bindingSet.Bind(_tmpSelectedName).For(v => v.text).To(vm => vm.Name);
            bindingSet.Bind(_toggle).For(v => v.isOn, v => v.onValueChanged).To(vm => vm.IsSelected).TwoWay();
            bindingSet.Bind(_toggle).For(v => v.onValueChanged).To(vm => vm.SelectCmd);
            bindingSet.Build();
        }

        protected override void OnEnable()
        {
            _toggle.onValueChanged.AddListener(OnValueChanged);
            _tmpSelectedName.OnPreRenderText += SelectedNameLoc_OnPreRenderText;
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            _toggle.onValueChanged.RemoveListener(OnValueChanged);
            _tmpSelectedName.OnPreRenderText -= SelectedNameLoc_OnPreRenderText;
            base.OnDisable();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (this.GetDataContext() is EditorTabCellViewModel vm)
            {
                vm.Dispose();
            }
        }

        private void SelectedNameLoc_OnPreRenderText(TMP_TextInfo obj)
        {
            _toggle.enabled = false;
            _toggle.enabled = true;
        }

        private void OnValueChanged(bool isOn)
        {
            if (!isOn && Group != null)
            {
                if (!Group.AnyTogglesOn())
                {
                    _toggle.isOn = true;
                    return;
                }
            }
        }
    }
}