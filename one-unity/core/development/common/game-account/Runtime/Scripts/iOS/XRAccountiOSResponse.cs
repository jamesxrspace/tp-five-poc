namespace TPFive.Game.Account
{
    using System;

    [Serializable]
    public class XRAccountiOSResponse<T>
    {
        public T Message { get; set; }

        public int Code { get; set; }
    }
}
