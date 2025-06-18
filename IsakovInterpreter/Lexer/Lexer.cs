using System;
using System.Collections.Generic;
using System.Text;

namespace IsakovInterpreter.Lexer
{
    public class Lexer
    {
        private readonly string _source;
        private int _position;
        private char _currentChar;
        private bool IsCurrentCharNull => _currentChar == '\0';
        
        private readonly Dictionary<char, Func<Token>> _specialTokenizers;

        private readonly Dictionary<char, TokenType> _singleCharTokens = new()
        {
            { '(', TokenType.LeftParenthesis },
            { ')', TokenType.RightParenthesis },
            { '[', TokenType.LeftBracket },
            { ']', TokenType.RightBracket },
            { '{', TokenType.LeftCurly },
            { '}', TokenType.RightCurly },
            { ':', TokenType.Colon },
            { ';', TokenType.SemiColon },
            { ',', TokenType.Comma },
            { '.', TokenType.Dot },
            { '@', TokenType.At }
        };
        private readonly Dictionary<string, TokenType> _reservedKeywords = new()
        {
            { "if", TokenType.If },
            { "else", TokenType.Else },
            { "while", TokenType.While },
            { "function", TokenType.Function },
            { "return", TokenType.Return },
            { "null", TokenType.Null },
            { "break", TokenType.Break },
            { "continue", TokenType.Continue }
        };
        private readonly Dictionary<string, TokenType> _booleanKeywords = new()
        {
            { "true", TokenType.Boolean },
            { "false", TokenType.Boolean }
        };
        
        public Lexer(string source)
        {
            _source = source;
            _position = 0;
            _currentChar = _source.Length > 0 ? _source[0] : '\0';
            
            _specialTokenizers = new()
            {
                { '+', TokenizePlus },
                { '-', TokenizeMinus },
                { '*', TokenizeStar },
                { '/', TokenizeSlash },
                { '%', TokenizePercent },
                { '=', TokenizeEquals }
            };
        }
        
        private char Peek()
        {
            if (_position + 1 < _source.Length)
            {
                return _source[_position + 1];
            }
            return '\0'; // null char
        }

        private void Advance()
        {
            if (++_position < _source.Length)
                _currentChar = _source[_position];
            else
                _currentChar = '\0'; // null char
        }

        private void SkipWhitespace()
        {
            while (char.IsWhiteSpace(_currentChar) && !IsCurrentCharNull)
            {
                Advance();
            }
        }

        private Token Number()
        {
            var result = new StringBuilder();
            bool isDouble = false;
            while (!IsCurrentCharNull && (char.IsDigit(_currentChar) || (!isDouble && _currentChar == '.')))
            {
                if (_currentChar == '.')
                    isDouble = true;
                result.Append(_currentChar);
                Advance();
            }
            var resultString = result.ToString();
            return isDouble
                ? new Token(TokenType.Double, double.Parse(resultString))
                : new Token(TokenType.Int, int.Parse(resultString));
        }

        private Token String(char openingQuote)
        {
            var result = new StringBuilder();
            Advance(); // skip opening quote
            while (!IsCurrentCharNull && _currentChar != openingQuote)
            {
                if (_currentChar == '\\')
                {
                    Advance(); // skip backslash
                    switch (_currentChar)
                    {
                        case 'n': result.Append('\n'); break;
                        case 't': result.Append('\t'); break;
                        case '\\': result.Append('\\'); break;
                        default: result.Append(_currentChar); break;
                    }
                }
                else
                {
                    result.Append(_currentChar);
                }
                Advance();
            }
            if (IsCurrentCharNull)
                throw new Exception("Unterminated string literal");
            Advance(); // skip closing quote
            return new Token(TokenType.String, result.ToString());
        }

        private Token Keyword()
        {
            var result = new StringBuilder();
            while (!IsCurrentCharNull && (char.IsLetterOrDigit(_currentChar) || _currentChar == '_'))
            {
                result.Append(_currentChar);
                Advance();
            }
            var keyword = result.ToString();
            TokenType tokenType;
            if (_reservedKeywords.TryGetValue(keyword, out tokenType))
                return new Token(tokenType, keyword);
            if (_reservedKeywords.TryGetValue(keyword, out tokenType))
                return new Token(tokenType, keyword == "true");
            return new Token(TokenType.Identifier, keyword);
        }

        private Token TokenizePlus()
        {
            Advance();
            if (_currentChar == '+')
            {
                Advance();
                return new Token(TokenType.Increment, "++");
            }
            if (_currentChar == '=')
            {
                Advance();
                return new Token(TokenType.PlusAssign, "+=");
            }
            return new Token(TokenType.Plus, "+");
        }
        
        private Token TokenizeMinus()
        {
            Advance();
            if (_currentChar == '-')
            {
                Advance();
                return new Token(TokenType.Decrement, "--");
            }
            if (_currentChar == '=')
            {
                Advance();
                return new Token(TokenType.MinusAssign, "-=");
            }
            return new Token(TokenType.Minus, "-");
        }
        
        private Token TokenizeStar()
        {
            Advance();
            if (_currentChar == '*')
            {
                Advance();
                if (_currentChar == '=')
                {
                    Advance();
                    return new Token(TokenType.ExponentAssign, "**=");
                }
                return new Token(TokenType.Exponent, "**");
            }

            if (_currentChar == '=')
            {
                Advance();
                return new Token(TokenType.StarAssign, "*=");
            }
            return new Token(TokenType.Star, "*");
        }

        private Token TokenizeSlash()
        {
            Advance();
            if (_currentChar == '=')
            {
                Advance();
                return new Token(TokenType.SlashAssign, "/=");
            }
            return new Token(TokenType.Slash, "/");
        }

        private Token TokenizePercent()
        {
            Advance();
            if (_currentChar == '=')
            {
                Advance();
                return new Token(TokenType.ModuloAssign, "%=");
            }
            return new Token(TokenType.Modulo, "%");
        }
        
        private Token TokenizeEquals()
        {
            Advance();
            if (_currentChar == '=')
            {
                Advance();
                return new Token(TokenType.Equals, "==");
            }
            return new Token(TokenType.Assign, "=");
        }

        public Token GetNextToken()
        {
            while (!IsCurrentCharNull)
            {
                if (char.IsWhiteSpace(_currentChar))
                {
                    SkipWhitespace();
                    continue;
                }

                if (char.IsDigit(_currentChar))
                    return Number();
                
                if (char.IsLetterOrDigit(_currentChar) || _currentChar == '_')
                    return Keyword();
                
                if (_currentChar is '"' or '\'')
                    return String(_currentChar);

                if (_singleCharTokens.TryGetValue(_currentChar, out var tokenType))
                {
                    var token = new Token(tokenType, _currentChar.ToString());
                    Advance();
                    return token;
                }
                if (_specialTokenizers.TryGetValue(_currentChar, out var tokenizer))
                {
                    return tokenizer();
                }
                throw new Exception($"Unexpected character: '{_currentChar}' at position {_position}");
            }
            return new Token(TokenType.EndOfFile, "EOF");
        }
    }
}
