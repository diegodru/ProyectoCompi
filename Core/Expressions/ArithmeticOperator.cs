﻿using System;
using System.Collections.Generic;

namespace Core.Expressions
{
  public class ArithmeticOperator : TypedBinaryOperator
  {
    private readonly Dictionary<(Type, Type), Type> _typeRules;
    public ArithmeticOperator(Token token, TypedExpression leftExpression, TypedExpression rightExpression)
      : base(token, leftExpression, rightExpression, null)
    {
      _typeRules = new Dictionary<(Type, Type), Type>
      {
        { (Type.Float, Type.Float), Type.Float },
        { (Type.Int, Type.Int), Type.Int },
        { (Type.String, Type.String), Type.String },
        { (Type.Float, Type.Int), Type.Float },
        { (Type.Int, Type.Float), Type.Float },
        { (Type.String, Type.Int), Type.String  },
        { (Type.String, Type.Float), Type.String  }
      };
    }

    public override dynamic Evaluate()
    {
      return Token.TokenType switch
      {
        TokenType.Plus => left.Evaluate() + right.Evaluate(),
        TokenType.Minus => left.Evaluate() - right.Evaluate(),
        TokenType.Asterisk => left.Evaluate() * right.Evaluate(),
        TokenType.Division => left.Evaluate() / right.Evaluate(),
        TokenType.Modulus => left.Evaluate() % right.Evaluate(),
        _ => throw new NotImplementedException()
      };
    }

    public override string Generate()
    {
      return $"{left.Generate()} {Token.Lexeme} {right.Generate()}";
    }

    public override Type GetExpressionType()
    {
      if (_typeRules.TryGetValue((left.GetExpressionType(), right.GetExpressionType()), out var resultType))
      {
        return resultType;
      }
      throw new ApplicationException($"No se puede hacer hacer la expresion {left.GetExpressionType()} {Token.Lexeme} {right.GetExpressionType()}");
    }
  }
}
