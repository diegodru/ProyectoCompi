using System;
using System.Collections.Generic;

namespace Core.Expressions
{
  public class LogicalOperator : TypedBinaryOperator
  {
    private readonly Dictionary<(Type, Type), Type> _typeRules;

    public LogicalOperator(Token token,
        TypedExpression left,
        TypedExpression right) : base(token, left, right, null)
    {
      _typeRules = new Dictionary<(Type, Type), Type>
      {
        { (Type.Bool, Type.Bool), Type.Bool },
      };
    }

    public override dynamic Evaluate()
    {
      return Token.TokenType switch
      {
        TokenType.AND => left.Evaluate() && right.Evaluate(),
        TokenType.OR => left.Evaluate() || right.Evaluate(),
        TokenType.NOT => !left.Evaluate(),
        _ => throw new NotImplementedException(), // <----------- Throw not implmented
      };
    }

    public override string Generate()
    {
      return $"{left.Generate()} {Token.Lexeme} {right.Generate()}";
    }

    public override Type GetExpressionType()
    {
      return Type.Bool;
    }
  }
}
