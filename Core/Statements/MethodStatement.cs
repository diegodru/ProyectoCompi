using System;
using Core.Expressions;
namespace Core.Statements
{
  public class MethodStatement2 : Statement
  {
    public MethodStatement2(Type type, Id id, ArgumentExpression arguments,  Statement statements, TypedExpression returnExpression)
    {
      Type = type;
      Id = id;
      Arguments = arguments;
      Statements = statements;
      ReturnExpression = returnExpression;
    }

    public Type Type { get; }
    public Id Id { get; }
    public ArgumentExpression Arguments { get; }
    public Statement Statements { get; }
    public TypedExpression ReturnExpression { get; }
    
    public override string Generate()
    {
      return $"{Id.Generate()}({Arguments?.Generate()}){{\n{Statements?.Generate()}\n{ReturnExpression?.Generate()}\n}}";
    }

    public override void Interpret()
    {
    }

    public override void ValidateSemantic()
    {
      if(ReturnExpression != null)
      {
        if(ReturnExpression.GetExpressionType() != Type)
        {
          throw new ApplicationException($"El metodo {Id.Generate()} debe retornar un valor de tipo {ReturnExpression.GetExpressionType()} y no de tipo {Type}"); 
        }
      }
    }
  }
}
