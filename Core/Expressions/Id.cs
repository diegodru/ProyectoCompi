namespace Core.Expressions
{
  public class Id : TypedExpression
  {
    public Id(Token token, Type type) : base(token, type)
    {
    }

    public override dynamic Evaluate()
    {
      return EnvironmentManager.GetSymbolForEvaluation(Token.Lexeme).Value;
    }
    
    public override string Generate()
    {
      return Evaluate().ToString();
    }

    public override Type GetExpressionType()
    {
      return type;
    }
  }
}
