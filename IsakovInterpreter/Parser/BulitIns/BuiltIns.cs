using System.Linq;
using IsakovInterpreter.Parser.Types;

namespace IsakovInterpreter.Parser.BulitIns;

public class BuiltIns
{
    private static readonly string _root = "7a8d77e7-300a-4580-a4b2-84c20ee3d294";
    private static readonly string[] _registeredBuiltIns = new[]
    {
        "print"
    };
    
    public static IdentifierNode GetIdentifier(string name)
    {
        if (!_registeredBuiltIns.Contains(name))
            throw new System.Exception($"Built-in function '{name}' does not exist.");
        return new IdentifierNode([new StringNode(_root), new StringNode(name)]);
    }
}