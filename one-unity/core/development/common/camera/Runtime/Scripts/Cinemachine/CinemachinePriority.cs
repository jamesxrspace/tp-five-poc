using System;
using UnityEngine;

namespace TPFive.Extended.Camera
{
    /// <summary>
    /// Indicates the priority of camera.<br/><br/>
    /// A higher value indicates a higher priority.<br/><br/>
    /// Cinemachine Brain chooses the next live Virtual Camera from all Virtual Cameras
    /// that are activated and have the same or higher priority as the current live Virtual Camera.
    /// </summary>
    [Serializable]
    public struct CinemachinePriority : IEquatable<CinemachinePriority>
    {
        [SerializeField]
        private int value;

        public CinemachinePriority(int value)
        {
            this.value = value;
        }

        public static CinemachinePriority Zero => new CinemachinePriority(0);

        public int Value { readonly get => value; set => this.value = value; }

        public static implicit operator CinemachinePriority(int value) => new (value);

        public static implicit operator int(CinemachinePriority priority) => priority.value;

        public static bool operator ==(CinemachinePriority left, CinemachinePriority right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CinemachinePriority left, CinemachinePriority right)
        {
            return !(left == right);
        }

        public readonly override bool Equals(object obj)
        {
            return obj is CinemachinePriority priority && Equals(priority);
        }

        public readonly bool Equals(CinemachinePriority other)
        {
            return value == other.value;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(value);
        }
    }
}
