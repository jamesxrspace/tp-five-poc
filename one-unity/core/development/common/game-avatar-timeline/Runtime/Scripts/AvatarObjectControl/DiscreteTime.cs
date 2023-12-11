using System;

namespace TPFive.Game.Avatar.Timeline.AvatarObjectControl
{
    /// <summary>
    /// It's a copy of Unity Timeline DiscreteTime.
    /// This struct is the same as Unity Timeline DiscreteTime.
    /// The unit of time for this is in seconds.
    /// </summary>
    public struct DiscreteTime : IComparable
    {
        public static readonly DiscreteTime MaxTime = new DiscreteTime(long.MaxValue);

        private const double Tick = 1e-12;

        private readonly long _discreteTime;

        public DiscreteTime(DiscreteTime time)
        {
            _discreteTime = time._discreteTime;
        }

        public DiscreteTime(long time)
        {
            _discreteTime = time;
        }

        public DiscreteTime(double time)
        {
            _discreteTime = DoubleToDiscreteTime(time);
        }

        public DiscreteTime(float time)
        {
            _discreteTime = FloatToDiscreteTime(time);
        }

        public DiscreteTime(int time)
        {
            _discreteTime = IntToDiscreteTime(time);
        }

        public DiscreteTime(int frame, double fps)
        {
            _discreteTime = DoubleToDiscreteTime(frame * fps);
        }

        public static double TickValue
        {
            get { return Tick; }
        }

        public static explicit operator double(DiscreteTime b)
        {
            return ToDouble(b._discreteTime);
        }

        public static explicit operator float(DiscreteTime b)
        {
            return ToFloat(b._discreteTime);
        }

        public static explicit operator long(DiscreteTime b)
        {
            return b._discreteTime;
        }

        public static explicit operator DiscreteTime(double time)
        {
            return new DiscreteTime(time);
        }

        public static explicit operator DiscreteTime(float time)
        {
            return new DiscreteTime(time);
        }

        public static implicit operator DiscreteTime(int time)
        {
            return new DiscreteTime(time);
        }

        public static explicit operator DiscreteTime(long time)
        {
            return new DiscreteTime(time);
        }

        public static bool operator ==(DiscreteTime lhs, DiscreteTime rhs)
        {
            return lhs._discreteTime == rhs._discreteTime;
        }

        public static bool operator !=(DiscreteTime lhs, DiscreteTime rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator >(DiscreteTime lhs, DiscreteTime rhs)
        {
            return lhs._discreteTime > rhs._discreteTime;
        }

        public static bool operator <(DiscreteTime lhs, DiscreteTime rhs)
        {
            return lhs._discreteTime < rhs._discreteTime;
        }

        public static bool operator <=(DiscreteTime lhs, DiscreteTime rhs)
        {
            return lhs._discreteTime <= rhs._discreteTime;
        }

        public static bool operator >=(DiscreteTime lhs, DiscreteTime rhs)
        {
            return lhs._discreteTime >= rhs._discreteTime;
        }

        public static DiscreteTime operator +(DiscreteTime lhs, DiscreteTime rhs)
        {
            return new DiscreteTime(lhs._discreteTime + rhs._discreteTime);
        }

        public static DiscreteTime operator -(DiscreteTime lhs, DiscreteTime rhs)
        {
            return new DiscreteTime(lhs._discreteTime - rhs._discreteTime);
        }

        public static DiscreteTime FromTicks(long ticks)
        {
            return new DiscreteTime(ticks);
        }

        public static DiscreteTime Min(DiscreteTime lhs, DiscreteTime rhs)
        {
            return new DiscreteTime(Math.Min(lhs._discreteTime, rhs._discreteTime));
        }

        public static DiscreteTime Max(DiscreteTime lhs, DiscreteTime rhs)
        {
            return new DiscreteTime(Math.Max(lhs._discreteTime, rhs._discreteTime));
        }

        public static double SnapToNearestTick(double time)
        {
            long discreteTime = DoubleToDiscreteTime(time);
            return ToDouble(discreteTime);
        }

        public static float SnapToNearestTick(float time)
        {
            long discreteTime = FloatToDiscreteTime(time);
            return ToFloat(discreteTime);
        }

        public static long GetNearestTick(double time)
        {
            return DoubleToDiscreteTime(time);
        }

        public override readonly string ToString()
        {
            return _discreteTime.ToString();
        }

        public override readonly int GetHashCode()
        {
            return _discreteTime.GetHashCode();
        }

        public readonly DiscreteTime OneTickBefore()
        {
            return new DiscreteTime(_discreteTime - 1);
        }

        public readonly DiscreteTime OneTickAfter()
        {
            return new DiscreteTime(_discreteTime + 1);
        }

        public readonly long GetTick()
        {
            return _discreteTime;
        }

        public readonly int CompareTo(object obj)
        {
            if (obj is DiscreteTime time)
            {
                return _discreteTime.CompareTo(time._discreteTime);
            }

            return 1;
        }

        public readonly bool Equals(DiscreteTime other)
        {
            return _discreteTime == other._discreteTime;
        }

        public override bool Equals(object obj)
        {
            if (obj is DiscreteTime time)
            {
                return Equals(time);
            }

            return false;
        }

        private static long DoubleToDiscreteTime(double time)
        {
            double number = (time / Tick) + 0.5;
            if (number < long.MaxValue && number > long.MinValue)
            {
                return (long)number;
            }

            throw new ArgumentOutOfRangeException("Time is over the discrete range.");
        }

        private static long FloatToDiscreteTime(float time)
        {
            float number = (time / (float)Tick) + 0.5f;
            if (number < long.MaxValue && number > long.MinValue)
            {
                return (long)number;
            }

            throw new ArgumentOutOfRangeException("Time is over the discrete range.");
        }

        private static long IntToDiscreteTime(int time)
        {
            return DoubleToDiscreteTime(time);
        }

        private static double ToDouble(long time)
        {
            return time * Tick;
        }

        private static float ToFloat(long time)
        {
            return (float)ToDouble(time);
        }
    }
}