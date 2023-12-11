using Animancer;
using UniRx;
using UnityEngine;

namespace TPFive.Game.Avatar.Tracking
{
    public sealed class BodyTrackingMotion : System.IDisposable
    {
        private readonly AnimancerLayer layer;
        private readonly System.Func<HumanPose> getHumanPose;
        private readonly BodyTrackingState bodyTrackingState;
        private readonly float fadeDuration;
        private readonly Animator avatarAnimator;
        private readonly AnimationIKExecutor animationIKExecutor;

        private HumanPose humanPose;
        private bool isEnable;
        private bool shouldUpdatePositionAndRotation;
        private bool disposed;
        private System.IDisposable updateSubscription;

        public BodyTrackingMotion(
            AnimancerLayer layer,
            System.Func<HumanPose> getHumanPose,
            Animator avatarAnimator,
            AnimationIKExecutor animationIKExecutor,
            float fadeDuration = 0.25f)
        {
            this.layer = layer ?? throw new System.ArgumentNullException(nameof(layer));
            this.getHumanPose = getHumanPose ?? throw new System.ArgumentNullException(nameof(getHumanPose));
            this.avatarAnimator = avatarAnimator;
            this.animationIKExecutor = animationIKExecutor;
            this.fadeDuration = fadeDuration;
            this.bodyTrackingState = new BodyTrackingState();
            this.layer.GetOrCreateState(bodyTrackingState);
            this.animationIKExecutor.OnAnimatorIKUpdate += OnAnimatorAnimatorIK;
        }

        ~BodyTrackingMotion()
        {
            Dispose(false);
        }

        public bool IsEnable
        {
            get => isEnable;
            set
            {
                if (value == isEnable)
                {
                    return;
                }

                isEnable = value;
                OnStatusChanged();
            }
        }

        public void SetMask(AvatarMask mask)
        {
            layer.SetMask(mask);
        }

        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        private void OnStatusChanged()
        {
            shouldUpdatePositionAndRotation = IsEnable;

            if (IsEnable)
            {
                layer.Play(bodyTrackingState, fadeDuration);
                updateSubscription?.Dispose();
                updateSubscription = Observable.EveryUpdate().Subscribe(Update);

                return;
            }

            layer.StartFade(0f, fadeDuration);
            Observable.Timer(System.TimeSpan.FromSeconds(fadeDuration))
                .Subscribe(CancelUpdateIfNeed);
        }

        private void Update(long count)
        {
            humanPose = getHumanPose();
            bodyTrackingState.UpdateHumanPose(ref humanPose);
        }

        private void OnAnimatorAnimatorIK(int layerIndex)
        {
            if (!shouldUpdatePositionAndRotation)
            {
                return;
            }

            avatarAnimator.bodyPosition = GetBodyPosition(humanPose, avatarAnimator);
            avatarAnimator.bodyRotation = GetBodyRotation(humanPose, avatarAnimator);
        }

        private Vector3 GetBodyPosition(HumanPose sourcePose, Animator animator)
        {
            var bodyOffset = sourcePose.bodyPosition;
            var rootPosition = animator.transform.parent ? animator.transform.parent.position : Vector3.zero;
            return rootPosition + bodyOffset;
        }

        private Quaternion GetBodyRotation(HumanPose sourcePose, Animator animator)
        {
            var bodyRotation = sourcePose.bodyRotation;
            var rootRotation = animator.transform.parent ? animator.transform.parent.rotation : Quaternion.identity;
            return rootRotation * bodyRotation;
        }

        private void CancelUpdateIfNeed(long count)
        {
            if (isEnable || updateSubscription == null)
            {
                return;
            }

            updateSubscription.Dispose();
            updateSubscription = null;
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                updateSubscription?.Dispose();
                animationIKExecutor.OnAnimatorIKUpdate -= OnAnimatorAnimatorIK;
            }

            disposed = true;
        }
    }
}