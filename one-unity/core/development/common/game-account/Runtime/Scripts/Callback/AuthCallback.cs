namespace TPFive.Game.Account
{
    using System;

    public interface IAuthCallback<T>
    {
        void OnSuccess(T message);

        void OnFailure(int errorCode, string errorMessage);

        Action<int, string> GetFailureAction();
    }

    public class AuthCallback<T> : IAuthCallback<T>
    {
        private readonly Action<T> success;
        private readonly Action<int, string> failure;

        public AuthCallback(Action<T> success, Action<int, string> failure)
        {
            this.success = success;
            this.failure = failure;
        }

        public void OnSuccess(T extra)
        {
            success?.Invoke(extra);
        }

        public void OnFailure(int errorCode, string errorMessage)
        {
            failure?.Invoke(errorCode, errorMessage);
        }

        public Action<int, string> GetFailureAction()
        {
            return failure;
        }
    }
}