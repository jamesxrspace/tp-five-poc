using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TPFive.SCG.ServiceEco.CodeGen.ServiceProviderManagement
{
    using TPFive.SCG.ServiceEco.Abstractions;
    using TPFive.SCG.Utility;

    public class SyntaxReceiver : ISyntaxReceiver
    {
        private static readonly string AttributeShort =
            nameof(ServiceProviderManagementAttribute).TrimEnd("Attribute");

        private readonly List<ClassDeclarationSyntax> _candidates = new();

        public IReadOnlyList<ClassDeclarationSyntax> Candidates => _candidates;

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax classSyntax
                && classSyntax.HaveAttribute(AttributeShort))
            {
                _candidates.Add(classSyntax);
            }
        }
    }
}
