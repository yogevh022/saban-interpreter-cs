using System.Collections.Generic;

namespace IsakovInterpreter.Parser.Types;

public class ObjectNode : BaseTypeNode
{
    public List<ObjectPropertyNode> Properties = new();
}