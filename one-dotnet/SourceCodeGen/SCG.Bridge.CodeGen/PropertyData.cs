namespace TPFive.SCG.Bridge.CodeGen
{
    public record PropertyData(string Key, string Value)
    {
        public PropertyData(string Key) : this(Key, Key) { }
    }
}
