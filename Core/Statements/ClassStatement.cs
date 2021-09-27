using System;
using Core.Expressions;

namespace Core.Statements
{
  public class ClassStatement : Statement
  {
    public ClassStatement(Id id, Statement declarations)
    {
      Identifier = id;
      Declarations = declarations;
    }
    public Id Identifier { get; }
    public Statement Declarations { get; }

    public override string Generate()
    {
      return $"class {Identifier.Generate()} {{\n{Declarations?.Generate()}\n}}";
    }

    public override void Interpret()
    {
    }

    public override void ValidateSemantic()
    {
    }
  }
}
