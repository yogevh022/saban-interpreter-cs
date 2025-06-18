namespace IsakovInterpreter.Parser.Types;

public class ObjectPropertyNode(PrimitiveNode key, BaseTypeNode value) : BaseTypeNode
{
    public PrimitiveNode Key = key;
    public BaseTypeNode Value = value;
}