using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TPFive.SCG.AsyncStartable.CodeGen
{
    using TPFive.SCG.AsyncStartable.Abstractions;
    using TPFive.SCG.Utility;

    /// <summary>
    /// This keeps the candidate for code gen.
    /// </summary>
    public class SyntaxReceiver : ISyntaxReceiver
    {
        private static readonly string AttributeShort =
            nameof(AsyncStartableAttribute).TrimEnd("Attribute");

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
