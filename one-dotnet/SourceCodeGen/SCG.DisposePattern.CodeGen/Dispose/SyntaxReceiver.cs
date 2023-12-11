using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TPFive.SCG.DisposePattern.CodeGen.Dispose
{
    using TPFive.SCG.DisposePattern.Abstractions;
    using TPFive.SCG.Utility;

    public class SyntaxReceiver : ISyntaxReceiver
    {
        private static readonly string AttributeShort =
            nameof(DisposeAttribute).TrimEnd("Attribute");

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
