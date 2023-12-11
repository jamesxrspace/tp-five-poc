using System.Collections.Generic;
using UnityEngine;

namespace TPFive.Game.Messages
{
    public struct BootstrapJustStarted
    {
        public string Category;
    }

    public struct BootstrapSetupDone
    {
        public string Category;
        public bool Success;
    }

    public struct SceneLoading
    {
        public string Category;
        public string Title;
        public int CategoryOrder;
        public int SubOrder;

        public override string ToString()
        {
            return $"Category={Category}, Title={Title}, CategoryOrder={CategoryOrder}, SubOrder={SubOrder}";
        }
    }

    public struct SceneLoaded
    {
        public string Category;
        public string Title;
        public int CategoryOrder;
        public int SubOrder;
    }

    public struct SceneUnloading
    {
        public string Category;
        public string Title;
        public int CategoryOrder;
        public int SubOrder;
    }

    public struct SceneUnloaded
    {
        public string Category;
        public string Title;
        public int CategoryOrder;
        public int SubOrder;
    }

    public struct MultiPhaseSetupDone
    {
        public string Phase;
        public string Category;
        public bool Success;
    }

    public struct ApplicationQuit
    {
    }

    public struct ApplicationFoucs
    {
        public bool Focus;
    }

    public struct ApplicationPause
    {
        public bool Pause;
    }

    public struct FirebaseInitialize
    {
        public bool Success;
    }

    public struct BackToHome
    {
    }

    public struct LoadContentLevel
    {
        public string Title;
        public string LevelBundleId;
    }

    public struct UnloadContentLevel
    {
        public string LevelBundleId;
    }

    public struct ContentLevelFullyLoaded
    {
    }

    public struct NotifyNetLoaderToUnload
    {
        public System.Action OnCompleteAction;
    }

    public struct AssistMode
    {
        public bool On;
        public GameObject AssistModeGameObject;
    }

    // Hud
    public class HudMessage
    {
        public List<int> IntParams { get; set; }

        public List<float> FloatParams { get; set; }

        public List<string> StringParams { get; set; }

        public List<GameObject> GameObjectParams { get; set; }
    }
}
