using System.Linq;
using IsakovInterpreter.Lexer;
namespace IsakovInterpreter.Lexer
{
    public static class TokenCategories
    {
        public static readonly TokenType[] UnaryOperators =
        {
            TokenType.Increment,
            TokenType.Decrement
        };
        
        public static readonly TokenType[] EndLineTokens =
        {
            TokenType.SemiColon,
            TokenType.EndOfFile
        };
        
        public static readonly TokenType[] AssignmentOperators =
        {
            TokenType.Assign,
            TokenType.PlusAssign,
            TokenType.MinusAssign,
            TokenType.StarAssign,
            TokenType.SlashAssign,
            TokenType.ModuloAssign,
            TokenType.ExponentAssign
        };
        
        public static readonly TokenType[] ArithmeticOperators =
        {
            TokenType.Plus,
            TokenType.Minus,
            TokenType.Star,
            TokenType.Slash,
            TokenType.Modulo,
            TokenType.Exponent
        };
        
        public static readonly TokenType[] ComparisonOperators =
        {
            TokenType.Equals,
            TokenType.LessThan,
            TokenType.GreaterThan,
            TokenType.LessThanOrEqual,
            TokenType.GreaterThanOrEqual,
            TokenType.NotEquals
        };
        
        public static readonly TokenType[] ReservedKeywords =
        {
            TokenType.If,
            TokenType.Else,
            TokenType.While,
            TokenType.Function,
            TokenType.Return,
            TokenType.Null,
            TokenType.Break,
            TokenType.Continue
        };
        
        public static readonly TokenType[] PunctuationTokens =
        {
            TokenType.Colon,
            TokenType.SemiColon,
            TokenType.Comma,
            TokenType.Dot
        };
        
        public static readonly TokenType[] Types = 
        {
            TokenType.Identifier,
            TokenType.Int,
            TokenType.Double,
            TokenType.String,
            TokenType.Boolean
        };

        public static bool IsArithmetic(TokenType tokenType) => ArithmeticOperators.Contains(tokenType);
        public static bool IsComparison(TokenType tokenType) => ComparisonOperators.Contains(tokenType);
        public static bool IsAssignment(TokenType tokenType) => AssignmentOperators.Contains(tokenType);
        public static bool IsUnary(TokenType tokenType) => UnaryOperators.Contains(tokenType);
        public static bool IsEndLine(TokenType tokenType) => EndLineTokens.Contains(tokenType);
        public static bool IsReservedKeyword(TokenType tokenType) => ReservedKeywords.Contains(tokenType);
        public static bool IsPunctuation(TokenType tokenType) => PunctuationTokens.Contains(tokenType);
        public static bool IsType(TokenType tokenType) => Types.Contains(tokenType);
    }
}