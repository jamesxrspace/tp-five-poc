using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TPFive.SCG.Bridge.CodeGen
{
    using TPFive.SCG.Bridge.Abstractions;
    using TPFive.SCG.Utility;

    public class SyntaxReceiver : ISyntaxReceiver
    {
        private static readonly string AttributeShort =
            nameof(DelegateFromAttribute).TrimEnd("Attribute");

        private readonly List<(ClassDeclarationSyntax, List<MethodDeclarationSyntax>)> _candidates = new();

        public IReadOnlyList<(ClassDeclarationSyntax, List<MethodDeclarationSyntax>)> Candidates => _candidates;

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax)
            {
                var mdsList = classDeclarationSyntax.Members
                    .OfType<MethodDeclarationSyntax>()
                    .Where(x => x.AttributeLists.Any(y => y.Attributes.Any(z => z.Name.ToString().Contains(AttributeShort))))
                    .ToList();

                if (mdsList.Any())
                {
                    _candidates.Add((classDeclarationSyntax, mdsList));
                }
            }
        }
    }
}
