using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace MultiLineString
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MultiLineStringCodeFixProvider)), Shared]
    public class MultiLineStringCodeFixProvider : CodeFixProvider
    {
        private const string title = "Convert to single line literal";

        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(MultiLineStringAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.First();

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedDocument: c => ConvertToSingleLine(context.Document, diagnostic, c), 
                    equivalenceKey: title),
                diagnostic);

            return Task.CompletedTask;
        }

        private async Task<Document> ConvertToSingleLine(Document document, Diagnostic diagnostic, CancellationToken c)
        {
            var root = await document.GetSyntaxRootAsync(c).ConfigureAwait(false);
            var originalExpr = root.FindNode(diagnostic.Location.SourceSpan);
            var token = originalExpr.GetFirstToken();

            // trim all that noise (tabs, spaces, newlines)
            string singleLine;
            using (StringReader reader = new StringReader(token.ValueText))
            using (StringWriter writer = new StringWriter())
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    writer.Write(line.TrimStart() + " ");
                }
                singleLine = writer.ToString();
            }

            var newToken = SyntaxFactory.Literal(singleLine);
            var newRoot = root.ReplaceToken(token, newToken);
            
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
