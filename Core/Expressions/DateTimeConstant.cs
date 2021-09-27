namespace Core.Expressions
{
  public class DateTimeConstant : TypedExpression
  {
    public DateTimeConstant(Token token, ArgumentExpression list)
      : base(token, Type.DateTime)
    {
      Arguments = list;
    }

    public ArgumentExpression Arguments { get; }

    public override string Generate()
    {
      return $"new Date({Arguments?.Generate()})";
    }

    public override Type GetExpressionType()
    {
      return Type.DateTime;
    }

    public override dynamic Evaluate()
    {
        throw new System.NotImplementedException();
    }
      
  }
}
