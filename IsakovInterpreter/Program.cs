
using IsakovInterpreter.Interpreter.Runtime;
using IsakovInterpreter.Lexer;
using IsakovInterpreter.Parser;

var code = "a = {'hello': 'what is up brosky', 'nothing much this value is an object': {'see': 'lol', 3: 44}}";
var lexer = new Lexer(code);
var parser = new Parser(lexer);
var ast = parser.Parse();

var runtime = new Runtime();
foreach (var node in ast)
{
    var eval = runtime.Execute(node);
    Console.WriteLine(eval);
}
