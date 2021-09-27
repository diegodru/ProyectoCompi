namespace Core.Expressions
{
  public class ParenthesesExpression : TypedExpression
  {
    public ParenthesesExpression(TypedExpression expression)
      : base(expression.Token, expression.GetExpressionType())
    {
      Expression = expression;
    }

    public TypedExpression Expression { get; }

    public override string Generate()
    {
      return $"({Expression.Generate()})";
    }

    public override Type GetExpressionType()
    {
      return Expression?.GetExpressionType();
    }

    public override dynamic Evaluate()
    {
      return Expression?.Evaluate();
    }
      
  }
}
