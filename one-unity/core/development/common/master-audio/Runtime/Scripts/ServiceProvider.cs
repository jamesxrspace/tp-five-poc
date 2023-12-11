using UnityEngine;

namespace TPFive.Extended.MasterAudio
{
    using TPFive.Game.Logging;

    using GameAudio = TPFive.Game.Audio;

    public sealed partial class ServiceProvider :
        GameAudio.IServiceProvider
    {
        //
        public void PlaySound(string name)
        {
            // DarkTonic.MasterAudio.MasterAudio.PlaySound("S01");
            // DarkTonic.MasterAudio.MasterAudio.PlaySoundAndForget(name);

            DarkTonic.MasterAudio.MasterAudio.PlaySound3DAtVector3(name, Vector3.zero);

            // var mag = DarkTonic.MasterAudio.MasterAudio.GrabGroup(name);
            // Logger.LogEditorDebug("{Mag}", mag);
        }
    }
}
