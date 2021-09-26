namespace Core.Expressions
{
  public class ArgumentExpression : BinaryOperator
  {
    public ArgumentExpression(Token token, TypedExpression left, TypedExpression right) : 
      base(token, left, right, null)
    {
    }
    public ArgumentExpression(Token token, TypedExpression left) : 
      base(token, left, null, null)
    {
    }

    public override string Generate()
    {
      if (RightExpression != null)
      {
        return $"{LeftExpression.Generate()} {Token.Lexeme} {RightExpression.Generate()}";
      }
      return LeftExpression.Generate();
    }
  }
}
