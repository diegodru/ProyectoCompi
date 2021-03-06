using System;

namespace Core.Expressions
{
  public class Constant : TypedExpression
  {
    public Constant(Token token, Type type)
      :base(token, type)
    {
    }

    public override dynamic Evaluate()
    {
      return Token.TokenType switch
      {
          TokenType.BoolConstant => Convert.ToBoolean(Token.Lexeme),
          TokenType.IntConstant => Convert.ToInt32(Token.Lexeme),
          TokenType.FloatConstant => float.Parse(Token.Lexeme),
          TokenType.StringConstant => Token.Lexeme,
          _ => throw new NotImplementedException()
      };
    }

    public override string Generate()
    {
      return Token.Lexeme;
    }

    public override Type GetExpressionType()
    {
      return type;
    }
      
  }
}
