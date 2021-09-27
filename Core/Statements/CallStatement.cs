using System;
using Core.Expressions;

namespace Core.Statements
{
  public class CallStatement : Statement
  {
    public CallStatement(Symbol variable, Symbol symbol, ArgumentExpression args)
    {
      Variable = variable;
      Symbol = symbol;
      Args = args;
    }

    public Symbol Variable { get; }
    public Symbol Symbol { get; }
    public ArgumentExpression Args { get; }

    public override string Generate()
    {
      return $"{Variable.Id.Generate()}.{Symbol.Id.Generate()}({Args.Generate()})";
    }

    public override void ValidateSemantic()
    {
      TypedBinaryOperator expression = Args as TypedBinaryOperator;
      foreach(Id attr in (Symbol.Value as Method).atributos)
      {
        if(attr.GetExpressionType() != expression.GetExpressionType())
        {
          throw new ApplicationException("atributos no coinciden con tipos");
        }
        expression = expression.right as TypedBinaryOperator;
      }
      if(expression == null)
      {
        return;
      }
      throw new ApplicationException("numero equivocado de argumentos");
    }

    public override void Interpret()
            {
                throw new System.NotImplementedException();
            }
      
  }
}
