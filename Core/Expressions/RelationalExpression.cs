using System;
using System.Collections.Generic;

namespace Core.Expressions
{
  public class RelationalExpression : TypedBinaryOperator
  {
    private readonly Dictionary<(Type, Type), Type> _typeRules;

    public RelationalExpression(Token token,
       TypedExpression left,
       TypedExpression right) : base(token, left, right, null)
    {
      _typeRules = new Dictionary<(Type, Type), Type>
      {
        { (Type.Float, Type.Float), Type.Bool },
        { (Type.Int, Type.Int), Type.Bool },
        { (Type.String, Type.String), Type.Bool },
        { (Type.Int, Type.Float), Type.Bool },
        { (Type.Float, Type.Int), Type.Bool },
      };
    }
    public override dynamic Evaluate()
    {
      return Token.TokenType switch
      {
        TokenType.GreaterThan => left.Evaluate() > right.Evaluate(),
        TokenType.GreaterThanOrEqual => left.Evaluate() >= right.Evaluate(),
        TokenType.LessThan => left.Evaluate() < right.Evaluate(),
        TokenType.LessThanOrEqual => left.Evaluate() <= right.Evaluate(),
        TokenType.Equal => left.Evaluate() == right.Evaluate(),
        TokenType.NotEqual => left.Evaluate() != right.Evaluate(),
        _ => throw new NotImplementedException() // <----------- Throw not implmented
      };
    }

    public override string Generate()
    {
      return $"{left.Generate()} {Token.Lexeme} {right.Generate()}";
    }

    public override Type GetExpressionType()
    {
      if(_typeRules.TryGetValue((left.GetExpressionType(), right.GetExpressionType()),
            out var resultType))
          {
            return resultType;
          }
      throw new ApplicationException($"");
    }
  }
}
