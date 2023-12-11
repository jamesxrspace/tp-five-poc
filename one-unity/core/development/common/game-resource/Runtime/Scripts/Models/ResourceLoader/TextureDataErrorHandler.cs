using System;

namespace TPFive.Game.Resource
{
    public abstract class TextureDataErrorHandler
    {
        public abstract UnsuccessfulReason Handle(RemoteResponse response);
    }

    public class DefaultTextureDataErrorHandler : TextureDataErrorHandler
    {
        public override UnsuccessfulReason Handle(RemoteResponse response)
        {
            var message = response.ErrorMessage;

            if (string.IsNullOrEmpty(message))
            {
                return UnsuccessfulReason.None;
            }

            if (message.IndexOf("VipsJpeg") != -1)
            {
                return UnsuccessfulReason.FormatNotSupported;
            }

            return UnsuccessfulReason.Unknown;
        }
    }
}