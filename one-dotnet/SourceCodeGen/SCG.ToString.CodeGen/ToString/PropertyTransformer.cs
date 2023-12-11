using Microsoft.CodeAnalysis;
using TPFive.SCG.Utility;

namespace TPFive.SCG.ToString.CodeGen
{
    public class PropertyTransformer
    {
        private readonly bool _displayCollections;
        private readonly INamedTypeSymbol _enumerableSymbol;

        public PropertyTransformer(Compilation compilation, bool displayCollections)
        {
            _displayCollections = displayCollections;
            _enumerableSymbol = compilation.GetTypeByMetadataName("System.Collections.IEnumerable");
        }

        public PropertyData Transform(IPropertySymbol propertySymbol)
        {
            var propertyName = propertySymbol.Name;
            var value = propertySymbol.Type
                switch
                {
                    { SpecialType: SpecialType.System_String } => propertyName,
                    { IsReferenceType : true } symbol when
                        symbol.Implements(_enumerableSymbol) && _displayCollections =>
                        TransformAsCollection(propertyName), _ => propertyName,
                };

            return new(propertyName, value);
        }

        public string TransformAsCollection(string propertyName) =>
            $@"({propertyName} is null ? null : $""[{{string.Join("","", System.Linq.Enumerable.Select({propertyName}, c => c.ToString()))}}]"")";
    }
}
