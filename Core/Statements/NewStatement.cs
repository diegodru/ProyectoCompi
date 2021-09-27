using System;
using Core.Expressions;

namespace Core.Statements
{
  public class NewStatement : Statement
  {
    public NewStatement(Class clase, ArgumentExpression arguments)
    {
      Class = clase;
      Arguments = arguments;
    }

    public Class Class { get; }
    public ArgumentExpression Arguments { get; }

    public override string Generate()
    {
      return $"new {Class.Identifier.Generate()}({Arguments.Generate()})";
    }

    public override void ValidateSemantic()
    {
      if(!EnvironmentManager.ClassExists(Class.Identifier.Generate()))
      {
        throw new ApplicationException($"No existe la clase {Class.Identifier.Generate()}");
      }
    }

    public override void Interpret()
            {
                throw new NotImplementedException();
            }

      
      
  }
}
