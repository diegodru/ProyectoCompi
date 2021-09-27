using System;
using Core.Expressions;
namespace Core.Statements
{
  public class ParameterDeclStatment : Statement
  {
    public ParameterDeclStatment(Id id)
    {
      Identifier = id;
    }

    public Id Identifier { get; }

    public override void ValidateSemantic()
    {
      var idtype = Identifier.GetExpressionType();
      if(idtype == Type.Int
          || idtype == Type.Float
          || idtype == Type.String
          || idtype == Type.Bool
          || idtype == Type.DateTime
          || EnvironmentManager.ClassExists(Identifier.GetExpressionType().Lexeme)
        )
        return;
      throw new ApplicationException($"No existe el tipo {idtype}");
    }

    public override void Interpret()
    {

    }

    public override string Generate()
    {
      return Identifier.Generate();
    }
  }
}

