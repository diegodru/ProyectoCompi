using System;
using Core.Expressions;

namespace Core.Statements
{
  public class ConstructorStatement : Statement
  {
    public ConstructorStatement(Statement parameters, Statement statements)
    {
      Parameters = parameters;
      Statements = statements;
    }

    public Statement Parameters { get; }
    public Statement Statements { get; }

    public override string Generate()
    {
      return $"Constructor({Parameters?.Generate()}){{\n{Statements?.Generate()}\n}}";
    }

    public override void ValidateSemantic()
    {
      Parameters?.ValidateSemantic();
      Statements?.ValidateSemantic();
    }

    public override void Interpret()
            {
                throw new NotImplementedException();
            }



      

      

  }
}
