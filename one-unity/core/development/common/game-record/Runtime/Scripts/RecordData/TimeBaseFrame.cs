namespace TPFive.Game.Record
{
    public class TimeBaseFrame
    {
        private readonly float timestamp;
        private readonly object frame;

        public TimeBaseFrame(float timestamp, object frame)
        {
            this.timestamp = timestamp;
            this.frame = frame;
        }

        public float Timestamp => timestamp;

        public object Frame => frame;
    }
}
