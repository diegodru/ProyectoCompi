using System;
namespace Core.Expressions
{
  public class NewExpression : TypedExpression
  {
    public NewExpression(Token token, string lexeme, ArgumentExpression args)
      : base (token, null)
    {
      Lexeme = lexeme;
      Args = args;
    }
    public string Lexeme { get; }
    public ArgumentExpression Args { get; } 

    public override string Generate()
    {
      return $"new {Lexeme}({Args.Generate()})";
    }

    public override Type GetExpressionType()
    {
      return new Type(Lexeme, TokenType.ClassType);
    }

    public override dynamic Evaluate()
            {
                throw new NotImplementedException();
            }

  }
}
