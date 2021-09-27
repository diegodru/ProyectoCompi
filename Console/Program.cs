using Core;
using Lexer;
using System;
using System.IO;
using test;

namespace Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var code = File.ReadAllText("Code.txt");
            var input = new Input(code);
            var scanner = new Scanner(input);
            var parser = new Parser2(scanner);
            var engine = new CompilerEngine(parser);
            engine.Run();
        }
    }
}
