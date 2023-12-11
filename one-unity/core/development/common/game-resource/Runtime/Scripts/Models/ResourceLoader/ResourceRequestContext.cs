using System;

namespace TPFive.Game.Resource
{
    /// <summary>
    /// This serves as a control use for Resource Loader.
    /// </summary>
    public abstract class ResourceRequestContext
    {
        private TimeSpan _timeout;
        private int _maxRetry;

        public string Key => Url;

        public string Url { get; set; }

        public TimeSpan Timeout
        {
            get
            {
                if (_timeout == default)
                {
                    _timeout = TimeSpan.FromSeconds(60);
                }

                return _timeout;
            }
            set => _timeout = value;
        }

        public int MaxRetry
        {
            get
            {
                return _maxRetry;
            }
            set => _maxRetry = value;
        }

        public override string ToString()
        {
            return $"url: {Url}";
        }
    }
}