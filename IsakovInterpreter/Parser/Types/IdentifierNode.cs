using System.Collections.Generic;

namespace IsakovInterpreter.Parser.Types;

public class IdentifierNode : BaseTypeNode
{
    public List<BaseTypeNode> Address = new();
    
    public IdentifierNode() {}
    
    public IdentifierNode(List<BaseTypeNode> address)
    {
        Address = address;
    }
}