using Core.Expressions;
using System;

namespace Core.Statements
{
  public class WhileStatement : Statement
  {
    public WhileStatement(TypedExpression check, Statement loop)
    {
      Check = check;
      Loop = loop;
    }
    
    public TypedExpression Check { get; }
    public Statement Loop { get; }

    public override string Generate()
    {
      return $"while({Check.Generate()}){{\n{Loop.Generate()}\n}}";
    }

    public override void Interpret()
    {
    }

    public override void ValidateSemantic()
    {
      if(Check?.GetExpressionType() != Type.Bool)
      {
          throw new ApplicationException("el while solo puede evaluar bools");
      }
    }
  }
}
