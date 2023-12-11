#pragma warning disable SA1401
namespace TPFive.Cross.Editor
{
    public sealed partial class Bridge
    {
        public static BuildWithResolverDelegate BuildWithResolver;
        public static GetBuilderWithResolverDelegate GetBuilderWithResolver;

        public delegate void BuildWithResolverDelegate(object builder, object resolver);

        public delegate (object, object) GetBuilderWithResolverDelegate();
    }
}
#pragma warning restore SA1401