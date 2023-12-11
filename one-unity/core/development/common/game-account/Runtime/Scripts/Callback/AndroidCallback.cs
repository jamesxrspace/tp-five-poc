namespace TPFive.Game.Account
{
#pragma warning disable SA1300 // The methods in the "ResultListener" Java class, such as onSuccess and onFailure, begins with lowercase letters.
    using UnityEngine;

    public class AndroidCallback<T> : AndroidJavaProxy
    {
        private readonly IAuthCallback<T> listener;

        public AndroidCallback(IAuthCallback<T> listener)
            : base("com.xrspace.xrauth.callback.ResultListener")
        {
            this.listener = listener;
        }

        public void onSuccess(T result)
        {
            listener?.OnSuccess(result);
        }

        public void onFailure(int errorCode, string errorMessage)
        {
            listener?.OnFailure(errorCode, errorMessage);
        }
    }
#pragma warning restore SA1300
}