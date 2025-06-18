namespace IsakovInterpreter.Lexer
{
    public enum TokenType
    {
        // types
        Identifier,
        Int,
        Double,
        String,
        Boolean,
        
        // comparison
        Equals,
        LessThan,
        GreaterThan,
        LessThanOrEqual,
        GreaterThanOrEqual,
        NotEquals,
        
        // punctuation
        Colon,
        SemiColon,
        Comma,
        Dot,
        
        // special
        At,
        Blank,
        EndOfFile,
        
        // brackets and parentheses
        LeftParenthesis,
        RightParenthesis,
        LeftBracket,
        RightBracket,
        LeftCurly,
        RightCurly,
        
        // arithmetic
        Plus,
        Minus,
        Star,
        Slash,
        Modulo,
        Exponent,
        
        // unary
        Increment,
        Decrement,
        
        // assignment
        Assign,
        PlusAssign,
        MinusAssign,
        StarAssign,
        SlashAssign,
        ModuloAssign,
        ExponentAssign,
        
        // reserved keywords
        If,
        Else,
        While,
        Function,
        Return,
        Break,
        Continue,
        Null,
    }
}