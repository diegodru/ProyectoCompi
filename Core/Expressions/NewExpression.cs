using System;
namespace Core.Expressions
{
  public class NewExpression : TypedExpression
  {
    public NewExpression(Token token, Class clase, ArgumentExpression args)
      : base (token, null)
    {
      Clase = Clase;
      Args = args;
      EnvironmentManager.UpdateVariable(token.Lexeme, clase);
    }
    public Class Clase { get; }
    public ArgumentExpression Args { get; } 

    public override string Generate()
    {
      return $"new {Token.Lexeme}({Args.Generate()})";
    }

    public override Type GetExpressionType()
    {
      return new Type(Token.Lexeme, TokenType.ClassType);
    }

    public override dynamic Evaluate()
            {
                throw new NotImplementedException();
            }

  }
}
