namespace TPFive.Game.Config
{
    public static class Constants
    {
        public const ServiceProviderKind NullProviderKind = ServiceProviderKind.NullServiceProvider;
        public const ServiceProviderKind RuntimeLocalProviderKind = ServiceProviderKind.Rank1ServiceProvider;
        public const ServiceProviderKind FirebaseProviderKind = ServiceProviderKind.Rank2ServiceProvider;
        public const ServiceProviderKind UnityProviderKind = ServiceProviderKind.Rank3ServiceProvider;

        public const int NullProviderIndex = (int)NullProviderKind;
        public const int RuntimeLocalProviderIndex = (int)RuntimeLocalProviderKind;
        public const int FirebaseProviderIndex = (int)FirebaseProviderKind;
        public const int UnityProviderIndex = (int)UnityProviderKind;
    }
}
