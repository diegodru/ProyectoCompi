using System;

namespace Core.Statements
{
  public class ParameterStatement : Statement
  {
    public ParameterStatement(Statement left, Statement right)
    {
      Left = left;
      Right = right;
    }

    public Statement Left { get; }
    public Statement Right { get; }

    public override string Generate()
    {
      if(Left != null && Right != null)
      {
        return $"{Left.Generate()}, {Right.Generate()}";
      }
      return Left?.Generate();
    }

    public override void ValidateSemantic()
    {
      Left?.ValidateSemantic();
      Right?.ValidateSemantic();
    }

    public override void Interpret()
            {
                throw new NotImplementedException();
            }

      
      
  }
}
