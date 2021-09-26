using Core.Expressions;
using Core.Interfaces;
using System;

namespace Core.Statements
{
  public class ElseStatement : Statement
  {
    public ElseStatement(TypedExpression expression, Statement trueStatement, Statement falseStatement)
    {
      Expression = expression;
      TrueStatement = trueStatement;
      FalseStatement = falseStatement;
    }

    public TypedExpression Expression { get; }
    public Statement TrueStatement { get; }
    public Statement FalseStatement { get; }

    public override string Generate()
    {
      var code = $"if({Expression.Generate()}){{\n";
      code += $"{TrueStatement.Generate()}\n";
      code += $"}}\n";
      code += $"else{{\n";
      code += $"{FalseStatement.Generate()}\n}}";
      return code;
    }

    public override void Interpret()
    {
      if (Expression.Evaluate())
      {
        TrueStatement.Interpret();
      }
      else
      {
        FalseStatement.Interpret();
      }
    }

    public override void ValidateSemantic()
    {
      if (Expression.GetExpressionType() != Type.Bool)
      {
        throw new ApplicationException("solo bools en los ifs");
      }
    }
  }
}
