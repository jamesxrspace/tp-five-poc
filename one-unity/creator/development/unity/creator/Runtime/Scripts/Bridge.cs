namespace TPFive.Creator
{
    public class Bridge
    {
        public delegate ICreator GetCreatorDelegate();

        public static GetCreatorDelegate GetCreator { get; set; }
    }
}