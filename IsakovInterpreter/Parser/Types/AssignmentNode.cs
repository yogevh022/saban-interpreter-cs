namespace IsakovInterpreter.Parser.Types;

public class AssignmentNode(IdentifierNode identifier, BaseTypeNode value, bool returnBeforeAssignment = false) : BaseTypeNode
{
    public IdentifierNode Identifier = identifier;
    public BaseTypeNode Value = value;
    public bool ReturnBeforeAssignment = returnBeforeAssignment;
}