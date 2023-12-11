using System.Collections.Generic;
using UnityEngine;

namespace TPFive.Game.Record.Entry
{
    [CreateAssetMenu(fileName = "Record UI State", menuName = "TPFive/Record/UIStateSettings")]
    public class UIStateSettings : ScriptableObject
    {
        [SerializeField]
        private List<FlutterUIState> flutterUIStateList;

        public List<FlutterUIState> FlutterUIStateList => flutterUIStateList;
    }
}
