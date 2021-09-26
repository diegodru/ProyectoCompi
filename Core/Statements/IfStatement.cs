using Core.Expressions;
using System;
namespace Core.Statements
{
  public class IfStatement : Statement
  {
    public IfStatement(TypedExpression expression, Statement statement)
    {
      Expression = expression;
      Statement = statement;
    }
    
    public TypedExpression Expression { get; }
    public Statement Statement { get; }

    public override string Generate()
    {
      return $"if({Expression.Generate()})\n{{\n{Statement.Generate()}}}\n";
    }

    public override void Interpret() 
    {
      if (Expression.Evaluate())
      {
        Statement.Interpret();
      }
    }

    public override void ValidateSemantic()
    {
      if (Expression.GetExpressionType() != Type.Bool)
      {
        throw new ApplicationException("La expresion debe de ser un booleano para poder evaluar un if");
      }
    }

  }
}

