namespace TPFive.Game.Record
{
    public class FileHeaderValidateResult
    {
        public string Error { get; set; }

        public bool Ok => string.IsNullOrEmpty(Error);
    }
}
