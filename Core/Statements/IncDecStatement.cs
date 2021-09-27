using System;
using Core.Expressions;

namespace Core.Statements
{
  public class IncDecStatement : Statement
  {
    public IncDecStatement(Id id, TokenType type, bool esPre)
    {
      Id = id;
      TokenType = type;
      EsPre = esPre;
    }
    public Id Id { get; }
    public TokenType TokenType { get; }
    public bool EsPre { get; }

    public override string Generate()
    {
      if(EsPre)
        return $"{((TokenType == TokenType.Increment) ? "++" : "--")}{Id.Generate()}";
      return $"{Id.Generate()}{((TokenType == TokenType.Increment) ? "++" : "--")}";
    }

    public override void Interpret()
    {
    }

    public override void ValidateSemantic()
    {
      if(Id.GetExpressionType() != Type.Int)
      {
        throw new ApplicationException($"Solo las variables de tipo entero pueden incrementarse o decrementarse con la notacion ++ o --");
      }
    }
  }
}
