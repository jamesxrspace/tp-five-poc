namespace TPFive.Game.Account
{
    public class AuthResult<T>
    {
        public bool IsSuccess => Error == null && Payload != null;

        public T Payload { get; set; }

        public AuthError Error { get; set; }

        public static AuthResult<T> Success(T payload)
        {
            return new AuthResult<T>()
            {
                Payload = payload,
            };
        }

        public static AuthResult<T> Failure(AuthError error)
        {
            return new AuthResult<T>()
            {
                Error = error,
            };
        }

        public static AuthResult<T> Failure(int code, string message)
        {
            return new AuthResult<T>()
            {
                Error = new AuthError(code, message),
            };
        }
    }
}