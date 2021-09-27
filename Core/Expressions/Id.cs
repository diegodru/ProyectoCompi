namespace Core.Expressions
{
  public class Id : TypedExpression
  {
    public Id(Token token, Type type) : base(token, type)
    {
      IsAttribute = false;
    }

    public Id(Token token, Type type, bool isAttribute) : base(token, type)
    {
      IsAttribute = isAttribute;
    }

    public bool IsAttribute { get; }

    public override dynamic Evaluate()
    {
      return EnvironmentManager.GetSymbolForEvaluation(Token.Lexeme).Value;
    }
    
    public override string Generate()
    {
      if(IsAttribute)
        return $"this.{Token.Lexeme}";
      return Token.Lexeme;
    }

    public override Type GetExpressionType()
    {
      return type;
    }
  }
}
