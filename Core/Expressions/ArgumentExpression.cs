namespace Core.Expressions
{
  public class ArgumentExpression : TypedBinaryOperator
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
      if (right != null)
      {
        return $"{left?.Generate()}, {right.Generate()}";
      }
      return left?.Generate();
    }

    public override Type GetExpressionType()
    {
      return left.GetExpressionType();
    }

    public override dynamic Evaluate()
            {
                throw new System.NotImplementedException();
            }
      
  }
}
