using System.Collections.Generic;

namespace IsakovInterpreter.Parser.Types;

public class ListNode : BaseTypeNode
{
    public List<BaseTypeNode> Elements = new();
}