namespace IsakovInterpreter.Parser.Types;

public class IntNode(int value) : NumberNode
{
    public readonly int Value = value;
}