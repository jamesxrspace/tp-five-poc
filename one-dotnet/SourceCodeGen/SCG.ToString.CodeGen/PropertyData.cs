namespace TPFive.SCG.ToString.CodeGen
{
    public record PropertyData(string Key, string Value)
    {
        public PropertyData(string key)
            : this(key, key)
        {
        }
    }
}
