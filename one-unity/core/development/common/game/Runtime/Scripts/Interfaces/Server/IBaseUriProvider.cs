using System;

namespace TPFive.Game.Server
{
    /// <summary>
    /// Provides base uri.
    /// <br/>
    /// e.g. server base uri, api base uri, etc.
    /// </summary>
    public interface IBaseUriProvider
    {
        public Uri BaseUri { get; }
    }
}