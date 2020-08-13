namespace Lex.CodeAnalysis.Syntax
{
    public enum SyntaxKind
    {
        // Tokens
        BadToken,
        EndOfFileToken,
        WhitespaceToken,
        NumberToken,
        PlusToken,
        MinusToken,
        StarToken,
        StarStarToken,
        SlashToken,
        RemainderToken,
        LessThanToken,
        GreaterThanToken,
        LessOrEqualToken,
        GreaterOrEqualToken,
        EaqlesToken,
        BangToken,
        EaqulesEaqlesToken,
        BangEaqlesToken,
        AmpersandAmpersandToken,
        PipePieToken,
        OpenParenthesisToken,
        CloseParenthesisToken,      
        OpenBraceToken,
        CloseBraceToken,
        IdentifierToken,


        //keywords
        ElseKeyword,
        TrueKeyword,
        IfKeyword,
        LetKeyword,
        FalseKeyword,
        VarKeyword,
              
        

        CompilationUnit,
        
        ElseClouse,

        //Statements
        BlockStatement,
        ExpressionStatemnet,
        IfStatement,
        VeriableDeclaration,


        // Expressions
        LiteralExpression,
        NameExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression,
        AssigmentExpression,

    }
}