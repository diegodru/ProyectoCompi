namespace Core.Expressions
{
  public class ArgumentExpression : BinaryOperator
  {
    public ArgumentExpression(TypedExpression left, TypedExpression right) : 
      base(null, left, right, null)
    {
    }
    public ArgumentExpression(TypedExpression left) : 
      base(null, left, null, null)
    {
    }

    public override string Generate()
    {
      if (RightExpression != null)
      {
        return $"{LeftExpression?.Generate()}, {RightExpression.Generate()}";
      }
      return LeftExpression?.Generate();
    }
  }
}
