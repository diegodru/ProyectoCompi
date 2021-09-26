using Core.Expressions;
namespace Core.Statements
{
  public class DeclarationStatement : Statement
  {
    public DeclarationStatement(Id id)
    {
      Identifier = id;
    }

    public Id Identifier { get; }

    public override void ValidateSemantic()
    {
    }

    public override void Interpret()
    {

    }

    public override string Generate()
    {
      return $"let {Identifier.Generate()}";
    }
  }
}
