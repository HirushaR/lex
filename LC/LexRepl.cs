using System;
using Lex.CodeAnalysis;
using Lex.CodeAnalysis.Syntax;
using System.Collections.Generic;
using System.Linq;
using Lex.CodeAnalysis.Text;
using Lex.CodeAnalysis.Symbols;

namespace Lex
{
    internal sealed class LexRepl : Repl
    {
        private Compilation _previous;
        private bool _showTree;
        private bool _showProgram;
        private readonly Dictionary<VariableSymble, object> _variables = new Dictionary<VariableSymble, object>();

        protected override void RenderLine(string line)
        {
            var tokens = SyntaxTree.ParseTokens(line);
            foreach(var token in tokens)
            {
                var isKeyWord = token.Kind.ToString().EndsWith("Keyword");
                var isIdentifier = token.Kind == SyntaxKind.IdentifierToken;
                var isNumber = token.Kind == SyntaxKind.NumberToken;       
                var isString = token.Kind == SyntaxKind.StringToken;       

                if(isKeyWord)
                    Console.ForegroundColor = ConsoleColor.Blue;
                else if (isIdentifier)
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                else if (isNumber)
                    Console.ForegroundColor = ConsoleColor.Cyan;
                else if (isString)
                    Console.ForegroundColor = ConsoleColor.Magenta;
                else
                    Console.ForegroundColor = ConsoleColor.DarkGray;

                Console.Write(token.Text);

                Console.ResetColor();
            }
        }
      
        protected override void EvaluateMetaCommand(string input)
        {
            switch (input)
            {
                case "#showTree":
                    _showTree = !_showTree;
                    Console.WriteLine(_showTree ? "Showing parse trees." : "Not showing parse trees.");
                    break;
                case "#showProgram":
                    _showProgram = !_showProgram;
                    Console.WriteLine(_showProgram ? "Showing bound tree." : "Not showing bound tree.");
                    break;
                case "#cls":
                    Console.Clear();
                    break;
                case "#reset":
                    _previous = null;
                    _variables.Clear();
                    break;
                default:
                    base.EvaluateMetaCommand(input);
                    break;
            }
        }

        protected override bool IsCompleteSubmition(string text)
        {
            if (string.IsNullOrEmpty(text))
                return true;
            
            var LastTwoLinesAreBlank = text.Split(Environment.NewLine).Reverse().TakeWhile(s=>string.IsNullOrEmpty(s)).Take(2).Count() == 2;

            if(LastTwoLinesAreBlank)
                return true;

            var syntaxTree = SyntaxTree.Parse(text);

            //if (syntaxTree.Diagnostics.Any())
           if (syntaxTree.Root.Members.Last().GetLastToken().isMissing)
                return false;
            
            return true;
        }

     

        protected override void EvaluateSubmition(string text)
        {
            var syntaxTree = SyntaxTree.Parse(text);

            var compilation = _previous == null
                                ? new Compilation(syntaxTree)
                                : _previous.ContinueWith(syntaxTree);

            if (_showTree)
                syntaxTree.Root.WriteTo(Console.Out);

            if (_showProgram)
                compilation.EmitTree(Console.Out);

            var result = compilation.Evaluate(_variables);

            if (!result.Diagnostics.Any())
            {
                if(result.Value != null)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(result.Value);
                    Console.ResetColor();
                }
               
                _previous = compilation;
            }
            else
            {
               Console.Out.WriteDiagnostics(result.Diagnostics, syntaxTree);
            }
        }
    }
}
