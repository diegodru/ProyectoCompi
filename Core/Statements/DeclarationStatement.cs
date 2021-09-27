using System;
using Core.Expressions;
namespace Core.Statements
{
  public class DeclarationStatement : Statement
  {
    public DeclarationStatement(Id id, bool isParameter)
    {
      Identifier = id;
      IsParameter = isParameter;
    }

    public Id Identifier { get; }
    public bool IsParameter { get; }

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
      if(IsParameter)
        return Identifier.Generate();
      return $"let {Identifier.Generate()}";
    }
  }
}
