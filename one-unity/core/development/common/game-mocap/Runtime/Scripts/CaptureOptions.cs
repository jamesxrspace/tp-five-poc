using System;

namespace TPFive.Game.Mocap
{
    public struct CaptureOptions : IEquatable<CaptureOptions>
    {
        public static CaptureOptions None = default;

        private CaptureFlags flags;

        [Flags]
        private enum CaptureFlags
        {
            None = 0,
            UpperBody = 1 << 0,
            FullBody = 1 << 1,
            Neck = 1 << 2,
            Face = 1 << 3,
        }

        public readonly bool IsEnableBody => flags.HasFlag(CaptureFlags.FullBody) | flags.HasFlag(CaptureFlags.UpperBody);

        public readonly bool IsEnableFullBody => flags.HasFlag(CaptureFlags.FullBody);

        public readonly bool IsEnableUpperBody => flags.HasFlag(CaptureFlags.UpperBody);

        public readonly bool IsEnableFace => flags.HasFlag(CaptureFlags.Face);

        public static bool operator ==(CaptureOptions left, CaptureOptions right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CaptureOptions left, CaptureOptions right)
        {
            return !(left == right);
        }

        public void EnableFullBody()
        {
            BodyIsDisabledOrThrowException();

            flags |= CaptureFlags.FullBody;
        }

        public void EnableUpperBody()
        {
            BodyIsDisabledOrThrowException();

            flags |= CaptureFlags.UpperBody;
        }

        public void EnableFace()
        {
            flags |= CaptureFlags.Neck | CaptureFlags.Face;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is CaptureOptions options && Equals(options);
        }

        public readonly bool Equals(CaptureOptions other)
        {
            return flags == other.flags;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(flags);
        }

        private void Enable(CaptureFlags flags)
        {
            this.flags |= flags;
        }

        private void Disable(CaptureFlags flags)
        {
            this.flags &= ~flags;
        }

        private readonly void BodyIsDisabledOrThrowException()
        {
            if (flags.HasFlag(CaptureFlags.FullBody | CaptureFlags.UpperBody))
            {
                throw new InvalidOperationException($"{nameof(EnableFullBody)} or {nameof(EnableUpperBody)} can be set only once");
            }
        }
    }
}
