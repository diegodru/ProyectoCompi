namespace Core.Expressions
{
  public class ReturnExpression : TypedExpression
  {
    public ReturnExpression(Token token, Type type, TypedExpression returnExpression)
      : base(token, type)
    {
      Return = returnExpression;
    }

    public TypedExpression Return{ get; }

    public override string Generate()
    {
      return $"return {Return?.Generate()}";
    }

    public override Type GetExpressionType()
    {
      return Return.GetExpressionType();
    }

    public override dynamic Evaluate()
    {
      return Return.Evaluate();
    }

  }
}
