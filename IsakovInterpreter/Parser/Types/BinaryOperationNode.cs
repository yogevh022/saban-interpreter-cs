#nullable enable
using System;
using IsakovInterpreter.Lexer;

namespace IsakovInterpreter.Parser.Types;

public class BinaryOperationNode : BaseTypeNode
{
    public readonly TokenType Operator;
    public readonly BaseTypeNode Left;
    public readonly BaseTypeNode Right;

    public BinaryOperationNode(TokenType op, BaseTypeNode left, BaseTypeNode? right)
    {
        Left = left;
        switch (op)
        {
            case TokenType.Increment:
                Operator = TokenType.Plus;
                Right = new IntNode(1);
                break;
            case TokenType.Decrement:
                Operator = TokenType.Minus;
                Right = new IntNode(1);
                break;
            default:
                if (TokenCategories.IsArithmetic(op))
                {
                    if (right == null)
                        throw new ArgumentNullException(nameof(right), "Right operand cannot be null for non-unary operations.");
                    Operator = op;
                    Right = right!;
                    break;
                }
                throw new ArgumentException($"Invalid unary operator: {op}");
        }
    }
}