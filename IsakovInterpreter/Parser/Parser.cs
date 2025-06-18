using IsakovInterpreter.Lexer;
using IsakovInterpreter.Parser.BulitIns;
using IsakovInterpreter.Parser.Types;

namespace IsakovInterpreter.Parser;

public class Parser
{
    private Lexer.Lexer _lexer;
    private Token _currentToken;

    private readonly Dictionary<TokenType, Func<Token, BaseTypeNode>> _typeNodeHandlers;
    private readonly Dictionary<TokenType, Func<IdentifierNode, Token, bool, IdentifierNode?>> _identityHandlers;
    private readonly Dictionary<TokenType, TokenType> _augmentedAssignmentToBaseArithmetic;
    
    public Parser(Lexer.Lexer lexer)
    {
        _lexer = lexer;
        _currentToken = lexer.GetNextToken();
        _typeNodeHandlers = new()
        {
            { TokenType.Int, HandleInteger },
            { TokenType.Double, HandleDouble },
            { TokenType.String, HandleString },
            { TokenType.Identifier, HandleIdentifier },
            { TokenType.Minus, HandleMinus },
            { TokenType.LeftParenthesis, HandleParenthesizedExpression },
            { TokenType.LeftCurly, HandleObject },
            { TokenType.LeftBracket, HandleArray },
            { TokenType.Increment, HandleUnary },
            { TokenType.Decrement, HandleUnary }
        };
        _identityHandlers = new()
        {
            { TokenType.Identifier, HandleIdentityIdentifier },
            { TokenType.Dot, HandleIdentityDot },
            { TokenType.LeftBracket, HandleIdentityLeftBracket },
            { TokenType.LeftParenthesis, HandleIdentityLeftParenthesis }
        };
        _augmentedAssignmentToBaseArithmetic = new()
        {
            { TokenType.PlusAssign, TokenType.Plus },
            { TokenType.MinusAssign, TokenType.Minus },
            { TokenType.StarAssign, TokenType.Star },
            { TokenType.SlashAssign, TokenType.Slash },
            { TokenType.ModuloAssign, TokenType.Modulo },
            { TokenType.ExponentAssign, TokenType.Exponent }
        };
    }
    
    
    private void Eat(TokenType tokenType)
    {
        if (_currentToken.Type == tokenType)
            _currentToken = _lexer.GetNextToken();
        else
            throw new Exception($"Expected token type {tokenType}, but got {_currentToken.Type}");
    }

    private IntNode HandleInteger(Token token)
    {
        Eat(TokenType.Int);
        return new IntNode((int)token.Value);
    }
    
    private DoubleNode HandleDouble(Token token)
    {
        Eat(TokenType.Double);
        return new DoubleNode((double)token.Value);
    }
    
    private StringNode HandleString(Token token)
    {
        Eat(TokenType.String);
        return new StringNode((string)token.Value);
    }

    private NumberNode HandleMinus(Token token)
    {
        Eat(TokenType.Minus);
        NumberNode number = _currentToken switch
        {
            { Type: TokenType.Int, Value: int v } => new IntNode(-v),
            { Type: TokenType.Double, Value: double v } => new DoubleNode(-v),
            _ => throw new Exception($"Expected number after '-', but got {_currentToken.Type}")
        };
        Eat(_currentToken.Type);
        return number;
    }
    
    private BaseTypeNode HandleIdentifier(Token token)
    {
        var identifier = GetIdentity(); // identifier or function call (which results in Identifier)
        token = _currentToken;
        if (TokenCategories.IsUnary(token.Type))
        {
            Eat(token.Type);
            var value = new BinaryOperationNode(token.Type, identifier, null);
            return new AssignmentNode(identifier, value, true);
        }

        return identifier;
    }

    private IdentifierNode GetIdentity()
    {
        var identity = new IdentifierNode();
        var accessDot = true;
        while (_identityHandlers.TryGetValue(_currentToken.Type, out Func<IdentifierNode, Token, bool, IdentifierNode?>? handler))
        {
            var token = _currentToken;
            IdentifierNode? identifierOverride = handler(identity, token, accessDot);
            identity = identifierOverride ?? identity;
            accessDot = token.Type is TokenType.Dot;
        }
        if (accessDot)
            throw new Exception("Unexpected dot at the end of identifier");
        return identity;
    }

    private IdentifierNode? HandleIdentityIdentifier(IdentifierNode identity, Token token, bool accessDot)
    {
        if (!accessDot)
            throw new Exception($"Expected '.' got: {token.Type}");
        Eat(TokenType.Identifier);
        identity.Address.Add(new StringNode((string)token.Value));
        return null;
    }

    private IdentifierNode? HandleIdentityDot(IdentifierNode identity, Token token, bool accessDot)
    {
        if (accessDot)
            throw new Exception($"Unexpected double dot");
        Eat(TokenType.Dot);
        return null;
    }

    private IdentifierNode? HandleIdentityLeftBracket(IdentifierNode identity, Token token, bool accessDot)
    {
        Eat(TokenType.LeftBracket);
        var index = Expression();
        Eat(TokenType.RightBracket);
        identity.Address.Add(index);
        return null;
    }
    
