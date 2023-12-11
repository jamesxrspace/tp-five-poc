using Doozy.Runtime.Signals;

namespace TPFive.Extended.Doozy
{
    [System.Serializable]
    public class SignalBindingData
    {
        public string streamCategory;
        public string streamName;
    }

    public class SignalBinding
    {
        public SignalStream SignalStream { get; set; }
        public SignalReceiver SignalReceiver { get; set; }
        public SignalBindingData SignalBindingData { get; set; }
    }
}
