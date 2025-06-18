namespace IsakovInterpreter.Parser.Types;

public class StringNode(string value) : PrimitiveNode
{
    public readonly string Value = value;
}