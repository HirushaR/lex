﻿using System.Collections.Generic;
using System.Linq;

namespace lex.CodeAnalysis.Syntax
{
     sealed class SyntexTree
    {
        public SyntexTree(IEnumerable<string> diagnostics, BoundExpression root, SyntaxToken endOfFileToken)
        {
            Diagnostics = diagnostics.ToArray();
            Root = root;
            EndOfFileToken = endOfFileToken;
        }

        public IReadOnlyList<string> Diagnostics { get; }
        public BoundExpression Root { get; }
        public SyntaxToken EndOfFileToken { get; }

        public static SyntexTree Parse(string text)
        {
            var parser = new Parser(text);
            return parser.Parse();
        }
    }
}


