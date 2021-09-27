using System;
using Core.Expressions;

namespace Core.Statements
{
  public class LogicalNotStatement : Statement
  {
    public LogicalNotStatement(TypedExpression booleano)
    {
      Boolean = booleano;
    }

    public TypedExpression Boolean;
    
    public override string Generate()
    {
      return $"!({Boolean.Generate()})";
    }

    public override void ValidateSemantic()
    {
      if(Boolean.GetExpressionType() != Type.Bool)
      {
        throw new ApplicationException("El operador logico not solo puede actuar en valores booleanos");
      }
    }
    public override void Interpret()
            {
                throw new NotImplementedException();
            }
      
  }
}
