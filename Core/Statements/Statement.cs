using Core.Interfaces;

namespace Core.Statements
{
  public abstract class Statement : ISemanticValidation, IStatementEvaluate
  {
    public abstract void Interpret();

    public abstract void ValidateSemantic();

    public abstract string Generate();
  }
}
