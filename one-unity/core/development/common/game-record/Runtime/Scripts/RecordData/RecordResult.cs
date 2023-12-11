namespace TPFive.Game.Record
{
    public class RecordResult
    {
        public string Error { get; set; }

        public bool Ok => string.IsNullOrEmpty(Error);
    }
}