    private IdentifierNode HandleIdentityLeftParenthesis(IdentifierNode identity, Token token, bool accessDot)
    {
        Eat(TokenType.LeftParenthesis);
        var arguments = _currentToken.Type is not TokenType.RightParenthesis
            ? HandleArguments()
            : Array.Empty<BaseTypeNode>();
        Eat(TokenType.RightParenthesis);
        return new IdentifierNode([new FunctionCallNode(identity, arguments)]);
    }

    private BaseTypeNode HandleUnary(Token token)
    {
        Eat(token.Type);
        var expression = Expression();
        if (expression is not IdentifierNode identifierNode)
            throw new Exception($"Cannot apply unary operation to {expression.GetType().Name}");
        var value = new BinaryOperationNode(token.Type, identifierNode, null);
        return new AssignmentNode(identifierNode, value);
    }
    
    private BaseTypeNode HandleParenthesizedExpression(Token token)
    {
        Eat(TokenType.LeftParenthesis);
        var expression = Expression();
        Eat(TokenType.RightParenthesis);
        return expression;
    }

    private ObjectNode HandleObject(Token token)
    {
        var obj = new ObjectNode();
        Eat(TokenType.LeftCurly);
        while (_currentToken.Type is not TokenType.RightCurly)
        {
            var key = Expression();
            if (key is not PrimitiveNode primitiveKey)
                throw new Exception($"Object key must be a primitive type, but got {key.GetType().Name}");
            Eat(TokenType.Colon);
            obj.Properties.Add(new ObjectPropertyNode(primitiveKey, Expression()));
            if (_currentToken.Type is TokenType.Comma)
                Eat(TokenType.Comma);
        }
        Eat(TokenType.RightCurly);
        return obj;
    }

    private ListNode HandleArray(Token token)
    {
        var array = new ListNode();
        Eat(TokenType.LeftBracket);
        while (_currentToken.Type is not TokenType.RightBracket)
        {
            array.Elements.Add(Expression());
            if (_currentToken.Type is TokenType.Comma)
                Eat(TokenType.Comma);
        }
        Eat(TokenType.RightBracket);
        return array;
    }
    
    private BaseTypeNode[] HandleArguments(){
        var arguments = new List<BaseTypeNode>();
        while (_currentToken.Type is TokenType.Comma)
        {
            Eat(TokenType.Comma);
            arguments.Add(Expression());
        }
        return arguments.ToArray();
    }

    private AssignmentNode HandleAssignment(BaseTypeNode node)
    {
        if (node is not IdentifierNode identifierNode)
            throw new Exception($"Expected identifier, but got {node.GetType().Name}");
        var token = _currentToken;
        Eat(token.Type);
        var value = Expression();
        value = token.Type is TokenType.Assign
            ? value
            : new BinaryOperationNode(_augmentedAssignmentToBaseArithmetic[token.Type], identifierNode, value);
        return new AssignmentNode(identifierNode, value);
    }

    private IdentifierNode MacroPrint()
    {
        Eat(TokenType.At);
        var arguments = HandleArguments();
        if (!TokenCategories.IsEndLine(_currentToken.Type))
            throw new Exception($"Expected end of line after $macro print, but got {_currentToken.Type}");
        
        return new IdentifierNode([new FunctionCallNode(BuiltIns.GetIdentifier("print"), arguments)]);
    }
    
    private BaseTypeNode Factor()
    {
        var token = _currentToken;
        if (_typeNodeHandlers.TryGetValue(token.Type, out var handler))
            return handler(token);
        throw new Exception($"Unexpected token type {token.Type}");
    }
    
    private BaseTypeNode Exponent()
    {
        var node = Factor();
        while (_currentToken.Type is TokenType.Exponent)
        {
            var token = _currentToken;
            Eat(TokenType.Exponent);
            node = new BinaryOperationNode(token.Type, node, Factor());
        }

        return node;
    }
    
    private BaseTypeNode Term()
    {
        var node = Exponent();
        while (_currentToken.Type is TokenType.Star or TokenType.Slash)
        {
            var token = _currentToken;
            Eat(token.Type);
            node = new BinaryOperationNode(token.Type, node, Exponent());
        }

        return node;
    }

    private BaseTypeNode Expression()
    {
        var node = Term();
        while (_currentToken.Type is TokenType.Plus or TokenType.Minus)
        {
            var token = _currentToken;
            Eat(token.Type);
            node = new BinaryOperationNode(token.Type, node, Term());
        }

        if (TokenCategories.IsAssignment(_currentToken.Type))
            return HandleAssignment(node);
        return node;
    }

    private BaseTypeNode Statement()
    {
        if (TokenCategories.IsReservedKeyword(_currentToken.Type))
            throw new Exception($"Unexpected reserved keyword {_currentToken.Type} at the start of statement");

        if (_currentToken.Type is TokenType.At)
            return MacroPrint();
        
        return Expression();
    }
    
    public BaseTypeNode[] Parse()
    {
        var ast = new List<BaseTypeNode>();
        while (_currentToken.Type is not TokenType.EndOfFile)
        {
            if (TokenCategories.IsEndLine(_currentToken.Type))
            {
                Eat(_currentToken.Type);
                continue;
            }
            ast.Add(Statement());
        }
        return ast.ToArray();
    }
}