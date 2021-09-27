namespace Core.Expressions
{
  public class ArgumentList : Expression
  {
    public ArgumentList(ArgumentExpression first, ArgumentExpression second)
      : base(null, null)
    {
      First = first;
      Second = second;
    }
    public ArgumentExpression First { get; }
    public ArgumentExpression Second { get; }

    public override string Generate()
    {
      if(First != null && Second != null)
        return $"{First?.Generate()}, {Second?.Generate()}";
      return First?.Generate();
    }

  }
}
