using System;
using TPFive.Game.Avatar;
using UnityEngine;

namespace TPFive.Game.Record.Entry
{
    public sealed class ReelPlayer : IDisposable
    {
        private bool isDisposed;

        public ReelPlayer(GameObject root)
        {
            Root = root;
            Avatar = root.GetComponentInChildren<IAvatarContextProvider>();
        }

        ~ReelPlayer()
        {
            Dispose(false);
        }

        public GameObject Root { get; }

        public IAvatarContextProvider Avatar { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }

            if (disposing && Root != null)
            {
                UnityEngine.Object.Destroy(Root);
            }

            isDisposed = true;
        }
    }
}
