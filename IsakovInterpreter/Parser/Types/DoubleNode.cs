namespace IsakovInterpreter.Parser.Types;

public class DoubleNode(double value) : NumberNode
{
    public readonly double Value = value;
}