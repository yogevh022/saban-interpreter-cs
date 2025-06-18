namespace IsakovInterpreter.Lexer
{
    public readonly struct Token
    {
        public TokenType Type { get; }
        public object Value { get; }

        public Token(TokenType type, object value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Type}({Value})";
        }
    }
}