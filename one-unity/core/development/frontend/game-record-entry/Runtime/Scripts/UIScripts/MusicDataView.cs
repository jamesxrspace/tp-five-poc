using Loxodon.Framework.Binding;
using Loxodon.Framework.Views;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TPFive.Game.Record.Entry
{
    public class MusicDataView : UIView
    {
        [SerializeField]
        private Toggle selectButton;

        [SerializeField]
        private Image thumbnail;

        [SerializeField]
        private TMP_Text songName;

        [SerializeField]
        private TMP_Text singerName;

        [SerializeField]
        private Toggle playToggle;

        public Toggle PlayToggle => playToggle;

        protected override void Start()
        {
            base.Start();

            var bindingSet = this.CreateBindingSet<MusicDataView, MusicDataViewModel>();
            bindingSet.Bind(this.selectButton).For(v => v.onValueChanged).To(vm => vm.SelectCommand).OneWay();
            bindingSet.Bind(this.songName).For(v => v.text).To(vm => vm.SongName).OneWay();
            bindingSet.Bind(this.singerName).For(v => v.text).To(vm => vm.SingerName).OneWay();
            bindingSet.Bind(this.playToggle).For(v => v.onValueChanged).To(vm => vm.PlayCommand).OneWay();
            bindingSet.Build();
        }
    }
}
