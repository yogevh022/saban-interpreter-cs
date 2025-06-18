namespace IsakovInterpreter.Parser.Types;

public class FunctionCallNode(IdentifierNode identifier, BaseTypeNode[] arguments) : BaseTypeNode
{
    public IdentifierNode Identifier = identifier;
    public BaseTypeNode[] Arguments = arguments;
}