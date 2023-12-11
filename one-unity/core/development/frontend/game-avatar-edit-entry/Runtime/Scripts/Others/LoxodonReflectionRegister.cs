using TPFive.Extended.LoxodonFramework.Binding;
using UnityEngine;

namespace TPFive.Game.AvatarEdit.Entry
{
    public static class LoxodonReflectionRegister
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            PreserveProxy.Register<PresetAvatarScrollViewModel, int>("CurrentIndex", t => t.CurrentIndex, (t, v) => t.CurrentIndex = v);
        }
    }
}