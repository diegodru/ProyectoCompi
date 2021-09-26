using System;
using Core.Expressions;

namespace Core.Statements
{
  public class AssignationStatement : Statement
  {
    public AssignationStatement(Id id, TypedExpression expression)
    {
      Id = id;
      Expression = expression;
    }

    public Id Id { get; }
    public TypedExpression Expression { get; }

    public override string Generate()
    {
      return $"{Id.Generate()} = {Expression.Generate()};\n";
    }

    public override void Interpret()
    {
      EnvironmentManager.UpdateVariable(Id.Token.Lexeme, Expression.Evaluate());
    }

    public override void ValidateSemantic()
    {
      if (Id.GetExpressionType() != Expression.GetExpressionType())
      {
        throw new ApplicationException($"El tipo {Id.GetExpressionType()} no se le puede asignar el tipo {Expression.GetExpressionType()}");
      }
    }
  }
}
