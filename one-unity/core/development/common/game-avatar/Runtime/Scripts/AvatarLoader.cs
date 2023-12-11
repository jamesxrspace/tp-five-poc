using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TPFive.Game.Avatar.Factory;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

namespace TPFive.Game.Avatar
{
    public sealed class AvatarLoader : MonoBehaviour
    {
        [Inject]
        private IAvatarFactory factory;

        [SerializeField]
        private Transform root;

        [SerializeField]
        private UnityEvent onLoaded = new UnityEvent();

        [SerializeField]
        private string avatarFormatJson;

        [SerializeField]
        private bool loadOnStart;

        public Transform AvatarRoot => root;

        public bool IsDone { get; private set; }

        public UnityEvent OnLoaded => onLoaded;

        public async UniTask Load(AvatarFormat avatarFormat, CancellationToken token = default)
        {
            try
            {
                await factory.Setup(root.gameObject, avatarFormat, token);
                IsDone = true;
                onLoaded?.Invoke();
            }
            catch (OperationCanceledException)
            {
            }
        }

        public async UniTask Load(AvatarFormat avatarFormat, OptionBase optionBase, CancellationToken token = default)
        {
            try
            {
                await factory.Setup(root.gameObject, avatarFormat, optionBase, token);
                IsDone = true;
                onLoaded?.Invoke();
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void Awake()
        {
            if (root == null)
            {
                root = this.transform;
            }
        }

        private void Start()
        {
            if (!loadOnStart)
            {
                return;
            }

            var (avatarFormat, error) = AvatarFormat.Deserialize(avatarFormatJson);
            if (error == null)
            {
                Load(avatarFormat, destroyCancellationToken).Forget();
            }
        }
    }
}
