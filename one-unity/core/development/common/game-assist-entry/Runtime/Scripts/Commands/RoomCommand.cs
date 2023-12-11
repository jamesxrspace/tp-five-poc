#if HAS_HIGH_PRIORITY_ENTRY
using System;
using System.Linq;
using MessagePipe;
using QFSW.QC;
using TPFive.Game.App.Entry;
using TPFive.Game.SceneFlow;
using TPFive.Home.Entry;
using TPFive.Room;
using UnityEngine;
using UnityEngine.Assertions;
using VContainer;

namespace TPFive.Game.Assist.Entry
{
    public class RoomCommand : MonoBehaviour
    {
        [SerializeField]
        private string homeEntryTitle = "HomeEntry";
        [SerializeField]
        private string roomEntryTitle = "RoomEntry";
        [SerializeField]
        private Assist.Entry.LifetimeScope assistEntrylifetimeScope;
    }
}
#endif
