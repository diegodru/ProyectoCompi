using System;
using Core.Expressions;
namespace Core.Statements
{
  public class MethodStatement : Statement
  {
    public MethodStatement(Id id, Statement parameters,  Statement statements, ReturnExpression returnExpression)
    {
      Id = id;
      Parameters = parameters;
      Statements = statements;
      ReturnExpression = returnExpression;
    }

    public Id Id { get; }
    public Statement Parameters { get; }
    public Statement Statements { get; }
    public ReturnExpression ReturnExpression { get; }
    
    public override string Generate()
    {
      return $"{Id.Generate()}({Parameters?.Generate()}){{\n{Statements?.Generate()}\n{(ReturnExpression?.Generate())}\n}}";
    }

    public override void Interpret()
    {
    }

    public override void ValidateSemantic()
    {
      if(ReturnExpression != null)
      {
        if(this.ReturnExpression?.Return != null)
        {
          //if(Id.GetExpressionType() == Type.Void)
          //{
            //throw new ApplicationException($"Un metodo de tipo void no puede retornar un valor de ninguno tipo");
          //}
        }
        if(ReturnExpression.GetExpressionType() != Id.GetExpressionType())
        {
          throw new ApplicationException($"El metodo {Id.Generate()} debe retornar un valor de tipo {ReturnExpression.GetExpressionType()} y no de tipo {Id.GetExpressionType()}");
        }
      }

      Parameters?.ValidateSemantic();
      Statements?.ValidateSemantic();
    }

    public System.Type lol()
    {
      return typeof(MethodStatement);
    }
  }
}
